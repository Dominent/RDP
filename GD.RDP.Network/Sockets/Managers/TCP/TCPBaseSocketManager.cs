namespace GD.RDP.Network.Sockets.Managers.TCP
{
    using GD.RDP.Contracts;
    using GD.RDP.Core;
    using GD.RDP.Network.Sockets.Managers.Extensions;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    public abstract class TCPBaseSocketManager : BaseSocketManager
    {
        protected Action<IAsyncResult> SendCallback
        {
            get
            {
                return (resp) =>
                {
                    try
                    {
                        var context = (SocketContext)resp.AsyncState;

                        var socket = context.Socket;

                        socket.EndSend(resp);

                        this.OnSend(context);

                        var endpoint = (IPEndPoint)context.EndPoint;

                        Injector.Instance
                           .Get<ILogger>()
                           .Info($"Packet sent to: {endpoint.Address}:{endpoint.Port}{Environment.NewLine}" +
                            $"{Injector.Instance.Get<IDumper>().Dump(context.Buffer)}");
                    }
                    catch (SocketException ex)
                    {
                        Injector.Instance.Get<ILogger>().Error(ex);
                    }
                };
            }
        }

        protected Action<IAsyncResult> ReceiveCallback
        {
            get
            {
                return (resp) =>
                {
                    try
                    {
                        var context = (SocketContext)resp.AsyncState;

                        var handler = context.Socket;
                        var bytesReceived = handler.EndReceive(resp);

                        var buffer = context.Buffer;

                        //TODO(PPavlov): Fix Empty Byte Array Send as FIN, ACK when client socket call shutdown.
                        // Cannot handle with this implementation.
                        // When Rewriting, Use Raw Socket and Handle properly!
                        this.OnReceive(new SocketContext()
                        {
                            Socket = handler,
                            Buffer = buffer
                                .Take(bytesReceived)
                                .ToArray()
                        });

                        var endpoint = ((IPEndPoint)handler.RemoteEndPoint);

                        Injector.Instance
                            .Get<ILogger>()
                            .Info($"Packet received from: {endpoint.Address}:{endpoint.Port}{Environment.NewLine}" +
                            $"{Injector.Instance.Get<IDumper>().Dump(buffer)}");

                        handler.TryBeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                             new AsyncCallback(this.ReceiveCallback),
                             new SocketContext() { Socket = handler, Buffer = buffer });
                    }
                    catch (SocketException ex)
                    {
                        this.Clients
                            .RemoveAll(x => x.EndPoint.Equals(
                                ((SocketContext)resp.AsyncState).Socket.RemoteEndPoint));

                        Injector.Instance.Get<ILogger>().Error(ex);
                    }
                };
            }
        }

        protected Action<IAsyncResult> DisconnectCallback
        {
            get
            {
                return (resp) =>
                {
                    try
                    {
                        var context = (SocketContext)resp.AsyncState;

                        var socket = context.Socket;

                        if (!socket.Connected)
                        {
                            return;
                        }

                        socket.EndDisconnect(resp);

                        this.OnDisconnect(context);

                        Injector.Instance
                               .Get<ILogger>()
                               .Info("Socket Disconnected successfully");
                    }
                    catch (SocketException ex)
                    {
                        Injector.Instance.Get<ILogger>().Error(ex);
                    }
                };
            }
        }

        public override ISocketManager Send(SocketContext context)
        {
            var data = context.Buffer;
            var socket = context.Socket;

            if (socket == null)
            {
                socket = this._socket;
            }

            socket.BeginSend(data, 0, data.Length, 0,
                new AsyncCallback(this.SendCallback),
                new SocketContext() {
                    Socket = socket,
                    Buffer = data,
                    EndPoint = context.EndPoint
                });

            return this;
        }

        public override ISocketManager Receive(SocketContext context)
        {
            var socket = context.Socket;
            var buffer = context.Buffer;

            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                new AsyncCallback(this.ReceiveCallback),
                    new SocketContext() { Socket = socket, Buffer = buffer });

            return this;
        }

        public override ISocketManager Disconnect()
        {
            this._socket.Shutdown(SocketShutdown.Both);

            this._socket.BeginDisconnect(false,
                new AsyncCallback(this.DisconnectCallback),
                new SocketContext() { Socket = this._socket, Buffer = null });

            return this;
        }
    }
}

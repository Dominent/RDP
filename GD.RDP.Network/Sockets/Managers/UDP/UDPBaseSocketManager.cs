namespace GD.RDP.Network.Sockets.Managers.UDP
{
    using GD.RDP.Contracts;
    using System;
    using System.Linq;
    using System.Net.Sockets;

    public abstract class UDPBaseSocketManager : BaseSocketManager
    {
        protected Action<IAsyncResult> ReceiveFromCallback
        {
            get
            {
                return (resp) =>
                {
                    var context = (SocketContext)resp.AsyncState;

                    var handler = context.Socket;
                    var buffer = context.Buffer;
                    var endpoint = context.EndPoint;

                    var bytesReceived = handler.EndReceiveFrom(resp, ref endpoint);

                    this.OnReceive(new SocketContext()
                    {
                        Socket = handler,
                        Buffer = buffer
                         .Take(bytesReceived)
                         .ToArray(),
                        EndPoint = endpoint
                    });

                    this.Receive(new SocketContext()
                    {
                        Socket = handler,
                        Buffer = buffer,
                        EndPoint = endpoint
                    });
                };
            }
        }

        protected Action<IAsyncResult> SendToCallback
        {
            get
            {
                return (resp) =>
                {
                    var context = (SocketContext)resp.AsyncState;

                    var socket = context.Socket;

                    socket.EndSendTo(resp);

                    this.OnSend(context);
                };
            }
        }

        protected Action<IAsyncResult> DisconnectCallback
        {
            get
            {
                return (resp) =>
                {
                    var context = (SocketContext)resp.AsyncState;

                    var socket = context.Socket;

                    socket.EndDisconnect(resp);

                    this.OnDisconnect(context);
                };
            }
        }

        public override ISocketManager Disconnect()
        {
            this._socket.Shutdown(SocketShutdown.Both);

            this._socket.Close();

            return this;
        }

        public override ISocketManager Receive(SocketContext context)
        {
            var buffer = context.Buffer;
            var socket = context.Socket;
            var endpoint = context.EndPoint;

            socket.BeginReceiveFrom(buffer, 0, buffer.Length, 0,
                    ref endpoint,
                    new AsyncCallback(this.ReceiveFromCallback),
                    new SocketContext()
                    {
                        Socket = this._socket,
                        Buffer = buffer,
                        EndPoint = endpoint
                    }
                );

            return this;
        }

        public override ISocketManager Send(SocketContext context)
        {
            var socket = context.Socket != null ? context.Socket : this._socket;
            var buffer = context.Buffer;
            var endpoint = context.EndPoint;

            socket.BeginSendTo(buffer, 0, buffer.Length, 0,
                endpoint,
                new AsyncCallback(this.SendToCallback),
                new SocketContext() { Socket = socket, Buffer = buffer, EndPoint = endpoint });

            return this;
        }
    }
}

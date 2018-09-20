namespace GD.RDP.Network.Sockets.Managers.TCP
{
    using GD.RDP.Contracts;
    using GD.RDP.Core;
    using System;
    using System.Net;
    using System.Net.Sockets;

    public class TCPServerSocketManager : TCPBaseSocketManager
    {
        protected Action<IAsyncResult> AcceptCallback
        {
            get
            {
                return (resp) =>
                {
                    try
                    {
                        var context = (SocketContext)resp.AsyncState;

                        Socket handler = null;
                        Socket listener = context.Socket;

                        var buffer = context.Buffer;

                        handler = listener.EndAccept(resp);

                        this.Receive(new SocketContext() { Socket = handler, Buffer = buffer });

                        var remoteEndPoint = (IPEndPoint)handler.RemoteEndPoint;

                        this._clients.Add(new ClientContext()
                        {
                            EndPoint = remoteEndPoint,
                            Socket = handler
                        });

                        Injector.Instance
                            .Get<ILogger>()
                            .Info($"Remote client {remoteEndPoint.Address}:{remoteEndPoint.Port}, connected");

                        listener.BeginAccept(
                           new AsyncCallback(this.AcceptCallback),
                           new SocketContext() { Socket = listener, Buffer = new byte[Constants.MAX_BUFFER_SIZE] });
                    }
                    catch (SocketException ex)
                    {
                        Injector.Instance.Get<ILogger>().Error(ex);
                    }
                };
            }
        }

        public override ISocketManager Initialize(IPEndPoint endpoint)
        {
            this._endpoint = endpoint;

            this._socket = new Socket(
                endpoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            this._socket.Bind(this._endpoint);

            this.OnInitialize(new SocketContext()
            {
                Socket = this._socket,
                EndPoint = this._endpoint
            });

            Injector.Instance
                .Get<ILogger>()
                .Info($"{this.GetType().Name} has been initialized successfully");

            return this;
        }

        public override ISocketManager Start()
        {
            this._socket.Listen(Constants.MAX_ALLOWED_CONNECTIONS);

            Injector.Instance
              .Get<ILogger>().Info($"{this.GetType().Name} is now listening on: {this._endpoint.Address}:{_endpoint.Port}");

            this._socket.BeginAccept(
               new AsyncCallback(this.AcceptCallback),
               new SocketContext() { Socket = this._socket, Buffer = new byte[Constants.MAX_BUFFER_SIZE] });

            return this;
        }
    }
}

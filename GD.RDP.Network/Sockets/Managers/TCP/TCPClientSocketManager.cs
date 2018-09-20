namespace GD.RDP.Network.Sockets.Managers.TCP
{
    using GD.RDP.Contracts;
    using GD.RDP.Core;
    using System;
    using System.Net;
    using System.Net.Sockets;

    public class TCPClientSocketManager : TCPBaseSocketManager
    {
        protected Action<IAsyncResult> ConnectCallback
        {
            get
            {
                return (resp) =>
                {
                    try
                    {
                        var context = (SocketContext)resp.AsyncState;

                        var socket = context.Socket;
                        var buffer = context.Buffer;

                        socket.EndConnect(resp);

                        ConnectResetEvent.Set();

                        this.Receive(new SocketContext() { Socket = this._socket, Buffer = buffer });
                    }
                    catch (SocketException ex)
                    {
                        Injector.Instance.Get<ILogger>().Error(ex);

                        ConnectResetEvent.Set();
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

            this.OnInitialize(new SocketContext()
            {
                Socket = this._socket,
                EndPoint = this._endpoint
            });

            return this;
         }

        public override ISocketManager Start()
        {
            this._socket
                .BeginConnect(this._endpoint,
                    new AsyncCallback(this.ConnectCallback),
                    new SocketContext() { Socket = this._socket, Buffer = new byte[Constants.MAX_BUFFER_SIZE] });

            this.BlockEvent(EventType.CONNECT);

            return this;
        }
    }
}

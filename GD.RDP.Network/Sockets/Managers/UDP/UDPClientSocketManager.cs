namespace GD.RDP.Network.Sockets.Managers.UDP
{
    using GD.RDP.Contracts;
    using GD.RDP.Core;
    using System.Net;
    using System.Net.Sockets;

    public class UDPClientSocketManager : UDPBaseSocketManager
    {
        public override ISocketManager Initialize(IPEndPoint endpoint)
        {
            this._endpoint = endpoint;

            this._socket = new Socket(
                endpoint.AddressFamily,
                SocketType.Dgram,
                ProtocolType.Udp);

            this._socket.Bind(endpoint);

            this.OnInitialize(new SocketContext()
            {
                Socket = this._socket,
                EndPoint = this._endpoint
            });

            return this;
        }

        public override ISocketManager Start()
        {
            this.Receive(new SocketContext()
            {
                Socket = this._socket,
                Buffer = new byte[Constants.MAX_BUFFER_SIZE],
                EndPoint = this._endpoint
            });

            return this;
        }
    }
}

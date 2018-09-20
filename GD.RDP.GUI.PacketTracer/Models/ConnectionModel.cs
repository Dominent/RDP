namespace GD.RDP.GUI.PacketTracer.Models
{
    using System.Net;

    public class ConnectionModel
    {
        public IPEndPoint EndPoint { get; set; }

        public byte[] Data { get; set; }
    }
}

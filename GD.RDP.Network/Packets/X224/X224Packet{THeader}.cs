namespace GD.RDP.Network.Packets.X224
{
    using GD.RDP.Contracts;
    
    public class X224Packet<THeader> : IPacket<X224Header>
        where THeader : X224Header
    {
        public X224Header Header { get; }

        public byte[] Data { get; }
    }
}

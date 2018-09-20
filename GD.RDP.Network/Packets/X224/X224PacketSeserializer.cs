namespace GD.RDP.Network.Packets.X224
{
    using GD.RDP.Packets;
    using System;

    public class X224PacketSeserializer 
        : BasePacketSerializer<X224Packet<X224Header>>
    {
        public override byte[] SerializePacket(X224Packet<X224Header> packet)
        {
            throw new NotImplementedException();
        }
    }
}

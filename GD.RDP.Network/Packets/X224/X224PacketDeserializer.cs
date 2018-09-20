namespace GD.RDP.Network.Packets.X224
{
    using System;

    public class X224PacketDeserializer 
        : BasePacketDeserializer<X224Packet<X224Header>>
    {
        public X224PacketDeserializer(byte[] buffer)
            : base(buffer)
        {
        }

        public override X224Packet<X224Header> DeserializePacket()
        {
            throw new NotImplementedException();
        }
    }
}

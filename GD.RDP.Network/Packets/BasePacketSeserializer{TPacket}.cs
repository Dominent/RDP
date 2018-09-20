namespace GD.RDP.Packets
{
    using GD.RDP.Contracts;

    public abstract class BasePacketSerializer<TPacket> 
        : IPacketSerializer<TPacket>
    {
        public abstract byte[] SerializePacket(TPacket packet);
    }
}

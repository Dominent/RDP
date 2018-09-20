namespace GD.RDP.Contracts
{
    public interface IPacketSerializer<TPacket>
    {
        byte[] SerializePacket(TPacket packet);
    }
}

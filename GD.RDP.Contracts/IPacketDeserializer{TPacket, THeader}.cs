namespace GD.RDP.Contracts
{
    public interface IPacketDeserializer<TPacket>
    {
        TPacket DeserializePacket();
    }
}

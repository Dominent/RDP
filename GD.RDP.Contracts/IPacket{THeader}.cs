namespace GD.RDP.Contracts
{
    public interface IPacket<THeader> 
        where THeader : class
    {
        THeader Header { get; }

        byte[] Data { get; }
    }
}

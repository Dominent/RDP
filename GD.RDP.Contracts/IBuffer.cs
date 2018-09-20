namespace GD.RDP.Core.Contracts
{
    public interface IBuffer
    {
        T Read<T>() where T : struct;

        IBuffer Write<T>(T value) where T : struct;

        IBuffer Write(long value, int offset, int count);

        IBuffer Write(long value, int offset);

        bool IsEmpty();

        int Count { get; }

        byte[] Value { get; }
    }
}

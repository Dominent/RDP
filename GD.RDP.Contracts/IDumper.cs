namespace GD.RDP.Contracts
{
    public interface IDumper
    {
        string Dump(byte[] buffer);

        byte[] Load(string dump);
    }
}

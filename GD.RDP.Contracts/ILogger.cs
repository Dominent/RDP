namespace GD.RDP.Contracts
{
    using System;

    public interface ILogger
    {
        ILogger Log(string message);

        ILogger Info(string message);

        ILogger Error(Exception ex);
    }
}

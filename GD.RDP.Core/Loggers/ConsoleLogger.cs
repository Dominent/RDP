namespace GD.RDP.Core.Loggers
{
    using System;
    using GD.RDP.Contracts;

    public class ConsoleLogger : ILogger
    {
        public ILogger Error(Exception ex)
        {
            this.Log($"[ERROR]{ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}");

            return this;
        }

        public ILogger Info(string message)
        {
            this.Log($"[INFO]{message}{Environment.NewLine}");

            return this;
        }

        public ILogger Log(string message)
        {
            Console.Write($"[{DateTimeOffset.Now.ToUnixTimeSeconds()}]{message}");

            return this;
        }
    }
}

namespace GD.RDP.Core.Loggers
{
    using GD.RDP.Contracts;

    using System;
    using System.IO;
    using System.Reflection;

    public class FileLogger : ILogger
    {
        private static readonly object _mutex = new Object();

        private string _path;

        public ILogger Info(string message)
        {
            this.Log($"[INFO]{message}{Environment.NewLine}");

            return this;
        }

        public ILogger Error(Exception ex)
        {
            this.Log($"[ERROR]{ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}");

            return this;
        }

        public ILogger Log(string message)
        {
            if (this._path == null)
            {
                lock (_mutex)
                {
                    this._path = this.BuildFilePath();

                    using (File.Create(this._path)) { };
                }
            }

            lock (_mutex)
            {
                File.AppendAllText(this._path, $"[{DateTimeOffset.Now.ToUnixTimeSeconds()}]{message}");
            }

            return this;
        }

        private string BuildFilePath()
        {
            var prefix = Assembly.GetEntryAssembly().GetName().Name;

            return $"{prefix}.{{{DateTimeOffset.Now.ToUnixTimeSeconds()}}}.log.txt";
        }
    }
}

namespace GD.RDP.Console.Client
{
    using GD.RDP.Core;
    using GD.RDP.Engine;
    using System.Net;
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            new RDPEngine()
                .Ignition(new IPEndPoint(
                    IPAddress.Parse("192.168.1.2"),
                    Constants.DEFAULT_RDP_PORT))
                .Start();

            Console.Read();
        }
    }
}

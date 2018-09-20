namespace GD.RDP.Console.MITM
{
    using GD.RDP.Contracts;
    using GD.RDP.Core;
    using GD.RDP.Core.Loggers;
    using GD.RDP.Debug;
    using GD.RDP.Network.Sockets.Managers.TCP;
    using System;
    using System.Linq;
    using System.Net;

    public class Program
    {
        private const int RDP_DEFAULT_PORT = 3389;

        public static void Main(string[] args)
        {
            Console.WriteLine("Enter IPAddress Of RDP Client:");
            var clientIPAddress = IPAddress.Parse(Console.ReadLine());
            var serverIPAddress = IPAddress.Any;

            Injector.Instance
               .Register<ILogger>(new ConsoleLogger())
               .Register<IDumper>(new HexDumper())
               .Register<InputHelper>(new InputHelper())
               .Register<TCPServerSocketManager>((TCPServerSocketManager)new TCPServerSocketManager()
                   .Initialize(new IPEndPoint(serverIPAddress, RDP_DEFAULT_PORT))
                   .Start())
                .Register<TCPClientSocketManager>((TCPClientSocketManager)new TCPClientSocketManager()
                   .Initialize(new IPEndPoint(clientIPAddress, RDP_DEFAULT_PORT)));


            Injector.Instance
                .Get<TCPClientSocketManager>()
                .Subscribe(EventType.RECEIVE, (snd, ctx) =>
                {
                    var server = Injector.Instance
                        .Get<TCPServerSocketManager>();

                    var client = server.Clients.FirstOrDefault();

                    server
                        .Send(new SocketContext
                        {
                            EndPoint = client.EndPoint,
                            Socket = client.Socket,
                            Buffer = ctx.Buffer
                        });
                });

            //TODO(PPavlov): Implement Accept subscription
            Injector.Instance
                .Get<TCPServerSocketManager>()
                .Subscribe(EventType.ACCEPT, (snd, ctx) =>
                {
                    //TODO(PPavlov): Separate Connect from start
                    Injector.Instance
                        .Get<TCPClientSocketManager>()
                        .Start();
                })
                .Subscribe(EventType.RECEIVE, (snd, ctx) =>
                {
                    Injector.Instance
                     .Get<TCPClientSocketManager>()
                        .Send(new SocketContext { Buffer = ctx.Buffer });
                });
        }
    }
}

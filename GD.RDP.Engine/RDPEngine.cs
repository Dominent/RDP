namespace GD.RDP.Engine
{
    using GD.RDP.Contracts;
    using GD.RDP.Core;
    using GD.RDP.Core.Loggers;
    using GD.RDP.Debug;
    using GD.RDP.Network.Sockets.Managers.TCP;
    using System;
    using System.Net;

    public class RDPEngine : IRDPEngine
    {
        public IRDPEngine Ignition(IPEndPoint endpoint)
        {
            Injector.Instance
                .Register<IPEndPoint>(endpoint)
                .Register<ILogger>(new FileLogger())
                .Register<IDumper>(new HexDumper())
                .Register<ISocketManager>(new TCPClientSocketManager().Initialize(endpoint).Start());

#if DEBUG
            Injector.Instance.Get<ISocketManager>()
                .Subscribe(EventType.RECEIVE, (snd, ctx) =>
                {
                    Injector.Instance.Get<ILogger>()
                        .Info(Injector.Instance.Get<IDumper>()
                            .Dump(ctx.Buffer));
                });
#endif

            return this;
        }

        public IRDPEngine Start()
        {
            Injector.Instance
                .Get<ISocketManager>()
                .Send(new SocketContext()
                {
                    Buffer = new byte[] { 70, 0 },
                    EndPoint = Injector.Instance.Get<IPEndPoint>()
                })
                .Subscribe(EventType.RECEIVE, (snd, ctx) =>
                {
                    Console.WriteLine(ctx.Buffer);
                });

            return this;
        }
    }
}

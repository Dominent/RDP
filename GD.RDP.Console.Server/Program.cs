namespace GD.RDP.Console.Server
{
    using System;
    using System.IO;
    using System.Net;
    using System.Xml;
    using System.Linq;
    using System.Globalization;

    using GD.RDP.Core;
    using GD.RDP.Debug;
    using GD.RDP.Contracts;
    using GD.RDP.Network.Sockets.Managers.TCP;
    using GD.RDP.Core.Loggers;

    public class Program
    {
        private const string SEQUENCES_PATH = "_sequences";
        private const string SERVER_NODE = "server";
        private const string CLIENT_NODE = "client";
        private const string SEQUENCE_NODE = "sequence";
        private const int RDP_DEFAULT_PORT = 3389;

        static void Main(string[] args)
        {
            Injector.Instance
                .Register<ILogger>(new ConsoleLogger())
                .Register<IDumper>(new HexDumper())
                .Register<InputHelper>(new InputHelper())
                .Register<ISocketManager>(new TCPServerSocketManager()
                    .Initialize(new IPEndPoint(IPAddress.Any, RDP_DEFAULT_PORT))
                    .Start());

            bool isBlocked = true;
            byte[] buffer = new byte[] { };

            Injector.Instance
                      .Get<ISocketManager>()
                      .Subscribe(EventType.RECEIVE, (rec, ctx) =>
                      {
                          var dump = Injector.Instance
                                      .Get<IDumper>()
                                          .Dump(ctx.Buffer);

                          isBlocked = !ctx.Buffer.SequenceEqual(buffer);
                      });

            try
            {
                ReadSequencesXML(ChooseSequence(),
                (seq) =>
                {
                    isBlocked = true;

                    buffer = Injector.Instance
                               .Get<InputHelper>()
                               .TextToByteArray(
                                   seq.InnerText,
                                   new string[] { "\t", "\n", "\r" },
                                   NumberStyles.HexNumber);

                    while (isBlocked) ;
                },
                (seq) =>
                {
                    var server = Injector.Instance
                        .Get<ISocketManager>();

                    var context = new SocketContext()
                    {
                        Socket = server.Clients.FirstOrDefault().Socket,
                        EndPoint = server.Clients.FirstOrDefault().EndPoint,
                        Buffer = Injector.Instance
                           .Get<InputHelper>()
                           .TextToByteArray(
                                    seq.InnerText,
                                    new string[] { "\t", "\n", "\r" },
                                    NumberStyles.HexNumber)
                    };

                    server.Send(context);
                });
            }
            catch (Exception ex)
            {
                Injector.Instance.Get<ILogger>().Error(ex);
            }
        }

        private static void ReadSequencesXML(
            string[] sequences,
            Action<XmlNode> clientSeqCallback,
            Action<XmlNode> serverSeqCallback
        )
        {
            string path = sequences[Convert.ToInt32(System.Console.ReadLine()) % sequences.Length];

            var document = new XmlDocument();

            document.Load(path);

            foreach (XmlNode node in document.FirstChild.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;

                if (node.Name == SEQUENCE_NODE)
                {
                    foreach (XmlNode seq in node.ChildNodes)
                    {
                        if (seq.Name == SERVER_NODE)
                        {
                            serverSeqCallback.Invoke(seq);
                        }
                        else if (seq.Name == CLIENT_NODE)
                        {
                            clientSeqCallback.Invoke(seq);
                        }
                    }
                }
            }
        }

        private static string[] ChooseSequence()
        {
            var sequences = Directory.EnumerateFiles(SEQUENCES_PATH)
                .ToArray();

            sequences.Select((x, i) => new { Index = i, Value = x })
                .ToList()
                .ForEach(x => System.Console.WriteLine($"[{x.Index}]:{x.Value}"));

            return sequences;
        }
    }
}

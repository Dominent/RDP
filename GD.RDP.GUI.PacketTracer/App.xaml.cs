namespace GD.RDP.GUI.PacketTracer
{
    using GD.RDP.Contracts;
    using GD.RDP.Core;
    using GD.RDP.Core.Loggers;
    using GD.RDP.Debug;
    using System.Windows;

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Injector.Instance
              .Register<ILogger>(new FileLogger())
              .Register<IDumper>(new HexDumper())
              .Register<InputHelper>(new InputHelper());
        }
    }
}

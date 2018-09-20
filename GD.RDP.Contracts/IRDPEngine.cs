using System.Net;

namespace GD.RDP.Contracts
{
    public interface IRDPEngine
    {
        IRDPEngine Ignition(IPEndPoint endpoint);

        IRDPEngine Start();
    }
}

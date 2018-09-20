namespace GD.RDP.Core.Helpers
{
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    public class IPAddressHelper
    {
        public static IPAddress GetLocalIPAddress()
        {
            return Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.Where(al => al.AddressFamily == AddressFamily.InterNetwork)
                .AsEnumerable().FirstOrDefault();
        }
    }
}

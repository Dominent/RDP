namespace GD.RDP.Core.Extensions
{
    using System;
    using System.Net.Sockets;

    public static class SocketExtensions
    {
        public static void BeginReceive(this Socket socket, byte[] buffer, Action<IAsyncResult> action)
        {
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(action), null);
        }
    }
}

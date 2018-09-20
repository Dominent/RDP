namespace GD.RDP.Network.Sockets.Managers.Extensions
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    public static class SocketExtensions
    {
        private const int WSAECONNABORTED = 10053;
        private const int WSAESHUTDOWN = 10058;

        public static void TryBeginReceive(this Socket socket,
            byte[] buffer, int offset, int size, SocketFlags socketFlags, AsyncCallback callback, object state)
        {
            try
            {
                socket.BeginReceive(buffer, offset, size, socketFlags, callback, state);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode != WSAECONNABORTED &&
                    ex.ErrorCode != WSAESHUTDOWN)
                {
                    throw ex;
                }

                var remoteEndPoint = (IPEndPoint)socket.RemoteEndPoint;

                socket.Close();

                Console.WriteLine($"Remote client {remoteEndPoint.Address}:{remoteEndPoint.Port}, disconnected!");
            }
        }
    }
}

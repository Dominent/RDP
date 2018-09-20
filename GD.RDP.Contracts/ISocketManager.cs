namespace GD.RDP.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    public interface ISocketManager
    {
        ISocketManager Initialize(IPEndPoint endpoint);
        
        ISocketManager Start();

        ISocketManager Send(SocketContext context);

        ISocketManager Receive(SocketContext context);

        ISocketManager Disconnect();

        ISocketManager Subscribe(EventType eventType, Action<Object, SocketContext> action);

        ISocketManager BlockEvent(EventType type);

        ISocketManager ResetEvent(EventType type);

        IEnumerable<ClientContext> Clients { get; }
    }

    public struct SocketContext
    {
        public Socket Socket { get; set; }

        public byte[] Buffer { get; set; }

        public EndPoint EndPoint { get; set; }
    }

    public struct ClientContext
    {
        public IPEndPoint EndPoint { get; set; }

        public Socket Socket { get; set; }
    }

    public enum EventType
    {
        CONNECT,
        ACCEPT,
        SEND,
        RECEIVE,
        DISCONNECT,
        INITIALIZE
    }
}
namespace GD.RDP.Network.Sockets.Managers
{
    using GD.RDP.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public abstract class BaseSocketManager : ISocketManager
    {
        public BaseSocketManager()
        {
            this._clients = new List<ClientContext>();
        }

        protected IPEndPoint _endpoint;
        protected Socket _socket;

        protected List<ClientContext> _clients;

        public ISocketManager Subscribe(EventType eventType, Action<Object, SocketContext> action)
        {
            switch (eventType)
            {
                //TODO(PPavlov): Rework Connect separate from start, initialize
                //TODO(PPavlov): Add Accept Connection, Client
                case EventType.CONNECT: break;
                case EventType.ACCEPT: break;
                case EventType.DISCONNECT: this.DisconnectHandler += (snd, ctx) => action(snd, ctx); break;
                case EventType.SEND: this.SendHandler += (snd, ctx) => action(snd, ctx); break;
                case EventType.RECEIVE: this.ReceiveHandler += (snd, ctx) => action(snd, ctx); break;
                case EventType.INITIALIZE: this.InitializeHandler += (snd, ctx) => action(snd, ctx); break;
                default: break;

            }

            return this;
        }

        public event EventHandler<SocketContext> ReceiveHandler;
        public event EventHandler<SocketContext> SendHandler;
        public event EventHandler<SocketContext> DisconnectHandler;
        public event EventHandler<SocketContext> InitializeHandler;

        protected static ManualResetEvent SendResetEvent { get; } = new ManualResetEvent(false);
        protected static ManualResetEvent ConnectResetEvent { get; } = new ManualResetEvent(false);
        protected static ManualResetEvent ReceiveResetEvent { get; } = new ManualResetEvent(false);

        public IEnumerable<ClientContext> Clients
        {
            get
            {
                return this._clients.Select(x => x);
            }
        }

        protected void OnReceive(SocketContext context)
        {
            if (this.ReceiveHandler == null)
            {
                return;
            }

            this.ReceiveHandler.Invoke(this, context);
        }

        protected void OnSend(SocketContext context)
        {
            if (this.SendHandler == null)
            {
                return;
            }

            this.SendHandler.Invoke(this, context);
        }

        protected void OnDisconnect(SocketContext context)
        {
            if (this.DisconnectHandler == null)
            {
                return;
            }

            this.DisconnectHandler.Invoke(this, context);
        }

        protected void OnInitialize(SocketContext context)
        {
            if (this.InitializeHandler == null)
            {
                return;
            }

            this.InitializeHandler.Invoke(this, context);
        }

        public abstract ISocketManager Initialize(IPEndPoint endpoint);
        public abstract ISocketManager Start();
        public abstract ISocketManager Send(SocketContext context);
        public abstract ISocketManager Receive(SocketContext context);
        public abstract ISocketManager Disconnect();

        public ISocketManager BlockEvent(EventType type)
        {
            switch (type)
            {
                case EventType.CONNECT:
                    ConnectResetEvent.WaitOne();
                    break;
                case EventType.RECEIVE:
                    ReceiveResetEvent.WaitOne();
                    break;
                case EventType.SEND:
                    SendResetEvent.WaitOne();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return this;
        }

        public ISocketManager ResetEvent(EventType type)
        {
            switch (type)
            {
                case EventType.CONNECT:
                    ConnectResetEvent.Set();
                    break;
                case EventType.RECEIVE:
                    ReceiveResetEvent.Set();
                    break;
                case EventType.SEND:
                    SendResetEvent.Set();
                    break;
                default:
                    throw new NotImplementedException();
            }

            return this;
        }
    }
}

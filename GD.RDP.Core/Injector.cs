namespace GD.RDP.Core
{
    using GD.RDP.Contracts;
    using System;
    using System.Collections.Concurrent;

    public class Injector : IInjector
    {
        public static IInjector Instance { get; } =
            new Injector();

        private Injector()
        {
            this._container = new ConcurrentDictionary<Type, object>();
        }

        private ConcurrentDictionary<Type, object> _container;

        public T Get<T>() where T : class
        {
            return (T)this._container[typeof(T)];
        }

        public IInjector Register<T>(T instance) where T : class
        {
            if(this._container.ContainsKey(typeof(T)))
            {
                Object value;
                if (!this._container.TryRemove(typeof(T), out value))
                {
                    throw new InvalidOperationException(value.ToString());
                }
            }

            this._container.GetOrAdd(typeof(T), instance);

            return this;
        }
    }
}

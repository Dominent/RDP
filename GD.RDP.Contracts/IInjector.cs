namespace GD.RDP.Contracts
{
    public interface IInjector
    {
        IInjector Register<T>(T instance) where T : class;

        T Get<T>() where T : class;
    }
}
namespace NAutowired.Core
{
    public interface IConsoleHost
    {
        void Run<TStartup>() where TStartup : Startup, new();
    }
}

using System;

namespace NAutowired.Core
{
    public interface IConsoleHost
    {
        void Run<TStartup>() where TStartup : Startup, new();

        T GetService<T>();

        object GetService(Type type);
    }
}

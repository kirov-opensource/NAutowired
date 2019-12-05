using System;

namespace NAutowired.Core
{
    public interface IConsoleHost
    {
        void Run<TStartup>() where TStartup : Startup, new();

        TInterface GetService<TInterface>();

        TInterface GetService<TInterface, TImplement>() where TImplement : TInterface;

        object GetService(Type type);
    }
}

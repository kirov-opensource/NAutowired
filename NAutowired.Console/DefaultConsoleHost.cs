using Microsoft.Extensions.DependencyInjection;
using NAutowired.Core;
using System;

namespace NAutowired.Console
{
    public class DefaultConsoleHost : IConsoleHost
    {
        private string[] args;
        private IServiceProvider serviceProvider;

        public DefaultConsoleHost(IServiceCollection serviceCollection, string[] args)
        {
            this.args = args;
            this.serviceProvider = serviceCollection.BuildServiceProvider();
        }


        public void Run<TStartup>() where TStartup : Startup, new()
        {
            var instance = new TStartup();
            DependencyInjection.Resolve(serviceProvider, instance);
            instance.Run(args);
        }

        public T GetService<T>()
        {
            var instance = serviceProvider.GetService<T>();
            if (instance == null)
                return default;
            DependencyInjection.Resolve(serviceProvider, instance);
            return instance;
        }

        public object GetService(Type type)
        {
            var instance = serviceProvider.GetService(type);
            if (instance == null)
                return default;
            DependencyInjection.Resolve(serviceProvider, instance);
            return instance;
        }
    }
}

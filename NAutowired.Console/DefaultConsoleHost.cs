using Microsoft.Extensions.DependencyInjection;
using NAutowired.Core;

namespace NAutowired.Console
{
    public class DefaultConsoleHost : IConsoleHost
    {
        private string[] args;
        private IServiceCollection serviceCollection;

        public DefaultConsoleHost(IServiceCollection serviceCollection, string[] args)
        {
            this.serviceCollection = serviceCollection;
            this.args = args;
        }


        public void Run<TStartup>() where TStartup : Startup, new()
        {
            var instance = new TStartup();
            DependencyInjection.Resolve(serviceCollection.BuildServiceProvider(), instance);
            instance.Run(args);
        }
    }
}

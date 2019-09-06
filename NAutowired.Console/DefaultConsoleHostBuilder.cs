using Microsoft.Extensions.DependencyInjection;
using NAutowired.Core;

namespace NAutowired.Console
{
    public class DefaultConsoleHostBuilder : IConsoleHostBuilder
    {
        private string[] args;

        private IServiceCollection serviceCollection;

        public DefaultConsoleHostBuilder(IServiceCollection serviceCollection)
        {
            this.serviceCollection = serviceCollection;
        }

        public DefaultConsoleHostBuilder(IServiceCollection serviceCollection, string[] args)
        {
            this.args = args;
            this.serviceCollection = serviceCollection;
        }

        /// <summary>
        /// Build ConsoleHost
        /// </summary>
        /// <returns></returns>
        public IConsoleHost Build()
        {
            return new DefaultConsoleHost(serviceCollection, args);
        }

    }
}

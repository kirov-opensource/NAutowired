using NAutowired.Core;
using System;

namespace NAutowired
{
    public class DefaultConsoleHostBuilder : IConsoleHostBuilder
    {
        private string[] args;

        private Type startupType;

        public DefaultConsoleHostBuilder() { }

        public DefaultConsoleHostBuilder(string[] args)
        {
            this.args = args;
        }

        public DefaultConsoleHostBuilder(Type startupType, string[] args)
        {
            this.args = args;
            this.startupType = startupType;
        }

        /// <summary>
        /// Build ConsoleHost
        /// </summary>
        /// <returns></returns>
        public IConsoleHost Build()
        {
            return new DefaultConsoleHost(startupType, args);
        }

    }
}

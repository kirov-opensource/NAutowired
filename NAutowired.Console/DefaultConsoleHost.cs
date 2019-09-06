using NAutowired.Core;
using System;

namespace NAutowired.Console
{
    public class DefaultConsoleHost : IConsoleHost
    {
        private Type startupType;
        private string[] args;
        public DefaultConsoleHost(Type startupType, string[] args)
        {
            this.startupType = startupType;
            this.args = args;
        }


        public void Run()
        {
            //startupType.g
            throw new NotImplementedException();
        }
    }
}

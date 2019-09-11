using NAutowired.Core.Attributes;
using NAutowired.Core.Models;
using System;

namespace NAutowired.Console.Test.TestClass
{
    [Service(Lifetime.Singleton)]
    public class SingletonLifetimeService
    {
        private readonly Guid guid;

        public SingletonLifetimeService()
        {
            guid = new Guid();
        }

        public Guid GetGuid() { return guid; }
    }
}

using NAutowired.Core.Attributes;
using NAutowired.Core.Models;
using System;

namespace NAutowired.Console.Test.TestClass
{
    [Service(Lifetime.Scoped)]
    public class ScopedLifetimeService
    {
        private readonly Guid guid;

        public ScopedLifetimeService()
        {
            guid = new Guid();
        }

        public Guid GetGuid() { return guid; }
    }
}

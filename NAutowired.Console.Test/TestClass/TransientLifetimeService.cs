using NAutowired.Core.Attributes;
using NAutowired.Core.Models;
using System;

namespace NAutowired.Console.Test.TestClass
{
    [Service(Lifetime.Transient)]
    public class TransientLifetimeService
    {
        private readonly Guid guid;

        public TransientLifetimeService()
        {
            guid = new Guid();
        }

        public Guid GetGuid() { return guid; }
    }
}

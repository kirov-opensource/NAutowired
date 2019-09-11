using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    [Service]
    public class LifetimeService
    {

        [Autowired]
        private readonly TransientLifetimeService transientLifetimeService;
        [Autowired]
        private readonly ScopedLifetimeService scopedLifetimeService;
        [Autowired]
        private readonly SingletonLifetimeService singletonLifetimeService;

        public TransientLifetimeService GetTransientLifetimeService()
        {
            return transientLifetimeService;
        }
        public ScopedLifetimeService GetScopedLifetimeService()
        {
            return scopedLifetimeService;
        }
        public SingletonLifetimeService GetSingletonLifetimeService()
        {
            return singletonLifetimeService;
        }

    }
}

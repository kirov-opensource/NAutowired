using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    public class Service
    {
        [Autowired]
        private readonly BarService barService;

        [Autowired]
        private FooService fooService { get; set; }

        public FooService GetFooService() { return fooService; }

        public BarService GetBarService() { return barService; }
    }
}

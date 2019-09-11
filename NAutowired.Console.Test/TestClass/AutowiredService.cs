using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    [Service]
    public class AutowiredService
    {
        [Autowired]
        private readonly BarService barService;

        [Autowired]
        private FooService fooService { get; set; }

        public BarService GetBarService() { return barService; }
        public FooService GetFooService() { return fooService; }

    }
}

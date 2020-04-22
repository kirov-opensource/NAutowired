using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    [Service]
    public class ImplicitImplementMultipleInterfaceService
    {
        [Autowired]
        private readonly IFooService fooService;

        [Autowired]
        private readonly IBarService barService;

        public IFooService GetFooService()
        {
            return fooService;
        }

        public IBarService GetBarService()
        {
            return barService;
        }

    }
}

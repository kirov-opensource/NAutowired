using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    [Service]
    public class ConstructorResolvedService
    {
        [Autowired]
        private readonly BarService barService;

        public BarService GetBarService() { return barService; }
    }
}

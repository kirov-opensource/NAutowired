using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    [Service]
    public class CircularDependencyAInterfaceService : IAService
    {
        [Autowired]
        private readonly IBService bService;

        public IBService GetCircularDependencyBService() { return bService; }

    }
}

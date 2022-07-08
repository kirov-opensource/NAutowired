using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    [Service]
    public class CircularDependencyBInterfaceService : IBService
    {
        [Autowired]
        private readonly IAService aService;

        public IAService GetCircularDependencyAService() { return aService; }
    }
}

using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    [Service]
    public class CircularDependencyFooService
    {
        [Autowired]
        private readonly CircularDependencyBarService circularDependencyBarService;

        public CircularDependencyBarService GetCircularDependencyBarService() { return circularDependencyBarService; }
    }
}

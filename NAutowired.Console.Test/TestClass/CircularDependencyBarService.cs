using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    [Service]
    public class CircularDependencyBarService
    {
        [Autowired]
        private readonly CircularDependencyFooService circularDependencyFooService;

        public CircularDependencyFooService GetCircularDependencyFooService() { return circularDependencyFooService; }

    }
}

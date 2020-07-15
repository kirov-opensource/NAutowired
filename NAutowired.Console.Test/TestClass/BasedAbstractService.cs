using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    [Service]
    public class BasedAbstractService : AbstractService
    {
        public FooService GetFooService() { return fooService; }
    }
}

using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    public abstract class AbstractService
    {
        [Autowired]
        protected readonly FooService fooService;
    }
}

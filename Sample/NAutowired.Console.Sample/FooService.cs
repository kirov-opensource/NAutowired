using NAutowired.Core.Attributes;

namespace NAutowiredConsoleSample
{
    [Service]
    public class FooService
    {
        public string Foo()
        {
            return "Hello World";
        }

    }
}

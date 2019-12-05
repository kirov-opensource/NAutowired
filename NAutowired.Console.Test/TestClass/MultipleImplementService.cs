using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    [Service]
    public class MultipleImplementService
    {
        [Autowired(typeof(MultipleImplementBarService))]
        private readonly IMultipleImplement barMultipleImplement;

        [Autowired(typeof(MultipleImplementFooService))]
        private readonly IMultipleImplement fooMultipleImplement;

        public string BarSayHello()
        {
            return barMultipleImplement.SayHello();
        }
        public string FooSayHello()
        {
            return fooMultipleImplement.SayHello();
        }

    }
}

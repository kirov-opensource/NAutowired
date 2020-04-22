using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    [Service]
    public class ImplicitMultipleImplementService
    {
        [Autowired]
        private readonly IMultipleImplement multipleImplement;

        public string SayHello()
        {
            return multipleImplement.SayHello();
        }

    }
}

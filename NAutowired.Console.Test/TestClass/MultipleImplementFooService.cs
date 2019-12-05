using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    [Service(implementInterface: typeof(IMultipleImplement))]
    public class MultipleImplementFooService : IMultipleImplement
    {
        public string SayHello()
        {
            return nameof(MultipleImplementFooService);
        }
    }
}

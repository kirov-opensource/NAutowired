using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    [Service(implementInterface: typeof(IMultipleImplement))]
    public class MultipleImplementBarService : IMultipleImplement
    {
        public string SayHello()
        {
            return nameof(MultipleImplementBarService);
        }
    }
}

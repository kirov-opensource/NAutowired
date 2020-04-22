using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{
    [Service]
    public class ImplementMultipleInterfaceService : IBarService, IFooService
    {
    }
}

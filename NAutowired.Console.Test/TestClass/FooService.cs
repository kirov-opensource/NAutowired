using NAutowired.Core.Attributes;

namespace NAutowired.Console.Test.TestClass
{


    [Service]
    public class FooService
    {

        private readonly ConstructorResolvedService constructorResolvedService;

        public FooService(ConstructorResolvedService constructorResolvedService)
        {
            this.constructorResolvedService = constructorResolvedService;
        }

        public BarService GetBarService()
        {
            return constructorResolvedService.GetBarService();
        }

    }
}

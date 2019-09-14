using NAutowired.Core.Attributes;

namespace NAutowiredConsoleSample
{
    public class Startup : NAutowired.Core.Startup
    {
        [Autowired]
        private readonly FooService fooService;

        public override void Run(string[] args)
        {
            System.Console.WriteLine(fooService.Foo());
            System.Console.ReadLine();
        }
    }
}

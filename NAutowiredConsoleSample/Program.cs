using NAutowired.Console;
using System.Collections.Generic;

namespace NAutowiredConsoleSample
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHost.CreateDefaultBuilder(new List<string> { "NAutowiredConsoleSample" }, args).Build().Run<Startup>();
        }
    }
}

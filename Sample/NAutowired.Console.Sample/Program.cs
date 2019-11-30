using Microsoft.Extensions.DependencyInjection;
using NAutowired.Console;

namespace NAutowiredConsoleSample {
  class Program {
    static void Main(string[] args) {
      ConsoleHost.CreateDefaultBuilder(() => {
        var serviceDescriptors = new ServiceCollection();
        serviceDescriptors.AddTransient<FooService>();
        return serviceDescriptors;
      }, args).Build().Run<Startup>();
    }
  }
}

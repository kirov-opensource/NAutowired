using Microsoft.Extensions.DependencyInjection;
using NAutowired.Console.Test.TestClass;
using NAutowired.Core;
using System.Collections.Generic;
using Xunit;

namespace NAutowired.Console.Test
{
    public class UnitTest1
    {

        private readonly IConsoleHost consoleHost = ConsoleHost.CreateDefaultBuilder(() =>
        {
            var service = new ServiceCollection();
            service.AddScoped<BarService>();
            return service;
        }, new List<string> { "NAutowired.Console.Test" }).Build();


        [Fact]
        public void TestGetService()
        {
            var barService = consoleHost.GetService<BarService>();
            Assert.NotNull(barService);
            var fooService = consoleHost.GetService<FooService>();
            Assert.NotNull(fooService);
        }

        [Fact]
        public void TestAutowired()
        {
            var autowiredService = consoleHost.GetService<AutowiredService>();
            Assert.NotNull(autowiredService);
            Assert.NotNull(autowiredService.GetFooService());
            Assert.NotNull(autowiredService.GetBarService());
        }

        [Fact]
        public void TestLifetime()
        {
            var scopedLifetimeService = consoleHost.GetService<ScopedLifetimeService>();
            var singletonLifetimeService = consoleHost.GetService<SingletonLifetimeService>();
            var transientLifetimeService = consoleHost.GetService<TransientLifetimeService>();
            var lifetimeService = consoleHost.GetService<LifetimeService>();
            Assert.NotNull(scopedLifetimeService);
            Assert.NotNull(singletonLifetimeService);
            Assert.NotNull(transientLifetimeService);
            Assert.NotNull(lifetimeService);
            var transientLifetimeService2 = lifetimeService.GetTransientLifetimeService();
            var scopedLifetimeService2 = lifetimeService.GetScopedLifetimeService();
            var singletonLifetimeService2 = lifetimeService.GetSingletonLifetimeService();
            Assert.NotNull(transientLifetimeService2);
            Assert.NotNull(scopedLifetimeService2);
            Assert.NotNull(singletonLifetimeService2);
            Assert.NotEqual(transientLifetimeService, transientLifetimeService2);
            Assert.Equal(scopedLifetimeService, scopedLifetimeService2);
            Assert.Equal(singletonLifetimeService, singletonLifetimeService2);
        }
        //todo TestCircularDependence
        //todo TestBaseAutowired
    }
}

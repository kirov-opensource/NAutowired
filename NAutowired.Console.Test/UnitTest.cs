using NAutowired.Console.Test.TestClass;
using NAutowired.Core;
using NAutowired.Core.Exceptions;
using System.Collections.Generic;
using Xunit;

namespace NAutowired.Console.Test
{
    public class UnitTest
    {

        private readonly IConsoleHost consoleHost = ConsoleHost.CreateDefaultBuilder(new List<string> { "NAutowired.Console.Test" }).Build();

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

        [Fact]
        public void TestCircularDependency()
        {
            var circularDependencyFooService = consoleHost.GetService<CircularDependencyFooService>();
            Assert.NotNull(circularDependencyFooService);
            Assert.NotNull(circularDependencyFooService.GetCircularDependencyBarService());
            var circularDependencyBarService = consoleHost.GetService<CircularDependencyBarService>();
            Assert.NotNull(circularDependencyBarService);
            Assert.NotNull(circularDependencyBarService.GetCircularDependencyFooService());
        }

        [Fact]
        public void TestCircularInterfaceDependency()
        {
            var circularDependencyFooService = consoleHost.GetService<CircularDependencyBInterfaceService>();
            Assert.NotNull(circularDependencyFooService);
            Assert.NotNull(circularDependencyFooService.GetCircularDependencyAService());
            var circularDependencyBarService = consoleHost.GetService<CircularDependencyAInterfaceService>();
            Assert.NotNull(circularDependencyBarService);
            Assert.NotNull(circularDependencyBarService.GetCircularDependencyBService());
        }

        [Fact]
        public void TestBaseAutowired()
        {
            var baseService = consoleHost.GetService<BaseService>();
            Assert.NotNull(baseService);
            Assert.NotNull(baseService.GetBarService());
            Assert.NotNull(baseService.GetFooService());
        }

        /// <summary>
        /// 多实现测试
        /// </summary>
        [Fact]
        public void TestMultipleImplement()
        {
            var instanceBar = consoleHost.GetService<IMultipleImplement, MultipleImplementBarService>();
            var instanceFoo = consoleHost.GetService<IMultipleImplement, MultipleImplementFooService>();
            Assert.NotNull(instanceBar);
            Assert.NotNull(instanceFoo);
            Assert.Equal(nameof(MultipleImplementBarService), instanceBar.SayHello());
            Assert.Equal(nameof(MultipleImplementFooService), instanceFoo.SayHello());
        }

        /// <summary>
        /// 接口多实现还原测试
        /// </summary>
        [Fact]
        public void TestMultipleImplementAutowired()
        {
            var instance = consoleHost.GetService<MultipleImplementService>();
            Assert.NotNull(instance);
            Assert.Equal(nameof(MultipleImplementBarService), instance.BarSayHello());
            Assert.Equal(nameof(MultipleImplementFooService), instance.FooSayHello());
        }

        /// <summary>
        /// 单类多接口实现 还原
        /// </summary>
        [Fact]
        public void TestImplicitImplementMultipleInterface()
        {
            var service = consoleHost.GetService<ImplicitImplementMultipleInterfaceService>();
            Assert.IsType<ImplementMultipleInterfaceService>(service.GetFooService());
            Assert.IsType<ImplementMultipleInterfaceService>(service.GetBarService());
        }

        [Fact]
        public void TestBasedAbstractService()
        {
            var service = consoleHost.GetService<BasedAbstractService>();
            Assert.NotNull(service.GetFooService());
            Assert.IsType<FooService>(service.GetFooService());
        }

        //[Fact]
        //public void TestConstructorResolveService()
        //{
        //    var service = consoleHost.GetService<FooService>();
        //    Assert.NotNull(service.GetBarService());
        //    Assert.IsType<BarService>(service.GetBarService());
        //}

        /// <summary>
        /// 当接口具有多个实现时，可以注入集合
        /// </summary>
        [Fact]
        public void TestMultipleImplementCollectionResolve()
        {
            var instance = consoleHost.GetService<EnumerableMultipleImplementService>();
            Assert.NotNull(instance);

            var implements = instance.SayHello();
            Assert.Contains(implements, i => i.GetType() == typeof(MultipleImplementBarService));
            Assert.Contains(implements, i => i.GetType() == typeof(MultipleImplementFooService));
        }
    }
}

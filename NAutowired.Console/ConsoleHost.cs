using Microsoft.Extensions.DependencyInjection;
using NAutowired.Core;
using NAutowired.Core.Extensions;
using System;
using System.Collections.Generic;

namespace NAutowired.Console
{
    public static class ConsoleHost
    {
        public static IConsoleHostBuilder CreateDefaultBuilder(IServiceCollection serviceCollection)
        {
            return new DefaultConsoleHostBuilder(serviceCollection);
        }

        public static IConsoleHostBuilder CreateDefaultBuilder(IServiceCollection serviceCollection, string[] args)
        {
            return new DefaultConsoleHostBuilder(serviceCollection, args);
        }

        public static IConsoleHostBuilder CreateDefaultBuilder(Func<IServiceCollection> func)
        {
            return new DefaultConsoleHostBuilder(func.Invoke());
        }

        public static IConsoleHostBuilder CreateDefaultBuilder(Func<IServiceCollection> func, string[] args)
        {
            return new DefaultConsoleHostBuilder(func.Invoke(), args);
        }

        public static IConsoleHostBuilder CreateDefaultBuilder(Action<IServiceCollection> action)
        {
            var serviceCollection = new ServiceCollection();
            action.Invoke(serviceCollection);
            return new DefaultConsoleHostBuilder(serviceCollection);
        }

        public static IConsoleHostBuilder CreateDefaultBuilder(Action<IServiceCollection> action, string[] args)
        {
            var serviceCollection = new ServiceCollection();
            action.Invoke(serviceCollection);
            return new DefaultConsoleHostBuilder(serviceCollection, args);
        }

        public static IConsoleHostBuilder CreateDefaultBuilder(IEnumerable<string> assemblies)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AutoRegisterDependency(assemblies);
            return new DefaultConsoleHostBuilder(serviceCollection);
        }

        public static IConsoleHostBuilder CreateDefaultBuilder(IEnumerable<string> assemblies, string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AutoRegisterDependency(assemblies);
            return new DefaultConsoleHostBuilder(serviceCollection, args);
        }

        public static IConsoleHostBuilder CreateDefaultBuilder(Func<IServiceCollection> func, IEnumerable<string> assemblies)
        {
            var serviceCollection = func.Invoke();
            serviceCollection.AutoRegisterDependency(assemblies);
            return new DefaultConsoleHostBuilder(serviceCollection);
        }

        public static IConsoleHostBuilder CreateDefaultBuilder(Func<IServiceCollection> func, IEnumerable<string> assemblies, string[] args)
        {
            var serviceCollection = func.Invoke();
            serviceCollection.AutoRegisterDependency(assemblies);
            return new DefaultConsoleHostBuilder(serviceCollection, args);
        }
    }
}

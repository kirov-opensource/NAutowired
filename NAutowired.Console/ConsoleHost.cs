using NAutowired.Core;

namespace NAutowired.Console
{
    public static class ConsoleHost
    {
        public static IConsoleHostBuilder CreateDefaultBuilder()
        {
            return new DefaultConsoleHostBuilder();
        }

        public static IConsoleHostBuilder CreateDefaultBuilder(string[] args)
        {
            return new DefaultConsoleHostBuilder(args);
        }

        public static IConsoleHostBuilder CreateDefaultBuilder<TStartup>(string[] args) where TStartup : class
        {
            return new DefaultConsoleHostBuilder(typeof(TStartup), args);
        }

    }
}


# NAutowired
[![NuGet](https://img.shields.io/nuget/v/NAutowired.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/NAutowired)
[![Publish Workflow](https://github.com/kirov-opensource/NAutowired/actions/workflows/publish.yml/badge.svg)](https://github.com/kirov-opensource/NAutowired/actions/workflows/publish.yml)
[![NuGet](https://img.shields.io/nuget/dt/NAutowired?logo=nuget&style=flat-square)](https://www.nuget.org/packages/NAutowired)
[![GitHub issues](https://img.shields.io/github/issues/kirov-opensource/NAutowired.svg?style=flat-square&logo=github)](https://github.com/kirov-opensource/NAutowired/issues)
![GitHub repo size in bytes](https://img.shields.io/github/repo-size/kirov-opensource/NAutowired.svg?style=flat-square&logo=github)
![GitHub top language](https://img.shields.io/github/languages/top/kirov-opensource/NAutowired.svg?style=flat-square&logo=github)

ASP.NET CORE Field Injection

* [中文](./README_CN.md)

## Idea and positioning
* We don't make containers, we are just porters of `NetCore Container` (add some features to the default container).
* Don't use `NAutowired` in the constructor.
* We have not replaced `NetCore` default `Container`, which means you can still add services to `Container` using `IServiceCollection` in `Startup`. And use `NAutowired` to resolve dependencies.
* Spring DI style is [ugly](https://dzone.com/articles/spring-di-patterns-the-good-the-bad-and-the-ugly)([Explicit dependencies](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles#explicit-dependencies)), but who cares?

### How to use
* Introducing `NAutowired` and `NAutowired.Core` in the nuget.
* The `NAutowired` package should only be referenced in the web project, and the `NAutowired.Core` package is referenced in projects that need to add features.

### `ASP.NET Core 3.0`
* [ASP.NET Core 3 WebAPI Sample](./Sample/NAutowired.WebAPI.Sample)

### `ASP.NET Core 6.0`
* [ASP.NET Core 6 WebAPI Sample](./Sample/NAutowired.AasNetCore6.WebAPI.Sample)

By default, when `ASP.NET Core` generates `Controller`, dependencies in the `Controller` constructor are resolved from the container, but the controller is not resolved from the container, which results in:
* The lifetime of the `Controller` is handled by the framework, not the lifetime of the request
* The lifetime of parameters in the `Controller` constructor is handled by the request lifetime
* In `Controller` use `Field Injection` won’t work

You must use the `AddControllersAsServices` method to register the `Controller` as a Service so that the `Controller` can use the `Field Injection` when resolve.
Use `AddControllersAsServices` in `Startup.cs` and replace `IControllerActivator` as `NAutowiredControllerActivator`.
#### Replace the default `IControllerActivator` implementation with `NAutowiredControllerActivator` in `Startup.cs`
```csharp
public void ConfigureServices(IServiceCollection services) {
    //register controllers as services
    services.AddControllers().AddControllersAsServices();
    //replace `IControllerActivator` implement.
    services.Replace(ServiceDescriptor.Transient<IControllerActivator, NAutowiredControllerActivator>());
}
```

```csharp
// This method gets called by the runtime. Use this method to add services to the container.
public void ConfigureServices(IServiceCollection services) {
    //Add FooService to container.
    services.AddScoped<FooService>();
    //Add IBarService implements to container.
    services.AddScoped<IBarService, MyBarService1>();
    services.AddScoped<IBarService, MyBarService2>();
}
```
```csharp
  [Route("api/[controller]")]
  [ApiController]
  public class FooController : ControllerBase {

    //Use Autowired injection.
    [Autowired]
    private readonly FooService fooService;

    //Also supports IEnumerable<T> injection.
    [Autowired]
    private readonly IEnumerable<IBarService> barServices;

    [HttpGet]
    public ActionResult<string> Get() {
      return fooService == null ? "failure" : "success";
    }

    [HttpPost]
    public ActionResult<string> Baz() {
      return barServices?.Count > 0 ? "success" : "failure";
    }
  }
```
#### Use in `Filter`
```csharp
  public class Startup {
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      //Add Filter to container.
      services.AddScoped<AuthorizationFilter>();
    }
  }
```
```csharp
  //Use ServiceFilter like ASP.NET CORE ServiceFilter.
  [NAutowired.Attributes.ServiceFilter(typeof(AuthorizationFilter))]
  public class FooController : ControllerBase {

  }
```
```csharp
  public class AuthorizationFilter : IAuthorizationFilter {
    [Autowired]
    private readonly FooService fooService;

    public void OnAuthorization(AuthorizationFilterContext context) {
      System.Console.WriteLine($"{fooService.ToString()} in filter");
      return;
    }
  }
```
#### Get Configuration
```csharp
  public class Startup {
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      //add config to ioc container
      services.Configure<SnowflakeConfig>(Configuration.GetSection("Snowflake"));
    }
  }
```
```csharp
public class FooController : ControllerBase {
  //use autowired get configuration
    [Autowired]
    private IOptions<SnowflakeConfig> options { get; set; }

    [HttpGet("snowflake")]
    public IActionResult GetSnowflakeConfig()
    {
        return Ok(options.Value);
    }
}
```
`SnowflakeConfig.cs`
```csharp
public class SnowflakeConfig
{
    public int DataCenter { get; set; }

    public int Worker { get; set; }
}
```
`appsettings.json`
```json
{
  "Snowflake": {
    "DataCenter": 1,
    "Worker": 1
  }
}
```

### `NET Core` Console >= 3.0
* [Console Sample](./Sample/NAutowired.Console.Sample)
#### Create a new `Srartup.cs` file and inherit from `NAutowired.Core.Startup`
```csharp
public class Startup : NAutowired.Core.Startup
{
    [Autowired]
    private readonly FooService fooService;

    //Program start up func
    public override void Run(string[] args)
    {
        System.Console.WriteLine(fooService.Foo());
        System.Console.ReadLine();
    }
}
```
#### In `Program.cs`
```csharp
class Program
{
    static void Main(string[] args)
    {
        ConsoleHost.CreateDefaultBuilder(new List<string> {  "assemblyName" }, args).Build().Run<Startup>();
        //You can also let NAutowired use the IServiceCollection you passed
        /*
        ConsoleHost.CreateDefaultBuilder(() => {
          var serviceDescriptors = new ServiceCollection();
          serviceDescriptors.AddTransient<FooService>();
          return serviceDescriptors;
        }, new List<string> { "NAutowiredConsoleSample" }, args).Build().Run<Startup>();
        */
    }
}
```
### Unit Test
* [Unit Test Sample](./NAutowired.Console.Test)

### Advanced
#### You can inject a specific type with the `[Autowired(Type)]` method
```csharp
  [Route("api/[controller]")]
  [ApiController]
  public class FooController : ControllerBase {

    //Inject a specific instance.
    [Autowired(typeof(FooService))]
    private readonly IFooService fooService;

    [HttpGet]
    public ActionResult<string> Get() {
      return fooService == null ? "failure" : "success";
    }
  }
```
#### `NAutowired` provides the `AutoRegisterDependency(assemblyName)` method for automatic container injection. This way you don't need to add the type to the container one by one in `Startup.cs`
```csharp
  public class Startup {
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      //services.AddScoped<FooService>();
      //Use automatic injection.
      services.AutoRegisterDependency(new List<string> { "NAutowiredSample" });
    }
  }
```
#### Use the `[Service] [Repository] [Component] [ServiceFilter]` attribute tag class, these classes will be added to the container when `AutoRegisterDependency` is executed
```csharp
  //The default Lifetime value is Scoped
  [Service]
  //Lifetime to choose the life cycle of dependency injection
  //[Service(Lifetime.Singleton)]
  public class FooService {
  }

  [Service(implementInterface: typeof(IService))]
  //injection interface to container like services.AddScoped(typeof(IService), typeof(FooService));
  public class FooService: IService {
  }
```
 `NAutowired` will automatically scan all classes under the assembly configured by the `AutoRegisterDependency(assemblyName)` method, and inject the class with the `[Service] [Repository] [Component] [ServiceFilter]` property into the container.
 
 #### `NAutowired` provides `WithAutowired`、`GetServiceWithAutowired` extension methods，which can obtain services from containers and automatically resolve their `[Autowired]` dependencies. It's particularly convenient when you need to manually obtain services or resolve existing instances.
```csharp
services.AddSingleton(sp =>
{
    var foo = sp.GetServiceWithAutowired<IFooService>();
    return foo.Create();
});

```



## Stargazers over time

[![Stargazers over time](https://starchart.cc/kirov-opensource/NAutowired.svg)](https://starchart.cc/kirov-opensource/NAutowired)


# NAutowired
[![NuGet](https://img.shields.io/nuget/v/NAutowired.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/NAutowired)
[![NuGet](https://img.shields.io/nuget/dt/NAutowired?logo=nuget&style=flat-square)](https://www.nuget.org/packages/NAutowired)
[![GitHub issues](https://img.shields.io/github/issues/kirov-opensource/NAutowired.svg?style=flat-square&logo=github)](https://github.com/kirov-opensource/NAutowired/issues)
![GitHub repo size in bytes](https://img.shields.io/github/repo-size/kirov-opensource/NAutowired.svg?style=flat-square&logo=github)
![GitHub top language](https://img.shields.io/github/languages/top/kirov-opensource/NAutowired.svg?style=flat-square&logo=github)

ASP.NET CORE Field Injection

* [中文](./README.md)

## Idea and positioning
* We don't make containers, we are just porters of `NetCore Container` (add some features to the default container).
* Don't use `NAutowired` in the constructor.
* We have not replaced `NetCore` default `Container`, which means you can still add services to `Container` using `IServiceCollection` in `Startup`. And use `NAutowired` to resolve dependencies.
* Spring DI style is [ugly](https://dzone.com/articles/spring-di-patterns-the-good-the-bad-and-the-ugly), but who cares?

### How to use
* Introducing `NAutowired` and `NAutowired.Core` in the nuget.
* The `NAutowired` package should only be referenced in the web project, and the `NAutowired.Core` package is referenced in projects that need to add features.

### `ASP.NET Core 3.0`
* [WebAPI Sample](./Sample/NAutowired.WebAPI.Sample)

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
}
```
```csharp
  [Route("api/[controller]")]
  [ApiController]
  public class FooController : ControllerBase {

    //Use Autowired injection.
    [Autowired]
    private readonly FooService fooService;

    [HttpGet]
    public ActionResult<string> Get() {
      return fooService == null ? "failure" : "success";
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

### `NET Core 3.0` Console
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
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
      //services.AddScoped<FooService>();
      //Use automatic injection.
      services.AutoRegisterDependency(new List<string> { "NAutowiredSample" });
    }
  }
```
#### Use the `[Service] [Repository] [Component] [ServiceFilter]` attribute tag class
```csharp
  //The default Lifetime value is Scoped
  [Service]
  //Lifetime to choose the life cycle of dependency injection
  //[Service(Lifetime.Singleton)]
  public class FooService {
  }
```
 `NAutowired` will automatically scan all classes under the assembly configured by the `AddAutoDependencyInjection(assemblyName)` method, and inject the class with the `[Service] [Repository] [Component] [ServiceFilter]` property into the container.


# NAutowired
[![NuGet](https://img.shields.io/nuget/v/NAutowired.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/NAutowired/)

ASP.NET CORE Field Injection Implement

* [中文](./README.md)

### Why to use
`ASP.NET CORE` provided official DI framework,Need to inject through the constructor.
```csharp
  [Route("api/[controller]")]
  [ApiController]
  public class FooController : ControllerBase {

    private readonly FooService fooService;

    //Constructor
    public FooController(FooService fooService) {
      this.fooService = fooService;
    }

    [HttpGet]
    public ActionResult<string> Get() {
      return fooService == null ? "failure" : "success";
    }
  }
```
As the project grows larger, a Service class may need to inject dozens of dependencies, and the constructor is extremely bloated.
`NAutowired` implements `Field Injection`, Can be injected directly through the properties of the class.

### How to use
* [Sample](https://github.com/FatTigerWang/NAutowiredSample).
* Replace the default `IControllerActivator` implementation with `NAutowiredControllerActivator` in `Startup.cs`.

```csharp
  public class Startup {
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
      //Replace.
      //When creating the Controller,Program will find if there is a class that implements IControllerActivator in IServiceProvider. If it finds it, it will use it to construct Controller, otherwise it will use DefaultControllerActivator.
      //Reference https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1#default-service-container-replacement.
      services.AddSingleton<IControllerActivator, NAutowiredControllerActivator>();
    }
```

```csharp
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
    private FooService FooService { get; set; }

    [HttpGet]
    public ActionResult<string> Get() {
      return FooService == null ? "failure" : "success";
    }
  }
```
* Use in `Filter`.
```csharp
  public class Startup {
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
    private FooService FooService { get; set; }

    public void OnAuthorization(AuthorizationFilterContext context) {
      System.Console.WriteLine($"{FooService.ToString()} in filter");
      return;
    }
  }
```
`NAutowired` uses the DI container of ASP.NET CORE to get the instance, it just adds the way to inject dependencies, so you can still add `FooService` to the container using `services.AddScope<FooService>()`.

### Advanced
* You can inject a specific type with the `[Autowired(Type)]` method.
```csharp
  [Route("api/[controller]")]
  [ApiController]
  public class FooController : ControllerBase {

    //Inject a specific instance.
    [Autowired(typeof(FooService))]
    private IFooService FooService { get; set; }

    [HttpGet]
    public ActionResult<string> Get() {
      return FooService == null ? "failure" : "success";
    }
  }
```
* `NAutowired` provides the `AddAutoDependencyInjection(assemblyName)` method for automatic container injection. This way you don't need to add the type to the container one by one in `Startup.cs`.
```csharp
  public class Startup {
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
      //services.AddScoped<FooService>();
      //Use automatic injection.
      services.AddAutoDependencyInjection("NAutowiredSample");
    }
  }
```
Use the `[Service] [Repository] [Component] [ServiceFilter]` attribute tag class.
```csharp
  //The default DependencyInjectionModeEnum value is Scoped
  [Service]
  //DependencyInjectionModeEnum to choose the life cycle of dependency injection
  //[Service(DependencyInjectionModeEnum.Singleton)]
  public class FooService {
  }
```
`NAutowired` will automatically scan all classes under the assembly configured by the `AddAutoDependencyInjection(assemblyName)` method, and inject the class with the `[Service] [Repository] [Component] [ServiceFilter]` property into the container.

### Explanation
* Since `NAutowired` does not replace the default DI mode of `ASP.NET CORE`, you can still inject dependencies through the constructor. `NAutowired` is fully compatible with the default DI mode of `ASP.NET CORE`.
* Using `Field Injection` is an anti-pattern thing that violates the [Explicit dependencies](https://docs.microsoft.com/en-us/dotnet/standard/modern-web-apps-azure-architecture/architectural-principles#explicit-dependencies) principle of `ASP.NET CORE`.


# NAutowired [![NuGet](https://img.shields.io/nuget/v/NAutowired.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/NAutowired/)
ASP.NET CORE Field Injection Implement

* [中文](./README.md)

### Why use it?
`ASP.NET CORE` provided official DI framework,Need to inject through the constructor. For example  
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

### How to use.
* [Expanmle](https://github.com/FatTigerWang/NAutowiredSample)
* Replace the default `IControllerActivator` implementation with `NAutowiredControllerActivator` in `Startup.cs`.

```csharp
  public class Startup {
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
      //Replace
      //When creating the Controller,Program will find if there is a class that implements IControllerActivator in IServiceProvider. If it finds it, it will use it to construct Controller, otherwise it will use DefaultControllerActivator.
      //Reference https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1#default-service-container-replacement
      services.AddSingleton<IControllerActivator, NAutowiredControllerActivator>();
    }
```

```csharp
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
      //将FooService加到容器
      services.AddScoped<FooService>();
    }
```
```csharp
  [Route("api/[controller]")]
  [ApiController]
  public class FooController : ControllerBase {

    //使用Autowired Attribute注入实例
    [Autowired]
    private FooService FooService { get; set; }

    [HttpGet]
    public ActionResult<string> Get() {
      return FooService == null ? "failure" : "success";
    }
  }
```
* 在`Filter`中使用`NAutowired`
```csharp
  public class Startup {
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
      //将Filter加到容器
      services.AddScoped<AuthorizationFilter>();
    }
  }
```
```csharp
  //使用 ServiceFilterAttribute
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


`NAutowired`使用ASP.NET CORE自带的DI容器获取实例, 它解决的仅仅是注入依赖的方式, 因此您依旧可以使用`services.AddScope<FooService>()`方式将`FooService`加入到容器.
### 进阶
* 您可以通过`[Autowired(Type)]`方式注入特定的类型.
```csharp
  [Route("api/[controller]")]
  [ApiController]
  public class FooController : ControllerBase {

    //注入特定的实例
    [Autowired(typeof(FooService))]
    private IFooService FooService { get; set; }

    [HttpGet]
    public ActionResult<string> Get() {
      return FooService == null ? "failure" : "success";
    }
  }
```
* `NAutowired`提供了`AddAutoDependencyInjection(assemblyName)`方法进行自动容器注入.这种方式让您无需在`Startup.cs`中一个个的将类型加入到容器.
```csharp
  public class Startup {
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
      //services.AddScoped<FooService>();
      //使用自动注入
      services.AddAutoDependencyInjection("NAutowiredSample");
    }
  }
```
使用`[Service] [Repository] [Component] [ServiceFilter]`特性标记类
```csharp
  //默认DependencyInjectionModeEnum值为Scoped
  [Service]
  //DependencyInjectionModeEnum可供选择依赖注入的生命周期
  //[Service(DependencyInjectionModeEnum.Singleton)]
  public class FooService {
  }
```
`NAutowired`会自动扫描`AddAutoDependencyInjection(assemblyName)`方法配置的程序集下的所有类, 并将具有`[Service] [Repository] [Component] [ServiceFilter]`特性的类注入到容器.

### 说明
* 由于`NAutowired`并没有替换`ASP.NET CORE`默认的DI方式, 所以您依然可以通过构造函数注入依赖, `NAutowired`与`ASP.NET CORE`默认的DI方式完全兼容.
* 使用`Field Injection`是一个反模式的东西, 它违反了`ASP.NET CORE`的[显式依赖](https://docs.microsoft.com/zh-cn/dotnet/standard/modern-web-apps-azure-architecture/architectural-principles#explicit-dependencies)原则.

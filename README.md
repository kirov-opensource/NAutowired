# NAutowired 
[![NuGet](https://img.shields.io/nuget/v/NAutowired.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/NAutowired)
[![NuGet](https://img.shields.io/nuget/dt/NAutowired?logo=nuget&style=flat-square)](https://www.nuget.org/packages/NAutowired)
[![GitHub issues](https://img.shields.io/github/issues/kirov-opensource/NAutowired.svg?style=flat-square&logo=github)](https://github.com/kirov-opensource/NAutowired/issues)
![GitHub repo size in bytes](https://img.shields.io/github/repo-size/kirov-opensource/NAutowired.svg?style=flat-square&logo=github)
![GitHub top language](https://img.shields.io/github/languages/top/kirov-opensource/NAutowired.svg?style=flat-square&logo=github)

ASP.NET Core 通过属性注入依赖
* [English](./README_EN.md)

## 理念与定位
* 我们不做容器，我们只是`NET Core Container`的搬运工（在默认容器的基础上增加了一些功能）。
* 不要在构造函数中使用`NAutowired`。
* 由于我们与那些`妖艳的`第三方`IoC Container`有些不同，我们没有替换`NetCore`默认的`Container`，这意味着您依然可以在`Startup`里使用`IServiceCollection`将服务加入到`Container`并使用`NAutowired`还原这些依赖。
* 虽然有人觉得Spring风格的DI有点[反模式](https://dzone.com/articles/spring-di-patterns-the-good-the-bad-and-the-ugly)（[显式依赖](https://docs.microsoft.com/zh-cn/dotnet/architecture/modern-web-apps-azure/architectural-principles#explicit-dependencies)），但是写起来爽。

## 如何使用
* `nuget`包管理器中引入`NAutowired`和`NAutowired.Core`。
* `NAutowired`包应该只在Web或Console项目中被引用，`NAutowired.Core`包则在需要添加特性的项目中被引用。

### `ASP.NET Core 3.0`
* [WebAPI 样例](./Sample/NAutowired.WebAPI.Sample)

默认情况下，`ASP.NET Core`生成`Controller`时从容器中解析`Controller`构造函数中的依赖，但是不从容器中还原控制器，这导致：
* `Controller`的生命周期由框架处理，而不是请求的生命周期
* `Controller`构造函数中参数的生命周期由请求生命周期处理
* 在`Controller`中通过`属性注入`将不起作用

您必须使用`AddControllersAsServices`方法，将`Controller`注册为`Service`，以便`Controller`在还原时使用`属性注入`。

#### 在`Startup.cs`中使用`AddControllersAsServices`并替换`IControllerActivator`实现为`NAutowiredControllerActivator`

```csharp
public void ConfigureServices(IServiceCollection services) {
    //register controllers as services
    services.AddControllers().AddControllersAsServices();
    //replace `IControllerActivator` implement.
    services.Replace(ServiceDescriptor.Transient<IControllerActivator, NAutowiredControllerActivator>());
}
```

#### 使用`Autowired`
```csharp
  public class Startup {
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      //将FooService加到容器
      services.AddScoped<FooService>();
    }
  }
```
```csharp
  [Route("api/[controller]")]
  [ApiController]
  public class FooController : ControllerBase {

    //使用Autowired Attribute注入实例
    [Autowired]
    private readonly FooService fooService;

    [HttpGet]
    public ActionResult<string> Get() {
      return fooService == null ? "failure" : "success";
    }
  }
```
#### 在`Filter`中使用`NAutowired`
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
    private readonly FooService fooService;

    public void OnAuthorization(AuthorizationFilterContext context) {
      System.Console.WriteLine($"{fooService.ToString()} in filter");
      return;
    }
  }
```

### `NET Core 3.0` Console
* [Console 样例](./Sample/NAutowired.Console.Sample)
#### 新建`Srartup.cs`文件，并且继承自`NAutowired.Core.Startup`
```csharp
public class Startup : NAutowired.Core.Startup
{
    [Autowired]
    private readonly FooService fooService;

    //程序启动时将会执行此方法
    public override void Run(string[] args)
    {
        System.Console.WriteLine(fooService.Foo());
        System.Console.ReadLine();
    }
}
```
#### 在`Program.cs`中
```csharp
class Program
{
    static void Main(string[] args)
    {
        ConsoleHost.CreateDefaultBuilder(new List<string> {  "assemblyName" }, args).Build().Run<Startup>();
        //你也可以让NAutowired使用你传递的IServiceCollection
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
### 单元测试
* [单元测试 样例](./NAutowired.Console.Test)

## 进阶
#### 您可以通过`[Autowired(Type)]`方式注入特定的类型
```csharp
  [Route("api/[controller]")]
  [ApiController]
  public class FooController : ControllerBase {

    //注入特定的实例
    [Autowired(typeof(FooService))]
    private readonly IFooService fooService;

    [HttpGet]
    public ActionResult<string> Get() {
      return fooService == null ? "failure" : "success";
    }
  }
```
#### `NAutowired`提供了`AutoRegisterDependency(assemblyNames)`方法进行自动容器注入.这种方式让您无需在`Startup.cs`中一个个的将类型加入到容器
```csharp
  public class Startup {
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      //使用自动注入
      services.AutoRegisterDependency(new List<string> { "NAutowiredSample" });
    }
  }
```
#### 使用`[Service] [Repository] [Component] [ServiceFilter]`特性标记类
```csharp
  //默认Lifetime值为Scoped
  [Service]
  //Lifetime可供选择依赖注入的生命周期
  //[Service(Lifetime.Singleton)]
  public class FooService {
  }
```
`NAutowired`会自动扫描`AutoRegisterDependency(assemblyNames)`方法配置的程序集下的所有类，并将具有`[Service] [Repository] [Component] [ServiceFilter]`特性的类注入到容器。

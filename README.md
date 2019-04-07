# NAutowired
Lightweight ASP.NET CORE DI library
### 为什么要使用`NAutowired`
`ASP.NET CORE`自带的DI框架，需要通过构造函数进行注入，例如  
```csharp
  [Route("api/[controller]")]
  [ApiController]
  public class FooController : ControllerBase {

    private readonly FooService fooService;

    //构造函数注入方式
    public FooController(FooService fooService) {
      this.fooService = fooService;
    }

    [HttpGet]
    public ActionResult<string> Get() {
      return fooService == null ? "failure" : "success";
    }
  }
```
这种方式当项目越来越大的情况下, 一个Service类中可能需要注入数十个依赖, 这时构造函数就显得极为`ugly`.
且当所有Service都继承某个BaseService时, BaseService增加一个依赖所有Service都需要传递依赖给BaseService, 这时如果你有几十个Service继承BaseService, 则改动是巨大的.

### 如何使用`NAutowired`
* 在`Startup.cs`中替换默认的`IControllerActivator`实现为`NAutowiredControllerActivator`.

```csharp
  public class Startup {
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
      //替换默认的`IControllerActivator`实现.
      services.AddSingleton<IControllerActivator, NAutowiredControllerActivator>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      } else {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseMvc();
    }
  }
```

* 使用`Autowried`
```csharp
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
      //将FooService加到容器
      services.AddScoped<FooService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      } else {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseMvc();
    }
  }
```
```csharp
  [Route("api/[controller]")]
  [ApiController]
  public class FooController : ControllerBase {

    //使用Autowried Attribute注入实例
    [Autowried]
    private FooService FooService { get; set; }

    [HttpGet]
    public ActionResult<string> Get() {
      return FooService == null ? "failure" : "success";
    }
  }
```

`NAutowired`使用ASP.NET CORE自带的DI容器获取实例, 它解决的仅仅是注入依赖的方式, 因此您依旧可以使用`services.AddScope<FooService>()`方式将`FooService`加入到容器.
### 进阶
* 您可以通过`[Autowried(Type)]`方式注入特定的类型.
```csharp
  [Route("api/[controller]")]
  [ApiController]
  public class FooController : ControllerBase {

    //注入特定的实例
    [Autowried(typeof(FooService))]
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
    public Startup(IConfiguration configuration) {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
      //services.AddScoped<FooService>();
      //使用自动注入
      services.AddAutoDependencyInjection("NAutowiredSample");
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
      if (env.IsDevelopment()) {
        app.UseDeveloperExceptionPage();
      } else {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseMvc();
    }
  }
```
使用`[Service] [Repository] [Component]`特性标记类
```csharp
  //默认DependencyInjectionModeEnum值为Scoped
  [Service]
  //DependencyInjectionModeEnum可供选择依赖注入的生命周期
  //[Service(DependencyInjectionModeEnum.Singleton)]
  public class FooService {
  }
```
`NAutowired`会自动扫描`AddAutoDependencyInjection(assemblyName)`方法配置的程序集下的所有类, 并将具有`[Service] [Repository] [Component]`特性的类注入到容器.

### 说明
* 由于`NAutowired`并没有替换`ASP.NET CORE`默认的DI方式, 所以您依然可以通过构造函数注入依赖, `NAutowired`与`ASP.NET CORE`默认的DI方式完全兼容.

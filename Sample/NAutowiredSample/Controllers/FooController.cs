using Microsoft.AspNetCore.Mvc;
using NAutowired.Core.Attributes;
using NAutowiredSample.Filters;
using NAutowiredSample.Service;

namespace NAutowiredSample.Controllers {
  [Route("api/[controller]")]
  [ApiController]
  [NAutowired.Attributes.ServiceFilter(typeof(AuthorizationFilter))]
  public class FooController : ControllerBase {

    [Autowired(typeof(FooService))]
    private IFooService FooService { get; set; }

    [HttpGet("")]
    public IActionResult Get() {
      System.Console.WriteLine($"{FooService.ToString()} in controller");
      return Ok(FooService.GetFoo());
    }
  }
}
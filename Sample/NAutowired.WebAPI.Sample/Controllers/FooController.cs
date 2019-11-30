using Microsoft.AspNetCore.Mvc;
using NAutowired.Core.Attributes;
using NAutowired.WebAPI.Sample.Filters;
using NAutowired.WebAPI.Sample.Service;

namespace NAutowired.WebAPI.Sample.Controllers {
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
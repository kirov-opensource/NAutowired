using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NAutowired.AspNetCore6.WebAPI.Sample.Config;
using NAutowired.AspNetCore6.WebAPI.Sample.Filters;
using NAutowired.AspNetCore6.WebAPI.Sample.Service;
using NAutowired.Core.Attributes;

namespace NAutowired.AspNetCore6.WebAPI.Sample.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [NAutowired.Attributes.ServiceFilter(typeof(AuthorizationFilter))]
    public class FooController : ControllerBase {

        [Autowired(typeof(FooService))]
        private IFooService FooService { get; set; }

        [Autowired]
        private IOptions<SnowflakeConfig> options { get; set; }


        [HttpGet("")]
        public IActionResult Get() {
            System.Console.WriteLine($"{FooService.ToString()} in controller");
            return Ok(FooService.GetFoo());
        }

        [HttpGet("snowflake")]
        public IActionResult GetSnowflakeConfig()
        {
            return Ok(options.Value);
        }
    }
}
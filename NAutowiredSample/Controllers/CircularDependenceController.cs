using Microsoft.AspNetCore.Mvc;
using NAutowired.Core.Attributes;
using NAutowiredSample.Service;

namespace NAutowiredSample.Controllers {
  [Route("api/[controller]")]
  [ApiController]
  public class CircularDependenceController : ControllerBase {

    [Autowired]
    private CircularDependenceAService CyclicDependenceAService { get; set; }

    [Autowired]
    private CircularDependenceBService CyclicDependenceBService { get; set; }

    [HttpGet("")]
    public IActionResult Get() {
      System.Console.WriteLine($"{CyclicDependenceAService.ToString()} in controller");
      System.Console.WriteLine($"{CyclicDependenceBService.ToString()} in controller");
      return Ok($"{CyclicDependenceAService.GetCyclicDependenceA()}-{CyclicDependenceBService.GetCyclicDependenceB()}");
    }
  }
}
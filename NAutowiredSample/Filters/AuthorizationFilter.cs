using Microsoft.AspNetCore.Mvc.Filters;
using NAutowired.Core.Attributes;
using NAutowiredSample.Service;

namespace NAutowiredSample.Filters {
  [Filter]
  public class AuthorizationFilter : IAuthorizationFilter {

    [Autowired]
    private FooService FooService { get; set; }

    public void OnAuthorization(AuthorizationFilterContext context) {
      System.Console.WriteLine($"{FooService.ToString()} in filter");
      return;
    }
  }
}

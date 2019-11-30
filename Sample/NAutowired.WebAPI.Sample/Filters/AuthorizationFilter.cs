using Microsoft.AspNetCore.Mvc.Filters;
using NAutowired.Core.Attributes;
using NAutowired.WebAPI.Sample.Service;

namespace NAutowired.WebAPI.Sample.Filters {
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

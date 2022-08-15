using Microsoft.AspNetCore.Mvc.Filters;
using NAutowired.Core.Attributes;
using NAutowired.AspNetCore6.WebAPI.Sample.Service;

namespace NAutowired.AspNetCore6.WebAPI.Sample.Filters {
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

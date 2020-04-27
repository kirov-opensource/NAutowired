using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NAutowired.Core.Extensions;
using NAutowired.WebAPI.Sample.Config;
using System.Collections.Generic;

namespace NAutowired.WebAPI.Sample {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            //register controllers as services
            services.AddControllers().AddControllersAsServices();
            //replace `IControllerActivator` implement.
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, NAutowiredControllerActivator>());
            //add config to ioc container
            services.Configure<SnowflakeConfig>(Configuration.GetSection("Snowflake"));
            //use auto dependency injection
            services.AutoRegisterDependency(new List<string> { "NAutowired.WebAPI.Sample" });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}

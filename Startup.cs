using FastData.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FastApiGatewayDb.Ui.Filter;
using FastUntility.Core.Base;

namespace FastApiGatewayDb.Ui
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            FastMap.InstanceProperties("FastApiGatewayDb.DataModel", "FastApiGatewayDb.Ui.dll");
            FastMap.InstanceMap();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                //全局过滤器
                options.Filters.Add(new ErrorAttribute());
                options.Filters.Add(new PowerAttribute());
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");
            });            
        }
    }
}

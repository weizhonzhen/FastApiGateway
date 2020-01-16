using FastData.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FastApiGatewayDb.Ui.Mvc.Filter;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace FastApiGatewayDb.Ui.Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            FastMap.InstanceProperties("FastApiGatewayDb.DataModel", "FastApiGatewayDb.Ui.Mvc.dll");
            FastMap.InstanceTable("FastApiGatewayDb.DataModel", "FastApiGatewayDb.Ui.Mvc.dll");
            FastMap.InstanceMap();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
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
                     name: "Default",
                     template: "{controller}/{action}/{id?}",
                     defaults: new { controller = "Home", action = "Index" }
                 );
            });            
        }
    }
}

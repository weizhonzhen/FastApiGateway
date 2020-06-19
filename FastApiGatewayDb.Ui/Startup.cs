using FastData.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FastApiGatewayDb.Ui.Filter;
using Microsoft.AspNetCore.Diagnostics;
using FastUntility.Core.Base;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using FastData.Core.Repository;

namespace FastApiGatewayDb.Ui
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        { 
            Configuration = configuration;
            FastMap.InstanceProperties("FastApiGatewayDb.DataModel", "FastApiGatewayDb.Ui.dll");
            //FastMap.InstanceTable("FastApiGatewayDb.DataModel", "FastApiGatewayDb.Ui.dll");
            FastMap.InstanceMap();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IFastRepository, FastRepository>();
            services.AddMvc(options =>
            {
                options.Filters.Add(new PowerAttribute());
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler(error =>
            {
                error.Use(async (context, next) =>
                {
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        BaseLog.SaveLog(contextFeature.Error.Message, "error");
                        context.Response.ContentType = "application/json;charset=utf-8";
                        context.Response.StatusCode = 200;
                        var result = new Dictionary<string, object>();
                        result.Add("success", false);
                        result.Add("msg", contextFeature.Error.Message);
                        await context.Response.WriteAsync(BaseJson.ModelToJson(result));
                    }
                });
            });
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=login}/{id?}");
            });            
        }
    }
}

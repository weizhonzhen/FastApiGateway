using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Api.Gateway;

namespace ApiGateway
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {   
            //压缩
            services.AddResponseCompression();

            //注入api网关
            services.AddTransient<IResponse, ApiResponse>();

            //跨域
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });            
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //api网关
            app.UseMiddleware<ApiMiddleware>();
        }
    }
}

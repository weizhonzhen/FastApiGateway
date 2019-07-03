using FastApiGatewayDb;
using FastData.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace TestFastApiGateway.Db
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {            
            //注册gbk
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.GetEncoding("GB2312");
            
            //http请求
            services.AddHttpClient();

            //压缩
            services.AddResponseCompression();

            //注入
            services.AddTransient<IFastApiGatewayDb, FastApiGatewayDb.FastApiGatewayDb>();

            //跨域
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });

            //oracle
            FastMap.InstanceProperties("FastApiGatewayDb.DataModel.Oracle", "FastApiGatewayDb.dll");
            FastMap.InstanceTable("FastApiGatewayDb.DataModel.Oracle", "FastApiGatewayDb.dll");
            
            //mysql
           // FastMap.InstanceProperties("FastApiGatewayDb.DataModel.MySql", "FastApiGatewayDb.dll");
           // FastMap.InstanceTable("FastApiGatewayDb.DataModel.MySql", "FastApiGatewayDb.dll");
            
            //sqlserver
           // FastMap.InstanceProperties("FastApiGatewayDb.DataModel.SqlServer", "FastApiGatewayDb.dll");
           // FastMap.InstanceTable("FastApiGatewayDb.DataModel.SqlServer", "FastApiGatewayDb.dll");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<FastApiGatewayDbHandler>();

        }
    }
}

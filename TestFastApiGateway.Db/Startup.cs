using FastApiGatewayDb.DataModel.Oracle;
using FastData.Core;
using FastData.Core.Context;
using FastData.Core.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
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
            using (var db = new DataContext("ApiGateway"))
            {
                var list = FastRead.Query<ApiGatewayDownParam>(a => a.Protocol != "", 
                    a => new { a.Key, a.Url }).ToList<ApiGatewayDownParam>(db);
                foreach (var item in list)
                {
                    services.AddHttpClient(item.Key, client =>
                    {
                        client.BaseAddress = new Uri(item.Url);
                    }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
                        AllowAutoRedirect = false,
                        AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
                    });
                }
            }

            //压缩
            services.AddResponseCompression();

            //注入
            services.AddTransient<IFastApiGatewayDb, FastApiGatewayDb.FastApiGatewayDb>();            
            services.AddTransient<IFastRepository, FastRepository>();

            //跨域
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });

            //oracle
            FastMap.InstanceProperties("FastApiGatewayDb.DataModel.Oracle", "FastApiGatewayDb.dll");
            FastMap.InstanceTable("FastApiGatewayDb.DataModel.Oracle", "FastApiGatewayDb.dll");

            //MySql
            //FastMap.InstanceProperties("FastApiGatewayDb.DataModel.MySql", "FastApiGatewayDb.dll");
            //FastMap.InstanceTable("FastApiGatewayDb.DataModel.MySql", "FastApiGatewayDb.dll");

            //SqlServer
            //FastMap.InstanceProperties("FastApiGatewayDb.DataModel.SqlServer", "FastApiGatewayDb.dll");
            //FastMap.InstanceTable("FastApiGatewayDb.DataModel.SqlServer", "FastApiGatewayDb.dll");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<FastApiGatewayDbHandler>();

        }
    }
}

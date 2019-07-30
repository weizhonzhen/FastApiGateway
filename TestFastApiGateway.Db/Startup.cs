using FastApiGatewayDb;
using FastApiGatewayDb.DataModel;
using FastData.Core;
using FastData.Core.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
                var list = FastRead.Query<ApiGatewayDownParam>(a => a.Protocol.ToUpper() == "HTTP", a => new { a.Key, a.Url }).ToList<ApiGatewayDownParam>(db);
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

            //跨域
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });

            FastMap.InstanceProperties("FastApiGatewayDb.DataModel", "FastApiGatewayDb.dll");
            //FastMap.InstanceTable("FastApiGatewayDb.DataModel", "FastApiGatewayDb.dll");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMiddleware<FastApiGatewayDbHandler>();

        }
    }
}

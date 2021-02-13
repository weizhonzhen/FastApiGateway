using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace FastApiGatewayDb.Web
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.GetEncoding("GB2312");

            services.AddResponseCompression();

            services.AddFastApiGatewayDb(new ConfigData
            {
                dbFile = "db.json",
                dbKey = "ApiGateway",
                IsResource = false,
                IsCodeFirst = true,
                NamespaceCodeFirst = "FastApiGatewayDb.DataModel.Oracle",
                //NamespaceCodeFirst = "FastApiGatewayDb.DataModel.MySql",
                //NamespaceCodeFirst = "FastApiGatewayDb.DataModel.SqlServer",
                NamespaceProperties = "FastApiGatewayDb.DataModel.Oracle"
            });

            services.AddCors(options =>
            {
                options.AddPolicy("any", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseFastApiGatewayMiddleware();
        }
    }
}

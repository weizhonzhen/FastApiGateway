using FastApiGatewayDb;
using FastApiGatewayDb.DataModel.Oracle;
using FastData.Core;
using FastData.Core.Context;
using FastUntility.Core;
using Microsoft.AspNetCore.Builder;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FastApiGatewayDbExtension
    {
        public static IServiceCollection AddFastApiGatewayDb(this IServiceCollection serviceCollection, ConfigData config)
        {
            if (string.IsNullOrEmpty(config.dbKey))
                throw new Exception("ConfigData dbKey is not null");

            serviceCollection.AddFastData(config);

            serviceCollection.AddTransient<IFastApiGatewayDb, FastApiGatewayDb.FastApiGatewayDb>();
            ServiceContext.Init(new ServiceEngine(serviceCollection.BuildServiceProvider()));

            using (var db = new DataContext(config.dbKey))
            {
                var list = FastRead.Query<ApiGatewayDownParam>(a => a.Protocol != "", a => new { a.Key, a.Url }).ToList<ApiGatewayDownParam>(db);
                foreach (var item in list)
                {
                    serviceCollection.AddHttpClient(item.Key, client =>
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

            return serviceCollection;
        }

        public static IApplicationBuilder UseFastApiGatewayMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<FastApiGatewayDbHandler>();
        }
    }
}

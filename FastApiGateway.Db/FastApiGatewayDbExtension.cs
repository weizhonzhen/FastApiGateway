using FastApiGatewayDb;
using FastApiGatewayDb.Aop;
using FastApiGatewayDb.DataModel.Oracle;
using FastData.Core;
using FastData.Core.Context;
using FastData.Core.Model;
using FastUntility.Core;
using Microsoft.AspNetCore.Builder;
using System;
using System.Net;
using System.Net.Http;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FastApiGatewayDbExtension
    {
        public static IServiceCollection AddFastApiGatewayDb(this IServiceCollection serviceCollection, Action<ConfigData> Action, Action<FastRabbitMQ.Core.ConfigData> MqAction=null, IFastApiAop aop = null)
        {
            var config = new ConfigData();
            Action(config);

            if (string.IsNullOrEmpty(config.DbKey))
                throw new Exception("ConfigData dbKey is not null");

            serviceCollection.AddFastData(a=> {
                a.DbFile = config.DbFile;
                a.DbKey = config.DbKey;
                a.IsCodeFirst = config.IsCodeFirst;
                a.IsResource = config.IsResource;
                a.MapFile = config.MapFile;
                a.NamespaceCodeFirst = config.NamespaceCodeFirst;
                a.NamespaceProperties = config.NamespaceProperties;           
            });

            if (MqAction != null)
            {
                var mqConfig = new FastRabbitMQ.Core.ConfigData();
                MqAction(mqConfig);
                serviceCollection.AddFastRabbitMQ(a =>
                {
                    a.Host = mqConfig.Host;
                    a.PassWord = mqConfig.PassWord;
                    a.UserName = mqConfig.UserName;
                    a.Port = mqConfig.Port;
                    a.VirtualHost = mqConfig.VirtualHost;
                    a.aop = mqConfig.aop;
                });
            }

            serviceCollection.AddSingleton<IFastApiGatewayDb, FastApiGatewayDb.FastApiGatewayDb>();

            if (aop != null)
                serviceCollection.AddSingleton<IFastApiAop>(aop);

            ServiceContext.Init(new ServiceEngine(serviceCollection.BuildServiceProvider()));

            using (var db = new DataContext(config.DbKey))
            {
                var list = FastRead.Query<ApiGatewayDownParam>(a => a.Protocol != "", a => new { a.Key, a.Url }).ToList<ApiGatewayDownParam>(db);
                foreach (var item in list)
                {
                    serviceCollection.AddHttpClient(item.Key).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
                        AllowAutoRedirect = false,
                        AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
                    });
                }
            }

            return serviceCollection;
        }

        public static IApplicationBuilder UseFastApiGatewayMiddleware(this IApplicationBuilder app, Action<ConfigOption> optionsAction)
        {
            var options = new ConfigOption();
            optionsAction(options);
            return app.UseMiddleware<FastApiGatewayDbHandler>(options);
        }
    }
}

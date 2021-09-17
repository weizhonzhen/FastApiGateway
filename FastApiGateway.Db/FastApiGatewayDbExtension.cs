﻿using FastApiGatewayDb;
using FastApiGatewayDb.Aop;
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
        public static IServiceCollection AddFastApiGatewayDb(this IServiceCollection serviceCollection, Action<ConfigData> Action, IFastApiAop aop = null)
        {
            var config = new ConfigData();
            Action(config);

            if (string.IsNullOrEmpty(config.dbKey))
                throw new Exception("ConfigData dbKey is not null");

            serviceCollection.AddFastData(a=> {
                a.dbFile = config.dbFile;
                a.dbKey = config.dbKey;
                a.IsCodeFirst = config.IsCodeFirst;
                a.IsResource = config.IsResource;
                a.mapFile = config.mapFile;
                a.NamespaceCodeFirst = config.NamespaceCodeFirst;
                a.NamespaceProperties = config.NamespaceProperties;           
            });

            serviceCollection.AddTransient<IFastApiGatewayDb, FastApiGatewayDb.FastApiGatewayDb>();

            if (aop != null)
                serviceCollection.AddSingleton<IFastApiAop>(aop);

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

        public static IApplicationBuilder UseFastApiGatewayMiddleware(this IApplicationBuilder app, Action<ConfigOption> optionsAction)
        {
            var options = new ConfigOption();
            optionsAction(options);
            return app.UseMiddleware<FastApiGatewayDbHandler>(options);
        }
    }

    public class ConfigOption
    {
        public string dbKey { get; set; }
    }
}

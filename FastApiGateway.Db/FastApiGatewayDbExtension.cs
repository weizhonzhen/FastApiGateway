using FastApiGatewayDb;
using FastUntility.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FastApiGatewayDbExtension
    {
        public static IServiceCollection AddFastApiGatewayDb(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IFastApiGatewayDb, FastApiGatewayDb.FastApiGatewayDb>();
            ServiceContext.Init(new ServiceEngine(serviceCollection.BuildServiceProvider()));
            return serviceCollection;
        }
    }
}

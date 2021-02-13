using FastData.Core.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Threading.Tasks;

namespace FastApiGatewayDb
{
    public class FastApiGatewayDbHandler
    {
        private readonly ConfigOption option;

        public FastApiGatewayDbHandler(RequestDelegate request, ConfigOption _option)
        {
            option = _option;
        }

        public async Task Invoke(HttpContext context, IFastApiGatewayDb response, IHttpClientFactory client, IFastRepository IFast)
        {
            await response.ContentAsync(context, client,IFast, option).ConfigureAwait(false);
        }
    }
}
 

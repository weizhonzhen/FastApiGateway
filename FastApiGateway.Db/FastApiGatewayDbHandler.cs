using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace FastApiGatewayDb
{
    public class FastApiGatewayDbHandler
    {
        public FastApiGatewayDbHandler(RequestDelegate request) { }

        public async Task Invoke(HttpContext context, IFastApiGatewayDb response, IHttpClientFactory client)
        {
            await response.ContentAsync(context, client).ConfigureAwait(false);
        }
    }
}
 

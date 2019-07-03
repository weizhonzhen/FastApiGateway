using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace FastApiGatewayDb
{
    public class FastApiGatewayDbHandler
    {
        public FastApiGatewayDbHandler(RequestDelegate request) { }

        public Task InvokeAsync(HttpContext context, IFastApiGatewayDb response, IHttpClientFactory client)
        {
            return response.ContentAsync(context, client);
        }
    }
}
 

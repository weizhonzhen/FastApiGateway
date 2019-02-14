using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FastApiGatewayDb
{
    public class FastApiGatewayDbHandler
    {
        public FastApiGatewayDbHandler(RequestDelegate request) { }

        public Task InvokeAsync(HttpContext context, IFastApiGatewayDb response)
        {
            return response.ContentAsync(context);
        }
    }
}
 
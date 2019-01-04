using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FastApiGatewayDb
{
    public class FastApiGatewayDbHandler
    {
        public FastApiGatewayDbHandler(RequestDelegate request) { }

        public Task InvokeAsync(HttpContext context, IFastApiGatewayDb response)
        {
            response.Content(context);
            return Task.CompletedTask;
        }
    }
}
 
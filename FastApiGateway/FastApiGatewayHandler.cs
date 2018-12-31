using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FastApiGateway
{
    public class FastApiGatewayHandler
    {
        public FastApiGatewayHandler(RequestDelegate request) { }

        public Task InvokeAsync(HttpContext context, IFastApiGateway response)
        {
            response.Content(context);
            return Task.CompletedTask;
        }
    }
}
 
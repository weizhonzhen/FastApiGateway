using Microsoft.AspNetCore.Http;

namespace FastApiGatewayDb
{
    public interface IFastApiGatewayDb
    {
        void Content(HttpContext content);
    }
}

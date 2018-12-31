using Microsoft.AspNetCore.Http;

namespace FastApiGateway
{
    public interface IFastApiGateway
    {
        void Content(HttpContext content);
    }
}

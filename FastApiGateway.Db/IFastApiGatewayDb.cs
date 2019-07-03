using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace FastApiGatewayDb
{
    public interface IFastApiGatewayDb
    {
        Task ContentAsync(HttpContext content, IHttpClientFactory client);
    }
}

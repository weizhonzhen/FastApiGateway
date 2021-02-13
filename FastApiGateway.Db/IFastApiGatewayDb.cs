using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;
using FastData.Core.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace FastApiGatewayDb
{
    public interface IFastApiGatewayDb
    {
        Task ContentAsync(HttpContext content, IHttpClientFactory client,IFastRepository IFast, ConfigOption option);
    }
}

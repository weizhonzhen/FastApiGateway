using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fast.ApiGateway
{
    public interface IResponse
    {
        void Content(HttpContext content);
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastApiGateway
{
    public interface IResponse
    {
        void Content(HttpContext content);
    }
}

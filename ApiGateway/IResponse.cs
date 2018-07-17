using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Gateway
{
    public interface IResponse
    {
        void Content(HttpContext content);
    }
}

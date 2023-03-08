using System;

namespace FastApiGatewayDbClient
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ExcludeAttribute : Attribute
    {
    }
}

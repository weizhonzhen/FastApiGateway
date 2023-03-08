using System;

namespace FastApiGatewayDbClient.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ExcludeAttribute : Attribute
    {
    }
}

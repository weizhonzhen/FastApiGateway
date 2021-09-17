using FastApiGatewayDb.Model;
using FastUntility.Core;

namespace FastApiGatewayDb.Aop
{
    internal static class BaseAop
    {
        public static void After(string url,string key, ref string param, string protocol, ReturnModel data)
        {
            var aop = ServiceContext.Engine.Resolve<IFastApiAop>();
            if (aop != null)
            {
                var context = new AfterContext();
                context.data = data;
                context.key = key;
                context.protocol = protocol;
                context.url = url;
                aop.After(context);
            }
        }

        public static void Before(string url, string key,ref string param, string protocol)
        {
            var aop = ServiceContext.Engine.Resolve<IFastApiAop>();
            if (aop != null)
            {
                var context = new BeforeContext();
                context.key = key;
                context.protocol = protocol;
                context.url = url;
                aop.Before(context);
                param = context.param;
            }
        }
    }
}

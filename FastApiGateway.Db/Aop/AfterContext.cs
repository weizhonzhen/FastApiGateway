using FastApiGatewayDb.Model;

namespace FastApiGatewayDb.Aop
{
    public class AfterContext
    {
        public string key { get; internal set; }

        public string url { get; internal set; }

        public string protocol { get; internal set; }

        public string param { get; internal set; }

        public ReturnModel data { get; internal set; }
    }
}

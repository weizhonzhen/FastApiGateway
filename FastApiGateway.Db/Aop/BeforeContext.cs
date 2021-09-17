namespace FastApiGatewayDb.Aop
{
    public class BeforeContext
    {
        public string key { get;  internal set; }

        public string url { get; internal set; }

        public string protocol { get; internal set; }

        public string param { get; set; }
    }
}

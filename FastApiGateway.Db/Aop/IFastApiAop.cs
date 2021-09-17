namespace FastApiGatewayDb.Aop
{
    public interface IFastApiAop
    {
        void After(AfterContext context);

        void Before(BeforeContext context);
    }
}

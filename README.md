
 ![](https://raw.githubusercontent.com/weizhonzhen/FastApiGateway/master/img.png)
 
 nuget url: https://www.nuget.org/packages/Fast.ApiGateway/
 ```csharp
//注入
   services.AddFastApiGatewayDb(new ConfigData
   {
        dbFile = "db.json",
        dbKey = "ApiGateway",
        IsResource = false,
        IsCodeFirst = true,
        NamespaceCodeFirst = "FastApiGatewayDb.DataModel.Oracle",
        //NamespaceCodeFirst = "FastApiGatewayDb.DataModel.MySql",
        //NamespaceCodeFirst = "FastApiGatewayDb.DataModel.SqlServer",
        NamespaceProperties = "FastApiGatewayDb.DataModel.Oracle"
   });
   	or
  services.AddFastApiGatewayDb(a =>
  {
    a.dbFile = "db.json";
    a.dbKey = "ApiGateway";
    a.IsResource = false;
    a.IsCodeFirst = false;
    a.NamespaceCodeFirst = "FastApiGatewayDb.DataModel.Oracle";
    //a.NamespaceCodeFirst = "FastApiGatewayDb.DataModel.MySql";
    //a.NamespaceCodeFirst = "FastApiGatewayDb.DataModel.SqlServer";
    a.NamespaceProperties = "FastApiGatewayDb.DataModel.Oracle";
  },
 a => //RabbitMq
 {
   a.Host = "127.0.0.1";
   a.PassWord = "guest";
   a.UserName = "guest";
   a.Port = 5672;
   a.VirtualHost = "/";
   a.aop = new FastRabbitAop();
}, new FastAop());

//跨域
services.AddCors(options =>
{
	options.AddPolicy("any", builder => {
	builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
});

//Configure方法里 使用api网关 
app.UseFastApiGatewayMiddleware();


//aop
 public class FastAop : IFastApiAop
    {
	public void After(AfterContext context)
        {
                
        }

        public void Before(BeforeContext context)
        {
            context.param = null;
        }
    }
    
     public class FastRabbitAop : IFastRabbitAop
        {
            public void Delete(DeleteContext context)
            {
                //throw new System.NotImplementedException();
            }

            public void Exception(ExceptionContext context)
            {
                //throw new System.NotImplementedException();
            }

            public void Receive(ReceiveContext context)
            {
                //throw new System.NotImplementedException();
            }

            public void Send(SendContext context)
            {
                //throw new System.NotImplementedException();
            }
        }
```

	//all service add into GatewayDb
 	services.AddFastApiGatewayDbClient(a => {
                a.DbKey = "ApiGateway";
                a.LogType = FastApiGatewayDbClient.Core.LogType.TxtLog;
                a.TemplateType = FastApiGatewayDbClient.Core.TemplateType.ControllerActionPort;
                a.TemplateName = "test";
                a.Host= "127.0.0.1";
                a.Port = 1024;
            });

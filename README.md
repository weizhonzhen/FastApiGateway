
 ![](https://raw.githubusercontent.com/weizhonzhen/FastApiGateway/master/img.png)
 
 nuget url: https://www.nuget.org/packages/Fast.ApiGateway/
 ```csharp
//ConfigureServices 方法里 注入api网关
services.AddTransient<IFastApiGatewayDb, FastApiGatewayDb.FastApiGatewayDb>();

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

//跨域
services.AddCors(options =>
{
	options.AddPolicy("any", builder => {
	builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
});

//Configure方法里 使用api网关 
app.UseFastApiGatewayMiddleware();

```

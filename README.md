
 nuget url: https://www.nuget.org/packages/Fast.ApiGateway/
 ```csharp
//ConfigureServices 方法里 注入api网关
services.AddTransient<IFastApiGatewayDb, FastApiGatewayDb.FastApiGatewayDb>();

//注入
services.AddTransient<IFastApiGatewayDb, FastApiGatewayDb.FastApiGatewayDb>();

//跨域
services.AddCors(options =>
 {
 	options.AddPolicy("any", builder => {
	 builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
});

//oracle
FastMap.InstanceProperties("FastApiGatewayDb.DataModel.Oracle", "FastApiGatewayDb.dll");
FastMap.InstanceTable("FastApiGatewayDb.DataModel.Oracle", "FastApiGatewayDb.dll");
            
//mysql
FastMap.InstanceProperties("FastApiGatewayDb.DataModel.MySql", "FastApiGatewayDb.dll");
FastMap.InstanceTable("FastApiGatewayDb.DataModel.MySql", "FastApiGatewayDb.dll");
            
//sqlserver    
FastMap.InstanceProperties("FastApiGatewayDb.DataModel.SqlServer", "FastApiGatewayDb.dll");
FastMap.InstanceTable("FastApiGatewayDb.DataModel.SqlServer","FastApiGatewayDb.dll");

//Configure方法里 使用api网关 
app.UseMiddleware<FastApiGatewayDbHandler>();


```

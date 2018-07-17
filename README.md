```csharp
//ConfigureServices 方法里 注入api网关
services.AddTransient<IResponse, ApiResponse>();

//Configure方法里 使用api网关 
app.UseMiddleware();

```

```csharp
//ConfigureServices 方法里 注入api网关
services.AddTransient<IResponse, ApiResponse>();

//Configure方法里 使用api网关
app.UseMiddleware<ApiMiddleware>();

{
  "ApiGateway": [
    {
      "Key": "H001"，
      "Schema": "Composite", //Composite,Polling
      "DownParam": [
        {
          "Url": "http://10.88.88.25:8080/user/Base/Sex",
          "Method": "post",
          "IsBody": false
        },
        {
          "Url": "http://10.88.88.25:8080/user/Base/Sex",
          "Method": "post",
          "IsBody": false
        }
      ]
    }
  ]
}
```

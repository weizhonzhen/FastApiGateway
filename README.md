
//说明
public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {   
            services.AddResponseCompression();           
            services.AddTransient<IResponse, ApiResponse>();//注入api网关
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });//跨域
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {           
            app.UseMiddleware<ApiMiddleware>(); //api网关
        }
    }    
    
    api.json
    {
  "ApiGateway": [
    {
      "Key": "H001", //对外方法
      "Url": "http://10.88.88.25:8080/user/Base/Sex",//转发的地址
      "Method": "post",//请求方式
      "IsContent": false//是否post content(Fromcontent)
    }
  ]
}


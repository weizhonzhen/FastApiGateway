
//说明
public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {   
            //压缩
            services.AddResponseCompression();

            //注入api网关
            services.AddTransient<IResponse, ApiResponse>();

            //跨域
            services.AddCors(options =>
            {
                options.AddPolicy("any", builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
            });            
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //api网关
            app.UseMiddleware<ApiMiddleware>();
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


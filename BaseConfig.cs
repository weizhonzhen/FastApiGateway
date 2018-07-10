using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;

namespace Api.Gateway
{
    internal static class BaseConfig
    {
        public static List<T> GetListValue<T>(string key) where T : class, new()
        {
            var build = new ConfigurationBuilder();

            //内存
            build.AddInMemoryCollection();

            //目录
            build.SetBasePath(Directory.GetCurrentDirectory());

            //加载配置文件
            build.AddJsonFile("Api.json", optional: true, reloadOnChange: true);

            //编译成对象
            var config = build.Build();

            return new ServiceCollection().AddOptions().Configure<List<T>>(config.GetSection(key)).BuildServiceProvider().GetService<IOptions<List<T>>>().Value;
        }
    }
}

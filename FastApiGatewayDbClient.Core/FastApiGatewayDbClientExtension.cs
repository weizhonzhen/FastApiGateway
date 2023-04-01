using FastApiGatewayDbClient.Core;
using FastApiGatewayDbClient.Core.DataModel;
using FastData.Core;
using FastData.Core.Context;
using FastUntility.Core.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FastApiGatewayDbClientExtension
    {
        public static IServiceCollection AddFastApiGatewayDbClient(this IServiceCollection serviceCollection, Action<ConfigData> Action)
        {
            var config = new ConfigData();
            Action(config);

            if (string.IsNullOrEmpty(config.DbKey))
                throw new Exception("ConfigData dbKey is not null");

            using (var db = new DataContext(config.DbKey))
            {
                AppDomain.CurrentDomain.GetAssemblies().ToList().ForEach(a =>
                {
                    if (a.ExportedTypes != null)
                        a.ExportedTypes.ToList().ForEach(t =>
                        {
                           if(t.GetCustomAttribute<AspNetCore.Mvc.ApiControllerAttribute>()!=null)
                            {
                                var controller = string.Empty;
                                var url = new StringBuilder();
                                if (config.IsSsl)
                                    url.Append($"https://{config.Host}:{config.Port}/");
                                else
                                    url.Append($"http://{config.Host}:{config.Port}/");

                                var route = t.GetCustomAttribute<AspNetCore.Mvc.RouteAttribute>();

                                var reg = new Regex("controller", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                controller = reg.Replace(t.Name, "");
                                if (!string.IsNullOrEmpty(route.Template))
                                {
                                    reg = new Regex("\\[controller\\]", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                    url.Append($"{reg.Replace(route.Template, controller)}/");
                                }
                                
                                if(string.IsNullOrEmpty(route.Template)) 
                                    url.Append($"{controller}/");

                                t.GetMethods().ToList().ForEach(m => {
                                    if (m.GetCustomAttributes().ToList().Exists(r => r.GetType().BaseType == typeof(HttpMethodAttribute)))
                                    {
                                        var tempUrl = new StringBuilder();
                                        tempUrl.Append(url.ToString());
                                        if (m.GetCustomAttribute<ExcludeAttribute>() != null)
                                            return;

                                        var httpMethodAttribute = m.GetCustomAttributes().ToList().Find(r => r.GetType().BaseType == typeof(HttpMethodAttribute)) as HttpMethodAttribute;
                                        var routeAttribute = m.GetCustomAttributes().ToList().Find(r => r.GetType() == typeof(RouteAttribute)) as RouteAttribute;

                                        if (routeAttribute != null && string.IsNullOrEmpty(routeAttribute.Template))
                                            return;

                                        if (routeAttribute != null && routeAttribute.Template.Substring(0, 1) != "/")
                                            tempUrl.Append(routeAttribute.Template);
                                        else if (routeAttribute == null && !string.IsNullOrEmpty(httpMethodAttribute.Template))
                                            tempUrl.Append(httpMethodAttribute.Template);
                                        else
                                            tempUrl.Append(m.Name);

                                        var method = httpMethodAttribute.HttpMethods.First();
                                        var action = m.Name;
                                        var model = new ApiGatewayUrl();
                                        var downParam = new ApiGatewayDownParam();

                                        model.IsDbLog = config.LogType == LogType.DbLog ? 1 : 0;
                                        model.IsTxtLog = config.LogType == LogType.TxtLog ? 1 : 0;

                                        if (config.TemplateType == TemplateType.TemplateNameControllerAction)
                                            model.Key = $"{config.TemplateName}{controller}{action}";

                                        if (config.TemplateType == TemplateType.ControllerActionPort)
                                            model.Key = $"{controller}{action}{config.Port}";

                                        model.Name = model.Key;

                                        downParam.Key = model.Key;
                                        downParam.Name = model.Name;
                                        downParam.Url = tempUrl.ToString();
                                        downParam.Method = method;

                                        if (routeAttribute != null && routeAttribute.Template.Substring(0, 1) == "/")
                                            downParam.Url = $"https://{config.Host}:{config.Port}{routeAttribute.Template}";
                                        else
                                            downParam.Url = tempUrl.ToString();

                                        if (m.GetParameters().ToList().Exists(p => p.GetCustomAttribute<FromBodyAttribute>() != null))
                                            downParam.IsBody = 1;

                                        db.BeginTrans();
                                        if (FastRead.Query<ApiGatewayUrl>(q => q.Key.ToLower() == model.Key.ToLower()).ToCount(db) == 0)
                                        {
                                            db.Add(model);
                                            db.Add(downParam);
                                        }
                                        else
                                        {
                                            db.Update(model, d => d.Key == model.Key, d => new { d.IsDbLog, d.IsTxtLog, d.Name });
                                            db.Update(downParam, d => d.Key == downParam.Key, d => new { d.Name, d.Url, d.Method, d.IsBody });
                                        }
                                        db.SubmitTrans();
                                    }
                                });
                            }
                        });
                });
            }

            return serviceCollection;
        }
    }
}
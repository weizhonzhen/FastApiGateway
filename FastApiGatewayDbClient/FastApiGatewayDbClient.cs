using FastData.Context;
using FastData;
using FastUntility.Base;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using FastApiGatewayDbClient.DataModel;
using System.Web.Http.Controllers;

namespace FastApiGatewayDbClient
{
    public static class FastApiGatewayDbClient
    {
        public static void Init(Action<ConfigData> Action)
        {
            var config = new ConfigData();
            Action(config);

            if (string.IsNullOrEmpty(config.DbKey))
                throw new Exception("ConfigData dbKey is not null");


            using (var db = new DataContext(config.DbKey))
            {
                AppDomain.CurrentDomain.GetAssemblies().ToList().ForEach(a =>
                {
                    try
                    {
                        if (a.ExportedTypes != null)
                            a.ExportedTypes.ToList().ForEach(t =>
                            {
                                if (t.GetCustomAttribute<RoutePrefixAttribute>() != null || t.BaseType == typeof(ApiController))
                                {
                                    var controller = string.Empty;
                                    var url = new StringBuilder();
                                    if (config.IsSsl)
                                        url.Append($"https://{config.Host}:{config.Port}/");
                                    else
                                        url.Append($"http://{config.Host}:{config.Port}/");

                                    var reg = new Regex("controller", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                    var route = t.GetCustomAttribute<RoutePrefixAttribute>() ?? new RoutePrefixAttribute("");

                                    if (!string.IsNullOrEmpty(route.Prefix))
                                        url.Append(route.Prefix);

                                    controller = reg.Replace(t.Name, "");

                                    if (string.IsNullOrEmpty(route.Prefix))
                                        url.Append(reg.Replace(t.Name, ""));

                                    t.GetMethods(BindingFlags.Instance | BindingFlags.Public).ToList().ForEach(m =>
                                    {
                                        if (m.GetCustomAttribute<ExcludeAttribute>() != null)
                                            return;

                                        var isAttri = m.GetCustomAttributes().ToList().Exists(r => r.GetType().GetInterfaces().ToList().Exists(i => i == typeof(IActionHttpMethodProvider)));

                                        if (isAttri || m.DeclaringType == t)
                                        {
                                            var attribute = m.GetCustomAttributes().ToList().Find(r => r.GetType().BaseType == typeof(RouteAttribute)) as RouteAttribute ?? new RouteAttribute();

                                            if (!string.IsNullOrEmpty(attribute.Template))
                                                url.Append(attribute.Template);
                                            else
                                                url.Append(m.Name);

                                            var method = attribute.Name ?? "get";
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
                                            downParam.Url = url.ToString();
                                            downParam.Method = method;

                                            if (m.GetParameters().ToList().Exists(p => p.GetCustomAttribute<FromBodyAttribute>() != null))
                                                downParam.IsBody = 1;

                                            if (FastRead.Query<ApiGatewayUrl>(q => q.Key.ToLower() == model.Key.ToLower()).ToCount(db) == 0)
                                            {
                                                db.BeginTrans();
                                                var result = db.Add(model).writeReturn;
                                                if (result.IsSuccess)
                                                    result = db.Add(downParam).writeReturn;

                                                if (result.IsSuccess)
                                                    db.SubmitTrans();
                                                else
                                                    db.RollbackTrans();
                                            }
                                            else
                                                BaseLog.SaveLog($"{model.Key} is exists", "AddFastApiGatewayDbClient");
                                        }
                                    });
                                }
                            });
                    }
                    catch (Exception ex)
                    { }
                });
            }
        }
    }
}
using System.Collections.Generic;
using System;
using FastData.Core.Context;
using FastData.Core;
using FastApiGatewayDb.DataModel;
using Oracle.ManagedDataAccess.Client;
using FastUntility.Core.Base;
using System.Linq;

namespace FastApiGatewayDb.Ui
{
    public static class FastApiGateway
    {
        public static Dictionary<string, object> ApiInfo
        {
            get
            {
                using (var db = new DataContext(DbKey.Api))
                {                    
                    var dic = new Dictionary<string, object>();

                    dic.Add("api", FastRead.Query<ApiGatewayUrl>(a => a.Key != "").ToCount(db));
                    dic.Add("route", FastRead.Query<ApiGatewayDownParam>(a => a.Key != "").ToCount(db));
                    dic.Add("user", FastRead.Query<ApiGatewayUser>(a => a.AppKey != "").ToCount(db));
                    dic.Add("cache", FastRead.Query<ApiGatewayCache>(a => a.Key != "").ToCount(db));
                    dic.Add("exception", FastRead.Query<ApiGatewayWait>(a => a.Key != "").ToCount(db));
                    dic.Add("day", FastRead.Query<ApiGatewayLog>(a => a.ActionTime >= DateTime.Now.ToDate("yyyy-MM-dd").ToDate()).ToCount(db));

                    return dic;
                }
            }
        }
    }

    public static class DbKey
    {
        public readonly static string Api = "ApiGateway";
    }
}


using FastApiGatewayDb.Model;
using FastUntility.Core.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace FastApiGatewayDb
{
    /// <summary>
    /// post、get、put到url
    /// </summary>
    internal static class BaseUrl
    {
        private static int timeOut = 60;
        private static HttpClient http;

        static BaseUrl()
        {
            http = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip });
            http.DefaultRequestHeaders.Connection.Add("keep-alive");
            http.Timeout = new TimeSpan(0, 0, timeOut);
        }

        #region get url(select)
        /// <summary>
        /// get url(select)
        /// </summary>
        public static ReturnModel GetUrl(string url, string param, string key)
        {
            var model = new ReturnModel();
            try
            {
                var response = http.GetAsync(new Uri(string.Format("{0}{1}", url, param))).Result;
                model.status = (int)response.StatusCode; ;
                model.msg = response.Content.ReadAsStringAsync().Result;
                return model;
            }
            catch (Exception ex)
            {
                model.status = 408;
                model.msg = ex.Message;
                return model;
            }
        }
        #endregion

        #region post url(insert)
        /// <summary>
        /// post url(insert)
        /// </summary>
        public static ReturnModel PostUrl(string url, string param, string key)
        {
            var model = new ReturnModel();
            try
            {
                var content = new StringContent("", Encoding.UTF8, "application/json");
                var response = http.PostAsync(new Uri(string.Format("{0}{1}", url, param)), content).Result;
                model.status = (int)response.StatusCode; ;
                model.msg = response.Content.ReadAsStringAsync().Result;
                return model;
            }
            catch (Exception ex)
            {
                model.status = 408;
                model.msg = ex.Message;
                return model;
            }
        }
        #endregion

        #region post content(insert)
        /// <summary>
        /// post content(insert)
        /// </summary>
        public static ReturnModel PostContent(string url, string param, string key)
        {
            var model = new ReturnModel();
            try
            {
                var content = new StringContent(param, Encoding.UTF8, "application/json");
                var response = http.PostAsync(new Uri(url), content).Result;
                model.status = (int)response.StatusCode;
                model.msg = response.Content.ReadAsStringAsync().Result;
                return model;
            }
            catch (Exception ex)
            {
                model.status = 408;
                model.msg = ex.Message;
                return model;
            }
        }
        #endregion

        #region Soap url
        /// <summary>
        /// Soap url
        /// </summary>
        public static ReturnModel SoapUrl(string soapUrl, string soapParamName, string soapMethod, string soapParam)
        {
            var hash = new Hashtable();
            var model = new ReturnModel();
            var param = new Dictionary<string, object>();
            try
            {
                if (soapParam.Substring(0, 1) == "?")
                    soapParam = soapParam.Substring(1, soapParam.Length - 1);

                if (soapParam.IndexOf('&') > 0)
                {
                    foreach (var temp in soapParam.Split('&'))
                    {
                        if (temp.IndexOf('=') > 0)
                            param.Add(temp.Split('=')[0], temp.Split('=')[1]);
                    }
                }
                else
                {
                    if (soapParam.IndexOf('=') > 0)
                        param.Add(soapParam.Split('=')[0], soapParam.Split('=')[1]);
                }

                if (soapParamName.IndexOf('|') > 0)
                {
                    foreach (var temp in soapParamName.Split('|'))
                    {
                        hash.Add(temp, param.GetValue(temp));
                    }
                }
                else
                    hash.Add(soapParamName, param.GetValue(soapParamName));

                model.msg = BaseWebServiceSoap.QuerySoapWebServiceString(soapUrl, soapMethod, hash, timeOut);

                model.status = 200;
                return model;
            }
            catch (Exception ex)
            {
                model.status = 408;
                model.msg = ex.Message;
                return model;
            }
        }
        #endregion
    }
}

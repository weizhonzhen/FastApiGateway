using FastApiGatewayDb.Model;
using FastUntility.Core.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
                var response = http.GetAsync(new Uri(string.Format("{0}?{1}", url, param))).Result;
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
                var response = http.PostAsync(new Uri(string.Format("{0}?{1}", url, param)), content).Result;
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


        #region post soap
        /// <summary>
        /// post content(insert)
        /// </summary>
        private static string PostSoap(string url, string method, Dictionary<string, object> param)
        {
            var xml = new StringBuilder();
            xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xml.Append("<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            xml.Append("<soap:Body>");
            xml.AppendFormat("<{0} xmlns=\"http://openmas.chinamobile.com/pulgin\">", method);

            foreach (KeyValuePair<string, object> item in param)
            {
                xml.AppendFormat("<{0}>{1}</{0}>", item.Key, item.Value);
            }

            xml.AppendFormat("</{0}>", method);
            xml.Append("</soap:Body>");
            xml.Append("</soap:Envelope>");

            var content = new StringContent(xml.ToString(), Encoding.UTF8, "text/xml");
            var response = http.PostAsync(new Uri(url), content).Result;
            response.EnsureSuccessStatusCode();
            var result = response.Content.ReadAsStringAsync().Result;

            result = result.Replace("soap:Envelope", "Envelope");
            result = result.Replace("soap:Body", "Body");
            result = result.Replace(" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"", "");
            result = result.Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
            result = result.Replace(" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
            result = result.Replace(" xmlns=\"http://openmas.chinamobile.com/pulgin\"", "");
            return BaseXml.GetXmlString(result, string.Format("Envelope/Body/{0}Response/{0}Result", method)).Replace("&lt;", "<").Replace("&gt;", ">");
        }
        #endregion

        #region Soap url
        /// <summary>
        /// Soap url
        /// </summary>
        public static ReturnModel SoapUrl(string soapUrl, string soapParamName, string soapMethod, string soapParam)
        {
            var model = new ReturnModel();
            var dic = new Dictionary<string, object>();
            var param = new Dictionary<string, object>();
            try
            {
                if (soapParam.IndexOf('&') > 0)
                {
                    foreach (var temp in soapParam.Split('&'))
                    {
                        if (temp.IndexOf('=') > 0)
                            dic.Add(temp.Split('=')[0], temp.Split('=')[1]);
                    }
                }
                else
                {
                    if (soapParam.IndexOf('=') > 0)
                        dic.Add(soapParam.Split('=')[0], soapParam.Split('=')[1]);
                }

                if (soapParamName.IndexOf('|') > 0)
                {
                    foreach (var temp in soapParamName.Split('|'))
                    {
                        param.Add(temp, dic.GetValue(temp));
                    }
                }
                else
                    param.Add(soapParamName, dic.GetValue(soapParamName));

                model.msg = PostSoap(soapUrl, soapMethod, param);

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

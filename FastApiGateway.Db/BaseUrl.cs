using FastUntility.Core.Base;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using FastApiGatewayDb.Model;

namespace FastApiGatewayDb
{
    /// <summary>
    /// post、get、put到url
    /// </summary>
    internal static class BaseUrl
    {
        #region get url(select)
        /// <summary>
        /// get url(select)
        /// </summary>
        public static ReturnModel GetUrl(string url, string param, string key, IHttpClientFactory client,int marjor=1,int minor=1)
        {
            var http = client.CreateClient(key);
            var model = new ReturnModel();
            try
            {
                if (!url.Contains("?"))
                    param = string.Format("?{0}", param);

                var handle = new HttpRequestMessage();
                handle.Version = new Version(marjor, minor);
                handle.Content = new StringContent("", Encoding.UTF8, "application/json");
                handle.Method = HttpMethod.Get;
                handle.RequestUri = new Uri(string.Format("{0}{1}", url, param));

                var response = http.SendAsync(handle).Result;
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
        public static ReturnModel PostUrl(string url, string param, string key, IHttpClientFactory client, int marjor = 1, int minor = 1)
        {
            var http = client.CreateClient(key);
            var model = new ReturnModel();
            try
            {
                if (!url.Contains("?"))
                    param = string.Format("?{0}", param);

                var handle = new HttpRequestMessage();
                handle.Version = new Version(marjor, minor);
                handle.Content = new StringContent("", Encoding.UTF8, "application/json");
                handle.Method = HttpMethod.Post;
                handle.RequestUri = new Uri(string.Format("{0}{1}", url, param));

                var response = http.SendAsync(handle).Result;
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
        public static ReturnModel PostContent(string url, string param, string key, IHttpClientFactory client, int marjor = 1, int minor = 1)
        {
            var http = client.CreateClient(key);
            var model = new ReturnModel();
            try
            {
                var handle = new HttpRequestMessage();
                handle.Version = new Version(marjor, minor);
                handle.Content = new StringContent(param, Encoding.UTF8, "application/json");
                handle.Method = HttpMethod.Post;
                handle.RequestUri = new Uri(url);

                var response = http.SendAsync(handle).Result;
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
        private static string PostSoap(string url, string method, Dictionary<string, object> param, IHttpClientFactory client, string key, int marjor = 1, int minor = 1)
        {
            var http = client.CreateClient(key);
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
            
            var handle = new HttpRequestMessage();
            handle.Version = new Version(marjor, minor);
            handle.Content = new StringContent(xml.ToString(), Encoding.UTF8, "text/xml");
            handle.Method = HttpMethod.Post;
            handle.RequestUri = new Uri(url);

            var response = http.SendAsync(handle).Result;
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
        public static ReturnModel SoapUrl(string soapUrl, string soapParamName, string soapMethod, string soapParam, IHttpClientFactory client, string key)
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

                model.msg = PostSoap(soapUrl, soapMethod, param, client, key);

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

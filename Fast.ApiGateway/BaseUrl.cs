using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Text;
using Fast.ApiGateway.Model.Return;
using Fast.Untility.Core.Base;

namespace Fast.ApiGateway
{
    /// <summary>
    /// post、get、put到url
    /// </summary>
    internal static class BaseUrl
    {
        private static readonly HttpClient http;

        static BaseUrl()
        {
            http = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip });
            http.DefaultRequestHeaders.Connection.Add("keep-alive");
            http.Timeout = new TimeSpan(0, 0, 30);
        }

        #region get url(select)
        /// <summary>
        /// get url(select)
        /// </summary>
        public static ReturnModel GetUrl(string url,string param)
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
        public static ReturnModel PostUrl(string url,string param)
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
        public static ReturnModel PostContent(string url,string param)
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
        public static ReturnModel SoapUrl(string soapUrl,string soapParamName, string soapParam,string soapMethod)
        {
            var hash = new Hashtable();
            var model = new ReturnModel();
            try
            {
                hash.Add(soapParamName, soapParam);
                model.msg = BaseWebServiceSoap.QuerySoapWebServiceString(soapUrl, soapMethod, hash, 1);
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

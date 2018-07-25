using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Fast.ApiGateway.Model.Return;

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
    }
}

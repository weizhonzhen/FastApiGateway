using Api.Gateway.Model;
using System;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Api.Gateway
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
            http.Timeout = new TimeSpan(0, 0, 60);
        }

        #region get url(select)
        /// <summary>
        /// get url(select)
        /// </summary>
        public static ReturnModel GetUrl(DownParam item)
        {
            var model = new ReturnModel();
            try
            {
                var url = string.Format("{0}{1}", item.Url, item.Param);
                var response = http.GetAsync(new Uri(url)).Result;
                model.status = (int)response.StatusCode; ;
                model.msg = response.Content.ReadAsStringAsync().Result;
                return model;
            }
            catch (Exception ex)
            {
                model.status = 404;
                model.msg = ex.Message;
                return model;
            }
        }
        #endregion

        #region post url(insert)
        /// <summary>
        /// post url(insert)
        /// </summary>
        public static ReturnModel PostUrl(DownParam item)
        {
            var model = new ReturnModel();
            try
            {
                var url = string.Format("{0}{1}", item.Url, item.Param);

                var content = new StringContent("", Encoding.UTF8, "application/json");
                var response = http.PostAsync(new Uri(url), content).Result;
                model.status = (int)response.StatusCode; ;
                model.msg = response.Content.ReadAsStringAsync().Result;
                return model;
            }
            catch (Exception ex)
            {
                model.status = 404;
                model.msg = ex.Message;
                return model;
            }
        }
        #endregion

        #region post content(insert)
        /// <summary>
        /// post content(insert)
        /// </summary>
        public static ReturnModel PostContent(DownParam item)
        {
            var model = new ReturnModel();
            try
            {
                var content = new StringContent(item.Param, Encoding.UTF8, "application/json");
                var response = http.PostAsync(new Uri(item.Url), content).Result;
                model.status = (int)response.StatusCode;
                model.msg = response.Content.ReadAsStringAsync().Result;
                return model;
            }
            catch (Exception ex)
            {
                model.status = 404;
                model.msg = ex.Message;
                return model;
            }
        }
        #endregion
    }
}

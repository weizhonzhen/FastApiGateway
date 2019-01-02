using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FastApiGateway.Model.Return;
using FastMongoDb.Core.Base;
using RabbitMQ.Client;

namespace FastApiGateway
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
        }

        #region get url(select)
        /// <summary>
        /// get url(select)
        /// </summary>
        public static ReturnModel GetUrl(string url,string param,string key, int timeOut)
        {
            var model = new ReturnModel();
            try
            {
                http.Timeout = new TimeSpan(0, 0, timeOut);

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
        public static ReturnModel PostUrl(string url,string param,string key, int timeOut)
        {
            var model = new ReturnModel();
            try
            {
                http.Timeout = new TimeSpan(0, 0, timeOut);

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
        public static ReturnModel PostContent(string url,string param,string key,int timeOut)
        {
            var model = new ReturnModel();
            try
            {
                http.Timeout = new TimeSpan(0, 0, timeOut);

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
        public static ReturnModel SoapUrl(string soapUrl,string soapParamName, string soapParam,string soapMethod,int timeOut)
        {
            var hash = new Hashtable();
            var model = new ReturnModel();
            try
            {
                hash.Add(soapParamName, soapParam);
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

        #region Rabbit url
        /// <summary>
        /// Rabbit url
        /// </summary>
        /// <param name="soapUrl"></param>
        /// <param name="soapParamName"></param>
        /// <param name="soapParam"></param>
        /// <param name="soapMethod"></param>
        /// <returns></returns>
        public static ReturnModel RabbitUrl(string queueName, string message)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var config = BaseConfig.GetValue<ConfigModel>("Rabbit", "db.json");
                    var uri = new Uri(config.Host);
                    var factory = new ConnectionFactory
                    {
                        UserName = config.UserName,
                        Password = config.Password,
                        VirtualHost = config.VirtualHost,
                        Endpoint = new AmqpTcpEndpoint(uri),
                        RequestedHeartbeat = 0,
                        AutomaticRecoveryEnabled = true
                    };

                    using (var con = factory.CreateConnection())
                    {
                        using (var model = con.CreateModel())
                        {
                            model.QueueDeclare(queueName, true, false, false, null);
                            var properties = model.CreateBasicProperties();

                            //持久
                            properties.Persistent = true;
                            properties.DeliveryMode = 2;

                            //消息转换为二进制
                            var msgBody = Encoding.UTF8.GetBytes(message);

                            //消息发出到队列
                            model.BasicPublish("", queueName, properties, msgBody);
                        }
                    }
                });

                return new ReturnModel { status = 200, msg = "成功" };
            }
            catch (Exception ex)
            {
                return new ReturnModel { status = 408, msg = ex.Message };
            }
        }
        #endregion
    }

    /// <summary>
    /// Rabbit 配置
    /// </summary>
    internal class ConfigModel
    {
        /// <summary>
        /// 主机地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 虚拟主机名
        /// </summary>
        public string VirtualHost { get; set; }
    }
}

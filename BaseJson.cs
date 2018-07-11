using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Api.Gateway
{
    public static class BaseJson
    {
        #region json转dic
        /// <summary>
        /// json转dic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonValue"></param>
        /// <returns></returns>
        public static Dictionary<string, object> JsonToDic(string jsonValue)
        {
            try
            {
                var item = new Dictionary<string, object>();

                if (string.IsNullOrEmpty(jsonValue))
                    return item;

                var jo = JObject.Parse(jsonValue);

                foreach (var temp in jo)
                {
                    item.Add(temp.Key, temp.Value);
                }
                return item;
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }
        #endregion

        #region json转dics
        /// <summary>
        /// json转dics
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonValue"></param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> JsonToDics(string jsonValue)
        {
            try
            {
                var item = new List<Dictionary<string, object>>();

                if (string.IsNullOrEmpty(jsonValue))
                    return item;

                var ja = JArray.Parse(jsonValue);

                foreach (var jo in ja)
                {
                    item.Add(JsonToDic(jo.ToString()));
                }

                return item;
            }
            catch
            {
                return new List<Dictionary<string, object>>();
            }
        }
        #endregion
    }
}

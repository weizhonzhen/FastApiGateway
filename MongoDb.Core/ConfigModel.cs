namespace MongoDb.Core
{
    /// <summary>
    /// 配置实体
    /// </summary>
    internal class ConfigModel
    {
        /// <summary>
        /// 连接串
        /// </summary>
        public string ConnStr { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int Max { get; set; }


        /// <summary>
        /// 最小连接数
        /// </summary>
        public int Min { get; set; }
    }
}

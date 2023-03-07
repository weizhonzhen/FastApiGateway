namespace FastApiGatewayDbClient.Core
{
    public class ConfigData
    {
        /// <summary>
        /// db key
        /// </summary>
        public string DbKey { get; set; }

        public bool IsSsl { get; set; }

        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// key Template
        /// </summary>
        public TemplateType TemplateType { get; set; } = TemplateType.ControllerActionPort;

        /// <summary>
        /// TemplateName
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// LogT ype
        /// </summary>
        public LogType LogType { get; set; } = LogType.TxtLog;
    }

    public enum TemplateType
    {
        ControllerActionPort = 1,
        TemplateNameControllerAction = 2
    }

    public enum LogType
    {
        DbLog = 1,
        TxtLog = 2
    }
}

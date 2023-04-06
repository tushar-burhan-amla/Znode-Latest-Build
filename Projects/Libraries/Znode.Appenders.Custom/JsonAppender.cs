using log4net.Appender;
using log4net.Core;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Web;
using System.Net.NetworkInformation;

namespace Znode.Appenders.Custom
{
    public class JsonAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            string logFileName = HttpContext.Current.Server.MapPath("/JsonFile.log");

            if (!File.Exists(logFileName))
                using (File.Create(logFileName)) { }

            LoggerModel loggerModel = new LoggerModel()
            {
                Message = loggingEvent.RenderedMessage,
                Date = loggingEvent.TimeStamp,
                Level = loggingEvent.Level.Name,
                UserAgent = HttpContext.Current.Request.UserAgent,
                UserName = loggingEvent.UserName,
                Identity = loggingEvent.Identity,
                LoggerName = loggingEvent.LoggerName,
                StackTrace = loggingEvent.ExceptionObject?.StackTrace ?? string.Empty,
                DomainName = IPGlobalProperties.GetIPGlobalProperties()?.DomainName,
                HostName = HttpContext.Current.Request.Url.Host,
                SiteUrl = HttpContext.Current.Request.Url.AbsolutePath
            };
            string jsonLogData = JsonConvert.SerializeObject(loggerModel, Formatting.Indented);

            File.AppendAllLines(logFileName, new[] { jsonLogData });
        }
    }

    public class LoggerModel
    {
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string Level { get; set; }
        public string UserAgent { get; set; }
        public string UserName { get; set; }
        public string Identity { get; set; }
        public string LoggerName { get; set; }
        public string StackTrace { get; set; }
        public string DomainName { get; set; }
        public string HostName { get; set; }
        public string SiteUrl { get; set; }
    }
}

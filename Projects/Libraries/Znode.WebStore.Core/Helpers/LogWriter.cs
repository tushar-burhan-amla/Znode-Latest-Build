using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.WebStore
{
    [Obsolete("Using Znode Logging Instead")]
    public class LogWriter : IDisposable
    {
        private string m_exePath = string.Empty;
        private bool isDisposed;
        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);
        }

        ~LogWriter()
        {
            if (!isDisposed)
                Dispose();
        }

        public void Dispose()
        {

            isDisposed = true;
        }
        public static void LogWrite(string logMessage)
        {
            var path = HttpContext.Current.Server.MapPath("~/App_Data");
            var fullpath = Path.Combine(path, "Znode_Log.txt");
            try
            {
                using (StreamWriter w = File.AppendText(fullpath))
                {
                    Log(logMessage, w);
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
        }

        public static void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nTimeStamp: ");
                txtWriter.WriteLine("{0} {1}", DateTime.Now,
                    DateTime.Now.ToLongDateString());
                txtWriter.WriteLine("  ");
                txtWriter.WriteLine("{0}", logMessage);
                txtWriter.WriteLine("*****************************");
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
        }
    }
}

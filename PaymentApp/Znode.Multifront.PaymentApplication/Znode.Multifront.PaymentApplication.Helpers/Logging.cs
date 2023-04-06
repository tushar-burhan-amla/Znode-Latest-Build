using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Net;
using System.Text;
using System.Web;
using Znode.Multifront.PaymentFramework.Bussiness;

namespace Znode.Multifront.PaymentApplication.Helpers
{
    public class Logging
    {
        #region "Private Variables"

        private static string _FilePath = "~/config/znodepaymentapilog.txt";

        #endregion

        #region "Public Methods"
        [Obsolete("This method is not in use now, using Log4net instead of File logging")]
        public static void LogMessage(string message)
        {
            if (!Equals(ConfigurationManager.AppSettings["LogFilePath"], null))
            {
                _FilePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["LogFilePath"]);
                using (StreamWriter sw = (File.Exists(_FilePath)) ? File.AppendText(_FilePath) : File.CreateText(_FilePath))
                {
                    StringBuilder errMsg = new StringBuilder();
                    errMsg.AppendLine();
                    errMsg.AppendLine("*************************");
                    errMsg.AppendLine($"TimeStamp: {System.DateTime.Now.ToString()}");
                    errMsg.AppendLine(message);
                    errMsg.AppendLine("*************************");
                    try
                    {
                        errMsg.AppendLine($"Client IP: {getClientIP()}");
                        errMsg.AppendLine($"Url: {HttpContext.Current.Request.Url}");
                        errMsg.AppendLine($"User Agent: {HttpContext.Current.Request.UserAgent}");
                    }
                    catch
                    {

                    }
                    errMsg.AppendLine("*************************");
                    sw.WriteLine(errMsg.ToString());
                }
            }
        }

        /// <summary>
        /// Logs a message to a Mongo DB via Log4Net
        /// </summary>
        /// <param name="message">A message to log to the file.</param>
        public static void LogMessage(string message, string componentName = "", TraceLevel traceLevel = TraceLevel.Info, object obj = null, [CallerMemberName] string methodName = "", [CallerFilePath] string fileName = "", [CallerLineNumber] int lineNumber = 0)
        {
            try
            {
                ZnodeLogging.LogMessage(message, componentName, traceLevel , obj, methodName, fileName ,  lineNumber);
            }
            catch
            {
                // returning Nothing to avoid recursive loop if exception occurs while logging messages.
            }
        }


        /// <summary>
        /// Logs a message to a Mongo DB via Log4Net
        /// </summary>
        /// <param name="ex">An Exception object to log to the file.</param>
        public static void LogMessage(Exception ex, string componentName = "", TraceLevel traceLevel = TraceLevel.Info, object obj = null)
        {

            try
            {
                ZnodeLogging.LogMessage(ex, componentName, traceLevel, obj);
            }
            catch
            {
                // returning Nothing to avoid recursive loop if exception occurs while logging messages.
            }
         
        }

        #endregion

        #region "Enum"

        public enum Components
        {
            Payment
        }
        #endregion
        public static string getClientIP()
        {
            string hostName = "";
            hostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[addr.Length - 1].ToString();

        }

    }
}

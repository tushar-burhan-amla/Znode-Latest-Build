using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace Znode.Libraries.Data.Helpers
{
    public static class EntityLogging
    {
        #region Enum Mode

        // Specifies the text file access modes.
        public enum Mode
        {
            // Indicates text file read mode operation.
            Read,

            // Indicates text file write mode operation. It deletes the previous content.
            Write,

            // Indicates text file append mode operation. it preserves the previous content.
            Append
        }

        #endregion

        #region Declarations

        public const string LogComponentFilePath = "~/data/default/logs/{yyyy-mm-dd}/Znode_{ComponentName}_Log.log";
        public const string LogFilePath = "~/data/default/logs/{yyyy-mm-dd}/Znode_Log.log";

        #endregion

        #region Public Methods

        // Checks the web.config to see if text file logging is enabled.
        // Returns True - If logging is enabled, False - If logging is disabled.</returns>
        public static bool FileLoggingEnabled()
        {
            if (ConfigurationManager.AppSettings["EnableFileLogging"] != null)
            {
                return Convert.ToString(ConfigurationManager.AppSettings["EnableFileLogging"]).Equals("1");
            }
            else
            {
                return false;
            }
        }

        // Logs a message to a text file
        public static void LogMessage(string Message, string componentName = "")
        {
            if (string.IsNullOrEmpty(componentName))
            {
                WriteLogFiles(Message, LogFilePath, componentName);
            }
            else
            {
                WriteLogFiles(Message, LogComponentFilePath, componentName);
            }
        }

        // Serializes objects to a text file
        public static void LogObject(Type objectType, Object objectInstance, Exception ex, string componentName = "")
        {
            if (string.IsNullOrEmpty(componentName))
            {
                WriteObjects(objectType, objectInstance, LogFilePath, ex, componentName);
            }
            else
            {
                WriteObjects(objectType, objectInstance, LogComponentFilePath, ex, componentName);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method will write the messages to the log file for Admin as well as for demo site.
        /// </summary>
        /// <param name="message">string message</param>
        /// <param name="logFilePath">string log file path</param>
        private static void WriteLogFiles(string message, string logFilePath, string componentName)
        {
            if (FileLoggingEnabled())
            {
                StringBuilder errMsg = new StringBuilder();
                errMsg.AppendLine();
                errMsg.AppendLine();
                errMsg.AppendLine("*************************");
                errMsg.AppendLine("TimeStamp: " + System.DateTime.Now.ToString());
                errMsg.AppendLine(message);
                errMsg.AppendLine("*************************");

                string todaysDate = DateTime.Now.ToString("yyyy-MM-dd");
                string filePath = logFilePath.Replace("{yyyy-mm-dd}", todaysDate);
                if (!string.IsNullOrEmpty(componentName))
                {
                    filePath = filePath.Replace("{ComponentName}", componentName);
                }

                WriteTextStorage(errMsg.ToString(), filePath, Mode.Append);
            }
        }

        /// <summary>
        /// This method will write the objects to the log file for example dataset for Admin as well as for demo site.
        /// </summary>
        /// <param name="objectType">Type of an object</param>
        /// <param name="objectInstance">Complete object to be written</param>
        /// <param name="logFilePath">string log file path</param>
        private static void WriteObjects(Type objectType, Object objectInstance, string logFilePath, Exception ex, string componentName)
        {
            try
            {
                //first log a header
                LogMessage(ex.ToString(), componentName);

                StringWriter textWriter = new StringWriter();
                XmlSerializer xmlSerializer = new XmlSerializer(objectType);

                xmlSerializer.Serialize(textWriter, objectInstance);

                string todaysDate = DateTime.Now.ToString("yyyy-MM-dd");
                string filePath = logFilePath.Replace("{yyyy-mm-dd}", todaysDate);
                if (!string.IsNullOrEmpty(componentName))
                {
                    filePath = filePath.Replace("{ComponentName}", componentName);
                }

                WriteTextStorage(textWriter.ToString(), filePath, Mode.Append);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Writes text file to persistant storage.
        /// </summary>
        /// <param name="fileData">Specify the string that has the file content.</param>
        /// <param name="filePath">Specify the relative file path.</param>
        /// <param name="fileMode">Specify the file write mode operatation. </param>
        private static void WriteTextStorage(string fileData, string filePath, Mode fileMode)
        {
            try
            {
                // Create directory if not exists.
                FileInfo fileInfo = new FileInfo(HttpContext.Current.Server.MapPath(filePath));
                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }

                // Check file write mode and write content.
                if (Equals(fileMode, Mode.Append))
                {
                    File.AppendAllText(HttpContext.Current.Server.MapPath(filePath), fileData);
                }
                else
                {
                    File.WriteAllText(HttpContext.Current.Server.MapPath(filePath), fileData);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}

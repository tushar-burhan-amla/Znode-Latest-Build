using System;
using System.IO;
using System.Net;
using System.Text;

namespace Znode.Engine.ERPTaskScheduler
{
    class Program
    {
        //args[0] - ERPTaskSchedulerID.
        //args[1] - Portal ID.
        //args[2] - Log file path.
        //args[3] - API Domain URL.
        //args[4] - Scheduler Name.

        public static string erpExeLogFilePath = string.Empty;
        #region Main
        static void Main(string[] arguments)
        {
            try
            {
                erpExeLogFilePath = arguments[2];
                LogMessage("Start to run EXE for " + arguments[4] + "Scheduler.");
                Program program = new Program();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Please wait....");
                LogMessage("SyncCallApi method Calling.");
                program.SyncCallApi(arguments);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                LogMessage("Failed :" + ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// Connect to ERPTaskScheduler API 
        /// </summary>
        /// <param name="args">Parameter with task scheduler</param>
        /// <returns>Return responce by API controller</returns>
        public string SyncCallApi(string[] arguments)
        {
            string jsonString = string.Empty;
            string message = string.Empty;
            string requestPath = string.Concat(arguments[3], "/ERPTaskScheduler/TriggerSchedulerTask/", arguments[0]);
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-url-encoded";
                request.Timeout = 216000; // 1 hour
                LogMessage("SyncCallApi method Called and Set request parameter.");
                using (HttpWebResponse responce = (HttpWebResponse)request.GetResponse())
                {
                    Stream datastream = responce.GetResponseStream();
                    LogMessage("Got Response Stream.");
                    StreamReader reader = new StreamReader(datastream);
                    LogMessage("read Response Stream.");
                    jsonString = reader.ReadToEnd();
                    reader.Close();
                    datastream.Close();
                    LogMessage("API Call Successfully.");
                    //Call Send Scheduler Activity Log Method with "No Error" and scheduler status as true
                    LogMessage("Sending Scheduler Activity Log.");
                    SendSchedulerActivityLog("No Error", true, arguments[3], arguments[1], arguments[0]);
                }
            }
            catch (Exception ex)
            {
                //Call Send Scheduler Activity Log Method with "Error" and scheduler status as false
                SendSchedulerActivityLog(ex.Message, false, arguments[3], arguments[1], arguments[0]);
                LogMessage("Failed :" + ex.Message);
            }
            return jsonString;
        }

        private bool SendSchedulerActivityLog(string errorMessage, bool schedulerStatus, string domailUrl, string portalId, string eRPTaskSchedulerId)
        {
            //Create static json string
            string data = string.Concat("{", '"', "ErrorMessage", '"', ":", '"', errorMessage, '"', ',', '"', "SchedulerStatus", '"', ":", '"', schedulerStatus, '"', ',', '"', "PortalId", '"', ":", '"', portalId, '"', ',', '"', "ERPTaskSchedulerId", '"', ":", '"', eRPTaskSchedulerId, '"', "}");
            //Create endpoint
            string endpoint = string.Concat(domailUrl, "/TouchPointConfiguration/SendSchedulerActivityLog");
            return PostResourceToEndpoint(endpoint, data);
        }

        private bool PostResourceToEndpoint(string endpoint, string data)
        {
            LogMessage("Posting Resource To Endpoint For Email.");
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var req = (HttpWebRequest)WebRequest.Create(endpoint);
            req.KeepAlive = false; // Prevents "server committed a protocol violation" error
            req.Method = "POST";
            req.ContentType = "application/json";
            req.ContentLength = dataBytes.Length;
           
            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(dataBytes, 0, dataBytes.Length);
            }
            LogMessage("Read request stream for Email.");

            if (((HttpWebResponse)req.GetResponse()).StatusCode == HttpStatusCode.OK)
            {
                LogMessage("Email Sent Successfully.");
                return true;
            }
            else
            {
                LogMessage("Failed to Sent Email.");
                return false;
            }
        }

        /// <summary>
        /// Logs a message to a text file
        /// </summary>
        /// <param name="Message">A message to log to the file.</param>
        public static void LogMessage(string Message)
        {
            WriteLogFiles(Message, erpExeLogFilePath);
        }

        /// <summary>
        /// This method will write the messages to the log file for Admin as well as for demo site.
        /// </summary>
        /// <param name="message">string message</param>
        /// <param name="logFilePath">string log file path</param>
        private static void WriteLogFiles(string message, string logFilePath)
        {
            StringBuilder errMsg = new StringBuilder();
            errMsg.AppendLine();
            errMsg.AppendLine("************* "+ DateTime.Now.ToString() + " *************");
            errMsg.AppendLine(message);
            errMsg.AppendLine("***********************************************");

            string todaysDate = DateTime.Now.ToString("yyyy-MM-dd");
            string filePath = logFilePath.Replace("{yyyy-mm-dd}", todaysDate);
            WriteTextStorage(errMsg.ToString(), filePath, Mode.Append);
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
                string logFilePath = filePath;
                FileInfo fileInfo = new FileInfo(logFilePath);
                if (!fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();

                // Check file write mode and write content.
                if (Equals(fileMode, Mode.Append))
                    File.AppendAllText(logFilePath, fileData);
                else
                    File.WriteAllText(logFilePath, fileData);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private enum Mode
        {
            // Indicates text file read mode operation.
            Read,

            // Indicates text file write mode operation. It deletes the previous content.
            Write,

            // Indicates text file append mode operation. it preserves the previous content.
            Append
        }
    }
}

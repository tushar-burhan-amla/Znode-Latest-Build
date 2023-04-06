using System;
using System.Diagnostics;
using System.IO;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Helpers
{
    public static class Log4NetHelper
    {
        /// <summary>
        /// To replace log4net dll as per requirement in bin folder
        /// For Paymenttech orbital - log4net ver 4.0
        /// & for PayPal Express - log4net ver 1.2
        /// </summary>
        /// <param name="gatewayType">GatewayType gatewayType</param>
        public static void ReplaceLog4NetDLL(string gatewayType)
        {
            if (Equals(gatewayType, Convert.ToInt16(Enum.Parse(typeof(GatewayType), GatewayType.PAYMENTECH.ToString())).ToString()) ||
                Equals(gatewayType, Convert.ToInt16(Enum.Parse(typeof(GatewayType), GatewayType.PAYPALEXPRESS.ToString())).ToString()))
            {
                string log4NetVersion = ReadLog4NetVersion();
                if (!string.IsNullOrEmpty(log4NetVersion))
                {
                    if (Log4NetVersionMismatch(gatewayType, log4NetVersion))
                        ReplaceDLL(gatewayType);
                }
                else
                    ReplaceDLL(gatewayType);
            }
        }

        /// <summary>
        /// To check Log4Net Version is match
        /// </summary>
        /// <param name="gatewayType"></param>
        /// <param name="log4NetVersion"></param>
        /// <returns>returns true/false</returns>
        private static bool Log4NetVersionMismatch(string gatewayType, string log4NetVersion)
        {
            if (Equals(gatewayType, Convert.ToInt16(Enum.Parse(typeof(GatewayType), GatewayType.PAYMENTECH.ToString())).ToString()))
                return log4NetVersion.Contains("v4.0") ? false : true;
            else if (Equals(gatewayType, Convert.ToInt16(Enum.Parse(typeof(GatewayType), GatewayType.PAYPALEXPRESS.ToString())).ToString()))
                return log4NetVersion.Contains("v1.2") ? false : true;
            return false;
        }

        /// <summary>
        /// To replace log4Net dll as per required gateway
        /// </summary>
        /// <param name="gatewayType"></param>
        /// <returns>returns log4Net Version</returns>
        private static void ReplaceDLL(string gatewayType)
        {
            try
            {
                string sourceFileName = string.Empty;
                string log4NetVersion = string.Empty;
                if (Equals(gatewayType, Convert.ToInt16(Enum.Parse(typeof(GatewayType), GatewayType.PAYMENTECH.ToString())).ToString()))
                {
                    sourceFileName = "~/config/log4net-v4.0/log4net.dll";
                    log4NetVersion = "v4.0";
                }
                else if (Equals(gatewayType, Convert.ToInt16(Enum.Parse(typeof(GatewayType), GatewayType.PAYPALEXPRESS.ToString())).ToString()))
                {
                    sourceFileName = "~/config/log4net-v1.2/log4net.dll";
                    log4NetVersion = "v1.2";
                }

                string destinationFilePath = (System.Web.HttpContext.Current.Server.MapPath("~/bin"));
                string sourceFilePath = (System.Web.HttpContext.Current.Server.MapPath(sourceFileName));

                destinationFilePath = $"{destinationFilePath}\\log4net.dll";
                File.Copy(sourceFilePath, destinationFilePath, true);
                WriteLog4NetVersion(log4NetVersion);
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
            }
        }

        /// <summary>
        ///  To write log4Net Version in file
        /// </summary>
        /// <param name="version"></param>
        private static void WriteLog4NetVersion(string version)
        {
            try
            {
                // Compose a string that consists of three lines.
                string filePath = GetFilePath();
                if (!File.Exists(filePath))
                {
                    FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    StreamWriter writer = new StreamWriter(fileStream);
                    writer.Write(version);
                    writer.Close();
                }
                else
                {
                    // Write the string to a file.
                    StreamWriter file = new StreamWriter(filePath, false);
                    file.WriteLine(version);
                    file.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
            }
        }

        /// <summary>
        /// To get log4NetVersion file path
        /// </summary>
        /// <returns>returns log4NetVersion</returns>
        private static string GetFilePath()
        {
            try
            {
                return System.Web.HttpContext.Current.Server.MapPath("~/config/log4NetVersion.txt");
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
            }

            return string.Empty;
        }
        /// <summary>
        /// Read file
        /// </summary>
        /// <returns></returns>
        private static string ReadLog4NetVersion()
        {
            string log4NetVersion = string.Empty;
            string filePath = GetFilePath();
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // Read the stream to a string, and write the string to the console.
                    log4NetVersion = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
            }
            return log4NetVersion;
        }
    }
}

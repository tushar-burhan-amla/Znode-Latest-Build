using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace Znode.Multifront.PaymentApplication.Helpers
{
    public static class PaymentechHelper
    {
        public static void ReplacePaymentechURLs(bool isTestMode)
        {
            try
            {
                var appSettings = ConfigurationSettings.AppSettings;
                string PaymentechSDKPath = appSettings["PaymentechSDKPath"].ToString();
                if (!string.IsNullOrEmpty(PaymentechSDKPath))
                {
                    string lineHandlerTemplate = System.Web.HttpContext.Current.Server.MapPath("\\config\\linehandler - Template.properties");
                    string lineHandler = $"{PaymentechSDKPath}{"\\etc\\linehandler.properties"}";

                    string fileText = File.ReadAllText(lineHandlerTemplate);
                    if (isTestMode)
                    {
                        fileText = fileText.Replace("{#hostname}", appSettings["PaymentechTestHost"].ToString());
                        fileText = fileText.Replace("{#port}", appSettings["PaymentechTestPort"].ToString());
                        fileText = fileText.Replace("{#failoverhostname}", appSettings["PaymentechTestfailoverHost"].ToString());
                        fileText = fileText.Replace("{#failoverport}", appSettings["PaymentechTestfailoverPort"].ToString());
                    }
                    else
                    {
                        fileText = fileText.Replace("{#hostname}", appSettings["PaymentechProdHost"].ToString());
                        fileText = fileText.Replace("{#port}", appSettings["PaymentechProdPort"].ToString());
                        fileText = fileText.Replace("{#failoverhostname}", appSettings["PaymentechProdfailoverHost"].ToString());
                        fileText = fileText.Replace("{#failoverport}", appSettings["PaymentechProdfailoverPort"].ToString());
                    }

                    string curFile = lineHandler;
                    if (File.Exists(curFile))
                        File.Delete(lineHandler);

                    FileStream fileTextCreate = new FileStream(lineHandler, FileMode.Create);
                    StreamWriter fileTextStream = new StreamWriter(fileTextCreate);
                    fileTextStream.BaseStream.Seek(0, SeekOrigin.End);
                    fileTextStream.Write(fileText);
                    fileTextStream.Flush();
                    fileTextStream.Close();
                    fileTextCreate.Close();
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
            }
        }
        
    }
}

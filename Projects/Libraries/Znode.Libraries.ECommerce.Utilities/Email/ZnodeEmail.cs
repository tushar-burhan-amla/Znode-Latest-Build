using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using MimeKit;
using Znode.Libraries.Framework.Business;
using System.IO;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.Resources;

namespace Znode.Libraries.ECommerce.Utilities
{
    /// <summary>
    /// This class inherits from the ZnodeEmailBase class.
    /// </summary>
    public class ZnodeEmail : ZnodeEmailBase, IZnodeEmail
    {
        // If you wish to implement your own email functionality uncomment the code below to
        // override the base function.

        // Note that you may find the following properties in the base class usefull.
        // ZnodeEmailBase.SMTPPassword;
        // ZnodeEmailBase.SMTPUserName;
        // ZnodeEmailBase.SMTPServer;
        // ZnodeEmailBase.SMTPPort;
        // ZnodeEmailBase.EnableSSLForSMTP

        /// <summary>
        /// Sends email using SMTP, Uses default network credentials
        /// </summary>
        /// <param name="To">Email address of the recipient.</param>
        /// <param name="From">Email address of the sender.</param>
        /// <param name="BCC">Blind carbon copy email address.</param>
        /// <param name="Subject">The subject line of the email.</param>
        /// <param name="Body">The body of the email.</param>
        /// <param name="IsBodyHtml">Set to True to send this email in HTML format.</param>
        public static new void SendEmail(string to, string from, string bcc, string subject, string body, bool isBodyHtml, string attachedPath = "", string cc = "")
        {
            try
            {
                //Get smtp setting details.
                ZnodePortalSmtpSetting smtpSettings = GetSMTPSetting();

                //Assign from email address and bcc if smtp details is not null.
                if (HelperUtility.IsNotNull(smtpSettings))
                {
                    //"Disable all emails" flag should only affect the procedure for any particular store. If the call to this method has no PortalId, It should not affect anything.
                    if (IsEmailSendingEnabled(smtpSettings))
                        SendEmail(string.IsNullOrEmpty(smtpSettings.FromEmailAddress) ? ZnodeConfigManager.SiteConfig.AdminEmail : smtpSettings.FromEmailAddress, to, subject, body, cc, bcc, isBodyHtml, string.IsNullOrEmpty(smtpSettings.FromDisplayName) ? string.Empty : smtpSettings.FromDisplayName, string.IsNullOrEmpty(attachedPath) ? null : new List<string>() { attachedPath });
                    else
                        ZnodeLogging.LogMessage("Email sending disabled for this store.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose);
                }
                else
                    SendEmailByPortal(to, string.IsNullOrEmpty(smtpSettings.FromEmailAddress) ? ZnodeConfigManager.SiteConfig.AdminEmail : smtpSettings.FromEmailAddress, string.Empty, subject, body, true, "");
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Mail sending to customer failed.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error, ex);
            }
        }

        /// <summary>
        /// Sends email using SMTP, Uses default network credentials
        /// </summary>
        /// <param name="portalId">Current portalId.</param>
        /// <param name="To">Email address of the recipient.</param>
        /// <param name="From">Email address of the sender.</param>
        /// <param name="BCC">Blind carbon copy email address.</param>
        /// <param name="Subject">The subject line of the email.</param>
        /// <param name="Body">The body of the email.</param>
        /// <param name="IsBodyHtml">Set to True to send this email in HTML format.</param>
        public static new void SendEmail(int portalId, string to, string from, string bcc, string subject, string body, bool isBodyHtml, string attachedPath = "", string cc = "",string barcode="")
        {
            try
            {
                //Get smtp setting details.
                ZnodePortalSmtpSetting smtpSettings = GetSMTPSetting(portalId);

                //Assign from email address and bcc if smtp details is not null.
                if (HelperUtility.IsNotNull(smtpSettings))
                {
                    //"Disable all emails" flag should only affect the procedure for any particular store. If the call to this method has no PortalId, It should not affect anything.
                    if (IsEmailSendingEnabled(smtpSettings))
                    {
                        string fromEmailAddress = string.IsNullOrEmpty(smtpSettings.FromEmailAddress)
                                          ? (!string.IsNullOrEmpty(from) && IsValidEmail(from))
                                          ? from : ZnodeConfigManager.SiteConfig.AdminEmail : smtpSettings.FromEmailAddress;

                        SendEmail(fromEmailAddress, to, subject, body, cc, bcc, isBodyHtml, string.IsNullOrEmpty(smtpSettings.FromDisplayName) ? string.Empty : smtpSettings.FromDisplayName, string.IsNullOrEmpty(attachedPath) ? null : new List<string>() { attachedPath }, portalId,barcode);
                    }
                    else
                    {
                        ZnodeLogging.LogMessage("Email sending disabled for this store.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose);
                    }
                }
                else
                    SendEmailByPortal(to, string.IsNullOrEmpty(smtpSettings.FromEmailAddress) ? (string.IsNullOrEmpty(from)) ? ZnodeConfigManager.SiteConfig.AdminEmail : from : smtpSettings.FromEmailAddress, string.Empty, subject, body, true, "", portalId);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Mail sending to customer failed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error, ex);
            }
        }


        public static new void SendEmail(int portalId, string to, string from, string bcc, string subject, string body, bool isBodyHtml, ZnodePortalSmtpSetting smtpSettings, string userName, string attachedPath = "", string cc = "")
        {
            try
            {
                if (HelperUtility.IsNull(smtpSettings))
                    //Get smtp setting details.
                    smtpSettings = GetSMTPSetting(portalId);

                //Assign from email address and bcc if smtp details is not null.
                if (HelperUtility.IsNotNull(smtpSettings))
                {
                    //"Disable all emails" flag should only affect the procedure for any particular store. If the call to this method has no PortalId, It should not affect anything.
                    if (IsEmailSendingEnabled(smtpSettings))
                        SendEmail(string.IsNullOrEmpty(smtpSettings.FromEmailAddress) ? (string.IsNullOrEmpty(from)) ? ZnodeConfigManager.SiteConfig.AdminEmail : from : smtpSettings.FromEmailAddress, to, subject, body, cc, bcc, isBodyHtml, string.IsNullOrEmpty(smtpSettings.FromDisplayName) ? string.Empty : smtpSettings.FromDisplayName, string.IsNullOrEmpty(attachedPath) ? null : new List<string>() { attachedPath }, smtpSettings, userName, portalId);

                    else
                        ZnodeLogging.LogMessage("Email sending disabled for this store.", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose);
                }
                else
                    SendEmailByPortal(to, string.IsNullOrEmpty(smtpSettings.FromEmailAddress) ? (string.IsNullOrEmpty(from)) ? ZnodeConfigManager.SiteConfig.AdminEmail : from : smtpSettings.FromEmailAddress, string.Empty, subject, body, true, "", portalId);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Mail sending to customer failed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error, ex);
            }
        }

        //Checks for valid email address
        private static bool IsValidEmail(string from)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,63}){1,3})$");
            bool isValid = false;
            try
            {
                isValid = regex.Match(from).Success;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Email Address is not valid.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error, ex);
            }
            return isValid;
        }

        //Get smtp setting details.
        public static ZnodePortalSmtpSetting GetSMTPSetting(int portalId = 0)
        {
            IZnodeRepository<ZnodePortalSmtpSetting> _portalSmtpSetting = new ZnodeRepository<ZnodePortalSmtpSetting>();

            ZnodePortalSmtpSetting znodePortalSmtpSetting = null;

            //Get smtp setting details.
            if (portalId > 0)
                znodePortalSmtpSetting = _portalSmtpSetting.Table.FirstOrDefault(x => x.PortalId == portalId);

            //Get default portal SMTP
            if (znodePortalSmtpSetting == null)
                znodePortalSmtpSetting = _portalSmtpSetting.Table.FirstOrDefault(x => x.PortalId == ZnodeConfigManager.SiteConfig.PortalId);

            return znodePortalSmtpSetting;
        }

        /// <summary>
        /// Checks whether the email sending for this store/portal is enabled or not. Future developers can add their logic to this method to universally control the email sending.
        /// </summary>
        /// <param name="smtpSettings"></param>
        /// <returns></returns>
        private static bool IsEmailSendingEnabled(ZnodePortalSmtpSetting smtpSettings)
        {
            return !smtpSettings.DisableAllEmails;
        }

        public static string GetBccEmail(bool isEnableBcc, int portalId, string bcc = "")
        {
            string bccEmailId = string.Empty;
            //Get smtp setting details.
            if (isEnableBcc)
            {
                ZnodePortalSmtpSetting smtpSettings = GetSMTPSetting(portalId);
                bccEmailId = smtpSettings?.BccEmailAddress;
            }
            return bccEmailId;
        }

        #region Public Methods

        /// <summary>
        /// Sends email using SMTP, Uses default network credentials(Embed inline image)
        /// </summary>
        /// <param name="To">Email address of the recipient.</param>
        /// <param name="From">Email address of the sender.</param>
        /// <param name="BCC">Blind carbon copy email address.</param>
        /// <param name="Subject">The subject line of the email.</param>
        /// <param name="Body">The body of the email.</param>
        /// <param name="IsBodyHtml">Set to True to send this email in HTML format.</param>

        public static void SendEmail(string To, string From, string BCC, string Subject, bool IsBodyHtml) => SendCustomEmail(To, From, BCC, Subject, string.Empty, IsBodyHtml);

        /// <summary>
        /// Sends email using SMTP, Uses default network credentials.
        /// </summary>
        public static void SendEmail(string To, string From, string BCC, string Subject, string Body, bool IsBodyHtml) => SendCustomEmail(To, From, BCC, Subject, Body, IsBodyHtml);

        /// <summary>
        /// Sends email using SMTP, Uses default network credentials with barcode.
        /// </summary>
        public static void SendEmail(string To, string From, string BCC, string Subject, string Body, bool IsBodyHtml,string barcode) => SendCustomEmail(To, From, BCC, Subject, Body, IsBodyHtml,barcode);

        /// <summary>
        /// Sends email using SMTP, Uses default network credentials.
        /// </summary>
        public static void SendEmail(string To, string From, string BCC, string Subject, string Body, bool IsBodyHtml, ZnodePortalSmtpSetting znodePortalSmtpSetting) => SendCustomEmail(To, From, BCC, Subject, Body, IsBodyHtml, znodePortalSmtpSetting);

        /// <summary>
        /// Sends email using SMTP, Uses default network credentials.
        /// </summary>
        public static string SendEmail(string to, string from, string subject, string body) => SendEmail(from, to, subject, body, string.Empty, string.Empty, false, string.Empty, new List<string>());

        /// <summary>
        /// Sends email using SMTP, Uses default network credentials.
        /// </summary>
        public static string SendEmail(string to, string from, string subject, string body, string cc) => SendEmail(from, to, subject, body, cc, string.Empty, false, string.Empty, new List<string>());

        /// <summary>
        /// Sends email using SMTP, Uses default network credentials.
        /// </summary>
        public static string SendEmail(string to, string from, string subject, string body, string cc, string bcc) => SendEmail(from, to, subject, body, cc, bcc, false, string.Empty, new List<string>());

        /// <summary>
        /// Sends email using SMTP, Uses default network credentials.
        /// </summary>
        public static string SendEmail(string to, string from, string subject, string body, string cc, string bcc, List<string> attachmentFilePath) => SendEmail(from, to, subject, body, cc, bcc, false, string.Empty, attachmentFilePath);

        /// <summary>
        /// Sends email using SMTP, Uses default network credentials.
        /// </summary>
        public static string SendEmail(string to, string from, string subject, string body, string cc, string bcc, List<string> attachmentFilePath, bool isHTMLBody) => SendEmail(from, to, subject, body, cc, bcc, isHTMLBody, string.Empty, attachmentFilePath);

        /// <summary>
        /// Sends email using SMTP, Uses default network credentials.
        /// </summary>
        public static string SendEmail(string to, string from, string subject, string body, string cc, string bcc, List<string> attachmentFilePath, bool isHTMLBody, string fromAddressPrefix) => SendEmail(from, to, subject, body, cc, bcc, isHTMLBody, fromAddressPrefix, attachmentFilePath);

        /// <summary>
        /// Sends email using SMTP, Uses default network credentials.
        /// </summary>
        //public static string SendEmail(string to, string from, string subject, string body, string cc, string bcc, List<string> attachmentFilePath, bool isHTMLBody, string fromAddressPrefix) => SendEmail(from, to, subject, body, cc, bcc, isHTMLBody, fromAddressPrefix, attachmentFilePath);

        #endregion Public Methods

        #region Private Methods

        //Method used to Send an Emails based on the input types.
        private static string SendEmail(string from, string to, string subject, string body, string cc, string bcc, bool isHtmlEmail, string fromAddressPrefix, List<string> attachments, int portalId = 0,string barcode="")
        {
            MimeMessage message = SetEmailMessage(from, to, subject, body, cc, bcc, isHtmlEmail, fromAddressPrefix, attachments, portalId,barcode);
            ZnodePortalSmtpSetting znodePortalSmtpSetting = GetSMTPSetting(portalId);
            znodePortalSmtpSetting.UserName = new ZnodeEncryption().DecryptData(znodePortalSmtpSetting.UserName);
            return GetService<IZnodeEmail>().SendSMTPEmail(message, znodePortalSmtpSetting);
        }

        //Method used to Send an Emails based on the input types.
        private static string SendEmail(string from, string to, string subject, string body, string cc, string bcc, bool isHtmlEmail, string fromAddressPrefix, List<string> attachments, ZnodePortalSmtpSetting znodePortalSmtpSetting, string userName, int portalId = 0)
        {
            MimeMessage message = SetEmailMessage(from, to, subject, body, cc, bcc, isHtmlEmail, fromAddressPrefix, attachments, portalId);
            return GetService<IZnodeEmail>().SendSMTPEmail(message, znodePortalSmtpSetting);
        }

        // This method is responsible to send emails using MailKit SMTPClient.
        public virtual string SendSMTPEmail(MimeMessage message, ZnodePortalSmtpSetting znodePortalSmtpSetting)
        {
            if (HelperUtility.IsNull(znodePortalSmtpSetting))
            {
                znodePortalSmtpSetting = GetSMTPSetting();
                znodePortalSmtpSetting.UserName = new ZnodeEncryption().DecryptData(znodePortalSmtpSetting.UserName);
            }

            if (!znodePortalSmtpSetting.DisableAllEmails)
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    if (!znodePortalSmtpSetting.IsEnableSsl)
                    {
                        client.CheckCertificateRevocation = false;
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    }

                    if (znodePortalSmtpSetting.IsEnableSsl)
                        client.Connect(znodePortalSmtpSetting.ServerName, znodePortalSmtpSetting.Port.GetValueOrDefault(), znodePortalSmtpSetting.IsEnableSsl);
                    else
                        client.Connect(znodePortalSmtpSetting.ServerName, znodePortalSmtpSetting.Port.GetValueOrDefault());

                    //Note: only needed if the SMTP server requires authentication
                    if (!string.IsNullOrEmpty(znodePortalSmtpSetting.UserName) && !string.IsNullOrEmpty(znodePortalSmtpSetting.Password))
                        client.Authenticate(znodePortalSmtpSetting.UserName, znodePortalSmtpSetting.Password);

                    client.Send(message);
                    client.Disconnect(true);
                    return "SUCCESS";
                }
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.EmailSendingDisabledForStore, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose);
                return "FAILURE";
            }
        }

        private static MimeMessage SetEmailMessage(string from, string to, string subject, string body, string cc, string bcc, bool isHtmlEmail, string fromAddressPrefix, List<string> attachments, int portalId = 0,string barcode="")
        {
            char[] validSeperators = new char[] { ',', ':', ';' };
            string fromEmail = string.Empty;
            foreach (char seperator in validSeperators)
            {
                if (!string.IsNullOrEmpty(from) && from.Contains(seperator.ToString()))
                {
                    fromEmail = from.Split(seperator)[0];
                    break;
                }
            }
            from = !string.IsNullOrEmpty(fromEmail) ? fromEmail : from;
            bcc = bcc?.TrimEnd(',');
            cc = cc?.TrimEnd(',');

            //Email email = new Email(from, to, subject, body, cc, bcc, isHtmlEmail, fromAddressPrefix);

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromAddressPrefix, from));
            if (!string.IsNullOrEmpty(to))
            {
                message.To.AddRange(GetEmailList(to));
            }
            if (!string.IsNullOrEmpty(cc))
            {
                message.Cc.AddRange(GetEmailList(cc));
            }
            if (!string.IsNullOrEmpty(bcc))
            {
                if (Regex.IsMatch(bcc, "^((\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*)\\s*[,]{0,1}\\s*)+$", RegexOptions.IgnoreCase))
                {
                    message.Bcc.AddRange(GetEmailList(bcc));
                }
            }
            message.Subject = subject;
            var builder = new BodyBuilder();

            //If barcode is not null then embed barcode to mail body.
            if (!string.IsNullOrEmpty(barcode))
                EmbedBarcodeToEmailBody(barcode, builder);

            if (isHtmlEmail)
                builder.HtmlBody = body;
            else
                builder.TextBody = body;

            if (attachments?.Count > 0)
            {
                foreach (string attach in attachments)
                    builder.Attachments.Add(attach);
            }

            message.Body = builder.ToMessageBody();


            return message;
        }
        private static InternetAddressList GetEmailList(string address)
        {
            InternetAddressList addressList = new InternetAddressList();
            List<string> emailAddresses = address?.Split(',')?.Distinct()?.ToList();
            foreach (string email in emailAddresses)
            {
                if (!string.IsNullOrEmpty(email))
                    addressList.Add(new MailboxAddress(email));
            }
            return addressList;
        }
        //If portalId is greater than 0 set smtp settings according to portal id.
        private static Email SetEmail(Email email, int portalId, string cc, string bcc)
        {
            if (HelperUtility.IsNotNull(email))
            {
                //Get smtp setting details.
                ZnodePortalSmtpSetting smtpSettings = GetSMTPSetting(portalId);
                if (HelperUtility.IsNotNull(smtpSettings))
                {
                    email.SmtpServer = smtpSettings.ServerName;
                    email.SmtpPort = smtpSettings.Port.GetValueOrDefault();
                    email.SmtpSSL = smtpSettings.IsEnableSsl;

                    ZnodeEncryption encryption = new ZnodeEncryption();
                    smtpSettings.UserName = encryption.DecryptData(smtpSettings.UserName);

                    email.AuthenticateToServer(smtpSettings.UserName, smtpSettings.Password);
                    email.FromAddressPrefix = smtpSettings.FromDisplayName;
                }

                if (string.IsNullOrEmpty(smtpSettings?.FromEmailAddress))
                {
                    IZnodeRepository<ZnodePortal> _portal = new ZnodeRepository<ZnodePortal>();
                    email.FromAddress = _portal.Table.FirstOrDefault(x => x.PortalId == portalId)?.AdminEmail;
                }
                else
                    email.FromAddress = smtpSettings.FromEmailAddress;

            }
            return email;
        }

        //Method used to Send an Emails based on the input types.
        private static void SendCustomEmail(string To, string From, string BCC, string Subject, string Body, bool IsBodyHtml, string barcode="")
        {
            MimeMessage message = SendCustomEmailMessage(To, From, BCC, Subject, Body, IsBodyHtml,barcode);
            ZnodePortalSmtpSetting znodePortalSmtpSetting = GetSMTPSetting();
            znodePortalSmtpSetting.UserName = new ZnodeEncryption().DecryptData(znodePortalSmtpSetting.UserName);
            GetService<IZnodeEmail>().SendSMTPEmail(message, znodePortalSmtpSetting);
        }


        private static MimeMessage SendCustomEmailMessage(string To, string From, string BCC, string Subject, string Body, bool IsBodyHtml,string barcode="")
        {
            char[] validSeperators = new char[] { ',', ':', ';' };
            string fromEmail = string.Empty;
            foreach (char seperator in validSeperators)
            {
                if (!string.IsNullOrEmpty(From) && From.Contains(seperator.ToString()))
                {
                    fromEmail = From.Split(seperator)[0];
                    break;
                }
            }

            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(string.Empty, !string.IsNullOrEmpty(fromEmail) ? fromEmail : From));
            message.To.AddRange(GetEmailList(To));
            message.Subject = Subject;
            
            var builder = new BodyBuilder();
            //If barcode is not null then embed barcode to mail body.
            if (!string.IsNullOrEmpty(barcode))
                EmbedBarcodeToEmailBody(barcode, builder);

            if (IsBodyHtml)
                builder.HtmlBody = Body;
            else
                builder.TextBody = Body;

            message.Body = builder.ToMessageBody();

            return message;
        }

        //Method used to Send an Emails based on the input types.
        private static void SendCustomEmail(string To, string From, string BCC, string Subject, string Body, bool IsBodyHtml, ZnodePortalSmtpSetting znodePortalSmtpSetting)
        {
            MimeMessage message = SendCustomEmailMessage(To, From, BCC, Subject, Body, IsBodyHtml);
            if (HelperUtility.IsNull(znodePortalSmtpSetting))
            {
                znodePortalSmtpSetting = GetSMTPSetting();
                znodePortalSmtpSetting.UserName = new ZnodeEncryption().DecryptData(znodePortalSmtpSetting.UserName);
            }
            GetService<IZnodeEmail>().SendSMTPEmail(message, znodePortalSmtpSetting);
        }

        // This method used to send email using portal specific SMTP settings, if portal Id is not passed as argument then SMTP settings of default portal will be considered.
        public static void SendEmailByPortal(string To, string From, string BCC, string Subject, string Body, bool IsBodyHtml, string attachedPath, int portalId = 0)
        {
            char[] validSeperators = new char[] { ',', ':', ';' };
            string fromEmail = string.Empty;
            foreach (char seperator in validSeperators)
            {
                if (!string.IsNullOrEmpty(From) && From.Contains(seperator.ToString()))
                {
                    fromEmail = From.Split(seperator)[0];
                    break;
                }
            }

            MimeMessage message = BuildEmailMessage(To, From, BCC, Subject, Body, IsBodyHtml, attachedPath);

            ZnodePortalSmtpSetting znodePortalSmtpSetting = GetSMTPSetting(portalId);
            if (HelperUtility.IsNotNull(znodePortalSmtpSetting))
            {
                znodePortalSmtpSetting.UserName = new ZnodeEncryption().DecryptData(znodePortalSmtpSetting.UserName);
                GetService<IZnodeEmail>().SendSMTPEmail(message, znodePortalSmtpSetting);
            }

            //Remove specific file after sending mail to customer
            if (!string.IsNullOrEmpty(attachedPath))
                DeleteAttachedFile(attachedPath);
        }

        // This method is responsible to build email message.
        private static MimeMessage BuildEmailMessage(string To, string From, string BCC, string Subject, string Body, bool IsBodyHtml, string attachedPath)
        {
            //create mail message
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(From));
            message.To.Add(new MailboxAddress(To));
            if (!string.IsNullOrEmpty(To))
            {
                message.To.Add(new MailboxAddress(string.Empty, To));
            }
            if (!string.IsNullOrEmpty(BCC))
            {
                if (Regex.IsMatch(BCC, "^((\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*)\\s*[,]{0,1}\\s*)+$", RegexOptions.IgnoreCase))
                {
                    List<string> uniques = BCC?.Split(',')?.Distinct()?.ToList();
                    message.Bcc.Add(new MailboxAddress(string.Empty, string.Join(",", uniques)));
                }
            }
            message.Subject = Subject;
            BodyBuilder builder = new BodyBuilder();

            if (IsBodyHtml)
                builder.HtmlBody = Body;
            else
                builder.TextBody = Body;
            message.Subject = Subject;

            // Email Attachment
            if (!string.IsNullOrEmpty(attachedPath))
            {
                foreach (string attach in attachedPath.Split(','))
                    builder.Attachments.Add(attach);
            }
            message.Body = builder.ToMessageBody();
            return message;
        }

        //Method to remove attached file from specific path.
        private static void DeleteAttachedFile(string attachedPath)
        {
            try
            {
                File.SetAttributes(attachedPath, FileAttributes.Normal);
                File.Delete(attachedPath);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Deleting Attached File failed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error, ex);
            }
        }
        //Method to Embed barcode to bodybuilder object
        private static void EmbedBarcodeToEmailBody(string barcode, BodyBuilder builder)
        {
            Byte[] iconBytes = Convert.FromBase64String(barcode.Replace(ZnodeConstant.ReplaceKey, string.Empty).Trim());
            MemoryStream iconBitmap = new MemoryStream(iconBytes);
            var image = builder.LinkedResources.Add(ZnodeConstant.ReturnReceiptBarcode, iconBitmap, new ContentType(ZnodeConstant.Image, ZnodeConstant.Jpeg));
            image.ContentId = ZnodeConstant.ReturnReceiptBarcode;
        }

        #endregion Private Methods

        #region Email Class

        public class Email
        {
            #region Constructor

            public Email(string from, string to, string subject, string body, string cc, string bcc, bool isHtmlEmail, string fromAddressPrefix)
            {

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromAddressPrefix, from));
                if (!string.IsNullOrEmpty(to))
                {
                    message.To.AddRange(GetEmailList(to));
                }
                if (!string.IsNullOrEmpty(cc))
                {
                    message.Cc.AddRange(GetEmailList(cc));
                }
                if (!string.IsNullOrEmpty(bcc))
                {
                    if (Regex.IsMatch(bcc, "^((\\w+([-+.]\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*)\\s*[,]{0,1}\\s*)+$", RegexOptions.IgnoreCase))
                    {
                        message.Bcc.AddRange(GetEmailList(bcc));
                    }
                }
                message.Subject = Subject;
                message.Body = new TextPart("plain") { Text = Body };

                ZnodePortalSmtpSetting znodePortalSmtpSetting = GetSMTPSetting();
                znodePortalSmtpSetting.UserName = new ZnodeEncryption().DecryptData(znodePortalSmtpSetting.UserName);
                GetService<IZnodeEmail>().SendSMTPEmail(message, znodePortalSmtpSetting);
            }

            #endregion Constructor

            #region Public members

            /// <summary>
            /// The status of the mail being sent using the async method
            /// </summary>
            public static string MailStatus { get; set; } = string.Empty;

            /// <summary>
            /// Notifies the calling application of the email status
            /// </summary>
            public static event EventHandler NotifyCaller;

            #endregion Public members

            #region Properties

            #region Private

            /// <summary>
            /// Gets the SmtpClient
            /// </summary>
            private System.Net.Mail.SmtpClient MailClient { get; set; }

            /// <summary>
            /// Gets the MailMessage
            /// </summary>
            //private MailMessage MailMessage { get; set; }

            #endregion Private

            #region Public

            /// <summary>
            /// Gets or Sets the address the email is from
            /// </summary>
            public string FromAddress { get; set; }

            public string FromAddressPrefix { get; set; }

            /// <summary>
            /// Gets or Sets the subject of the email
            /// </summary>
            public string Subject { get; set; }

            /// <summary>
            /// Gets or Sets the body of the email
            /// </summary>
            public string Body { get; set; }

            /// <summary>
            /// Gets or Sets the smtp server address
            /// </summary>
            public string SmtpServer { get; set; }

            /// <summary>
            /// Gets or Sets if this is a html email.  True for html false for text
            /// </summary>
            public bool IsHtmlEmail { get; set; }

            /// <summary>
            ///
            /// </summary>
            public bool SmtpSSL { get; set; }

            /// <summary>
            /// Gets / sets the port number.
            /// </summary>
            public int SmtpPort { get; set; }

            /// <summary>
            /// Gets/ Sets the Priority of the Mail.
            /// </summary>
            //public MailPriority Priority { get; set; }

            public bool AsyncEmail { get; set; }

            #endregion Public

            #endregion Properties

            #region protected Methods

            /// <summary>
            /// Notifies the calling application about the status of the email being sent
            /// </summary>
            protected static void OnNotifyCaller()
            {
                if (!Equals(NotifyCaller, null))
                {
                    NotifyCaller(MailStatus, EventArgs.Empty);
                }
            }

            #endregion protected Methods

            #region Authentication Methods

            /// <summary>
            /// Creates authentication credentials that are passed to the mail client
            /// to access the SMTP server. To use this method
            /// use createMailObjects, set the properties for the email information
            /// </summary>
            /// <param name="username">The users account to access the SMTP server</param>
            /// <param name="password">The users password to access the SMTP server</param>
            public void AuthenticateToServer(string username, string password)
            {
                NetworkCredential credentials = new NetworkCredential(username, password);

                MailClient.Credentials = credentials;
            }

            /// <summary>
            /// Credentials that are passed to the mail client to access the SMTP server
            /// </summary>
            /// <param name="credentials">Network credential to access the SMTP server</param>
            public void AuthenticateToServer(System.Net.NetworkCredential credentials) => MailClient.Credentials = credentials;

            #endregion Authentication Methods

            #region Attachments Methods

            /// <summary>
            /// This method is use to Add the collection of attachment.
            /// </summary>
            /// <param name="filePaths">File path Collection</param>
            //public void AddAttachments(List<string> filePaths)
            //{
            //    foreach (string filePath in filePaths)
            //    {
            //        AddAttachment(filePath);
            //    }
            //}

            /// <summary>
            /// Creates an attachment and adds it to the mail message
            /// </summary>
            /// <param name="path">The path to the attachment</param>
            //public void AddAttachment(string path)
            //{
            //    Attachment attachData = new Attachment(path);

            //    MailMessage.Attachments.Add(attachData);
            //}

            /// <summary>
            /// Adds an attachment to the mail message
            /// </summary>
            /// <param name="attachData">An attachment object</param>
           // public void AddAttachment(Attachment attachData) => MailMessage.Attachments.Add(attachData);

            #endregion Attachments Methods

            #region Add Recipient Addresses Methods

            /// <summary>
            /// Used to add a to address to the To Address collection
            /// </summary>
            /// <param name="toAddress">The email address to add</param>
            //private void AddToAddress(string toAddress) => MailMessage.To.Add(toAddress);

            /// <summary>
            /// Used to add a cc address to the CC Address collection
            /// </summary>
            /// <param name="AddressForCC">The email address to add</param>
            //public void AddCCAddress(string addressForCC) => MailMessage.CC.Add(addressForCC);

            /// <summary>
            /// Used to add a bcc address to the Bcc Address collection
            /// </summary>
            /// <param name="AddressForBcc">The email address to add</param>
            //public void AddBCCAddress(string addressForBcc) => MailMessage.Bcc.Add(addressForBcc);

            #endregion Add Recipient Addresses Methods

            #region Event

            private static void MailClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
            {
                string token = (string)e.UserState;

                if (e.Cancelled)
                {
                    MailStatus = $" {token} Send canceled.";
                }

                if (!Equals(e.Error, null))
                {
                    MailStatus = $"Error on {token }: {e.Error.ToString()}";
                }
                else
                {
                    MailStatus = $"{token} mail sent.";
                }

                OnNotifyCaller();
            }

            #endregion Event
        }

        #endregion Email Class
    }
}
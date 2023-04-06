using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Diagnostics;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.MediaStorage
{
    public class LocalAgent : IConnector
    {
        public LocalAgent(FileUploadPolicyModel policymodel)
        {
            UploadPolicyModel = policymodel;
        }

        public FileUploadPolicyModel UploadPolicyModel { get; private set; }
        public string Upload(MemoryStream stream, string fileName, string folderName)
        {
            //Retrive physical path of file 
            string path = Path.Combine(HttpContext.Current.Server.MapPath($"~/{UploadPolicyModel.BucketName}/{folderName}"), fileName);

            //write to file
            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                stream.WriteTo(file);
            }

            //Return complete path of file
            return $"{ZnodeAdminSettings.ZnodeAdminUri}/{UploadPolicyModel.BucketName}/{Path.GetFileName(fileName)}";
        }

        public List<string> Upload(List<HttpPostedFileBase> files) => new List<string>();

        public List<string> Delete(string fileName, string folderName)
        {
            List<string> deletedMedia = new List<string>();
            if (!string.IsNullOrEmpty(fileName))
            {
                foreach (string _fileName in fileName.Split(','))
                {
                    string serverPath = string.IsNullOrEmpty(folderName) ? $"~/{UploadPolicyModel.BucketName}" : $"~/{UploadPolicyModel.BucketName}/{folderName}";

                    //Retrive physical path of file 
                    string filePath = Path.Combine(HttpContext.Current.Server.MapPath(serverPath), _fileName);
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            File.Delete(filePath);
                            deletedMedia.Add(_fileName);
                        }
                        catch (Exception ex)
                        {
                            ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                        }
                    }
                }
            }
            return deletedMedia;
        }

        public string GetServerUrl()
            => $"{HttpContext.Current.Request.Url.Scheme + "://"}{HttpContext.Current.Request.Url.Authority}/{UploadPolicyModel.BucketName}/";

        //For copying data 
        public Dictionary<string, long> Copy(string fileName, string folderName)
        {
            //For Future implementation
            return new Dictionary<string, long>();
        }
    }
}

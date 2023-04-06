using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Blobs.Models;

namespace Znode.Libraries.MediaStorage
{
    public class AzureAgent : IConnector
    {
        public AzureAgent(FileUploadPolicyModel policymodel)
        {
            UploadPolicyModel = policymodel;
        }

        public static FileUploadPolicyModel UploadPolicyModel { get; private set; }

        public string Upload(MemoryStream stream, string fileName, string folderName)
        {
            var filePath = string.IsNullOrEmpty(folderName) ? $"{fileName}" : $"{folderName}/{fileName}";

            //Format connection string
            string connectionString = $"DefaultEndpointsProtocol=https;AccountName={UploadPolicyModel.PublicKey};AccountKey={ UploadPolicyModel.PrivateKey}";

            //Create BlobServiceClient from the connection string.
            BlobServiceClient blobClient = new BlobServiceClient(connectionString);

            // Get and create the container for the blobs
            BlobContainerClient container = blobClient.GetBlobContainerClient(UploadPolicyModel.BucketName);
            container.CreateIfNotExists();

            // Retrieve reference to a blob named "myBlob".
            BlockBlobClient blockBlob = container.GetBlockBlobClient(filePath);
            
            BlobHttpHeaders blobHttpHeaders = new BlobHttpHeaders();

            // Set the MIME ContentType every time the properties 
            // are updated or the field will be cleared
            blobHttpHeaders.ContentType = ZnodeMimeMapping.GetMimeMapping(fileName);

            //Set the stream position to zero, otherwise it saves thumbnail images with 0 Kb size.
            stream.Position = 0;

            // Create or overwrite the "myBlob" blob with contents from a local file.
            blockBlob.Upload(stream);

            // Purge single file.
            PurgeContentFromAzure(fileName);

            //To delete cache of file after specified interval of time.
            if (!string.IsNullOrEmpty(ZnodeApiSettings.CacheControl)) { blobHttpHeaders.CacheControl = ZnodeApiSettings.CacheControl; }
            else { blobHttpHeaders.CacheControl = ZnodeApiSettings.DefaultCacheControl; }

            // Set the blob's properties.
            blockBlob.SetHttpHeaders(blobHttpHeaders);

            return blockBlob.Uri.ToString();

        }

        public List<string> Delete(string fileName, string folderName)
        {
            List<string> deletedMedia = new List<string>();
            if (!string.IsNullOrEmpty(fileName))
            {
                //Format connection string
                string connectionString = $"DefaultEndpointsProtocol=https;AccountName={UploadPolicyModel.PublicKey};AccountKey={ UploadPolicyModel.PrivateKey}";

                //Create BlobServiceClient from the connection string.
                BlobServiceClient blobClient = new BlobServiceClient(connectionString);

                // Get and create the container for the blobs
                BlobContainerClient container = blobClient.GetBlobContainerClient(UploadPolicyModel.BucketName);
                container.CreateIfNotExists();

                var fileNameArray = fileName.Split(',');
                foreach (string _fileName in fileNameArray)
                {
                    try
                    {
                        string filePath = string.IsNullOrEmpty(folderName) ? _fileName : $"{folderName}/{_fileName}";

                        // Retrieve reference to a blob named "myBlob.txt".
                        BlockBlobClient blockBlob = container.GetBlockBlobClient(filePath);

                        // Delete the blob.
                        blockBlob.Delete();
                        deletedMedia.Add(_fileName);

                        // Purge single file.                    
                        PurgeContentFromAzure(filePath);
                    }
                    catch(Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                    }
                }
            }
            return deletedMedia;
        }

        public List<string> Upload(List<HttpPostedFileBase> files) => new List<string>();

        public string GetServerUrl()
        {
            //Format connection string
            string connectionString = $"DefaultEndpointsProtocol=https;AccountName={UploadPolicyModel.PublicKey};AccountKey={ UploadPolicyModel.PrivateKey}";

            //Create BlobServiceClient from the connection string.
            BlobServiceClient blobClient = new BlobServiceClient(connectionString);

            // Get and create the container for the blobs
            BlobContainerClient container = blobClient.GetBlobContainerClient(UploadPolicyModel.BucketName);
            container.CreateIfNotExists();

            return container.Uri.OriginalString;
        }

        //For copying data 
        public Dictionary<string, long> Copy(string fileName, string folderName)
        {
            string filePath = string.IsNullOrEmpty(folderName) ? fileName : $"{folderName}/{fileName}";

            Dictionary<string, long> AzureBlobFile = new Dictionary<string, long>();
            //Format connection string
            string connectionString = $"DefaultEndpointsProtocol=https;AccountName={UploadPolicyModel.PublicKey};AccountKey={ UploadPolicyModel.PrivateKey}";

            //Create BlobServiceClient from the connection string.
            BlobServiceClient blobClient = new BlobServiceClient(connectionString);

            // Get and create the container for the blobs
            BlobContainerClient container = blobClient.GetBlobContainerClient(UploadPolicyModel.BucketName);
            container.CreateIfNotExists();

            BlockBlobClient blockBlob = container.GetBlockBlobClient(filePath);

            blockBlob.StartCopyFromUri(blockBlob.Uri);

            // Purge single file.                    
            PurgeContentFromAzure(filePath);

            return AzureBlobFile;
        }
        // Get Authentication token key from Azure Active Directory
        private static string GetAuthTokenByCredentials()
        {
            //Creating the variable for result
            string token = string.Empty;

            string clientId = ZnodeApiSettings.ClientId;
            string clientSecret = ZnodeApiSettings.ClientSecret;
            string tenanatId = ZnodeApiSettings.TenanatId;

            if (!string.IsNullOrEmpty(clientId))
            {
                //Creating the Authentication Context
                var authenticationContext = new AuthenticationContext("https://login.microsoftonline.com/" + tenanatId);
                //Creating Credentials
                ClientCredential clientCredential = new ClientCredential(clientId, clientSecret);
                //Fetching Token from Azure AD
                Task<AuthenticationResult> resultstr = authenticationContext.AcquireTokenAsync("https://management.core.windows.net/", clientCredential);

                // Checking if data received from Azure AD
                if (resultstr == null)
                {
                    throw new InvalidOperationException("Failed to obtain the JWT token");
                }
                //Getting token
                token = resultstr.Result.AccessToken;
            }

            //Returning the token
            return token;
        }

        // Purge single file from Azure
        private static string PurgeContentFromAzure(string fileName)
        {
            string result = string.Empty;

            string subscriptions = ZnodeApiSettings.SubscriptionId;
            string resourceGroups = ZnodeApiSettings.ResourceGroups;
            string profiles = ZnodeApiSettings.Profiles;
            string endpoints = ZnodeApiSettings.EndPoints;

            if (!string.IsNullOrEmpty(subscriptions))
            {
                // Get Authentication token key from Azure Active Directory
                string token = GetAuthTokenByCredentials();



                string purgeApiUrl= string.Format("https://management.azure.com/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.Cdn/profiles/{2}/endpoints/{3}/purge?api-version=2016-10-02", subscriptions, resourceGroups, profiles, endpoints);

                using (WebClient client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);
                    client.Headers.Add("api-version", "2016-10-02");
                    client.Headers.Add("Content-Type", "application/json");

                    string filepath = string.Empty;
                    dynamic content = new { ContentPaths = new List<string>() { string.Format("/{1}/{0}", fileName, UploadPolicyModel.BucketName), string.Format("/{1}/Thumbnail/{0}", fileName, UploadPolicyModel.BucketName) } };
                    string bodyText = JsonConvert.SerializeObject(content);

                    try
                    {
                        result = client.UploadString(purgeApiUrl, bodyText);
                    }
                    catch (Exception ex)
                    {
                        result = "Exception -" + ex.Message + "StackTrace" + ex.StackTrace;
                    }
                }
            }

            return result;
        }
    }
}

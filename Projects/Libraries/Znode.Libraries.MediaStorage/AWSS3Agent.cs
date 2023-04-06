using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.MediaStorage
{
    public class AWSS3Agent : IConnector
    {
        public AWSS3Agent(FileUploadPolicyModel policymodel)
        {
            UploadPolicyModel = policymodel;
        }

        public FileUploadPolicyModel UploadPolicyModel { get; private set; }
        public string Upload(MemoryStream stream, string fileName, string folderName)
        {
            IAmazonS3 client;
            using (client = AWSClientFactory.CreateAmazonS3Client(UploadPolicyModel.PublicKey, UploadPolicyModel.PrivateKey, RegionEndpoint.USEast1))
            {
                var filePath = string.IsNullOrEmpty(folderName) ? $"{fileName}" : $"{folderName}/{fileName}";
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = UploadPolicyModel.BucketName,
                    CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESSIBLE
                    Key = filePath,
                    InputStream = stream//SEND THE FILE STREAM
                };

                client.PutObject(request);
                GetPreSignedUrlRequest expiryUrlRequest = new GetPreSignedUrlRequest() { BucketName = UploadPolicyModel.BucketName, Key = filePath, Expires = DateTime.Now.AddDays(10) };

                return client.GetPreSignedURL(expiryUrlRequest).Split('?')[0];
            }

        }


        public List<string> Upload(List<HttpPostedFileBase> files)
        {
            IAmazonS3 client;
            List<string> _result = new List<string>();
            using (client = Amazon.AWSClientFactory.CreateAmazonS3Client(UploadPolicyModel.PublicKey, UploadPolicyModel.PrivateKey, RegionEndpoint.USEast1))
            {
                files.ForEach(_file =>
                {
                    var request = new PutObjectRequest()
                    {
                        BucketName = UploadPolicyModel.BucketName,
                        CannedACL = S3CannedACL.PublicRead,//PERMISSION TO FILE PUBLIC ACCESIBLE
                        Key = $"UPLOADS/{_file.FileName}",
                        InputStream = _file.InputStream//SEND THE FILE STREAM
                    };

                    client.PutObject(request);

                    var expiryUrlRequest = new GetPreSignedUrlRequest()
                    {
                        BucketName = UploadPolicyModel.BucketName,
                        Key =
                        $"UPLOADS/{_file.FileName}",
                        Expires = DateTime.Now.AddDays(10)
                    };

                    _result.Add(client.GetPreSignedURL(expiryUrlRequest).Split('?')[0]);

                });
            }

            return _result;
        }

        public List<string> Delete(string fileName, string folderName)
        {
            List<string> deletedMedia = new List<string>();
            if (!string.IsNullOrEmpty(fileName))
            {
                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(UploadPolicyModel.PublicKey, UploadPolicyModel.PrivateKey, RegionEndpoint.USEast1))
                {
                    var fileNameArray = fileName.Split(',');
                    foreach (string _fileName in fileNameArray)
                    {
                        try
                        {
                            DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
                            {
                                BucketName = UploadPolicyModel.BucketName,
                                Key = string.IsNullOrEmpty(folderName) ? _fileName : $"{folderName}/{_fileName}"
                            };

                            client.DeleteObject(deleteObjectRequest);
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

        public List<string> DeleteFolder(string fileName, string folderName)
        {
            List<string> deletedMedia = new List<string>();
            if (!string.IsNullOrEmpty(folderName))
            {
                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client(UploadPolicyModel.PublicKey, UploadPolicyModel.PrivateKey, RegionEndpoint.USEast1))
                {
                    try
                    {

                        ListObjectsRequest request = new ListObjectsRequest();
                        request.BucketName = UploadPolicyModel.BucketName;
                        request.Prefix = folderName;
                        request.Delimiter = "/";

                       ListObjectsResponse list = client.ListObjects(request);
                       DeleteObjectsRequest multiObjectDeleteRequest = new DeleteObjectsRequest();

                        multiObjectDeleteRequest.BucketName = UploadPolicyModel.BucketName;
                        foreach (var subFolder in list.CommonPrefixes)
                            /* list the sub-folders */
                            multiObjectDeleteRequest.AddKey(subFolder);

                        client.DeleteObjects(multiObjectDeleteRequest);
                    }
                    catch (AmazonS3Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                    }
                }
            }
            return deletedMedia;
        }

        public string GetServerUrl()
        {
            IAmazonS3 client;
            using (client = AWSClientFactory.CreateAmazonS3Client(UploadPolicyModel.PublicKey, UploadPolicyModel.PrivateKey, RegionEndpoint.USEast1))
            {
                GetPreSignedUrlRequest expiryUrlRequest = new GetPreSignedUrlRequest() { BucketName = UploadPolicyModel.BucketName, Expires = DateTime.Now.AddDays(10) };
                return client.GetPreSignedURL(expiryUrlRequest).Split('?')[0];
            }
        }

        public Dictionary<string, long> Copy(string fileName, string folderName)
        {
            try
            {
                IAmazonS3 client;
                ListObjectsResponse response;
                Dictionary<string, long> AmazonS3File = new Dictionary<string, long>();
                ListObjectsRequest request = new ListObjectsRequest
                {
                    BucketName = folderName
                };
                do
                {
                    using (client = AWSClientFactory.CreateAmazonS3Client(UploadPolicyModel.PublicKey, UploadPolicyModel.PrivateKey, RegionEndpoint.USEast1))
                    {
                        response = client.ListObjects(request);
                    }
                    // Process response.
                    foreach (S3Object entry in response.S3Objects)
                        AmazonS3File.Add(entry.Key, entry.Size);
                    request.Marker = response.NextMarker;
                } while (response.IsTruncated);

                return AmazonS3File;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                throw amazonS3Exception;
            }
        }
    }
}



namespace Znode.Libraries.MediaStorage
{
    public class FileUploadPolicyModel
    {
        public string PublicKey { get; private set; }
        public string PrivateKey { get; private set; }
        public string BucketName { get; private set; }
        public string ThumbnailFolderName { get; set; }
        public string NetworkURL { get; set; }
        public string NetworkDrivePath { get; set; }
        public FileUploadPolicyModel(string publickey, string privatekey, string bucketname, string thumbnailFolderName, string networkDrive, string networkPathUrl)
        {
            PublicKey = publickey;
            PrivateKey = privatekey;
            BucketName = bucketname;
            ThumbnailFolderName = thumbnailFolderName;
            NetworkURL = networkDrive;
            NetworkDrivePath = networkPathUrl;

        }
    }
}

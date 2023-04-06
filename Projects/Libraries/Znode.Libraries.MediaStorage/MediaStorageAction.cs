namespace Znode.Libraries.MediaStorage
{
    public static class MediaStorageAction
    {
        public const string Upload = "Upload";
        public const string Copy = "Copy";
        public const string Delete = "Delete";
        public const string Move = "Move";

    }
    public static class MediaStorageAgent
    {
        public const string AWSS3Agent = "AWSS3Agent";
        public const string AzureAgent = "AzureAgent";
        public const string LocalAgent = "LocalAgent";
    }

    
}

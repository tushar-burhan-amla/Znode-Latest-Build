namespace Znode.Engine.Admin.ViewModels
{
    public class TreeResponseMessage
    {
        public string Message { get; set; }
        public string FolderJsonTree { get; set; }
        public bool HasNoError { get; set; }
        public int Id { get; set; }
    }
}
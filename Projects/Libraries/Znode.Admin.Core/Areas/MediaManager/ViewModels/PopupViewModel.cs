namespace Znode.Engine.Admin.ViewModels
{
    public class PopupViewModel
    {
        public int FolderId { get; set; }
        public bool IsPopup { get; set; }
        public bool IsMultiSelect { get; set; } = true;
        public string IsOverrideFile { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    
    }
}
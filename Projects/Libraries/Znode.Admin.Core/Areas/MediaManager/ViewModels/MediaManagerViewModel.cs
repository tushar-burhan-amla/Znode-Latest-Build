using System.Web;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class MediaManagerViewModel: BaseViewModel
    {
        public TreebuttonViewModel TreeView { get; set; }
        public GridModel GridModel { get; set; }
        public PopupViewModel PopupViewModel { get; set; }
        public int ExportFileTypeId { get; set; }
        public int MediaId { get; set; }
        public string Path { get; set; }
        public string Size { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public string Type { get; set; }
        public string Length { get; set; }
        public string MediaType { get; set; }
        public int FolderId { get; set; }
        public string FileName { get; set; }
        public string ShortDescription { get; set; }
        public int MediaCategoryId { get; set; }
        public int MediaPathId { get; set; }
        public string Folder { get; set; }
        public string OriginalImagePath { get; set; }
        public string FamilyCode { get; set; }
        public string  DisplayName { get; set; }
        public HttpPostedFileBase FilePath { get; set; }
        public bool IsMediaReplace { get; set; }
    }
}
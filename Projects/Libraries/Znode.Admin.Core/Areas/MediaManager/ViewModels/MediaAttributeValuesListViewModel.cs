using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class MediaAttributeValuesListViewModel
    {
        public List<MediaAttributeValuesViewModel> MediaAttributeValues { get; set; }

        public string FileName { get; set; }
        public string Size { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public string Length { get; set; }
        public string Type { get; set; }
        public string CreatedDate { get; set; }
        public string ModifiedDate { get; set; }
        public int MediaId { get; set; }
        public string MediaPath { get; set; }
        public int? MediaPathId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSources, ResourceType = typeof(Admin_Resources))]
        public string MediaVirtualPath { get; set; }
        public string FamilyCode { get; set; }
        public string Path { get; set; }
        public string ShortDescription { get; set; }
        public string OriginalImagePath { get; set; }
        public NavigationViewModel navigationModel { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFileName, ResourceType = typeof(Admin_Resources))]
        [UIHint("FileUploader")]
        public HttpPostedFileBase FilePath { get; set; }
        public bool IsMediaReplace { get; set; }

        public MediaAttributeValuesListViewModel()
        {
            MediaAttributeValues = new List<MediaAttributeValuesViewModel>();
            navigationModel = new NavigationViewModel();
        }
    }
}
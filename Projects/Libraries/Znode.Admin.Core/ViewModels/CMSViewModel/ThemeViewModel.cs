using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ThemeViewModel : BaseViewModel
    {
        public int? CMSThemeId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelThemeName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredThemeName)]
        [RegularExpression(AdminConstants.FileNameRegularExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorThemeNameCharacter)]
        public string Name { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredParentTheme)]
        [Display(Name = ZnodeAdmin_Resources.LabelParentThemeName, ResourceType = typeof(Admin_Resources))]
        public int? ParentThemeId { get; set; }

        public string ParentThemeName { get; set; }

        [Display(Name=ZnodeAdmin_Resources.LabelIsParentTheme, ResourceType = typeof(Admin_Resources))]
        public bool IsParentTheme { get; set; }

        public string AssetType { get; set; }

        [FileTypeValidation(AdminConstants.ZipFileType, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ImageFileTypeErrorMessage)]
        [Display(Name = ZnodeAdmin_Resources.LabelFileName, ResourceType = typeof(Admin_Resources))]
        [UIHint("FileUploader")]
        public HttpPostedFileBase FilePath { get; set; }
        public List<CSSViewModel> CssList { get; set; }
        public bool IsFilePathExists { get; set; }

        public int CMSThemeAssetId { get; set; }
        public string Assets { get; set; }
        public string PageTitle { get; set; }
        public int CMSAssetId { get; set; }
        public List<ThemeAssetViewModel> ThemeAssets { get; set; }
        public Dictionary<int, string> ProductTypes { get; set; }
        public List<PDPAssetViewModel> PDPAssets { get; set; }
        public List<SelectListItem> ThemeList { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.Web;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CSSViewModel : BaseViewModel
    {
        public int CMSThemeCSSId { get; set; }
        public int CMSThemeId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelCSSName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredCSS)]
        public string CSSName { get; set; }
        public string ThemeName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelBrowseFile, ResourceType = typeof(Admin_Resources))]
        [FileTypeValidation(AdminConstants.CSSFileType, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ImageFileTypeErrorMessage)]
        [UIHint("FileUploader")]
        public HttpPostedFileBase[] FilePath { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.Web;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ImportViewModel : BaseViewModel
    {
        [Display(Name = ZnodeWebStore_Resources.LabelFileName, ResourceType = typeof(WebStore_Resources))]
        [FileMaxSizeValidation(WebStoreConstants.DocumentMaxFileSize, ErrorMessageResourceType = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.FileSizeExceededErrorMessage)]
        [UIHint("FileUploader")]
        public HttpPostedFileBase ImportData { get; set; }

        public ImportProcessLogsListViewModel ImportProcessLogsListViewModel { get; set; }
    }
}

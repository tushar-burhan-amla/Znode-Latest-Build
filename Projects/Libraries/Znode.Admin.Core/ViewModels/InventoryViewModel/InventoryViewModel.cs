using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class InventoryViewModel : BaseViewModel
    {
        public int InventoryListId { get; set; }
        public int WarehouseId { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorListCode)]
        [Display(Name = ZnodeAdmin_Resources.LabelListCode, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.InventoryListCodeRequiredMessage)]
        public string ListCode { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorListName)]
        [Display(Name = ZnodeAdmin_Resources.LabelName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.InventoryListNameRequiredMessage)]
        public string ListName { get; set; }
        public string WarehouseName { get; set; }

        public HttpPostedFileBase ImportFile { get; set; }
        public GridModel GridModel { get; set; }
        public List<ImportInventoryViewModel> ImportInventoryList { get; set; }
        public List<SelectListItem> FileTypes { get; set; }
    }
}


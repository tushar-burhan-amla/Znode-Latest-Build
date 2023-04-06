using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class GlobalAttributeFamilyViewModel : BaseViewModel
    {
        public int GlobalAttributeFamilyId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredAttributeFamilyCode, ErrorMessageResourceType = typeof(Admin_Resources))]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericStartWithAlphabet, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelFamilyCode, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.AttributeFamilyCode, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string FamilyCode { get; set; }
        public string AttributeFamilyName { get; set; }
        public List<SelectListItem> GlobalEntityType { get; set; }
        public string EntityName { get; set; }
        public int GlobalEntityId { get; set; }
    }
}

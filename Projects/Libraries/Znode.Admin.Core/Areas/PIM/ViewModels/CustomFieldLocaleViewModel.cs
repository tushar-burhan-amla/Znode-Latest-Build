using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class CustomFieldLocaleViewModel : BaseViewModel
    {
        public int CustomFieldLocaleId { get; set; }
        public int? CustomFieldId { get; set; }
        public string LocaleId { get; set; }

        [Required(ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.ErrorKeyRequired)]
        public string CustomKey { get; set; }

        [Required(ErrorMessageResourceType = typeof(PIM_Resources), ErrorMessageResourceName = ZnodePIM_Resources.ErrorValueRequired)]
        public string CustomKeyValue { get; set; }
    }
}
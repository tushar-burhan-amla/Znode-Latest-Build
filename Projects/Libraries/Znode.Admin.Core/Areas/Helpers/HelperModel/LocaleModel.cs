using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.AttributeValidationHelpers
{
    public class LocaleDataModel
    {
        public int LocaleId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.PleaseEnterAttributeLocale, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public string Value { get; set; }
    }
}
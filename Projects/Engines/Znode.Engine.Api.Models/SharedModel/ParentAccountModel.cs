using System.ComponentModel.DataAnnotations;

using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class ParentAccountModel : BaseModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.ErrorAccountCodeRequired)]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeApi_Resources.AlphanumericOnlyWithNoSpaces, ErrorMessageResourceType = typeof(Api_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeApi_Resources.AccountCodeMaxLength, ErrorMessageResourceType = typeof(Api_Resources))]
        public string AccountCode { get; set; }

        public int AccountId { get; set; }
        public string Name { get; set; }
        public int? ParentAccountId { get; set; }
        public string ParentAccountName { get; set; }
    }
}

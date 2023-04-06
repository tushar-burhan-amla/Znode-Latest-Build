using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeFamilyCreateModel  :BaseModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeApi_Resources.RequiredFamilyCode, ErrorMessageResourceType = typeof(Api_Resources))]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeApi_Resources.AlphaNumericOnly, ErrorMessageResourceType = typeof(Api_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeApi_Resources.AttributeFamilyCode, ErrorMessageResourceType = typeof(Api_Resources))]
        public string FamilyCode { get; set; }

        [Required]
        public string GlobalEntityName { get; set; }
        public int GlobalEntityId { get; set; }

        public List<GlobalAttributeFamilyLocaleModel> AttributeFamilyLocales { get; set; }
    }
}

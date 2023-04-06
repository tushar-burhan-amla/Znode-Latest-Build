using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class PIMAttributeFamilyModel : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredFamilyCode)]
        public string FamilyCode { get; set; }
        public string AttributeFamilyName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredFamilyCode)]
        public int ExistingAttributeFamilyId { get; set; }

        public int PimAttributeFamilyId { get; set; }
        public bool IsDefaultFamily { get; set; }
        public bool IsSystemDefined { get; set; }
        public bool IsCategory { get; set; }
    }
}

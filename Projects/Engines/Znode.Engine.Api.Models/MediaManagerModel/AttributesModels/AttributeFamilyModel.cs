using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class AttributeFamilyModel : BaseModel
    {
        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredFamilyCode)]
        public string FamilyCode { get; set; }
        public string LocaleName { get; set; }
        public int MediaAttributeFamilyId { get; set; }
        public int ExistingAttributeFamilyId { get; set; }
        public int LocaleId { get; set; }
        public bool IsDefaultFamily { get; set; }
        public bool IsSystemDefined { get; set; }
        public string AttributeFamilyName { get; set; }
    }
}

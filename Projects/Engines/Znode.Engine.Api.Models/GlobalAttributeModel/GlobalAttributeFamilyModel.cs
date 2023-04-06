using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeFamilyModel : BaseModel
    {
        public int GlobalAttributeFamilyId { get; set; }
        public string FamilyCode { get; set; }
        public List<GlobalAttributeFamilyLocaleModel> AttributeFamilyLocales { get; set; }
        public string AttributeFamilyName { get; set; }
        public int GlobalEntityId { get; set; }
        public string EntityName { get; set; }
    }
}

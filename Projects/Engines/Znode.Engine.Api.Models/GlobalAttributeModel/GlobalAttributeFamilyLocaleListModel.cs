using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeFamilyLocaleListModel : BaseListModel
    {
        public string FamilyCode { get; set; }
        public List<GlobalAttributeFamilyLocaleModel> AttributeFamilyLocales { get; set; }
    }
}

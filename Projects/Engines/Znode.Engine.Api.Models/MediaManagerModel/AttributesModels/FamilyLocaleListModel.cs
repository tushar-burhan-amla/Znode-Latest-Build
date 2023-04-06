using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class FamilyLocaleListModel : BaseListModel
    {
        public List<FamilyLocaleModel> FamilyLocales { get; set; }

        public string FamilyCode { get; set; }
    }
}

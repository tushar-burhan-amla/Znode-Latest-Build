using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PIMFamilyLocaleListModel : BaseListModel
    {
        public List<PIMFamilyLocaleModel> FamilyLocales { get; set; }

        public string FamilyCode { get; set; }
    }
}

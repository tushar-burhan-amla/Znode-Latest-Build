using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PIMAttributeFamilyListModel : BaseListModel
    {
        public List<PIMAttributeFamilyModel> PIMAttributeFamilies { get; set; }

        public PIMAttributeFamilyListModel()
        {
            PIMAttributeFamilies = new List<PIMAttributeFamilyModel>();
        }
    }
}

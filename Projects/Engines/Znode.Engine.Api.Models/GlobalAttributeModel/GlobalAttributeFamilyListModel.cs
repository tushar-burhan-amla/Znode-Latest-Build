using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeFamilyListModel : BaseListModel
    {
        public List<GlobalAttributeFamilyModel> AttributeFamilyList { get; set; }
    }
}

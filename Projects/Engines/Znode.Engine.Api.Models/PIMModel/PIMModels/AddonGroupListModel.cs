using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AddonGroupListModel : BaseListModel
    {
        public List<AddonGroupModel> AddonGroups { get; set; }

        public AddonGroupListModel()
        {
            AddonGroups = new List<AddonGroupModel>();
        }
        public List<LocaleModel> Locale { get; set; }
        public int LocaleId { get; set; }
    }
}

using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{ 
    public class AddonGroupLocaleListModel
    {
        public List<AddonGroupLocaleModel> AddonGroupLocales { get; set; }

        public AddonGroupLocaleListModel()
        {
            AddonGroupLocales = new List<AddonGroupLocaleModel>();
        }
    }
}

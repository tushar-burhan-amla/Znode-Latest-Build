using System.Collections.Generic;
namespace Znode.Engine.Api.Models
{
    public class AddonGroupModel : BaseModel
    {
        public int PimAddonGroupId { get; set; }
        public string DisplayType { get; set; }
        public int PimAddonProductId { get; set; }
        public string AddonGroupName { get; set; }
        public List<AddonGroupLocaleModel> PimAddonGroupLocales { get; set; }
        public ProductDetailsListModel AssociatedChildProducts { get; set; }

        public List<AddOnProductModel> PimAddOnProducts { get; set; }
        public int LocaleId { get; set; }
        public List<LocaleModel> Locale { get; set; }
    }
}

using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PIMFamilyDetailsModel : BaseModel
    {
        public int Id { get; set; }
        public int PimAttributeFamilyId { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public string AssociatedProductIds { get; set; }
        public string ConfigureAttributeIds { get; set; }
        public List<PIMAttributeGroupModel> Groups { get; set; }
        public List<PIMProductAttributeValuesModel> Attributes { get; set; }
        public List<PIMAttributeFamilyModel> Family { get; set; }
        public List<LocaleModel> Locale { get; set; }

        public int? ProductPublishId { get; set; }
    }
}

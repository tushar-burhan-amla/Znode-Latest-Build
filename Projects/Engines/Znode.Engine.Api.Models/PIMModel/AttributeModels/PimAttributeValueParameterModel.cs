using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PimAttributeValueParameterModel : BaseModel
    {
        public bool IsCategory { get; set; }
        public int LocaleId { get; set; }
        public int Id { get; set; }
        public List<PIMAttributeCodeValueModel> PIMAttributeCodeValueList { get; set; }
    }
}

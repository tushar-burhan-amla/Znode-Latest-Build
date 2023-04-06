using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeValueParameterModel : BaseModel
    {
        public int LocaleId { get; set; }
        public int PortalId { get; set; }
        public int Id { get; set; }
        public string EntityType { get; set; }
        public string AttributeCodeValues { get; set; }
        public List<GlobalAttributeCodeValueModel> GlobalAttributeCodeValueList { get; set; }
    }
}
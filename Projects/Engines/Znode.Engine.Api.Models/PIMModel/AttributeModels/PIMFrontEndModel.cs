namespace Znode.Engine.Api.Models
{
    public class PIMFrontPropertiesModel : BaseModel
    {
        //Frontend Properties
        public int PimAttributeId { get; set; }
        public bool? IsComparable { get; set; } = true;
        public bool? IsUseInSearch { get; set; } 
        public bool? IsAllowHtmlTag { get; set; } = true;
        public bool? IsFacets { get; set; }
    }
} 

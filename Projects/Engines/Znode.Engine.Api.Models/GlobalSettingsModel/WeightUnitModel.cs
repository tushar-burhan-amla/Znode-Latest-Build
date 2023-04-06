namespace Znode.Engine.Api.Models
{
    public class WeightUnitModel : BaseModel
    {
        public int WeightUnitId { get; set; }
        public string WeightUnitName { get; set; }
        public string WeightUnitCode { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDefault { get; set; }
    }
}

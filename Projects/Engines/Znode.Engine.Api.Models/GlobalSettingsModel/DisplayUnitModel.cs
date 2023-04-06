namespace Znode.Engine.Api.Models
{
    public class DisplayUnitModel : BaseModel
    {
        public int DisplayUnitId { get; set; }
        public string DisplayUnitName { get; set; }
        public string DisplayUnitCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
    }
}

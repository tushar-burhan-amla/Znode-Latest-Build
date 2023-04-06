namespace Znode.Engine.Api.Models
{
    public class PortalSortSettingModel : BaseModel
    {
        public int? PortalSortSettingId { get; set; }
        public int? PortalId { get; set; }
        public int? SortSettingId { get; set; }
        public string SortName { get; set; }
        public string SortDisplayName { get; set; }
        public int? SortValue { get; set; }
        public int? DisplayOrder { get; set; }
    }
}

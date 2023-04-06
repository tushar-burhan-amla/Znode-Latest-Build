namespace Znode.Engine.Api.Models
{
    public class PortalPageSettingModel : BaseModel
    {
        public int? PortalPageSettingId { get; set; }
        public int? PortalId { get; set; }
        public int? PageSettingId { get; set; }
        public string PageName { get; set; }
        public string PageDisplayName { get; set; }
        public int? PageValue { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsDefault { get; set; }
    }
}

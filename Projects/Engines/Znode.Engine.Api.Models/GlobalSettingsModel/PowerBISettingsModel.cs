namespace Znode.Engine.Api.Models
{
    public class PowerBISettingsModel : BaseModel
    {
        public string PowerBIGroupId { get; set; }
        public string PowerBIReportId { get; set; }
        public string PowerBIApplicationId { get; set; }
        public string PowerBITenantId { get; set; }
        public string PowerBIUserName { get; set; }
        public string PowerBIPassword { get; set; }
    }
}

namespace Znode.Engine.Api.Models
{
    public class DefaultGlobalConfigModel : BaseModel
    {
        public int ZNodeGlobalSettingId { get; set; }
        public string FeatureName { get; set; }
        public string FeatureValues { get; set; }
        public string FeatureSubValues { get; set; }
        public string SelectedIds { get; set; }
        public string Action { get; set; }
        public int PortalId { get; set; }
        public string PortalLocaleId { get; set; }
        public string LocaleId { get; set; }
        public bool IsDefault { get; set; }
        public string CountryCode { get; set; }
        public int PortalCountryId { get; set; }
    }
}

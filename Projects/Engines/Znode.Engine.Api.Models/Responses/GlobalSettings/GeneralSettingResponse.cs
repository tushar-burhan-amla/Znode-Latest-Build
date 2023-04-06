namespace Znode.Engine.Api.Models.Responses
{
    public class GeneralSettingResponse : BaseResponse
    {
        public GeneralSettingModel GeneralSetting { get; set; }
        public CacheListModel CacheData { get; set; }
        public CacheModel Cache { get; set; }
        public ConfigurationSettingModel ConfigurationSetting { get; set; }
        public PowerBISettingsModel PowerBISettings { get; set; }
        public StockNoticeSettingsModel StockNoticeSettings { get; set; }
    }
}

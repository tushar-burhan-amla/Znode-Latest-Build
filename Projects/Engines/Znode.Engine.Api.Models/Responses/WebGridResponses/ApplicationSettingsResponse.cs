
namespace Znode.Engine.Api.Models.Responses
{
    public class ApplicationSettingsResponse : BaseResponse
    {
        //Application setting XML
        public string FilterXML { get; set; }

        //application setting data model
        public ApplicationSettingDataModel XMLSetting { get; set; }
    }
}

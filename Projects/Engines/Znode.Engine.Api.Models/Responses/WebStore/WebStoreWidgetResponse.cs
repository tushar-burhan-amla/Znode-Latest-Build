namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreWidgetResponse : BaseResponse
    {
        //Slider Widget Response.
        public CMSWidgetConfigurationModel Slider { get; set; }

        //Text Widget Response
        public CMSTextWidgetConfigurationModel CMSTextWidget { get; set; }

        //Media Widget Response
        public CMSMediaWidgetConfigurationModel MediaWidget { get; set; }

        //Container Key Response
        public string ContainerKey { get; set; }

        public int CMSWidgetsId { get; set; }
    }
}

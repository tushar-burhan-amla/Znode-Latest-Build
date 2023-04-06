namespace Znode.Engine.Api.Models.Responses
{
    public class CMSWidgetConfigurationResponse : BaseResponse
    {
        public CMSWidgetConfigurationModel CMSWidgetConfiguration { get; set; }

        public LinkWidgetConfigurationModel LinkWidgetConfiguration { get; set; }
    }
}

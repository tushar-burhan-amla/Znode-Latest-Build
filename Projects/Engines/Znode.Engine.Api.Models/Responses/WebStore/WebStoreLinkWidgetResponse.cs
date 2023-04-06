namespace Znode.Engine.Api.Models.Responses
{
    public class WebStoreLinkWidgetResponse : BaseResponse
    {
        //Link Widget Response.
        public LinkWidgetConfigurationModel LinkData { get; set; }

        //Link Widget List Response.
        public LinkWidgetConfigurationListModel LinkDataList { get; set; }
    }
}

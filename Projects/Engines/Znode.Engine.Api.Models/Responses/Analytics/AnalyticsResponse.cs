namespace Znode.Engine.Api.Models.Responses
{
    public class AnalyticsResponse : BaseResponse
    {
        public AnalyticsModel AnalyticsDetails { get; set; }

        public string AnalyticsJSONKey { get; set; }
    }
}
namespace Znode.Engine.Api.Models.Responses
{
    public class CultureResponse : BaseResponse
    {
        public CultureModel Culture { get; set; }
        public string CultureSuffix { get; set; }
    }
}

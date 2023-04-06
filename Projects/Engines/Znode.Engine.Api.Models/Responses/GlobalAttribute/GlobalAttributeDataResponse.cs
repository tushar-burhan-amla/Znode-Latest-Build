namespace Znode.Engine.Api.Models.Responses
{
    public class GlobalAttributeDataResponse : BaseResponse
    {
        public GlobalAttributeDataModel GlobalAttributeDataModel { get; set; }
        public GlobalAttributeLocaleListModel Locales { get; set; }
        public GlobalAttributeDefaultValueModel DefaultValues { get; set; }
    }
}
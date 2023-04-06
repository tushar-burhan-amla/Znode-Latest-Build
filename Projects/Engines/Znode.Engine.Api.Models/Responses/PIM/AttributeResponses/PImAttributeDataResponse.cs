namespace Znode.Engine.Api.Models.Responses
{
    public class PIMAttributeDataResponse : BaseResponse
    {
        public PIMAttributeDataModel PIMAttributeDataModel { get; set; }

        public PIMAttributeLocaleListModel Locales { get; set; }

        public PIMAttributeDefaultValueModel DefaultValues { get; set; }
    }
}

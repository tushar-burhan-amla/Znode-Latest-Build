namespace Znode.Engine.Api.Models.Responses
{
    public class AttributesDataResponse:BaseResponse
    {
        public AttributesDataModel AttributeDataModel { get; set; }

        public AttributesLocaleListModel Locales { get; set; }

        public AttributesDefaultValueModel DefaultValues { get; set; }
    }
}

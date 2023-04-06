namespace Znode.Engine.Api.Models.Responses
{
    public class ThemeResponse : BaseResponse
    {
        public ThemeModel Theme { get; set; }
        public ThemeAssetModel ThemeAsset { get; set; }
    }
}

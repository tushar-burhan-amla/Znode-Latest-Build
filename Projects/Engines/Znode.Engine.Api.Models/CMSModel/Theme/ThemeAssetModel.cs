namespace Znode.Engine.Api.Models
{
    public class ThemeAssetModel : BaseModel
    {
        public int CMSThemeAssetId { get; set; }
        public int CMSAssetId { get; set; }
        public int CMSAssetTypeId { get; set; }
        public string ProductTypeId { get; set; }
        public string AssetName { get; set; }
        public string DisplayName { get; set; }
        public bool IsSelected { get; set; }
    }
}

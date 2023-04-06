namespace Znode.Engine.Admin.ViewModels
{
    public class ThemeAssetViewModel : BaseViewModel
    {
        public int CMSThemeAssetId { get; set; }
        public int CMSAssetId { get; set; }
        public int CMSAssetTypeId { get; set; }
        public string AssetName { get; set; }
        public string DisplayName { get; set; }
        public bool IsSelected { get; set; }
    }
}
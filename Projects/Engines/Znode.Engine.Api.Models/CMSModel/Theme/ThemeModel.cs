using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ThemeModel : BaseModel
    {
        public int CMSThemeId { get; set; }
        public string Name { get; set; }
        public int CMSAssetId { get; set; }
        public int CMSThemeAssetId { get; set; }
        public string Assets { get; set; }
        public bool IsParentTheme { get; set; }
        public int? ParentThemeId { get; set; }
        public List<ThemeAssetModel> ThemeAssets { get; set; }
        public List<CSSModel> CssList { get; set; }
        public List<PDPAssetModel> PDPAssets { get; set; }
        public string ParentThemeName { get; set; }        
    }
}

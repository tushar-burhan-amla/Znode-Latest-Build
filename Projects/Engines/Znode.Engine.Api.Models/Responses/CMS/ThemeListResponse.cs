using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ThemeListResponse : BaseListResponse
    {
        public List<ThemeModel> Themes { get; set; }
        public List<ThemeAssetModel> ThemeAssets { get; set; }
    }
}

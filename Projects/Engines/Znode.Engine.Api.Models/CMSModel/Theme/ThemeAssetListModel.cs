using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ThemeAssetListModel : BaseListModel
    {
        public List<ThemeAssetModel> ThemeAssets { get; set; }
    }
}

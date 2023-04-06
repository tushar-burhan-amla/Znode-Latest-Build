using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ThemeListModel : BaseListModel
    {
        public List<ThemeModel> Themes { get; set; }

        public ThemeListModel()
        {
            Themes = new List<ThemeModel>();
        }
    }
}

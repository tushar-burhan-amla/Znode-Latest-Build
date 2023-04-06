using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CSSModel : BaseModel
    {
        public int CMSThemeCSSId { get; set; }
        public int CMSThemeId { get; set; }
        public string CSSName { get; set; }
        public List<string> cssList { get; set; }

        public CSSModel()
        {
            cssList = new List<string>();
        }
    }
}

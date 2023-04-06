using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class CSSListModel : BaseListModel
    {
        public List<CSSModel> CSSs { get; set; }
        public string CMSThemeName { get; set; }

        public CSSListModel()
        {
            CSSs = new List<CSSModel>();
        }
    }
}

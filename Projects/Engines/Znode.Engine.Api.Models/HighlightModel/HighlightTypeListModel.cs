using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class HighlightTypeListModel : BaseListModel
    {
        public HighlightTypeListModel()
        {
            HighlightTypes = new List<HighlightTypeModel>();
        }

        public List<HighlightTypeModel> HighlightTypes { get; set; }
        public string TemplateTokens { get; set; }
    }
}

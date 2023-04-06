using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class HighlightListModel : BaseListModel
    {
        public List<HighlightModel> HighlightList { get; set; }
        public List<PIMAttributeDefaultValueModel> HighlightCodes { get; set; }
    }
}
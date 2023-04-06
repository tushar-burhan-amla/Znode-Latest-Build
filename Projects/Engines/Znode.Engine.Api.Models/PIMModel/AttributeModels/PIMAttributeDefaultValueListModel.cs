using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PIMAttributeDefaultValueListModel : BaseListModel
    {
        public List<PIMAttributeDefaultValueModel> DefaultValues { get; set; }
    }
}

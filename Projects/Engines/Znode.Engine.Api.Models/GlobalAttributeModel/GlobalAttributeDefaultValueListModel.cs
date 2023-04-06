using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeDefaultValueListModel : BaseListModel
    {
        public List<GlobalAttributeDefaultValueModel> DefaultValues { get; set; }
    }
}

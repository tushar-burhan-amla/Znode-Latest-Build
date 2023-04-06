using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class AttributesDefaultValueListModel:BaseListModel
    {
        public List<AttributesDefaultValueModel> DefaultValues { get; set; }
    }
}

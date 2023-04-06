using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PIMAttributeTypeListModel : BaseListModel
    {
        public List<PIMAttributeTypeModel> Types { get; set; }
    }
}

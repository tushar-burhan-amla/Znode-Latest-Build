using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class FamilyExtensionListModel : BaseListModel
    {
        public List<FamilyExtensionModel> FamilyExtensions { get; set; }
    }
}

using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ImportTemplateMappingListModel : BaseModel
    {
        public List<ImportTemplateMappingModel> Mappings { get; set; }
    }
}

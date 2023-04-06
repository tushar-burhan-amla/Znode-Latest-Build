using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ImportManageTemplateListModel: BaseListModel
    { 
        public List<ImportManageTemplateModel> ImportManageTemplates { get; set; }
    }
}

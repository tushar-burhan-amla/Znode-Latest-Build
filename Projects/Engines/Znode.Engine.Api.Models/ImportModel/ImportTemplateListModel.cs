using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ImportTemplateListModel :BaseModel
    {
        public ImportTemplateListModel()
        {
            TemplateList = new List<ImportTemplateModel>();
        }
        public List<ImportTemplateModel> TemplateList { get; set; }
    }
}

using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ImportTypeListModel : BaseModel
    {
        public ImportTypeListModel()
        {
            ImportTypeList = new List<ImportTypeModel>();
        }
        public List<ImportTypeModel> ImportTypeList { get; set; }
    }
}

using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ImportProductFamilyListModel : BaseListModel
    {
        public ImportProductFamilyListModel()
        {
            FamilyList = new List<ImportProductFamilyModel>();
        }
        public List<ImportProductFamilyModel> FamilyList { get; set; }
    }
}

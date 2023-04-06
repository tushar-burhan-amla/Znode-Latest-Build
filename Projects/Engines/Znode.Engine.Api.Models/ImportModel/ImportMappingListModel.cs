using System.Collections.Generic;
namespace Znode.Engine.Api.Models
{
    public class ImportMappingListModel : BaseListModel
    {
        public ImportMappingListModel()
        {
            DataMappings = new List<ImportMappingModel>();
        }
        public List<ImportMappingModel> DataMappings { get; set; }
    }
}

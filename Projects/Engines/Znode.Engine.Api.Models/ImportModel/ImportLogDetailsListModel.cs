using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ImportLogDetailsListModel : BaseListModel
    {
        public ImportLogDetailsListModel()
        {
            ImportLogDetails = new List<ImportLogDetailsModel>();
        }

        public List<ImportLogDetailsModel> ImportLogDetails { get; set; }
        public ImportLogsModel ImportLogs { get; set; }
    }
}

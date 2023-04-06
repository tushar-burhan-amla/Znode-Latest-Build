using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ImportLogsListModel : BaseListModel
    {
        public ImportLogsListModel()
        {
            ImportLogs = new List<ImportLogsModel>();
        }

        public List<ImportLogsModel> ImportLogs { get; set; }
    }
}

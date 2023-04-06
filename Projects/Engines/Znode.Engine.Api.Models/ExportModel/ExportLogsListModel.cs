using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
   public  class ExportLogsListModel : BaseListModel
    {
        public ExportLogsListModel()
        {
            ExportLogs = new List<ExportLogsModel>();
        }
        public List<ExportLogsModel> ExportLogs { get; set; }
    }
}

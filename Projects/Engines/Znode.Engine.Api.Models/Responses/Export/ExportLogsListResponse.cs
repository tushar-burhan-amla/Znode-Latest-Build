using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ExportLogsListResponse : BaseListResponse
    {
        public ExportLogsListResponse()
        {
            LogsList = new List<ExportLogsModel>();
        }
        public List<ExportLogsModel> LogsList { get; set; }
        
    }
}

using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ImportLogsListResponse : BaseListResponse
    {
        public ImportLogsListResponse()
        {
            LogsList = new List<ImportLogsModel>();
        }

        public List<ImportLogsModel> LogsList { get; set; }
    }
}

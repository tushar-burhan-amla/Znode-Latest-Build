using System;

namespace Znode.Engine.Api.Models
{
    public class ImportLogsModel : BaseModel
    {
        public int ImportProcessLogId { get; set; }
        public int? ImportTemplateId { get; set; }
        public DateTime? ProcessStartedDate { get; set; }
        public DateTime? ProcessCompletedDate { get; set; }
        public string Status { get; set; }
        public string TemplateName { get; set; }
        public string ImportName { get; set; }
        public long? SuccessRecordCount { get; set; }
        public long? FailedRecordcount { get; set; }
        public long? TotalProcessedRecords { get; set; }
    }
}

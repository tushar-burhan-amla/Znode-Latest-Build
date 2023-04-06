using System;

namespace Znode.Engine.Api.Models
{
    public class TouchPointConfigurationModel : BaseModel
    {
        public int ERPTaskSchedulerId { get; set; }
        public string Interface { get; set; }
        public string SchedulerName { get; set; }
        public string SchedulerType { get; set; }
        public string ConnectorTouchPoints { get; set; }
        public bool IsEnabled { get; set; }
        public bool Status { get; set; }
        public string Triggers { get; set; }
        public string NextRunTime { get; set; }
        public string LastRunResult { get; set; }
        public string SchedulerCreatedDate { get; set; }
        public string Level { get; set; }
        public string DateTime { get; set; }
        public string EventID { get; set; }
        public string TaskCategory { get; set; }
        public string OperationalCode { get; set; }
        public string CorrelationId { get; set; }
        public int ERPConfiguratorId { get; set; }
        public string RecordId { get; set; }
        public string LogName { get; set; }
        public string MachineName { get; set; }
        public string LastRunTime { get; set; }
        public string ClassName { get; set; }
        public string LogDetails { get; set; }
        public string ImportStatus { get; set; }
        public int ImportProcessLogId { get; set; }
        public string TouchPointName { get; set; }
        public DateTime? ProcessStartedDate { get; set; }
        public DateTime? ProcessCompletedDate { get; set; }

        public int? SuccessRecordCount { get; set; }
        public int? FailedRecordcount { get; set; }

        public string SchedulerCallFor { get; set; }
    }
}

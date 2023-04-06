using System;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class TouchPointConfigurationViewModel : BaseViewModel
    {
        public int ERPTaskSchedulerId { get; set; }
        public string Interface { get; set; }
        [Display(Name = ZnodeERP_Resources.LabelERPTaskSchedulerSchedulerName, ResourceType = typeof(ERP_Resources))]
        public string SchedulerName { get; set; }
        public string ConnectorTouchPoints { get; set; }
        public bool IsEnabled { get; set; }
        public bool Status { get; set; }
        public string Triggers { get; set; }
        public string NextRunTime { get; set; }
        public string LastRunResult { get; set; }
        public string SchedulerCreatedDate { get; set; }
        public string Level { get; set; }
        [Display(Name = ZnodeERP_Resources.LabelERPTaskSchedulerLogged, ResourceType = typeof(ERP_Resources))]
        public string DateTime { get; set; }
        public string EventID { get; set; }
        [Display(Name = ZnodeERP_Resources.LabelERPTaskSchedulerTaskCategory, ResourceType = typeof(ERP_Resources))]
        public string TaskCategory { get; set; }
        public string OperationalCode { get; set; }
        public string CorrelationId { get; set; }
        public string SchedulerType { get; set; }
        public string RecordId { get; set; }
        [Display(Name = ZnodeERP_Resources.LabelERPTaskSchedulerLogName, ResourceType = typeof(ERP_Resources))]
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
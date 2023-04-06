using System;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class ERPTaskSchedulerModel : BaseModel
    {
        public int ERPTaskSchedulerId { get; set; }

        public string SchedulerName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredTouchPointName)]
        public string TouchPointName { get; set; }

        public string SchedulerFrequency { get; set; }

        public DateTime? StartDate { get; set; }
        
        public bool IsEnabled { get; set; }        
        public int ERPConfiguratorId { get; set; }        
        public string ExeParameters { get; set; }        
        public int PortalId { get; set; }        
        public string IndexName { get; set; }        
        public int CatalogId { get; set; }
        
        public int CatalogIndexId { get; set; }        
        public bool IsAssignTouchPoint { get; set; }        
        public string SchedulerType { get; set; }        
        public string SchedulerCallFor { get; set; }        
        public string DomainName { get; set; }        
        public bool IsInstantJob { get; set; }        
        public string CronExpression { get; set; }
        public string HangfireJobId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class LogMessageViewModel : BaseViewModel
    {

        public string LogMessageId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.Component, ResourceType = typeof(Admin_Resources))]
        public string Component { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LogMessage, ResourceType = typeof(Admin_Resources))]
        public string LogMessage { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelTraceLevel, ResourceType = typeof(Admin_Resources))]
        public string TraceLevel { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelStackTraceMessage, ResourceType = typeof(Admin_Resources))]
        public string StackTraceMessage { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelDomainName, ResourceType = typeof(Admin_Resources))]
        public string DomainName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelApplicationType, ResourceType = typeof(Admin_Resources))]
        public string ApplicationType { get; set; }
    }
}

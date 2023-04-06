using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class RobotsTxtViewModel : BaseViewModel
    {
        public int RobotsTxtId { get; set; }
        public int PortalId { get; set; }
        public string StoreName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelRobotsTxtContent, ResourceType = typeof(Admin_Resources))]
        public string RobotsTxtContent { get; set; }
        [Display(Name = ZnodeAdmin_Resources.DefaultPageLevelRobotTag, ResourceType = typeof(Admin_Resources))]
        public RobotTag DefaultRobotTag { get; set; }
    }
}

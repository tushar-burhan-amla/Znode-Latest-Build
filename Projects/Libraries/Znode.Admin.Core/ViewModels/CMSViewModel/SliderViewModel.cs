using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class SliderViewModel : BaseViewModel
    {
        public int CMSSliderId { get; set; }


        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSliderName)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredName)]
        public string Name { get; set; }
        public bool? IsPublished { get; set; }
        public string PublishStatus { get; set; }
    }
}
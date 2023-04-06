using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchTriggersViewModel : BaseViewModel
    {
        public int SearchProfileTriggerId { get; set; }
        public int? ProfileId { get; set; }
        public int SearchProfileId { get; set; }

        public string Keyword { get; set; }
        public string UserProfile { get; set; }
        [Display(Name = ZnodeAdmin_Resources.TitleUserProfilesList, ResourceType = typeof(Admin_Resources))]
        public string[] ProfileIds { get; set; }

        public bool IsConfirmation { get; set; }

        public List<SelectListItem> UserProfileList { get; set; } = new List<SelectListItem>();
    }
}

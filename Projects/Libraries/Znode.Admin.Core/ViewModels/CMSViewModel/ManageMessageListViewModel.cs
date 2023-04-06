using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ManageMessageListViewModel : BaseViewModel
    {
        public List<ManageMessageViewModel> ManageMessages { get; set; }
        public GridModel GridModel { get; set; }
        public int LocaleId { get; set; }
        public List<SelectListItem> Locales { get; set; }
        public ManageMessageListViewModel()
        {
            ManageMessages = new List<ManageMessageViewModel>();
        }
    }
}
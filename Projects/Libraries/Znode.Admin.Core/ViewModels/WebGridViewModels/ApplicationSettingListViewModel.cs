using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ApplicationSettingListViewModel : BaseViewModel
    {
        public List<ApplicationSettingModel> List { get; set; }

        public ApplicationSettingListViewModel()
        {
            List = new List<ApplicationSettingModel>();
        }
    }
}
using System.Collections.Generic;
using Znode.Engine.WebStore.Models;

namespace Znode.Engine.WebStore.ViewModels
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
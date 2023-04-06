using System.Collections.Generic;
using Znode.Engine.Admin.Models;


namespace Znode.Engine.Admin.ViewModels
{
    public class PublishStateMappingListViewModel : BaseViewModel
    {
        public List<PublishStateMappingViewModel> PublishStateMappingList { get; set; }
        public GridModel GridModel { get; set; }

        public PublishStateMappingListViewModel()
        {
            PublishStateMappingList = new List<PublishStateMappingViewModel>();
        }
    }
}

using System.Collections.Generic;
namespace Znode.Engine.Admin.ViewModels
{
    public class MediaManagerListViewModel:BaseViewModel
    {

        public List<MediaManagerViewModel> MediaList { get; set; }

        public MediaManagerListViewModel()
        {
            MediaList = new List<MediaManagerViewModel>();
        }
}
}
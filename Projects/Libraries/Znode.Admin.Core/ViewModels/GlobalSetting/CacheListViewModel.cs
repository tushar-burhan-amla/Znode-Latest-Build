using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class CacheListViewModel :BaseViewModel
    {
        public CacheListViewModel()
        {
            CacheList = new List<CacheViewModel>();
        }

        public List<CacheViewModel> CacheList { get; set; }

    }
}

using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.WebStore.Core.ViewModels
{
    public class SiteMapProductListViewModel : BaseListModel
    {
        public List<SiteMapProductViewModel> ProductList { get; set; }
    }
}

using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PublishCatalogLogListViewModel : BaseViewModel
    {
        public List<PublishCatalogLogViewModel> PublishCatalogLog { get; set; }
        public GridModel GridModel { get; set; }
        public int PimCatalogId { get; set; }
        public string CatalogName { get; set; }
    }
}
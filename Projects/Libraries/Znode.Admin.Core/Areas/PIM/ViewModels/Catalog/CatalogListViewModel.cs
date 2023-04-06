using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CatalogListViewModel : BaseViewModel
    {
        public List<CatalogViewModel> Catalogs { get; set; }
        public GridModel GridModel { get; set; }
        public TreebuttonViewModel TreeView { get; set; }
        public CatalogListViewModel()
        {
            TreeView = new TreebuttonViewModel();
            Catalogs = new List<CatalogViewModel>();
        }
        public int? ProfileId { get; set; }
    }
}
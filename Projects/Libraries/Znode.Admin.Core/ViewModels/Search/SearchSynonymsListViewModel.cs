using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class SearchSynonymsListViewModel : BaseViewModel
    {
        public List<SearchSynonymsViewModel> SynonymsList { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSelectCatalog, ResourceType = typeof(Admin_Resources))]
        public int CatalogId { get; set; }

        public string CatalogName { get; set; }

        public GridModel GridModel { get; set; }
    }
}

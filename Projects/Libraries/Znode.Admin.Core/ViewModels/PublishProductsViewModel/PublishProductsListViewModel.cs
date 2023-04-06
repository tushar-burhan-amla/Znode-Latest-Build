using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class PublishProductsListViewModel : BaseViewModel
    {
        public List<PublishProductsViewModel> PublishProductsList { get; set; }
        public GridModel GridModel { get; set; }
        public PublishProductsListViewModel()
        {
            PublishProductsList = new List<PublishProductsViewModel>();
            GridModel = new GridModel();
        }

        public int publishCatalogId { get; set; }
        public int portalId { get; set; }
        public string ProductIds { get; set; }
        public int PromotionId { get; set; }
        public int UserId { get; set; }
    }
}
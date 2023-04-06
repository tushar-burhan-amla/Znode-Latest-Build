using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Agents
{
    public class EcommerceCatalogAgent : BaseAgent, IEcommerceCatalogAgent
    {

        #region Private Variables
        private readonly IEcommerceCatalogClient _ecommerceCatalogClient;
        #endregion

        #region Constructor
        public EcommerceCatalogAgent(IEcommerceCatalogClient ecommerceCatalogClient)
        {
            _ecommerceCatalogClient = GetClient<IEcommerceCatalogClient>(ecommerceCatalogClient);
        }
        #endregion

        #region public Methods

        //method Gets catalogs associated with portal as per passed portalId.
        public virtual PortalCatalogListViewModel GetAssociatedPortalCatalogByPortalId(int portalId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            PortalCatalogListModel categoryProductListModel = _ecommerceCatalogClient.GetAssociatedPortalCatalogByPortalId(portalId, expands, filters, sorts, pageIndex, pageSize);
            PortalCatalogListViewModel portalCatalogListViewModel = new PortalCatalogListViewModel { PortalCatalogs = categoryProductListModel?.PortalCatalogs?.ToViewModel<PortalCatalogViewModel>().ToList() };
            SetListPagingData(portalCatalogListViewModel, categoryProductListModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return portalCatalogListViewModel?.PortalCatalogs?.Count > 0 ? portalCatalogListViewModel : new PortalCatalogListViewModel();
        }

        //Get Portal catalog based on the Portal.
        public virtual PortalCatalogViewModel GetPortalCatalog(int portalCatalogId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            PortalCatalogViewModel model = _ecommerceCatalogClient.GetPortalCatalog(portalCatalogId)?.ToViewModel<PortalCatalogViewModel>();
            ZnodeLogging.LogMessage("PublishCatalogs", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info,new { PublishCatalogs = model? .PublishCatalogs });
            if (model?.PublishCatalogs?.Count > 0)
            {
                model.PublishCatalogs = model.PublishCatalogs.OrderBy(x => x.CatalogName).ToList();
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            return model;
        }

        //method Update catalog associated with portal.
        public virtual bool UpdatePortalCatalog(PortalCatalogViewModel portalCatalogViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
            try
            {
                _ecommerceCatalogClient.UpdatePortalCatalog(portalCatalogViewModel.ToModel<PortalCatalogModel>());
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Info);
                return true;
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        //Method Gets the tree structure for Catalog.
        public virtual List<CategoryTreeViewModel> GetCatalogTree(int catalogId, int categoryId)
            => _ecommerceCatalogClient.GetCatalogTree(catalogId, categoryId)?.ToViewModel<CategoryTreeViewModel>().ToList();

        //method Get Publish Catalog Details
        public virtual PublishDetailsViewModel GetPublishCatalogDetails(int publishCatalogId)
            => EcommerceCatalogMap.ToViewModel(_ecommerceCatalogClient.GetPublishCatalogDetails(publishCatalogId));

        //method Get Publish Category Details
        public virtual PublishDetailsViewModel GetPublishCategoryDetails(int publishCategoryId)
        => EcommerceCatalogMap.ToViewModel(_ecommerceCatalogClient.GetPublishCategoryDetails(publishCategoryId));

        //method Get Publish Product Details
        public virtual PublishDetailsViewModel GetPublishProductDetails(int publishProductId, int portalId)
         => EcommerceCatalogMap.ToViewModel(_ecommerceCatalogClient.GetPublishProductDetails(publishProductId, portalId));

        #endregion
    }
}
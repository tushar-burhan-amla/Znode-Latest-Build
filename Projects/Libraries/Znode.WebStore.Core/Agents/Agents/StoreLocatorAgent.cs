using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Exceptions;
using Znode.Engine.WebStore.Maps;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.Agents
{
    public class StoreLocatorAgent : BaseAgent, IStoreLocatorAgent
    {
        #region Private Variables
        private readonly IWebStoreLocatorClient _webStoreLocatorClient;
        #endregion

        #region Public Constructor
        public StoreLocatorAgent(IWebStoreLocatorClient webStoreLocatorClient)
        {
            _webStoreLocatorClient = GetClient<IWebStoreLocatorClient>(webStoreLocatorClient);
        }
        #endregion

        #region Public Method
        //Get distance list. 
        public virtual List<SelectListItem> GetDistanceList() => StoreLocatorViewModelMap.GetDistances();

        //Get StoreLocator from model data and distance list. 
        public virtual StoreLocatorViewModel GetPortalList(StoreLocatorViewModel model)
        {
            try
            {
                if (HelperUtility.IsNotNull(model))
                {
                    SortCollection sorts = new SortCollection();
                    sorts.Add(SortKeys.DisplayOrder, SortDirections.Ascending);

                    model.PortalList = _webStoreLocatorClient.GetPortalList(GetFilter(model), sorts)?.ToViewModel<StoreLocatorViewModel>().ToList();

                    if (HelperUtility.IsNull(model.PortalList) || model.PortalList.Count <= 0)
                        model.ErrorMessage = WebStore_Resources.NoStoresFoundMessage;

                    return model;
                }
                else
                    return new StoreLocatorViewModel { RadiusList = StoreLocatorViewModelMap.GetDistances() };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                model.ErrorMessage = WebStore_Resources.NoStoresFoundMessage;
                return null;
            }
        }
        #endregion

        #region Private Method   
        //Set filter values from model.
        private FilterCollection GetFilter(StoreLocatorViewModel model)
        {
            FilterCollection filter = new FilterCollection();

            filter.Add(ZnodePortalAddressEnum.PortalId.ToString(), FilterOperators.Equals, Convert.ToString(PortalAgent.CurrentPortal.PortalId));
            filter.Add(ZnodeAddressEnum.IsActive.ToString(), FilterOperators.Is, ZnodeConstant.TrueValue);

            if (!string.IsNullOrEmpty(model.PostalCode))
                filter.Add(ZnodeAddressEnum.PostalCode.ToString(), FilterOperators.Is, model.PostalCode?.Replace("'", "''"));
            if (!string.IsNullOrEmpty(model.CityName))
                filter.Add(ZnodeAddressEnum.CityName.ToString(), FilterOperators.Is, model.CityName?.Replace("'", "''"));
            if (!string.IsNullOrEmpty(model.StateName))
                filter.Add(ZnodeAddressEnum.StateName.ToString(), FilterOperators.Is, model.StateName?.Replace("'", "''"));

            return filter;
        }
        #endregion
    }
}

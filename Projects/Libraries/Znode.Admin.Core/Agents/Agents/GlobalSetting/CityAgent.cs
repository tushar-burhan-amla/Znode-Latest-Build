using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class CityAgent : BaseAgent, ICityAgent
    {
        #region Private Variables
        private readonly ICityClient _cityClient;
        #endregion

        #region Constructor
        public CityAgent(ICityClient cityClient)
        {
            _cityClient = GetClient<ICityClient>(cityClient);
        }
        #endregion

        #region public Methods

        //Method to get city list
        public virtual CityListViewModel GetCityList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);

            if (IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.ModifiedDate, DynamicGridConstants.DESCKey);
            }
            ZnodeLogging.LogMessage("Input parameters filters and sorts:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            CityListModel cityListModel = _cityClient.GetCityList(filters, sorts, pageIndex, pageSize);

            CityListViewModel listViewModel = new CityListViewModel { Cities = cityListModel?.Cities?.ToViewModel<CityViewModel>().ToList() };

            SetListPagingData(listViewModel, cityListModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return cityListModel?.Cities?.Count > 0 ? listViewModel : new CityListViewModel() { Cities = new List<CityViewModel>() };
        }

        //Method to get active city list
        public virtual List<SelectListItem> GetActiveCityList(FilterCollection filters, string countyCode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters expands and countyCode:", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, new { filters = filters, countyCode = countyCode });

            CityListViewModel cityList = GetCityList(filters);

            List<SelectListItem> selectedCityList = new List<SelectListItem>();
            if (cityList?.Cities?.Count > 0)
            {
                if (!string.IsNullOrEmpty(countyCode))
                    cityList.Cities.ForEach(item => { selectedCityList.Add(new SelectListItem() { Text = item.CityName, Value = item.CountyFIPS, Selected = item.CountyCode == countyCode ? true : false }); });
                cityList.Cities.ForEach(item => { selectedCityList.Add(new SelectListItem() { Text = item.CityName, Value = item.CountyFIPS }); });
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return selectedCityList;
        }

        #endregion
    }
}
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public class CityService : BaseService, ICityService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeCity> _cityRepository;
        #endregion

        #region Constructor
        public CityService()
        {
            _cityRepository = new ZnodeRepository<ZnodeCity>();
        }
        #endregion

        #region Public Methods

        //Get a list of cities
        public virtual CityListModel GetCityList(FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("PageListModel to generate cityList: ", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            CityListModel cityList = new CityListModel
            {
                Cities = _cityRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)
                .ToModel<CityModel>()?.ToList()
            };
            cityList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
            return cityList;
        }

        #endregion
    }
}

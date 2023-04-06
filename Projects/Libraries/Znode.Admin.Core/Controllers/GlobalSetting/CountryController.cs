using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class CountryController : BaseController
    {
        #region Private Variables
        private readonly ICountryAgent _countryAgent;
        #endregion

        #region Constructor
        public CountryController(ICountryAgent countryAgent)
        {
            _countryAgent = countryAgent;
        }
        #endregion

        #region Public Methods

        //Method to get Country list
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeCountry.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCountry.ToString(), model);
            //Get the list of countries.            
            CountryListViewModel countryListViewModel = _countryAgent.GetCountries(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model
            countryListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, countryListViewModel?.Countries, GridListType.ZnodeCountry.ToString(), string.Empty, null, true, true, countryListViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            countryListViewModel.GridModel.TotalRecordCount = countryListViewModel.TotalResults;

            //Returns the country list view
            return ActionView(countryListViewModel);
        }

        //Action for update country.
        [HttpPost]
        public virtual ActionResult Update(DefaultGlobalConfigViewModel model)
        {
            bool status = false;
            string message = string.Empty;
            status = _countryAgent.UpdateCountry(model, out message);
            TempData[AdminConstants.Notifications] = status ? GenerateNotificationMessages(Admin_Resources.UpdateMessage, NotificationType.success) : GenerateNotificationMessages(message, NotificationType.error);
            return Json(new { IsSuccess = status, status = status}, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
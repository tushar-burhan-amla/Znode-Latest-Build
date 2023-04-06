using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class CurrencyController : BaseController
    {
        #region Private Variables
        private readonly ICurrencyAgent _currencyAgent;
        #endregion

        #region Constructor
        public CurrencyController(ICurrencyAgent CurrencyAgent)
        {
            _currencyAgent = CurrencyAgent;
        }
        #endregion

        #region Public Methods

        //Method to get Currency list
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeCurrency.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCurrency.ToString(), model);
            //Get the list of currencies            
            CurrencyListViewModel currencyListViewModel = _currencyAgent.GetCurrencies(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model
            currencyListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, currencyListViewModel?.Currencies, GridListType.ZnodeCurrency.ToString(), string.Empty, null, true, true, currencyListViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            currencyListViewModel.GridModel.TotalRecordCount = currencyListViewModel.TotalResults;

            //Returns the currency list view
            return ActionView(currencyListViewModel);
        }

        //Action for Update Currency.
        [HttpPost]
        public virtual ActionResult Update(DefaultGlobalConfigViewModel model)
        {
            bool status = false;
            string message = string.Empty;
            status = _currencyAgent.UpdateCurrency(model, out message);
            TempData[AdminConstants.Notifications] = status ? GenerateNotificationMessages(Admin_Resources.UpdateMessage, NotificationType.success) : GenerateNotificationMessages(message, NotificationType.error);
            return Json(new { IsSuccess = status, status = status }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
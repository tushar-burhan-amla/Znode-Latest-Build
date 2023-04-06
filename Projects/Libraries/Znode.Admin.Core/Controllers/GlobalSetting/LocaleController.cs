using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class LocaleController : BaseController
    {
        #region Private Variables
        private readonly ILocaleAgent _localeAgent;
        #endregion

        #region Constructor
        public LocaleController(ILocaleAgent localeAgent)
        {
            _localeAgent = localeAgent;
        }
        #endregion

        #region Public Methods

        //Method to get Locale list
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeLocale.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeLocale.ToString(), model);
            //Get the list of locales            
            LocaleListViewModel localeListViewModel = _localeAgent.GetLocales(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model
            localeListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, localeListViewModel?.Locales, GridListType.ZnodeLocale.ToString(), string.Empty, null, true, true, localeListViewModel?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            localeListViewModel.GridModel.TotalRecordCount = localeListViewModel.TotalResults;

            //Returns the locale list view
            return ActionView(localeListViewModel);
        }

        //Action for Update Locale.
        [HttpPost]
        public virtual ActionResult Update(DefaultGlobalConfigViewModel model)
        {
            bool status = false;
            string message = string.Empty;
            status = _localeAgent.UpdateLocale(model, out message);
            TempData[AdminConstants.Notifications] = status ? GenerateNotificationMessages(Admin_Resources.UpdateMessage, NotificationType.success) : GenerateNotificationMessages(message, NotificationType.error);
            return Json(new { IsSuccess = status, status = status }, JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}
using MvcSiteMapProvider;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class PaymentController : BaseController
    {
        #region Private Variables
        private readonly IPaymentAgent _paymentAgent;
        private const string PaymentOptionView = "~/Views/Payment/PaymentOption.cshtml";
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        public PaymentController(IPaymentAgent paymentAgent)
        {
            _paymentAgent = paymentAgent;
        }

        #endregion

        /// <summary>
        /// Get List of PAyment Settings
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,PaymentOptionHeader", Key = "Payment", Area = "", ParentKey = "Setup")]
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodePayment.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodePayment.ToString(), model);
            PaymentSettingListViewModel paymentSettingList = _paymentAgent.List(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            paymentSettingList.GridModel = FilterHelpers.GetDynamicGridModel(model, paymentSettingList.PaymentSettings, GridListType.ZnodePayment.ToString(), string.Empty, null, true, true, paymentSettingList?.GridModel?.FilterColumn?.ToolMenuList);

            paymentSettingList.GridModel.TotalRecordCount = paymentSettingList.TotalResults;
            return ActionView(AdminConstants.ListView, paymentSettingList);
        }

        /// <summary>
        /// Get view for add new payment option 
        /// </summary>
        /// <returns>Returns ActionResult</returns>
        [HttpGet]
        public virtual ActionResult Create()
        {
            PaymentSettingViewModel model = _paymentAgent.GetPaymentSettingViewData();
            if (IsNotNull(model))
            {
                model.IsActive = true;
                return View(PaymentOptionView, model);
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.Error));
            return RedirectToAction<PaymentController>(x => x.List(null));
        }

        /// <summary>
        /// Add new payment option 
        /// </summary>
        /// <param name="model">PaymentSettingViewModel</param>
        /// <returns>Redirect to List Action</returns>
        [HttpPost]
        public virtual ActionResult Create(PaymentSettingViewModel model)
        {
            PaymentSettingViewModel paymentSetting = _paymentAgent.AddPaymentSetting(model);

            if (!paymentSetting.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                return RedirectToAction<PaymentController>(x => x.Edit(paymentSetting.PaymentSettingId));
            }
            else
            {
                _paymentAgent.GetPaymentSettingViewData(model);
                SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
                return View(PaymentOptionView, model);
            }
        }


        /// <summary>
        /// Get payment option data for edit
        /// </summary>
        /// <param name="PaymentSettingId">Payment option id</param>
        /// <returns>Returns ActionResult</returns>        
        [HttpGet]
        public virtual ActionResult Edit(int PaymentSettingId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            var model = _paymentAgent.GetPaymentSetting(PaymentSettingId);
            if (IsNotNull(model))
            {
                return View(PaymentOptionView, model);
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.Error));
            return RedirectToAction<PaymentController>(x => x.List(null));
        }

        /// <summary>
        /// Update payment option
        /// </summary>
        /// <param name="model">PaymentSettingViewModel</param>
        /// <returns>Redirect to list action</returns>
        [HttpPost]
        public virtual ActionResult Edit(PaymentSettingViewModel model)
        {
            PaymentSettingViewModel paymentSetting = _paymentAgent.UpdatePaymentSetting(model);
            if ((!paymentSetting?.HasError ?? true))
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<PaymentController>(x => x.Edit(paymentSetting.PaymentSettingId));
            }
            else
            {
                _paymentAgent.GetPaymentSettingViewData(model);
                SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
                return View(PaymentOptionView, model);
            }
        }

        /// <summary>
        /// Delete payment Setting 
        /// </summary>
        /// <param name="PaymentSettingId">Payment Setting Id</param>
        /// <returns></returns>
        public virtual ActionResult Delete(string PaymentSettingId)
        {
            string message = Admin_Resources.PaymentDeleteErrorMessage;
            bool status = false;
            if (!string.IsNullOrEmpty(PaymentSettingId))
            {
                status = _paymentAgent.DeletePaymentSetting(PaymentSettingId, out message);
                message = status ? Admin_Resources.DeleteMessage : message;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Payment Type View
        /// </summary>
        /// <param name="paymentName">string paymentName</param>
        /// <param name="paymentSetting">string paymentSetting</param>
        /// <param name="paymentCode">string paymentCode</param>
        /// <returns></returns>
        public virtual ActionResult GetPaymentTypeForm(string paymentName, string paymentSetting, string paymentCode = "")
        {
            PaymentSettingViewModel paymentSettingViewModel = string.IsNullOrEmpty(paymentSetting) ? null : _paymentAgent.ParseStringToPaymentSettingViewModel(paymentSetting);
            PaymentSettingViewModel model = IsNotNull(paymentSettingViewModel)
                        ? paymentSettingViewModel : _paymentAgent.GetPaymentSettingViewData(null, paymentCode);

            return PartialView(_paymentAgent.GetPaymentTypeView(paymentName), model);
        }

        /// <summary>
        ///Get payment Gateway Form 
        /// </summary>
        /// <param name="paymentGatewayId">paymentGatewayId</param>
        /// <returns></returns>
        public virtual ActionResult GetPaymentGetwayForm(string gatewayCode, string paymentSetting)
        {
            PaymentSettingViewModel paymentSettingViewModel = string.IsNullOrEmpty(paymentSetting) ? null : _paymentAgent.ParseStringToPaymentSettingViewModel(paymentSetting);
            PaymentSettingViewModel model = IsNotNull(paymentSettingViewModel)
                    ? paymentSettingViewModel : _paymentAgent.GetPaymentSettingViewData();

            return PartialView(_paymentAgent.GetPaymentGatewayView(gatewayCode), model);
        }

        /// <summary>
        /// Get Payment Setting Credentials
        /// </summary>
        /// <param name="paymentsettingId">payment setting Id</param>
        /// <param name="isTestMode">true for test mode else set false</param>
        /// <returns></returns>
        public virtual ActionResult GetPaymentSettingCredentials(string paymentCode, bool isTestMode, string gatewayCode, string paymentTypeCode = "")
        {
            PaymentSettingViewModel model = _paymentAgent.GetPaymentSettingCredentialsByPaymentCode(paymentCode, isTestMode, paymentTypeCode);
            return PartialView(_paymentAgent.GetPaymentGatewayView(gatewayCode), model);
        }

        /// <summary>
        /// Checks whether payment name exists.
        /// </summary>
        /// <param name="paymentCode">paymentCode</param>
        /// <param name="paymentsettingId">paymentsettingId</param>
        /// <returns></returns>
        [HttpPost]
        public virtual JsonResult IsPaymentCodeExist(string paymentCode, int paymentsettingId = 0)
            => Json(!_paymentAgent.CheckPaymentCodeExist(paymentCode, paymentsettingId), JsonRequestBehavior.AllowGet);

        /// <summary>
        /// Checks whether payment display name exists.
        /// </summary>
        /// <param name="paymentSettingValidationViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual JsonResult IsPaymentDisplayNameExists(PaymentSettingValidationViewModel paymentSettingValidationViewModel)
            => Json(!_paymentAgent.IsPaymentDisplayNameExists(paymentSettingValidationViewModel), JsonRequestBehavior.AllowGet);
    }
}
using MvcSiteMapProvider;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Controllers
{
    public class RMAManagerController : BaseController
    {
        #region Private Variables  
        private readonly IRMARequestAgent _rmaRequestAgent;
        private readonly IGiftCardAgent _giftCardAgent;

        #region Constant Variables
        private const string ViewFlag = "view";
        private const string EditRMAItemsView = "EditRMAItemsView";
        private const string EditFlag = "edit";
        private const string RMACreateFlag = "rmacreate";
        private const string EditMode = "Edit";
        private const string AppendFlag = "append";
        private const string RmaMode = "rma";
        private const string CreateEdit = "~/Views/GiftCard/CreateEdit.cshtml";

        #endregion
        #endregion

        #region Constructor
        public RMAManagerController(IRMARequestAgent rmaRequestAgent, IGiftCardAgent giftCardAgent)
        {
            _rmaRequestAgent = rmaRequestAgent;
            _giftCardAgent = giftCardAgent;
        }
        #endregion


        public virtual ActionResult CreateRMA(int omsOrderDetailsId, string OrderState, string OrderDate, int portalId, int omsOrderId)
        {
            RMARequestParameterViewModel rmaRequestParamModel = new RMARequestParameterViewModel();
            string message = string.Empty;
            bool isEnable = _rmaRequestAgent.IsRMAEnable(omsOrderDetailsId, OrderState, OrderDate, out message);

            if (isEnable)
                return View(EditRMAItemsView, _rmaRequestAgent.GetRMARequestItemListViewModel(rmaRequestParamModel = new RMARequestParameterViewModel { OmsOrderDetailsId = omsOrderDetailsId, PortalId = portalId, OMSOrderId = omsOrderId }, string.Empty));

            SetNotificationMessage(GetErrorNotificationMessage(message));
            return RedirectToAction<OrderController>(x => x.List(null, 0, 0, 0, null));
        }

        /// <summary>
        ///  To save RMA to database
        /// </summary>
        /// <param name="requestModel">CreateRequestViewModel requestModel</param>
        /// <returns>returns true/false</returns>
        [HttpPost]
        public virtual JsonResult CreateRMA(RMARequestViewModel requestModel = null)
        {
            bool status = false;
            string message = string.Empty;
            if (IsNotNull(requestModel))
            {
                status = _rmaRequestAgent.CreateRMARequest(requestModel);
                message = status ? Admin_Resources.RecordCreationSuccessMessage : Admin_Resources.ErrorFailedToCreate;
            }
            else
                message = Admin_Resources.ErrorSelectRMAItem;

            return Json(new { success = status, message = message, id = requestModel.RMARequestID });
        }

        //Bind RMARequest List
        [MvcSiteMapNode(Title = "$Resources:Admin_Resources.Admin_Resources,HeaderRMAManager", Key = "RMAManager", Area = "", ParentKey = "Setup")]
        public virtual ActionResult RMAList([ModelBinder(typeof(PageDataBinder))]FilterCollectionDataModel model, bool messageShowStatus = false)
        {
            RMARequestListViewModel rmaRequestListViewModel = _rmaRequestAgent.GetRMARequestList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            rmaRequestListViewModel.GridModel = FilterHelpers.GetDynamicGridModel(model, rmaRequestListViewModel?.RMARequestList, GridListType.ZnodeRmaRequest.ToString(), string.Empty, null, true, true, rmaRequestListViewModel?.GridModel?.FilterColumn?.ToolMenuList);
            rmaRequestListViewModel.GridModel.TotalRecordCount = rmaRequestListViewModel.TotalResults;
            if (messageShowStatus)
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));

            //Return RMARequest list
            return ActionView(rmaRequestListViewModel);
        }

        //Show RMA request item for RMA request
        [HttpGet]
        public virtual ActionResult View(RMARequestParameterViewModel rmaRequestParamModel)
        => View(EditRMAItemsView, _rmaRequestAgent.GetRMARequestItemListViewModel(rmaRequestParamModel, ViewFlag));

        [HttpGet]
        public virtual ActionResult Edit(RMARequestParameterViewModel rmaRequestParamModel)
        => View(EditRMAItemsView, _rmaRequestAgent.GetRMARequestItemListViewModel(rmaRequestParamModel, EditFlag));

        //Append RMA Request
        [HttpGet]
        public virtual ActionResult Append(RMARequestParameterViewModel rmaRequestParamModel)
        {

            RMARequestItemListViewModel rmaItemList = _rmaRequestAgent.GetRMARequestItemListViewModel(rmaRequestParamModel, AppendFlag);
            if (rmaItemList?.RMARequestItemList?.Count > 0)
                return View(EditRMAItemsView, rmaItemList);
            else
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorAllRMAItemsProcessed));
                return RedirectToAction<RMAManagerController>(x => x.RMAList(null, false));
            }
        }

        // Gets converted currency values for the provided decimal values.
        public virtual JsonResult GetConvertedCurrencyValues(decimal decimalValue, string currencyCode)
         => Json(new { _currencyValue = HelperMethods.FormatPriceWithCurrency(decimalValue, currencyCode) });

        //Issue gift card for RMA
        [HttpPost]
        public virtual ActionResult IssueGiftCard(RMARequestViewModel requestModel = null)
        {
            if (requestModel.Total == "0.00")
            {
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorSelectedPriceGreatedThanZero));
                return null;
            }
            //Get RMA Request
            RMARequestViewModel rmaRequest = _rmaRequestAgent.GetRMARequest(requestModel.RMARequestID);

            if (IsNotNull(rmaRequest))
            {
                //Update RMA Request
                rmaRequest.Comments = requestModel.Comments;
                _rmaRequestAgent.UpdateRMARequest(requestModel.RMARequestID, rmaRequest);
            }
            //Set RMA Data
            return View(CreateEdit, _giftCardAgent.SetRMAData(requestModel));

        }

    }
}

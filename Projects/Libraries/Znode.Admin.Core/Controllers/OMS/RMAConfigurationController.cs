using Newtonsoft.Json;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class RMAConfigurationController : BaseController
    {
        #region Private Variables
        private readonly IRMAConfigurationAgent _rmaConfigurationAgent;
        private const string IssueGiftCardPartialView = "_IssueGiftCard";
        private const string ReasonForReturnListPartialView = "_ReasonForReturnList";
        private const string CreateReasonForReturnPartialView = "_CreateReasonForReturn";
        private const string EditRequestStatusPartialView = "_EditRequestStatus";
        private const string RequestStatusListPartialView = "_RequestStatusList";
        private const string EmailId = "EmailId";
        #endregion

        #region Constructor
        public RMAConfigurationController(IRMAConfigurationAgent rmaConfigurationAgent)
        {
            _rmaConfigurationAgent = rmaConfigurationAgent;
        }
        #endregion

        #region General Tab
        //Open landing page of RMAConfiguration.
        [HttpGet]
        public virtual ActionResult CreateEdit() => View("_CreateEdit", _rmaConfigurationAgent.GetRMAConfiguration());

        //Post the data of RMAConfiguration.
        [HttpPost]
        public virtual ActionResult CreateEdit(RMAConfigurationViewModel rmaConfiguration)
        {
            if (ModelState.IsValid)
            {
                RMAConfigurationViewModel rmaConfigurationModel = _rmaConfigurationAgent.CreateRMAConfiguration(rmaConfiguration);

                if (!rmaConfigurationModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(rmaConfigurationModel.ActionMode==AdminConstants.Create? Admin_Resources.RecordCreationSuccessMessage : Admin_Resources.UpdateMessage));
                    return View("_CreateEdit", rmaConfigurationModel);
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(rmaConfiguration.RmaConfigurationId == 0 ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage));
            return View("_CreateEdit", new RMAConfigurationViewModel());
        }
        #endregion

        #region Issue Gift Card Tab
        //Open the IssueGiftCard PartialView.
        [HttpGet]
        public virtual ActionResult IssueGiftCard()
            => ActionView(IssueGiftCardPartialView, _rmaConfigurationAgent.GetRMAConfiguration());

        //Post the data about IssueGiftCard.
        [HttpPost]
        public virtual ActionResult IssueGiftCard(RMAConfigurationViewModel rmaConfiguration)
        {
            if (ModelState.IsValid)
            {
                RMAConfigurationViewModel rmaConfigurationModel = _rmaConfigurationAgent.CreateRMAConfiguration(rmaConfiguration);

                if (!rmaConfigurationModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(rmaConfigurationModel.ActionMode == AdminConstants.Create ? Admin_Resources.RecordCreationSuccessMessage : Admin_Resources.UpdateMessage));
                    return View(IssueGiftCardPartialView, rmaConfigurationModel);
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(rmaConfiguration.RmaConfigurationId == 0 ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage));
            return View(IssueGiftCardPartialView, new RMAConfigurationViewModel());
        }
        #endregion

        #region Reason For Return 
        //Get the List of all Reasons For Return.
        public virtual ActionResult GetReasonForReturnList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            // Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeRmaReasonForReturn.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeRmaReasonForReturn.ToString(), model);
            RequestStatusListViewModel requestStatusList = _rmaConfigurationAgent.GetReasonForReturnOrRequestStatusList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, false);
            //Get the grid model.
            requestStatusList.GridModel = FilterHelpers.GetDynamicGridModel(model, requestStatusList.RequestStatusList, GridListType.ZnodeRmaReasonForReturn.ToString(), string.Empty, null, true, true, requestStatusList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count.
            requestStatusList.GridModel.TotalRecordCount = requestStatusList.TotalResults;
            return ActionView(ReasonForReturnListPartialView, requestStatusList);
        }

        //Create New Reason For Return.
        [HttpGet]
        public virtual ActionResult CreateReasonForReturn() => ActionView(CreateReasonForReturnPartialView, new RequestStatusViewModel() { IsActive = true });

        //Create New Reason For Retrun.
        [HttpPost]
        public virtual ActionResult CreateReasonForReturn(RequestStatusViewModel reasonForReturn)
        {
            ModelState.Remove(AdminConstants.Name);
            if (ModelState.IsValid)
            {
                //Get details of newly created theme.
                reasonForReturn = _rmaConfigurationAgent.CreateReasonForReturn(reasonForReturn);
                if (reasonForReturn?.RmaReasonForReturnId > 0)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<RMAConfigurationController>(x => x.GetReasonForReturnList(null));
                }
            }
            //Set error message reason for return.            
            SetNotificationMessage(GetErrorNotificationMessage(reasonForReturn.ErrorMessage));
            return RedirectToAction<RMAConfigurationController>(x => x.GetReasonForReturnList(null));
        }


        public virtual JsonResult EditReasonForReturn(int RmaReasonForReturnId, bool IsActive, string data)
        {
            RequestStatusViewModel reasonForReturn = JsonConvert.DeserializeObject<RequestStatusViewModel[]>(data)[0];
            ModelState.Remove(AdminConstants.Name);
            if (ModelState.IsValid)
            {
                reasonForReturn = _rmaConfigurationAgent.UpdateReasonForReturnOrRequestStatus(reasonForReturn, false);
                if (!reasonForReturn.HasError)
                    return Json(new { status = true, message = Admin_Resources.UpdateMessage }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = PIM_Resources.UpdateErrorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Delete an existing Reason For Return
        public virtual JsonResult DeleteReasonForReturn(string rmaReasonForReturnId)
        {
            string errorMessage = Admin_Resources.DeleteErrorMessage;
            bool status = false;
            if (!string.IsNullOrEmpty(rmaReasonForReturnId))
                status = _rmaConfigurationAgent.DeleteReasonForReturnOrRequestStatus(rmaReasonForReturnId, false, out errorMessage);

            return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : errorMessage }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Request Status
        //Get the List of all Request Status.
        public virtual ActionResult GetRequestStatusList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeRmaRequestStatus.ToString(), model);
            //Get GiftCard list.
            RequestStatusListViewModel requestStatusList = _rmaConfigurationAgent.GetReasonForReturnOrRequestStatusList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage, true);

            //Get the grid model.
            requestStatusList.GridModel = FilterHelpers.GetDynamicGridModel(model, requestStatusList?.RequestStatusList, GridListType.ZnodeRmaRequestStatus.ToString(), string.Empty, null, true, true, requestStatusList?.GridModel?.FilterColumn?.ToolMenuList);
            requestStatusList.GridModel.TotalRecordCount = requestStatusList.TotalResults;

            //Returns the request status list.
            return ActionView(RequestStatusListPartialView, requestStatusList);
        }

        //Get:Edit Request Status.
        [HttpGet]
        public virtual ActionResult EditRequestStatus(int rmaRequestStatusId)
         => ActionView(EditRequestStatusPartialView, _rmaConfigurationAgent.GetReasonForReturnOrRequestStatus(0, rmaRequestStatusId, true));

        //Post:Edit Request Status.
        [HttpPost]
        public virtual ActionResult EditRequestStatus(RequestStatusViewModel requestStatusViewModel)
        {
            ModelState.Remove(AdminConstants.Reason);
            if (ModelState.IsValid)
            {
                SetNotificationMessage(_rmaConfigurationAgent.UpdateReasonForReturnOrRequestStatus(requestStatusViewModel, true).HasError
                ? GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage)
                : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<RMAConfigurationController>(x => x.GetRequestStatusList(null));
            }
            return View(EditRequestStatusPartialView, requestStatusViewModel);
        }

        //Delete Request Status.      
        public virtual JsonResult DeleteRequestStatus(string rmaRequestStatusId)
        {
            string errorMessage = Admin_Resources.DeleteErrorMessage;
            bool status = false;
            if (!string.IsNullOrEmpty(rmaRequestStatusId))
                status = _rmaConfigurationAgent.DeleteReasonForReturnOrRequestStatus(rmaRequestStatusId, true, out errorMessage);

            return Json(new { status = status, message = status ? Admin_Resources.DeleteMessage : errorMessage }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
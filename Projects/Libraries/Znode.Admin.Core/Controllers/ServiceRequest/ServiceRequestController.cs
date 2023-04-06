using System;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using Znode.Engine.Admin.Extensions;

namespace Znode.Engine.Admin.Controllers
{
    public class ServiceRequestController : BaseController
    {
        #region Private Readonly members
        private readonly ICaseRequestAgent _caseRequestAgent;
        private readonly string replyCustomerView = "_ReplyCustomer";
        #endregion

        #region Public Constructor
        public ServiceRequestController(ICaseRequestAgent caseRequestAgent)
        {
            _caseRequestAgent = caseRequestAgent;
        }
        #endregion

        #region Public Methods

        #region Service request
        //Get the list of service request.
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCaseRequest.ToString(), model);
            //List of case request.
            CaseRequestListViewModel list = _caseRequestAgent.GetCaseRequests(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            list.GridModel = FilterHelpers.GetDynamicGridModel(model, list.CaseRequestsList, GridListType.ZnodeCaseRequest.ToString(), string.Empty, null, true, true, list?.GridModel?.FilterColumn?.ToolMenuList);
            //set the total record count
            list.GridModel.TotalRecordCount = list.TotalResults;

            //returns the view
            return ActionView(list);
        }

        //Method return a View to add service request.
        [HttpGet]
        public virtual ActionResult Create()
        {
            CaseRequestViewModel model = new CaseRequestViewModel();
            //Bind store list,priority list and status list.
            _caseRequestAgent.BindPageDropdown(model);
            model.CaseOrigin = Admin_Resources.LabelAdmin;
            model.CreatedDate = DateTime.Now.ToDateTimeFormat();
            return View(AdminConstants.CreateEdit, model);
        }

        //Method to add a new service request, having parameter CaseRequestViewModel.
        [HttpPost]
        public virtual ActionResult Create(CaseRequestViewModel caseRequestViewModel)
        {
            ModelState.Remove("EmailSubject");
            ModelState.Remove("EmailMessage");
            if (ModelState.IsValid)
            {
                caseRequestViewModel = _caseRequestAgent.CreateCaseRequest(caseRequestViewModel);

                if (!caseRequestViewModel.HasError)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(caseRequestViewModel.ErrorMessage));
                return RedirectToAction<ServiceRequestController>(x => x.Edit(caseRequestViewModel.CaseRequestId));
            }
            SetNotificationMessage(GetErrorNotificationMessage(caseRequestViewModel.ErrorMessage));
            return View(AdminConstants.CreateEdit, caseRequestViewModel);
        }

        //Edit existing service request
        [HttpGet]
        public virtual ActionResult Edit(int caseRequestId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            CaseRequestViewModel model = _caseRequestAgent.GetCaseRequest(caseRequestId);

            if (HelperUtility.IsNotNull(model))
                //Bind store list,priority list and status list.
                _caseRequestAgent.BindPageDropdown(model);
            return ActionView(AdminConstants.CreateEdit, model);
        }

        //Update case request.
        [HttpPost]
        public virtual ActionResult Edit(CaseRequestViewModel caseRequestViewModel)
        {
            ModelState.Remove("EmailSubject");
            ModelState.Remove("EmailMessage");
            if (ModelState.IsValid)
            {
                SetNotificationMessage(_caseRequestAgent.UpdateCaseRequest(caseRequestViewModel).HasError
                ? GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage)
                : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<ServiceRequestController>(x => x.Edit(caseRequestViewModel.CaseRequestId));
            }
            _caseRequestAgent.BindPageDropdown(caseRequestViewModel);
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.UpdateErrorMessage));
            return RedirectToAction<ServiceRequestController>(x => x.Edit(caseRequestViewModel.CaseRequestId));
        }

        //Get data from case request id to reply the customer.
        [HttpGet]
        public virtual ActionResult ReplyCustomer(int caseRequestId)
            => View(replyCustomerView, _caseRequestAgent.GetCaseRequest(caseRequestId));


        //Reply to customer
        [HttpPost]
        public virtual ActionResult ReplyCustomer(CaseRequestViewModel model)
        {
            ModelState.Remove("Title");
            ModelState.Remove("Description");
            ModelState.Remove("PhoneNumber");
            ModelState.Remove("StoreName");
            if (ModelState.IsValid)
            {
                if (HelperUtility.IsNotNull(model))
                {
                    SetNotificationMessage(_caseRequestAgent.ReplyCustomer(model).HasError
                    ? GetErrorNotificationMessage(Admin_Resources.ErrorFailedToSendEmail)
                    : GetSuccessNotificationMessage(Admin_Resources.SuccessEmailMessage));
                    return RedirectToAction<ServiceRequestController>(x => x.GetCaseRequestMailHistory(null, model.CaseRequestId));
                }
                CaseRequestViewModel caseRequest = _caseRequestAgent.GetCaseRequest(model.CaseRequestId);
                SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorFailedToSendEmail));
                return View(replyCustomerView, caseRequest);
            }
            SetNotificationMessage(GetErrorNotificationMessage(model.ErrorMessage));
            return ActionView(replyCustomerView, model);
        }


        //Get mail history list to customer.
        public virtual ActionResult GetCaseRequestMailHistory([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int caseRequestId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCaseRequestHistory.ToString(), model);
            CaseRequestViewModel caseRequest = _caseRequestAgent.GetCaseRequest(caseRequestId);
            if (HelperUtility.IsNotNull(caseRequest))
            {
                _caseRequestAgent.SetFiltersForCaseRequestId(model?.Filters, caseRequestId);
                //Get mail history list to customer
                CaseRequestListViewModel caseRequestList = _caseRequestAgent.GetCaseRequestMailHistory(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
                caseRequestList.CaseRequestId = caseRequestId;
                caseRequestList.Title = caseRequest.Title;
                caseRequestList.GridModel = FilterHelpers.GetDynamicGridModel(model, caseRequestList?.CaseRequestsList, GridListType.ZnodeCaseRequestHistory.ToString(), string.Empty, null, true, true, null);
                caseRequestList.GridModel.TotalRecordCount = caseRequestList.TotalResults;
                return ActionView("_GetMailHistoryList", caseRequestList);
            }
            return ActionView("_GetMailHistoryList", null);
        }


        //Get note list
        public virtual ActionResult GetNoteList([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model, int caseRequestId)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeServiceRequestNote.ToString(), model);
            CaseRequestViewModel caseRequest = _caseRequestAgent.GetCaseRequest(caseRequestId);
            if (HelperUtility.IsNotNull(caseRequest))
            {
                _caseRequestAgent.SetFiltersForCaseRequestId(model?.Filters, caseRequestId);
                //Get note list as per caseRequestId.
                NoteListViewModel noteList = _caseRequestAgent.GetNotes(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
                noteList.CaseRequestId = caseRequestId;
                noteList.Title = caseRequest.Title;
                noteList.GridModel = FilterHelpers.GetDynamicGridModel(model, noteList?.Notes, GridListType.ZnodeServiceRequestNote.ToString(), string.Empty, null, true, true, noteList?.GridModel?.FilterColumn?.ToolMenuList);
                noteList.GridModel.TotalRecordCount = noteList.TotalResults;
                return ActionView("_NoteList", noteList);
            }
            return ActionView("_NoteList", null);
        }

        //Add note as per case request id.
        [HttpGet]
        public virtual ActionResult AddNote(int caseRequestId)
          => ActionView("_CreateNote", new NoteViewModel { CaseRequestId = caseRequestId });

        //Add note
        [HttpPost]
        public virtual ActionResult AddNote(NoteViewModel model)
        {
            if (ModelState.IsValid)
            {
                SetNotificationMessage(_caseRequestAgent.SaveCaseRequestsNotes(model).HasError
               ? GetErrorNotificationMessage(Admin_Resources.ErrorFailedToAddNote)
               : GetSuccessNotificationMessage(Admin_Resources.NoteCreationSuccessMessage));
                return RedirectToAction<ServiceRequestController>(x => x.GetNoteList(null, Convert.ToInt32(model.CaseRequestId)));
            }
            return View("_CreateNote", model);
        }
        #endregion

        #endregion
    }
}
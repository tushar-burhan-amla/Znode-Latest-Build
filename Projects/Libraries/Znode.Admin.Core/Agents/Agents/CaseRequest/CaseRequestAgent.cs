using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using Znode.Engine.Exceptions;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using System;
using System.IO;
using System.Web;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Admin.Agents
{
    public class CaseRequestAgent : BaseAgent, ICaseRequestAgent
    {
        #region Private Variables
        private readonly IWebStoreCaseRequestClient _caseRequestClient;
        private readonly IPortalClient _portalClient;
        private readonly IAccountClient _accountClient;
        #endregion

        #region Constructor
        public CaseRequestAgent(IAccountClient accountClient, IPortalClient portalClient, IWebStoreCaseRequestClient caseRequestClient)
        {
            _accountClient = GetClient<IAccountClient>(accountClient);
            _portalClient = GetClient<IPortalClient>(portalClient);
            _caseRequestClient = GetClient<IWebStoreCaseRequestClient>(caseRequestClient);
        }
        #endregion

        #region Public Methods

        #region Case request

        //Get the list service request.
        public virtual CaseRequestListViewModel GetCaseRequests(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null)
        {
						ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
						if (HelperUtility.IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(ZnodeCaseRequestEnum.CaseRequestId.ToString(), DynamicGridConstants.DESCKey);
            }
						ZnodeLogging.LogMessage("Input parameters:", string.Empty, TraceLevel.Verbose, new { filters = filters, sorts = sorts });

						//Get list of service request.
						WebStoreCaseRequestListModel caseRequestsList = _caseRequestClient.GetCaseRequests(null, filters, sorts, pageIndex, pageSize);
            CaseRequestListViewModel listViewModel = new CaseRequestListViewModel { CaseRequestsList = caseRequestsList?.CaseRequestList?.ToViewModel<CaseRequestViewModel>().ToList() };

            SetListPagingData(listViewModel, caseRequestsList);
						ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
						return caseRequestsList?.CaseRequestList?.Count > 0 ? listViewModel : new CaseRequestListViewModel() { CaseRequestsList = new List<CaseRequestViewModel>() };
        }

        //Create case Request.
        public virtual CaseRequestViewModel CreateCaseRequest(CaseRequestViewModel caseRequestViewModel)
        {
						ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
						try
            {
                caseRequestViewModel.UserId = Convert.ToInt32(SessionProxyHelper.GetUserDetails()?.UserId);
								ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
								return _caseRequestClient.CreateCaseRequest(caseRequestViewModel?.ToModel<WebStoreCaseRequestModel>())?.ToViewModel<CaseRequestViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    default:
                        return (CaseRequestViewModel)GetViewModelWithErrorMessage(caseRequestViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (CaseRequestViewModel)GetViewModelWithErrorMessage(caseRequestViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get service request by case request Id.
        public virtual CaseRequestViewModel GetCaseRequest(int caseRequestId)
        {
						ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
						WebStoreCaseRequestModel webStoreCaseRequestModel = _caseRequestClient.GetCaseRequest(caseRequestId);
            if (webStoreCaseRequestModel?.CaseRequestId > 0)
                return webStoreCaseRequestModel.ToViewModel<CaseRequestViewModel>();

            return null;
        }

        //Update service request.
        public virtual CaseRequestViewModel UpdateCaseRequest(CaseRequestViewModel caseRequestViewModel)
        {
						ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
						try
            {
                return _caseRequestClient.UpdateCaseRequest(caseRequestViewModel?.ToModel<WebStoreCaseRequestModel>())?.ToViewModel<CaseRequestViewModel>();
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (CaseRequestViewModel)GetViewModelWithErrorMessage(caseRequestViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Get note list.
        public virtual NoteListViewModel GetNotes(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
						ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
						ZnodeLogging.LogMessage("Input parameters:", string.Empty, TraceLevel.Verbose, new { filters = filters, sorts = sorts });

						NoteListModel noteListModel = _accountClient.GetAccountNotes(null, filters, sorts, pageIndex, recordPerPage);
            NoteListViewModel noteListViewModel = new NoteListViewModel { Notes = noteListModel?.Notes?.ToViewModel<NoteViewModel>().ToList() };
            SetListPagingData(noteListViewModel, noteListModel);

						ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
						return noteListViewModel?.Notes?.Count > 0 ? noteListViewModel
                : new NoteListViewModel() { Notes = new List<NoteViewModel>() };
        }

        //Create case request note.
        public virtual NoteViewModel SaveCaseRequestsNotes(NoteViewModel noteViewModel)
        {
						ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
						try
            {
                if (HelperUtility.IsNotNull(noteViewModel))
                {
                    noteViewModel.UserId = Convert.ToInt32(SessionProxyHelper.GetUserDetails()?.UserId);
										ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
										return _accountClient.CreateAccountNote(noteViewModel.ToModel<NoteModel>())?.ToViewModel<NoteViewModel>();
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (NoteViewModel)GetViewModelWithErrorMessage(noteViewModel, Admin_Resources.ErrorNoteAlreadyExists);
                    default:
                        return (NoteViewModel)GetViewModelWithErrorMessage(noteViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (NoteViewModel)GetViewModelWithErrorMessage(noteViewModel, Admin_Resources.ErrorFailedToCreate);
            }
            return new NoteViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
        }

        //Reply to customer.
        public virtual CaseRequestViewModel ReplyCustomer(CaseRequestViewModel caseRequestViewModel)
        {
						ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
						try
            {
                if (HelperUtility.IsNotNull(caseRequestViewModel.FilePath))
                    //Method to save file into the folder.
                    SaveFileToFolder(caseRequestViewModel);
								ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
								return _caseRequestClient.ReplyCustomer(caseRequestViewModel?.ToModel<WebStoreCaseRequestModel>())?.ToViewModel<CaseRequestViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                return (CaseRequestViewModel)GetViewModelWithErrorMessage(caseRequestViewModel, Admin_Resources.FailedToSendEmail);
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (CaseRequestViewModel)GetViewModelWithErrorMessage(caseRequestViewModel, Admin_Resources.FailedToSendEmail);
            }
        }

        //Get mail history list to customer.
        public virtual CaseRequestListViewModel GetCaseRequestMailHistory(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null)
        {
						ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
						ZnodeLogging.LogMessage("Input parameters:", string.Empty, TraceLevel.Verbose, new { filters = filters, sorts = sorts });

						//Add new IsCaseHistory Into filters to get list of mail history. 
						filters.Add(new FilterTuple(FilterKeys.IsCaseHistory.ToString(), FilterOperators.Equals, "1"));

            //Get mail history list to customer.
            WebStoreCaseRequestListModel caseRequestsList = _caseRequestClient.GetCaseRequests(null, filters, sorts, pageIndex, pageSize);
            CaseRequestListViewModel listViewModel = new CaseRequestListViewModel { CaseRequestsList = caseRequestsList?.CaseRequestList?.ToViewModel<CaseRequestViewModel>().ToList() };

            SetListPagingData(listViewModel, caseRequestsList);

            if (string.IsNullOrWhiteSpace(caseRequestsList?.CaseRequestList?.Select(x => x.CaseRequestHistoryId)?.FirstOrDefault().ToString()))
                listViewModel = new CaseRequestListViewModel() { CaseRequestsList = new List<CaseRequestViewModel>() };
						ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
						return caseRequestsList?.CaseRequestList?.Count > 0 ? listViewModel : new CaseRequestListViewModel() { CaseRequestsList = new List<CaseRequestViewModel>() };
        }
        //Bind store list,priority list and status list.
        public virtual CaseRequestViewModel BindPageDropdown(CaseRequestViewModel model)
        {
						ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
						//Portal list
						model.PortalList = GetStoreList();
            //Case priority list
            model.CasePriorityList = GetCasePriorityList();
            //Case Status list
            model.CaseStatusList = GetCaseStatusList();
						ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
						return model;
        }

        public virtual void SetFiltersForCaseRequestId(FilterCollection filters, int caseRequestId)
        {
						ZnodeLogging.LogMessage("Agent method execution started.", string.Empty, TraceLevel.Info);
						if (HelperUtility.IsNotNull(filters))
            {
                //Checking For caseRequestId already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1 == ZnodeCaseRequestEnum.CaseRequestId.ToString()))
                {
                    //If caseRequestId Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == ZnodeCaseRequestEnum.CaseRequestId.ToString());

                    //Add New caseRequestId Into filters
                    filters.Add(new FilterTuple(ZnodeCaseRequestEnum.CaseRequestId.ToString(), FilterOperators.Equals, caseRequestId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(ZnodeCaseRequestEnum.CaseRequestId.ToString(), FilterOperators.Equals, caseRequestId.ToString()));
            }
						ZnodeLogging.LogMessage("Parameters:", string.Empty, TraceLevel.Verbose, new { filters = filters});
						ZnodeLogging.LogMessage("Agent method executed.", string.Empty, TraceLevel.Info);
				}
				#endregion

				#region Private Methods
				//Method to save file into the folder temporarily.
				private void SaveFileToFolder(CaseRequestViewModel caseRequestViewModel)
        {
            string attachedPath = Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.TemporaryFolderPath), caseRequestViewModel.FilePath.FileName);
						ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.PIM.ToString(), TraceLevel.Verbose, new { attachedPath = attachedPath });
						caseRequestViewModel.AttachedPath = attachedPath;

            // If same name of file present then delete that file first
            if (File.Exists(attachedPath))
                File.Delete(attachedPath);

            //Save file in the temporary folder.
            caseRequestViewModel.FilePath.SaveAs(attachedPath);
        }

        //Get Store List
        private List<SelectListItem> GetStoreList()
        {
            PortalListModel portalList = _portalClient.GetPortalList(null, null, null, null, null);
            List<SelectListItem> selectedPortalList = new List<SelectListItem>();
            if (portalList?.PortalList?.Count > 0)
                portalList.PortalList.ToList().ForEach(item => { selectedPortalList.Add(new SelectListItem() { Text = item.StoreName, Value = item.PortalId.ToString() }); });
						ZnodeLogging.LogMessage("selectedPortalList list count:", string.Empty, TraceLevel.Verbose, selectedPortalList?.Count());
						return selectedPortalList;
        }

        //Get case priority list.
        private List<SelectListItem> GetCasePriorityList()
        {
            CasePriorityListModel priorityList = _caseRequestClient.GetCasePriorityList();
            List<SelectListItem> selectedCasePriorityList = new List<SelectListItem>();
            if (priorityList?.CasePriorities?.Count > 0)
                priorityList.CasePriorities.ToList().ForEach(item => { selectedCasePriorityList.Add(new SelectListItem() { Text = item.CasePriorityName, Value = item.CasePriorityId.ToString() }); });
						ZnodeLogging.LogMessage("selectedCasePriorityList list count:", string.Empty, TraceLevel.Verbose, selectedCasePriorityList?.Count());

						return selectedCasePriorityList;
        }

        //Get case status list.
        private List<SelectListItem> GetCaseStatusList()
        {
            CaseStatusListModel caseStatusList = _caseRequestClient.GetCaseStatusList();
            List<SelectListItem> selectedCaseStatusList = new List<SelectListItem>();
            if (caseStatusList?.CaseStatuses?.Count > 0)
                caseStatusList.CaseStatuses.ToList().ForEach(item => { selectedCaseStatusList.Add(new SelectListItem() { Text = item.CaseStatusName, Value = item.CaseStatusId.ToString() }); });
						ZnodeLogging.LogMessage("selectedCaseStatusList list count:", string.Empty, TraceLevel.Verbose, selectedCaseStatusList?.Count());

						return selectedCaseStatusList;
        }

        //Get case type list.
        private List<SelectListItem> GetCaseTypeList()
        {
            CaseTypeListModel caseTypeList = _caseRequestClient.GetCaseTypeList();
            List<SelectListItem> selectedCaseTypeList = new List<SelectListItem>();
            if (caseTypeList?.CaseTypes?.Count > 0)
                caseTypeList.CaseTypes.ToList().ForEach(item => { selectedCaseTypeList.Add(new SelectListItem() { Text = item.CaseTypeName, Value = item.CaseTypeId.ToString() }); });

            return selectedCaseTypeList;
        }
        #endregion

        #endregion
    }
}
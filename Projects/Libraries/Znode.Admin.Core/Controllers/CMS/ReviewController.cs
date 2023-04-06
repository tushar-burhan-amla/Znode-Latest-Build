using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class ReviewController : BaseController
    {
        #region Private Variables
        private readonly ICustomerReviewAgent _customerReviewAgent;
        #endregion

        #region Public Constructor
        public ReviewController(ICustomerReviewAgent customerReviewAgent)
        {
            _customerReviewAgent = customerReviewAgent;
        }
        #endregion

        #region Public Methods

        //Get: Customer Review list.
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.CustomerReviewList.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.CustomerReviewList.ToString(), model);
            //Get Customer Review list.
            CustomerReviewListViewModel customerReviewList = _customerReviewAgent.GetCustomerReviewList(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            customerReviewList.GridModel = FilterHelpers.GetDynamicGridModel(model, customerReviewList?.CustomerReviewList, GridListType.CustomerReviewList.ToString(), string.Empty, null, true, true, customerReviewList?.GridModel?.FilterColumn?.ToolMenuList);
            customerReviewList.GridModel.TotalRecordCount = customerReviewList.TotalResults;

            //Returns the customer review list.
            return ActionView(AdminConstants.ListView, customerReviewList);
        }

        //Get: Customer Review.
        [HttpGet]
        public virtual ActionResult Edit(int cmsCustomerReviewId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;
            if (cmsCustomerReviewId > 0)
            {
                //Get customer review details by its id.
                CustomerReviewViewModel customerReviewModel = _customerReviewAgent.GetCustomerReview(cmsCustomerReviewId);
                return ActionView(AdminConstants.Edit, (!Equals(customerReviewModel, null)) ? customerReviewModel : new CustomerReviewViewModel());
            }
            return RedirectToAction<ReviewController>(x => x.List(null));
        }

        //Post: Customer Review.
        [HttpPost]
        public virtual ActionResult Edit(CustomerReviewViewModel customerReviewViewModel)
        {
            CustomerReviewViewModel customerReview = _customerReviewAgent.UpdateCustomerReview(customerReviewViewModel);

            //Updates customer review details and redirect to list with success message else returns an error message.
            if (!customerReview.HasError)
            {
                SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<ReviewController>(x => x.Edit(customerReviewViewModel.CMSCustomerReviewId));
            }
            SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateErrorMessage));
            return RedirectToAction<ReviewController>(x => x.List(null));
        }

        //Delete: Customer Reviews.
        public virtual JsonResult Delete(string cmsCustomerReviewId)
        {
            string errorMessage = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(cmsCustomerReviewId))
            {
                //Delete customer reviews.
                status = _customerReviewAgent.DeleteCustomerReview(cmsCustomerReviewId, out errorMessage);

                if (status)
                    errorMessage = Admin_Resources.DeleteMessage;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Change review status in bulk. 
        public virtual ActionResult BulkStatusChange(string cmsCustomerReviewId, string statusId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(cmsCustomerReviewId) && !string.IsNullOrEmpty(statusId))
            {
                status = _customerReviewAgent.BulkStatusChange(cmsCustomerReviewId, statusId, out message);
                if (status && statusId == AdminConstants.New)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.SuccessMessageStatusNew));
                else if (status && statusId == AdminConstants.Active)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.SuccessMessageStatusActive));
                else if (status && statusId == AdminConstants.Inactive)
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.SuccessMessageStatusInactive));
                else
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorMessageFailedStatus));
            }
            return RedirectToAction<ReviewController>(x => x.List(null));
        }

        #endregion
    }
}
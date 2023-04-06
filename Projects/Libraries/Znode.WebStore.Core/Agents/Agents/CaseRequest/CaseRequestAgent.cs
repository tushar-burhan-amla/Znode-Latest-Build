using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
namespace Znode.Engine.WebStore.Agents
{
    public class CaseRequestAgent : BaseAgent, ICaseRequestAgent
    {
        #region Private Variables
        private string caseOriginContactUs = ZnodeConstant.ContactUsForm;
        private string caseRequestContactUsTitle =ZnodeConstant.ContactFormSubmission;
        private string caseOriginCustomerFeedback = ZnodeConstant.CustomerFeedbackForm;
        private string caseRequestCustomerFeedbackTitle = ZnodeConstant.UserFeedback;
        private readonly IWebStoreCaseRequestClient _caseRequestClient;

        #endregion

        #region Constructor

        public CaseRequestAgent(IWebStoreCaseRequestClient caseRequestClient)
        {
            _caseRequestClient = GetClient<IWebStoreCaseRequestClient>(caseRequestClient);
        }
        #endregion

        #region Public Methods

        //Create Case Request.
        public CaseRequestViewModel CreateContactUs(CaseRequestViewModel caseRequestViewModel)
        {
            //Assign user id of the logged in user if exists.
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name))
                CheckAndAssignUserId(caseRequestViewModel);

            //Assign values to case request model.
            caseRequestViewModel = GetConstantValues(caseRequestViewModel);
            caseRequestViewModel.CaseOrigin = caseOriginContactUs;
            caseRequestViewModel.Title = caseRequestContactUsTitle;
            WebStoreCaseRequestModel caseRequestModel = _caseRequestClient.CreateContactUs(caseRequestViewModel.ToModel<WebStoreCaseRequestModel>());
            return IsNotNull(caseRequestModel) ? caseRequestModel.ToViewModel<CaseRequestViewModel>() : new CaseRequestViewModel();
        }

        //Create Customer Feedback.
        public CaseRequestViewModel CreateCustomerFeedback(CaseRequestViewModel caseRequestViewModel)
        {
            //Assign user id of the logged in user if exists.
            if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name))
                CheckAndAssignUserId(caseRequestViewModel);

            //Assign values to case request model.
            caseRequestViewModel = CreateCaseRequestModel(caseRequestViewModel);
            WebStoreCaseRequestModel model = caseRequestViewModel.ToModel<WebStoreCaseRequestModel>();
            model.Description = caseRequestViewModel.Message;
            WebStoreCaseRequestModel caseRequestModel = _caseRequestClient.CreateContactUs(model);
            return IsNotNull(caseRequestModel) ? caseRequestModel.ToViewModel<CaseRequestViewModel>() : new CaseRequestViewModel();
        }

        
        #endregion

        #region Private Methods

        //Get constant values for model.
        private CaseRequestViewModel GetConstantValues(CaseRequestViewModel caseRequestViewModel)
        {
            caseRequestViewModel.PortalId = PortalAgent.CurrentPortal.PortalId;
            caseRequestViewModel.CaseStatusId = WebStoreConstants.CaseStatusId;
            caseRequestViewModel.CasePriorityId = WebStoreConstants.CasePriorityId;
            caseRequestViewModel.CaseTypeId = WebStoreConstants.CaseTypeId;
            caseRequestViewModel.LocaleId = PortalAgent.LocaleId;
            return caseRequestViewModel;
        }

        //Check if service request is made by a logged in user or not and then assign its userid in the model.
        private void CheckAndAssignUserId(CaseRequestViewModel caseRequestViewModel)
        {
            IUserAgent _userAgent = new UserAgent(GetClient<CountryClient>(), GetClient<WebStoreUserClient>(), GetClient<WishListClient>(), GetClient<UserClient>(), GetClient<PublishProductClient>(), GetClient<CustomerReviewClient>(), GetClient<OrderClient>(), GetClient<GiftCardClient>(), GetClient<AccountClient>(), GetClient<AccountQuoteClient>(), GetClient<OrderStateClient>(), GetClient<PortalCountryClient>(), GetClient<ShippingClient>(), GetClient<PaymentClient>(), GetClient<CustomerClient>(), GetClient<StateClient>(), GetClient<PortalProfileClient>());
            //Gets the saved data of user.
            UserViewModel userViewModel = _userAgent.GetUserViewModelFromSession();
            if (IsNotNull(userViewModel))
                caseRequestViewModel.UserId = userViewModel.UserId;
        }

        //Assign values to case request model.
        private CaseRequestViewModel CreateCaseRequestModel(CaseRequestViewModel caseRequestViewModel)
        {
            caseRequestViewModel.Message = (caseRequestViewModel.AllowSharingWithCustomer) ? string.Format(WebStore_Resources.MessageCustomerFeedbackTrue, caseRequestViewModel.Message, caseRequestViewModel.CustomerCity, caseRequestViewModel.CustomerState) : string.Format(WebStore_Resources.MessageCustomerFeedbackFalse, caseRequestViewModel.Message, caseRequestViewModel.CustomerCity, caseRequestViewModel.CustomerState);
            //Get constant values for model.
            caseRequestViewModel = GetConstantValues(caseRequestViewModel);

            caseRequestViewModel.CaseOrigin = caseOriginCustomerFeedback;
            caseRequestViewModel.Title = caseRequestCustomerFeedbackTitle;
            return caseRequestViewModel;
        }
        #endregion
    }
}
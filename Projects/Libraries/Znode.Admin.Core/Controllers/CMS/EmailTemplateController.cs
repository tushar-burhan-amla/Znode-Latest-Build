using System;
using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.Resources;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Controllers
{
    public class EmailTemplateController : BaseController
    {
        #region Private Variable
        private readonly IEmailTemplateAgent _emailTemplateAgent;
        private readonly ILocaleAgent _localeAgent;
        private readonly string _manageTemplateAreaView = "~/Views/EmailTemplate/ManageEmailTemplateArea.cshtml";
        #endregion

        #region Constructor
        public EmailTemplateController(IEmailTemplateAgent emailTemplateAgent, ILocaleAgent localeAgent)
        {
            _emailTemplateAgent = emailTemplateAgent;
            _localeAgent = localeAgent;
        }
        #endregion

        #region Public Methods

        //Get email template list.
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Get and Set Filters from Cookies if exists.
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeEmailTemplate.ToString(), model);
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeEmailTemplate.ToString(), model);
            //Get list of email template.
            EmailTemplateListViewModel emailTemplateList = _emailTemplateAgent.EmailTemplates(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            emailTemplateList.GridModel = FilterHelpers.GetDynamicGridModel(model, emailTemplateList.EmailTemplateList, GridListType.ZnodeEmailTemplate.ToString(), string.Empty, null, true, true, emailTemplateList?.GridModel?.FilterColumn?.ToolMenuList);

            emailTemplateList.GridModel.TotalRecordCount = emailTemplateList.TotalResults;

            return ActionView(emailTemplateList.GridModel);
        }

        //Get:Create Email Template.
        [HttpGet]
        public virtual ActionResult Create()
        {
            EmailTemplateViewModel emailTemplateViewModel = new EmailTemplateViewModel();
            emailTemplateViewModel.Locale = _localeAgent.GetLocalesList();
            emailTemplateViewModel.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
            _emailTemplateAgent.GetEmailTemplateToken(emailTemplateViewModel);
            return View(AdminConstants.CreateEditEmailTemplateView, emailTemplateViewModel);
        }

        //Post:Create Email Template.
        [HttpPost]
        public virtual ActionResult Create(EmailTemplateViewModel emailTemplateViewModel)
        {
            if (ModelState.IsValid)
            {
                emailTemplateViewModel = _emailTemplateAgent.CreateEmailTemplate(emailTemplateViewModel);
                _emailTemplateAgent.GetEmailTemplateToken(emailTemplateViewModel);
                if (!emailTemplateViewModel.HasError)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.RecordCreationSuccessMessage));
                    return RedirectToAction<EmailTemplateController>(x => x.Edit(emailTemplateViewModel.EmailTemplateId, emailTemplateViewModel.LocaleId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(emailTemplateViewModel.ErrorMessage));
            emailTemplateViewModel.Locale = _localeAgent.GetLocalesList(emailTemplateViewModel.LocaleId);
            return View(AdminConstants.CreateEditEmailTemplateView, emailTemplateViewModel);
        }

        //Get:Edit Email Template.
        [HttpGet]
        public virtual ActionResult Edit(int emailTemplateId, int localeId = 0)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;

            return Request.IsAjaxRequest() ? PartialView("_EmailTemplateForLocale", _emailTemplateAgent.GetEmailTemplate(emailTemplateId, localeId)) : ActionView(AdminConstants.CreateEditEmailTemplateView, _emailTemplateAgent.GetEmailTemplate(emailTemplateId, localeId));
        }
        //Post:Edit Email Template.
        [HttpPost]
        public virtual ActionResult Edit(EmailTemplateViewModel emailTemplateViewModel)
        {
            if (ModelState.IsValid)
            {
                SetNotificationMessage(_emailTemplateAgent.UpdateEmailTemplate(emailTemplateViewModel).HasError
                   ? GetErrorNotificationMessage(emailTemplateViewModel.ErrorMessage)
                   : GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                return RedirectToAction<EmailTemplateController>(x => x.Edit(emailTemplateViewModel.EmailTemplateId, emailTemplateViewModel.LocaleId));
            }
            return View(AdminConstants.CreateEditEmailTemplateView, emailTemplateViewModel);
        }

        [HttpGet]
        //Delete Email Template.
        public virtual JsonResult Delete(string emailtemplateId)
        {
            string message = string.Empty;
            bool status = false;
            if (!string.IsNullOrEmpty(emailtemplateId))
            {
                status = _emailTemplateAgent.DeleteEmailTemplate(emailtemplateId);

                message = status ? Admin_Resources.DeleteMessage : Admin_Resources.DeleteErrorMessage;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Display the Preview for the Email Template.
        [HttpGet]
        public virtual ActionResult Preview(string emailTemplateId)
         => PartialView("_Preview", _emailTemplateAgent.GetEmailTemplate(Convert.ToInt32(emailTemplateId), 0));


        //Get the details required for assigning permissions to menu.
        [HttpGet]
        public virtual ActionResult ManageEmailTemplateArea(int portalId = 0)
        {
            EmailTemplateAreaDataViewModel emailTemplateAreaDataViewModel = _emailTemplateAgent.GetMappedEmailTemplateArea(portalId);
            return (Request.IsAjaxRequest()) ? PartialView("_manageEmailTemplateArea", emailTemplateAreaDataViewModel) :
                ActionView(_manageTemplateAreaView, emailTemplateAreaDataViewModel);
        }


        //Delete Get Link Widget Configuration.
        public virtual JsonResult DeleteEmailTemplateAreaMapping(string areaMappingId)
        {
            string message = Admin_Resources.DeleteErrorMessage;
            bool status = false;
            if (!string.IsNullOrEmpty(areaMappingId))
            {
                status = _emailTemplateAgent.DeleteEmailTemplateAreaMapping(areaMappingId);

                if (status)
                    message = Admin_Resources.DeleteMessage;
            }
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }


        //Save Email Template Area Configuration data.
        [HttpPost]
        public virtual JsonResult SaveEmailTemplateAreaConfiguration(EmailTemplateAreaMapperViewModel viewModel)
        {
            string message = string.Empty;

            //Save Email Template Area Configuration details.
            bool status = _emailTemplateAgent.SaveEmailTemplateAreaConfiguration(viewModel);
            return Json(new
            {
                status = status,
                message = status ? (viewModel.EmailTemplateMapperId == 0 ? Admin_Resources.RecordCreationSuccessMessage : Admin_Resources.UpdateMessage)
                : viewModel.EmailTemplateMapperId == 0 ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage,
                emailTemplateMapperId = viewModel.EmailTemplateMapperId
            }, JsonRequestBehavior.AllowGet);
        }

        //Get Available Email Template Area.
        [HttpGet]
        public virtual ActionResult GetAvailableTemplateArea(int portalId = 0)
        {
            //Get List of Available Template Area. 
            EmailTemplateAreaMapperViewModel areaModel = _emailTemplateAgent.GetAvailableTemplateArea(portalId);

            //Convert View Result into the String.
            string partialView = RenderRazorViewToString("_PartialEmailTemplateArea", areaModel);
            bool isAreaAvailable = HelperUtility.IsNotNull(areaModel?.EmailTemplateAreaList) && areaModel?.EmailTemplateAreaList.Count > 0;
            return Json(new { html = partialView, status = isAreaAvailable, message = Admin_Resources.ErrorTemplateAreaNotFound }, JsonRequestBehavior.AllowGet);
        }


        //Get Email Template list based on search term.
        [HttpGet]
        public virtual JsonResult GetEmailTemplateListByName(string searchTerm)
           => Json(_emailTemplateAgent.GetEmailTemplateListByName(searchTerm), JsonRequestBehavior.AllowGet);

        #endregion
    }
}
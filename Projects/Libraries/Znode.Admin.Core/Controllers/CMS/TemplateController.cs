using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class TemplateController : BaseController
    {
        #region Private Variable
        private readonly ITemplateAgent _templateAgent;
        #endregion

        #region Constructor
        public TemplateController(ITemplateAgent templateAgent)
        {
            _templateAgent = templateAgent;
        }
        #endregion

        #region Public Methods

        //Method return a list view to display list of templates.
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView(GridListType.ZnodeCMSTemplate.ToString(), model);
            TemplateListViewModel templateList = _templateAgent.GetTemplates(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            //Get the grid model.
            templateList.GridModel = FilterHelpers.GetDynamicGridModel(model, templateList.Templates, GridListType.ZnodeCMSTemplate.ToString(), string.Empty, null, true, true, templateList?.GridModel?.FilterColumn?.ToolMenuList);

            //Set the total record count
            templateList.GridModel.TotalRecordCount = templateList.TotalResults;
            return ActionView(templateList);
        }

        //Method return a View to add new template.
        [HttpGet]
        public virtual ActionResult Create()
            => View(AdminConstants.CreateEdit, new TemplateViewModel());

        //Method to add a new template, having parameter TemplateViewModel contains template name.
        [HttpPost]
        public virtual ActionResult Create(TemplateViewModel templateViewModel)
        {
            if (ModelState.IsValid)
            {
                if (HelperUtility.IsNull(templateViewModel.FilePath))
                {
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.RequiredFile));
                    return View(AdminConstants.CreateEdit, templateViewModel);
                }

                //Get details of newly created template.
                templateViewModel = _templateAgent.CreateTemplate(templateViewModel);
                if (templateViewModel?.CMSTemplateId > 0)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.TemplateCreationSuccessMessage));
                    return RedirectToAction<TemplateController>(x => x.Edit(templateViewModel.CMSTemplateId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(templateViewModel.ErrorMessage));
            return View(AdminConstants.CreateEdit, templateViewModel);
        }

        [HttpPost]
        public virtual ActionResult TemplateName(string templateName)
        {
            bool isExist = _templateAgent.CheckTemplateName(templateName);
            return Json(isExist, JsonRequestBehavior.AllowGet);
        }

        //Method return a View to edit template.
        [HttpGet]
        public virtual ActionResult Edit(int cmsTemplateId)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;
            return View(AdminConstants.CreateEdit, _templateAgent.GetTemplate(cmsTemplateId));
        }

        //Update the template.
        [HttpPost]
        public virtual ActionResult Edit(TemplateViewModel themeViewModel)
        {
            if (ModelState.IsValid)
            {
                //Get details of newly created theme.
                themeViewModel = _templateAgent.UpdateTemplate(themeViewModel);
                if (themeViewModel?.CMSTemplateId > 0)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.UpdateMessage));
                    return RedirectToAction<TemplateController>(x => x.Edit(themeViewModel.CMSTemplateId));
                }
            }
            return View(AdminConstants.CreateEdit, themeViewModel);
        }

        //Method return a view to edit template.
        [HttpGet]
        public virtual ActionResult Copy(int cmsTemplateId = 0)
        {
            //If the parent id is lost then redirect to list page.
            if (cmsTemplateId == 0)
                return RedirectToAction<TemplateController>(x => x.List(null));

            TemplateViewModel templateViewModel = _templateAgent.GetTemplate(cmsTemplateId);
            templateViewModel.Name = "Copy_" + templateViewModel.Name;
            return ActionView(AdminConstants.CreateEdit, templateViewModel);
        }

        //Copy the template.
        [HttpPost]
        public virtual ActionResult Copy(TemplateViewModel themeViewModel)
        {
            ModelState.Remove("FilePath");
            if (ModelState.IsValid)
            {
                TemplateViewModel templateViewModel = _templateAgent.CopyTemplate(themeViewModel);
                if (templateViewModel?.CMSTemplateId > 0)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.TemplateCopyMessage));
                    return RedirectToAction<TemplateController>(x => x.Edit(templateViewModel.CMSTemplateId));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.TemplateNameAlreadyExist));
            return ActionView(AdminConstants.CreateEdit, themeViewModel);
        }

        //Delete template.
        public virtual JsonResult Delete(string cmsTemplateId, string fileName)
        {
            string errorMessage = Admin_Resources.ErrorFailedToDelete;
            bool status = false;
            if (!string.IsNullOrEmpty(cmsTemplateId))
            {
                //Delete template.
                status = _templateAgent.DeleteTemplate(cmsTemplateId, fileName, out errorMessage);

                if (status)
                    errorMessage = Admin_Resources.DeleteMessage;
            }
            return Json(new { status = status, message = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        //Download template.
        public virtual ActionResult DownloadTemplate(int cmsTemplateId, string fileName)
        {
            string filePath = _templateAgent.DownloadTemplate(cmsTemplateId, fileName);
            if (!string.IsNullOrEmpty(filePath))
            {
                // Read the contents of file in byte array and download the file.
                byte[] fileBytes = System.IO.File.ReadAllBytes($"{filePath}/{fileName}.cshtml");
                string file = fileName;
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName + ".cshtml");
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorDownloadTemplate));
            return RedirectToAction<TemplateController>(x => x.List(null));
        }
        #endregion
    }
}
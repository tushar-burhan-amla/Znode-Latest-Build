using System.Web.Mvc;
using Znode.Engine.Admin.Agents;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Controllers
{
    public class ContainerTemplateController : BaseController
    {
        #region Private Variable
        private readonly IContainerTemplateAgent _containerTemplateAgent;
        #endregion

        #region Constructor
        public ContainerTemplateController(IContainerTemplateAgent containerTemplateAgent)
        {
            _containerTemplateAgent = containerTemplateAgent;
        }
        #endregion

        //List of Container Template
        public virtual ActionResult List([ModelBinder(typeof(PageDataBinder))] FilterCollectionDataModel model)
        {
            FilterHelpers.GetSetFiltersFromCookies(GridListType.ZnodeCMSWidgetTemplate.ToString(), model);

            FilterHelpers.GetDefaultView(GridListType.ZnodeCMSWidgetTemplate.ToString(), model);

            ContainerTemplateListViewModel containerTemplateList = _containerTemplateAgent.List(model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            containerTemplateList.GridModel = FilterHelpers.GetDynamicGridModel(model, containerTemplateList.ContainerTemplates, GridListType.ZnodeCMSWidgetTemplate.ToString(), string.Empty, null, true, true, containerTemplateList?.GridModel?.FilterColumn?.ToolMenuList);

            containerTemplateList.GridModel.TotalRecordCount = containerTemplateList.TotalResults;
            return ActionView(containerTemplateList);
        }

        //Create Container Template
        [HttpGet]
        public virtual ActionResult Create()
            => View(AdminConstants.CreateEdit, new ContainerTemplateViewModel());

        //Create Container Template
        [HttpPost]
        public virtual ActionResult Create(ContainerTemplateViewModel containerTemplateViewModel)
        {
            if (ModelState.IsValid)
            {
                if (HelperUtility.IsNull(containerTemplateViewModel.FilePath))
                {
                    SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorRequiredFile));
                    return View(AdminConstants.CreateEdit, containerTemplateViewModel);
                }

                containerTemplateViewModel = _containerTemplateAgent.Create(containerTemplateViewModel);
                if (containerTemplateViewModel?.ContainerTemplateId > 0)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.ContainerTemplateCreate));
                    return RedirectToAction<ContainerTemplateController>(x => x.Edit(containerTemplateViewModel.Code));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(containerTemplateViewModel.ErrorMessage));
            return View(AdminConstants.CreateEdit, containerTemplateViewModel);
        }

        //Get Container Template
        [HttpGet]
        public virtual ActionResult Edit(string code)
        {
            ActionResult action = GotoBackURL();
            if (action != null)
                return action;
            return View(AdminConstants.CreateEdit, _containerTemplateAgent.GetContainerTemplate(code));
        }

        //Update Container Template
        [HttpPost]
        public virtual ActionResult Edit(ContainerTemplateViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                model = _containerTemplateAgent.Update(model);
                if (model?.ContainerTemplateId > 0)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.ContainerTemplateUpdate));
                    return RedirectToAction<ContainerTemplateController>(x => x.Edit(model.Code));
                }
            }
            return View(AdminConstants.CreateEdit, model);
        }

        //Delete Container Template
        public virtual JsonResult Delete(string containerTemplateId, string fileName)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(containerTemplateId))
            {
                bool status = _containerTemplateAgent.DeleteContainerTemplate(containerTemplateId, fileName, out message);
                return Json(new { status = status, message = status ? Admin_Resources.DeleteContainerTemplate : message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = false, message = message }, JsonRequestBehavior.AllowGet);
        }

        //Copy Container Template
        [HttpGet]
        public virtual ActionResult Copy(string code)
        {
            if (string.IsNullOrEmpty(code))
                return RedirectToAction<ContainerTemplateController>(x => x.List(null));

            ContainerTemplateViewModel templateViewModel = _containerTemplateAgent.GetContainerTemplate(code);
            
            ModelState.Clear();
            templateViewModel.Name = "Copy of " + templateViewModel.Name;
            templateViewModel.Code = string.Empty;
            templateViewModel.ContainerTemplateId = 0;
            return ActionView(AdminConstants.CreateEdit, templateViewModel);
        }

        //Copy Container Template
        [HttpPost]
        public virtual ActionResult Copy(ContainerTemplateViewModel model)
        {
            ModelState.Remove("FilePath");
            if (ModelState.IsValid)
            {
                model = _containerTemplateAgent.CopyContainerTemplate(model);
                if (model?.ContainerTemplateId > 0)
                {
                    SetNotificationMessage(GetSuccessNotificationMessage(Admin_Resources.TemplateCopyMessage));
                    return RedirectToAction<ContainerTemplateController>(x => x.Edit(model.Code));
                }
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.TemplateNameAlreadyExist));
            return ActionView(AdminConstants.CreateEdit, model);
        }

        //Download Container Template
        public virtual ActionResult DownloadWidgetTemplate(int containerTemplateId, string fileName)
        {
            string filePath = _containerTemplateAgent.DownloadContainerTemplate(containerTemplateId, fileName);
            if (!string.IsNullOrEmpty(filePath))
            {
                // Read the contents of file in byte array and download the file.
                byte[] fileBytes = System.IO.File.ReadAllBytes($"{filePath}/{fileName}.cshtml");
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName + ".cshtml");
            }
            SetNotificationMessage(GetErrorNotificationMessage(Admin_Resources.ErrorDownloadTemplate));
            return RedirectToAction<ContainerTemplateController>(x => x.List(null));
        }

        //Validate Container Template
        [HttpGet]
        public virtual ActionResult IsContainerTemplateExist(string code)
            => Json(new { data = _containerTemplateAgent.IsContainerTemplateExist(code) }, JsonRequestBehavior.AllowGet);
    }
}

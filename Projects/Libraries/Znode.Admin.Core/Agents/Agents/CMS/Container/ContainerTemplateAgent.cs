using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    public class ContainerTemplateAgent : BaseAgent, IContainerTemplateAgent
    {
        #region Private Variables
        private readonly IContainerTemplateClient _containerTemplateClient;
        #endregion

        #region Constructor
        public ContainerTemplateAgent(IContainerTemplateClient containerTemplateClient)
        {
            _containerTemplateClient = GetClient<IContainerTemplateClient>(containerTemplateClient);
        }
        #endregion

        //List of Container Template
        public virtual ContainerTemplateListViewModel List(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);

            ContainerTemplateListModel containerTemplateList = _containerTemplateClient.List(null, filters, sorts, pageIndex, pageSize);
            ContainerTemplateListViewModel listViewModel = new ContainerTemplateListViewModel { ContainerTemplates = containerTemplateList?.ContainerTemplates.ToViewModel<ContainerTemplateViewModel>()?.ToList() };

            SetListPagingData(listViewModel, containerTemplateList);
            //Set tool options for grid.
            SetToolMenus(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return listViewModel?.ContainerTemplates?.Count > 0 ? listViewModel : new ContainerTemplateListViewModel() { ContainerTemplates = new List<ContainerTemplateViewModel>() };
        }

        //Create Container Template
        public virtual ContainerTemplateViewModel Create(ContainerTemplateViewModel containerTemplateViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                if (HelperUtility.IsNotNull(containerTemplateViewModel))
                {
                    containerTemplateViewModel.FileName = GetFileNameWithoutExtension(containerTemplateViewModel?.FilePath?.FileName);
                    ContainerTemplateViewModel model = _containerTemplateClient.Create(containerTemplateViewModel.ToModel<ContainerTemplateCreateModel>())?.ToViewModel<ContainerTemplateViewModel>();
                    if (model?.ContainerTemplateId > 0)
                    {
                        SaveFile(containerTemplateViewModel);
                        return model;
                    }
                }
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return containerTemplateViewModel;
            }
            catch (ZnodeException ex)
            {
                return (ContainerTemplateViewModel)GetViewModelWithErrorMessage(containerTemplateViewModel, Admin_Resources.ErrorFailedToCreate);
            }
            catch (Exception ex)
            {
                return (ContainerTemplateViewModel)GetViewModelWithErrorMessage(containerTemplateViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get Container Template
        public virtual ContainerTemplateViewModel GetContainerTemplate(string templateCode)
         => _containerTemplateClient.GetContainerTemplate(templateCode).ToViewModel<ContainerTemplateViewModel>();

        //Update Container Template
        public virtual ContainerTemplateViewModel Update(ContainerTemplateViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNotNull(model.FilePath))
                model.FileName = GetFileNameWithoutExtension(model?.FilePath?.FileName);

            int containerTemplateId = _containerTemplateClient.Update(model?.ToModel<ContainerTemplateUpdateModel>()).ContainerTemplateId;
            if (containerTemplateId > 0)
            {
                if (HelperUtility.IsNotNull(model.FilePath))
                    SaveFile(model);
                return model;
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return (ContainerTemplateViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.UpdateErrorMessage);
        }

        //Delete Container Template
        public virtual bool DeleteContainerTemplate(string containerTemplateIds, string fileName, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                bool status = false;
                ContainerTemplateListModel list = _containerTemplateClient.List(null, null, null, null, null);
                if (!string.IsNullOrEmpty(containerTemplateIds))
                {
                    status = _containerTemplateClient.Delete(new ParameterModel { Ids = containerTemplateIds });
                    if (status && list?.ContainerTemplates?.Count > 0)
                    {
                        foreach (var item in fileName.Split(','))
                        {
                            if (list.ContainerTemplates.FindAll(x => x.FileName == item).Count == 1)
                            {
                                //Gets the actual file path.
                                string actualFilePath = Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.TemplatesPath), item + ".cshtml");

                                if (File.Exists(actualFilePath))
                                    File.Delete(actualFilePath);
                            }
                        }
                    }
                }
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return status;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                errorMessage = Admin_Resources.ContainerTemplateAssociationDeleteError;
                return false;

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        //Validate if the Container Template Exist
        public virtual bool IsContainerTemplateExist(string templateCode)
         => !string.IsNullOrEmpty(templateCode) ? _containerTemplateClient.IsContainerTemplateExist(templateCode) : true;

        //Copy Container Template
        public virtual ContainerTemplateViewModel CopyContainerTemplate(ContainerTemplateViewModel model)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _containerTemplateClient.Create(model?.ToModel<ContainerTemplateCreateModel>())?.ToViewModel<ContainerTemplateViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return null;
            }
        }

        //Download Container Template
        public virtual string DownloadContainerTemplate(int containerTemplateId, string fileName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (File.Exists(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.TemplatesPath), fileName + ".cshtml")))
                return HttpContext.Current.Server.MapPath(AdminConstants.TemplatesPath);
            return null;
        }

        //Save file
        protected virtual void SaveFile(ContainerTemplateViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNotNull(model?.FilePath))
            {
                string fileName = Path.GetFileName(model?.FilePath.FileName);
                ZnodeLogging.LogMessage("fileName: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { fileName = fileName });
                try
                {
                    //If folder is not present, then create new folder.
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath(AdminConstants.TemplatesPath)))
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AdminConstants.TemplatesPath));

                    //Saves the file in above folder.
                    string actualFilePath = Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.TemplatesPath), fileName);
                    model.FilePath.SaveAs(actualFilePath);
                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    //In case any exception occurs then delete the file.
                    File.Delete(fileName);
                }
            }
        }

        //Set tools dropdown
        protected virtual void SetToolMenus(ContainerTemplateListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('ContainerTemplateDeletePopUp')", ControllerName = "ContainerTemplate", ActionName = "Delete" });
            }
        }

        protected virtual string GetFileNameWithoutExtension(string fileName)
         => !string.IsNullOrEmpty(fileName) ? fileName.Substring(0, fileName.LastIndexOf('.')) : string.Empty;
    }
}

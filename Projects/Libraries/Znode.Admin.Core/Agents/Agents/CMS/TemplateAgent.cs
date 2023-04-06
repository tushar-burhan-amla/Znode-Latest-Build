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
    public class TemplateAgent : BaseAgent, ITemplateAgent
    {
        #region Private Variables
        private readonly ITemplateClient _templateClient;
        #endregion

        #region Constructor
        public TemplateAgent(ITemplateClient templateClient)
        {
            _templateClient = GetClient<ITemplateClient>(templateClient);
        }
        #endregion

        #region public Methods
        //Get the list template.
        public virtual TemplateListViewModel GetTemplates(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            TemplateListModel templateList = _templateClient.GetTemplates(null, filters, sorts, pageIndex, pageSize);
            TemplateListViewModel listViewModel = new TemplateListViewModel { Templates = templateList?.Templates?.ToViewModel<TemplateViewModel>().ToList() };

            SetListPagingData(listViewModel, templateList);
            //Set tool options for grid.
            SetToolMenus(listViewModel);
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return templateList?.Templates?.Count > 0 ? listViewModel : new TemplateListViewModel() { Templates = new List<TemplateViewModel>() };
        }

        //Create template.
        public virtual TemplateViewModel CreateTemplate(TemplateViewModel templateViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                if (HelperUtility.IsNotNull(templateViewModel))
                {
                    templateViewModel.FileName = GetFileNameWithoutExtension(templateViewModel?.FilePath?.FileName);
                    TemplateViewModel createdTemplate = _templateClient.CreateTemplate(templateViewModel.ToModel<TemplateModel>())?.ToViewModel<TemplateViewModel>();
                    if (createdTemplate?.CMSTemplateId > 0)
                    {
                        ZnodeLogging.LogMessage("CMSTemplateId of createdTemplate: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { CMSTemplateId = createdTemplate?.CMSTemplateId });
                        SaveFile(templateViewModel);
                        ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                        return createdTemplate;
                    }
                }
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return templateViewModel;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (TemplateViewModel)GetViewModelWithErrorMessage(templateViewModel, Admin_Resources.TemplateNameAlreadyExist);
                    default:
                        return (TemplateViewModel)GetViewModelWithErrorMessage(templateViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (TemplateViewModel)GetViewModelWithErrorMessage(templateViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Copy template.
        public virtual TemplateViewModel CopyTemplate(TemplateViewModel templateViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return _templateClient.CreateTemplate(templateViewModel?.ToModel<TemplateModel>())?.ToViewModel<TemplateViewModel>();
            }
            catch (Exception ex) 
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return null;
            }
        }

        //Get template by cms template Id
        public virtual TemplateViewModel GetTemplate(int cmsTemplateId)
            => _templateClient.GetTemplate(cmsTemplateId, null).ToViewModel<TemplateViewModel>();

        //Update template.
        public virtual TemplateViewModel UpdateTemplate(TemplateViewModel templateViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                if (HelperUtility.IsNotNull(templateViewModel.FilePath))
                    templateViewModel.FileName = GetFileNameWithoutExtension(templateViewModel?.FilePath?.FileName);
                ZnodeLogging.LogMessage("FileName of templateViewModel: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { FileName = templateViewModel?.FileName });
                if (_templateClient.UpdateTemplate(templateViewModel?.ToModel<TemplateModel>()).CMSTemplateId > 0)
                {
                    if (HelperUtility.IsNotNull(templateViewModel.FilePath))
                        SaveFile(templateViewModel);
                    ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                    return templateViewModel;
                }
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
                return (TemplateViewModel)GetViewModelWithErrorMessage(templateViewModel, Admin_Resources.UpdateErrorMessage);
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return (TemplateViewModel)GetViewModelWithErrorMessage(templateViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete template.
        public virtual bool DeleteTemplate(string cmsTemplateId, string fileName, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.ErrorFailedToDelete;
            try
            {
                bool status = false;
                TemplateListModel templateList = _templateClient.GetTemplates(null, null, null, null, null);
                if (!string.IsNullOrEmpty(cmsTemplateId))
                {
                    status = _templateClient.DeleteTemplate(new ParameterModel { Ids = cmsTemplateId });
                    if (status && templateList?.Templates?.Count > 0)
                    {
                        foreach (var item in fileName.Split(','))
                        {
                            if (templateList.Templates.FindAll(x => x.FileName == item).Count == 1)
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
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        errorMessage = Admin_Resources.ErrorFailToDeleteTemplate;
                        return false;
                    default:
                        return false;
                }
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Get template details.
        public virtual string DownloadTemplate(int cmsTemplateId, string fileName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (File.Exists(Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.TemplatesPath), fileName + ".cshtml")))
                return HttpContext.Current.Server.MapPath(AdminConstants.TemplatesPath);
            return null;
        }

        // Check duplicate template name.
        public virtual bool CheckTemplateName(string templateName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);           
            string selectedTemplateName = string.Empty;
            if (!string.IsNullOrEmpty(templateName))
            {
                TemplateListModel templateList = _templateClient.GetTemplates(null, null, null, null, null);
                TemplateListViewModel listViewModel = new TemplateListViewModel { Templates = templateList?.Templates?.ToViewModel<TemplateViewModel>().ToList() };
                selectedTemplateName = listViewModel?.Templates?.Where(w => w.Name.ToLower() == templateName.ToLower().Trim()).Select(s => s.Name).FirstOrDefault();
                ZnodeLogging.LogMessage("selectedTemplateName: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { selectedTemplateName = selectedTemplateName });
            }
            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            return string.IsNullOrEmpty(selectedTemplateName) ? true : false;
        }
        #endregion

        #region Private Methods
        //Saves the file to disk.
        private void SaveFile(TemplateViewModel createdTemplate)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNotNull(createdTemplate?.FilePath))
            {
                string fileName = Path.GetFileName(createdTemplate.FilePath.FileName);
                ZnodeLogging.LogMessage("fileName: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { fileName = fileName });
                try
                {
                    //If folder is not present, then create new folder.
                    if (!Directory.Exists(HttpContext.Current.Server.MapPath(AdminConstants.TemplatesPath)))
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(AdminConstants.TemplatesPath));

                    //Saves the file in above folder.
                    string actualFilePath = Path.Combine(HttpContext.Current.Server.MapPath(AdminConstants.TemplatesPath), fileName);
                    createdTemplate.FilePath.SaveAs(actualFilePath);
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

        #endregion

        #region Private Methods
        //Gets the file name without extension.
        private string GetFileNameWithoutExtension(string fileName)
            => !string.IsNullOrEmpty(fileName) ? fileName.Substring(0, fileName.LastIndexOf('.')) : string.Empty;

        //Set the Tool Menus forUrl Redirect List Grid View.
        private void SetToolMenus(TemplateListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = new Models.GridModel();
                model.GridModel.FilterColumn = new Models.FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<Models.ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new Models.ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('TemplateDeletePopup')", ControllerName = "Template", ActionName = "Delete" });
            }
        }
        #endregion
    }
}
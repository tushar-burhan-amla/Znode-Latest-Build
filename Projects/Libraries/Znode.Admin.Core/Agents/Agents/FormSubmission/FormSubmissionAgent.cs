using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{

    public class FormSubmissionAgent : BaseAgent, IFormSubmissionAgent
    {

        #region Private readonly members

        private readonly IFormSubmissionClient _formSubmissionClient;

        #endregion

        #region Constructor
        public FormSubmissionAgent(IFormSubmissionClient formSubmission)
        {
            _formSubmissionClient = GetClient<IFormSubmissionClient>(formSubmission);

        }
        #endregion

        #region Public Methods

        //Get the list of Form Submission.
        public virtual FormSubmissionListViewModel GetFormSubmissionList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(sorts))
            {
                sorts = new SortCollection();
                sorts.Add(ZnodeFormBuilderSubmitEnum.FormBuilderSubmitId.ToString(), DynamicGridConstants.DESCKey);
            }
            ZnodeLogging.LogMessage("Input parameters :", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });
            FormSubmissionListModel formSubmissionListModel = _formSubmissionClient.GetFormSubmissionList(expands, filters, sorts, pageIndex, pageSize);
            FormSubmissionListViewModel formSubmissionListViewModel = new FormSubmissionListViewModel { FormSubmissionList = formSubmissionListModel?.FormSubmissionList?.ToViewModel<FormSubmissionViewModel>().ToList() };
            //Set tool menu list for form submission.
            SetFormSubmissionToolMenuList(formSubmissionListViewModel);
            SetListPagingData(formSubmissionListViewModel, formSubmissionListModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return formSubmissionListModel?.FormSubmissionList?.Count > 0 ? formSubmissionListViewModel : new FormSubmissionListViewModel() { FormSubmissionList = new List<FormSubmissionViewModel>() };
        }

        //Get Details of Form Submission
        public virtual FormBuilderAttributeGroupViewModel GetFormSubmitDetails(int formSubmitId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            FormBuilderAttributeGroupViewModel model = new FormBuilderAttributeGroupViewModel();
            if (formSubmitId > 0)
                model = GlobalAttributeModelMap.ToFormBuilderAttributeGroupViewModel(_formSubmissionClient.GetFormSubmitDetails(formSubmitId));

            SetMediaDownloadPath(model);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return model;
        }


        //Get FormSubmissionList Response Message for Export.
        public virtual ExportResponseMessageModel GetExportFormSubmissionList(string exportType, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNull(filters))
                filters = new FilterCollection();

            ZnodeLogging.LogMessage("Input parameters filters and sorts of method GetExportFormSubmissionList: ", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            ExportModel exportFormSubmissionMessage = _formSubmissionClient.GetExportFormSubmissionList(exportType, null, filters, sorts, pageIndex, pageSize);

            ExportResponseMessageModel exportFormSubmissionResponseMessage = new ExportResponseMessageModel()
            {
                Message = exportFormSubmissionMessage.Message,
                HasError = exportFormSubmissionMessage.HasError
            };

            ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return exportFormSubmissionResponseMessage;
        }

        #endregion

        #region Private Method
        //Get Media Path
        private string GetMediaPath()
        {
            string filePath = ZnodeAdminSettings.ZnodeApiRootUri;
            if (!string.IsNullOrEmpty(filePath))
            {
                if (filePath.EndsWith("/"))
                    filePath = filePath.Remove(filePath.Length - 1);

                filePath += AdminConstants.FormBuilderMediaPath;
            }
            ZnodeLogging.LogMessage("filePath:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info, filePath);
            return filePath;
        }

        private void SetMediaDownloadPath(FormBuilderAttributeGroupViewModel model)
        {
            if (model?.Attributes?.Count > 0)
            {
                string mediaPath = GetMediaPath();
                foreach (GlobalAttributeValuesViewModel attr in model.Attributes)
                {
                    if (Equals(Regex.Replace(attr.AttributeTypeName, @"\s", ""), ControlTypes.File.ToString()) ||
                        Equals(Regex.Replace(attr.AttributeTypeName, @"\s", ""), ControlTypes.Image.ToString()))
                    {
                        string attributeValue = attr.AttributeValue;
                        attr.AttributeValue = GenerateDownloadUrl(attributeValue, mediaPath);
                    }
                }
            }
        }

        //to Generate Download Url
        private string GenerateDownloadUrl(string attributeValue, string mediaPath)
        {
            if (!string.IsNullOrEmpty(attributeValue))
            {
                attributeValue = attributeValue.Replace("~", string.Empty);
                string[] attrValues = attributeValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> values = new List<string>();
                foreach (string attr in attrValues)
                {
                    string[] result = attr.Split(new[] { "--" }, StringSplitOptions.RemoveEmptyEntries);
                    string filename = result?.Length > 1 ? result[1] : attr;
                    string filePath = $"{mediaPath}/{attr}";
                    values.Add($"<a href=\"{filePath}\" title=\"Click To Download\" target=\"_blank\" download=''><i class=\"z-download\"></i> {filename}</a></br>");
                }
                ZnodeLogging.LogMessage("values list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, values?.Count());
                return string.Join(string.Empty, values?.Select(x => x));
            }
            return attributeValue;
        }

        //Set Form Submission Tool Menu List.
        private void SetFormSubmissionToolMenuList(FormSubmissionListViewModel model)
        {
            if (HelperUtility.IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel
                {
                    DisplayText = Admin_Resources.CSV,
                    Value = "2",
                    Type = "CSV",
                    JSFunctionName = "FormSubmission.prototype.FormSubmissionExport(event)"
                });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel
                {
                    DisplayText = Admin_Resources.Excel,
                    Value = "1",
                    Type = "Excel",
                    JSFunctionName = "FormSubmission.prototype.FormSubmissionExport(event)"
                });

            }
        }
        #endregion
    }
}

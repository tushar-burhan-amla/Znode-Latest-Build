using System;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class FormSubmissionCache : BaseCache, IFormSubmissionCache
    {
        #region Private Variable
        private readonly IFormSubmissionService _formSubmissionService;
        #endregion

        #region Constructor
        public FormSubmissionCache(IFormSubmissionService formSubmissionService)
        {
            _formSubmissionService = formSubmissionService;
        }

        #endregion

        #region Public Methods

        public string GetFormSubmissionList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get form submission list.
                FormSubmissionListModel list = _formSubmissionService.GetFormSubmissionList(Expands, Filters, Sorts, Page);
                if (list?.FormSubmissionList?.Count > 0)
                {
                    //Create response.
                    FormSubmissionListResponse response = new FormSubmissionListResponse { FormSubmissionList = list.FormSubmissionList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Form Submission Details
        public virtual string GetFormSubmitDetails(int formSubmitId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get form submission list.
                FormBuilderAttributeGroupModel model = _formSubmissionService.GetFormSubmitDetails(formSubmitId);
                if (IsNotNull(model))
                {
                    //Create response.
                    FormBuilderAttributeGroupResponse response = new FormBuilderAttributeGroupResponse { FormBuilderAttributeGroup = model };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Response message for Form Submission export.
        public string GetFormSubmissionListforExport(string routeUri, string routeTemplate, string exportType)
        {
            ExportModel exportResponse = _formSubmissionService.GetAllFormsListForExport(Expands, Filters, Sorts, Page, exportType);
            //Generate Response
            ExportResponse response = new ExportResponse { ExportMessageModel = exportResponse };

            return ApiHelper.ToJson(response);
        }

        #endregion
    }
}

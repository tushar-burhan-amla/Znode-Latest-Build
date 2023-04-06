using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class FormSubmissionClient : BaseClient, IFormSubmissionClient
    {
        public virtual FormSubmissionListModel GetFormSubmissionList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Create Endpoint to get the list of form submission.
            string endpoint = FormSubmissionEndpoint.GetFormSubmissionList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();

            FormSubmissionListResponse response = GetResourceFromEndpoint<FormSubmissionListResponse>(endpoint, status);

            //Check the status of response of form submission list.
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            FormSubmissionListModel formSubmissionListModel = new FormSubmissionListModel { FormSubmissionList = response?.FormSubmissionList };
            formSubmissionListModel.MapPagingDataFromResponse(response);
            return formSubmissionListModel;
        }

        //Get Form Submit Details based on the Entity.
        public virtual FormBuilderAttributeGroupModel GetFormSubmitDetails(int formSubmitId)
        {
            string endpoint = FormSubmissionEndpoint.GetFormSubmitDetails(formSubmitId);

            ApiStatus status = new ApiStatus();
            FormBuilderAttributeGroupResponse response = GetResourceFromEndpoint<FormBuilderAttributeGroupResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.FormBuilderAttributeGroup;
        }

        //Get Response for Exported Data of Form Submission.
        public virtual ExportModel GetExportFormSubmissionList(string exportType, ExpandCollection  expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = FormSubmissionEndpoint.GetFormSubmissionListForExport(exportType);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ExportResponse response = GetResourceFromEndpoint<ExportResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return new ExportModel { Message = response.ExportMessageModel.Message, HasError = response.ExportMessageModel.HasError };
        }

    }
}

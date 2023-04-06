namespace Znode.Engine.Api.Client.Endpoints
{
    public class FormSubmissionEndpoint : BaseEndpoint
    {
        // Get form submission list endpoint.
        public static string GetFormSubmissionList() => $"{ApiRoot}/formsubmission/list";

        //Get default values by form submit id.
        public static string GetFormSubmitDetails(int FormSubmitId) => $"{ApiRoot}/formsubmission/getformsubmitdetails/{FormSubmitId}";

        // Get form submission list endpoint for export.
        public static string GetFormSubmissionListForExport(string exportType) => $"{ApiRoot}/formsubmission/formsubmissionexportlist/{exportType}";
    }
}

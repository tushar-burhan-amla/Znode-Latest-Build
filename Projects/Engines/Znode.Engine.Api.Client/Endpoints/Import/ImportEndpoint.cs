namespace Znode.Engine.Api.Client.Endpoints
{
    public class ImportEndpoint : BaseEndpoint
    {
        //Get all templates with respect to header id
        public static string GetAllTemplates(int importHeadId, int familyId, int promotionTypeId = 0) => $"{ApiRoot}/import/getalltemplates/{importHeadId}/{familyId}/{promotionTypeId}";
        
        //Get all import types
        public static string GetImportTypeList() => $"{ApiRoot}/import/getimporttypelist";
        
        //Get template data with respect to template id
        public static string GetTemplateData(int templateId, int importHeadId, int familyId, int promotionTypeId = 0) => $"{ApiRoot}/import/gettemplatedata/{templateId}/{importHeadId}/{familyId}/{promotionTypeId}";
        
        //Post and process import data
        public static string ImportData() => $"{ApiRoot}/import/importdata";

        //Download template
        public static string DownloadTemplate(int importHeadId, int downloadImportFamilyId, int downloadImportPromotionTypeId = 0) => $"{ApiRoot}/import/downloadtemplate/{importHeadId}/{downloadImportFamilyId}/{downloadImportPromotionTypeId}";

        //Get the Import logs
        public static string GetImportLogs() => $"{ApiRoot}/import/getimportlogs";

        //Get the Import logs status
        public static string GetImportLogStatus(int importProcessLogId) => $"{ApiRoot}/import/getlogstatus/{importProcessLogId}";

        //Get the import log details
        public static string GetImportLogDetails(int importProcessLogId) => $"{ApiRoot}/import/getimportlogdetails/{importProcessLogId}";

        //Delete the Import log records from ZnodeImportProcessLog as well as ZnodeImportLog table
        public static string DeleteLog() => $"{ApiRoot}/import/delete";

        //Get all families for product import
        public static string GetAllFamilies(bool isCategory) => $"{ApiRoot}/import/getfamilies/{isCategory}";

        //Update Mappings
        public static string UpdateMappings() => $"{ApiRoot}/import/updatemappings";

        //check import status
        public static string CheckImportProcess() => $"{ApiRoot}/import/checkimportprocess";

        // Get default template.
        public static string GetDefaultTemplate(string importType) => $"{ApiRoot}/import/getdefaulttemplate/{importType}";

        //Get custom import template list. It will not return the system defined import template.
        public static string GetCustomImportTemplateList() => $"{ApiRoot}/import/getcustomimporttemplatelist";

        //Deletes the custom import templates. It will not delete the system defined import templates.
        public static string DeleteImportTemplate() => $"{ApiRoot}/import/deleteimporttemplate";
        
        //Get the import log details for export csv,excel and pdf
        public static string GetImportLogDetailsList(string fileType,int ImportProcessLogId) => $"{ApiRoot}/import/getimportlogdetailslist/{fileType}/{ImportProcessLogId}";

        //check export status
        public static string CheckExportProcess() => $"{ApiRoot}/import/CheckExportProcess";
    }
}

namespace Znode.Engine.Api.Client.Endpoints
{
    public class EmailTemplateEndpoint : BaseEndpoint
    {
        //Get EmailTemplate List Endpoint
        public static string List() => $"{ApiRoot}/emailtemplate/list" ;

        //Create EmailTemplate Endpoint.
        public static string Create() => $"{ApiRoot}/emailtemplate";

        //Get EmailTemplate on the basis of emailTemplateId Endpoint.
        public static string Get(int emailTemplateId) => $"{ApiRoot}/emailtemplates/{emailTemplateId}";

        //Update EmailTemplate Endpoint.
        public static string Update() => $"{ApiRoot}/emailtemplate/update";

        //Delete EmailTemplate Endpoint.
        public static string Delete() => $"{ApiRoot}/emailtemplate/delete";

        //Get Email template tokens endpoint.
        public static string GetEmailTemplateTokens() => $"{ApiRoot}/emailtemplate/getemailtemplatetokens";

        //Get EmailTemplateArea List Endpoint
        public static string EmailTemplateAreaList() => $"{ApiRoot}/emailtemplate/emailtemplatearealist";

        //Get EmailTemplateAreaMapper List Endpoint
        public static string EmailTemplateAreaMapperList(int portalId) => $"{ApiRoot}/emailtemplate/emailtemplateareamapperlist/{portalId}";

        //Delete Email Template Area Configuration Endpoint.
        public static string DeleteEmailTemplateAreaConfiguration() => $"{ApiRoot}/emailtemplate/deletetemplateareaconfiguration";

        //Saves the Email Template Area Configuration Endpoint.
        public static string SaveEmailTemplateAreaConfiguration() => $"{ApiRoot}/emailtemplate/createupdatetemplateareaconfiguration";
    }
}

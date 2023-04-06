namespace Znode.Engine.Api.Client.Endpoints
{
    class TemplateEndpoint : BaseEndpoint
    {
        //Get template list endpoint.
        public static string List() => $"{ApiRoot}/template/list";

        //Create template Endpoint.
        public static string Create() => $"{ApiRoot}/template/create";

        //Get template on the basis of cmsTemplateId Endpoint.
        public static string Get(int cmsTemplateId) => $"{ApiRoot}/template/get/{cmsTemplateId}";

        //Update template Endpoint.
        public static string Update() => $"{ApiRoot}/template/update";

        //Delete template Endpoint.
        public static string Delete() => $"{ApiRoot}/template/delete";
    }
}

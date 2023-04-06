
namespace Znode.Engine.Api.Client.Endpoints
{
    public class ContainerTemplateEndpoint : BaseEndpoint
    {
        //Get the List of Container Templates
        public static string List() => $"{ApiRoot}/containertemplate/list";

        //Create Container Templates
        public static string Create() => $"{ApiRoot}/containertemplate/create";

        //Get Container Templates
        public static string Get(string templateCode) => $"{ApiRoot}/containertemplate/get/{templateCode}";

        //Update Container Templates
        public static string Update() => $"{ApiRoot}/containertemplate/update";

        //Delete Container Templates
        public static string Delete() => $"{ApiRoot}/containertemplate/delete";

        //Verify if the Container Templates Exists
        public static string IsContainerTemplateExist(string templateCode) => $"{ApiRoot}/containertemplate/iscontainertemplateexist/{templateCode}";




    }
}


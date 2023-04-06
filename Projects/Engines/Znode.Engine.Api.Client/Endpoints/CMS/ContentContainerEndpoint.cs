

namespace Znode.Engine.Api.Client.Endpoints
{
    public class ContentContainerEndpoint: BaseEndpoint
    {
        //Get List of Content Container
        public static string List() => $"{ApiRoot}/contentcontainer/list";

        //Create Content Container
        public static string Create() => $"{ApiRoot}/Contentcontainer/create";

        //Get Content Container
        public static string GetContentContainer(string containerKey) => $"{ApiRoot}/contentcontainer/getcontentcontainer/{containerKey}";

        //Update Content Container
        public static string Update() => $"{ApiRoot}/contentcontainer/update"; 

        //Get Associated Variants
        public static string GetAssociatedVariants(string containerKey) => $"{ApiRoot}/contentcontainer/getassociatedvariants/{containerKey}";

        //Associate Variants
        public static string AssociateVariant() => $"{ApiRoot}/contentcontainer/associatevariant";

        //Delete Variant
        public static string DeleteAssociateVariant() => $"{ApiRoot}/contentcontainer/deleteassociatedvariant/";

        //Delete Content Container
        public static string DeleteContentContainer() => $"{ApiRoot}/contentcontainer/deletecontentContainer";

        //Verify if the Content Container Exist
        public static string IsContainerKeyExist(string containerKey) => $"{ApiRoot}/contentcontainer/iscontainerkeyexists/{containerKey}";

        //Associate Container Template
        public static string AssociateContainerTemplate(int variantId, int containerTemplateId) => $"{ApiRoot}/contentcontainer/associatecontenttemplate/{variantId}/{containerTemplateId}";

        //Get List of Content Container Variants
        public static string GetAssociatedVariantList() => $"{ApiRoot}/contentcontainer/getassociatedvariantlist";

        //Add Content Container variant data
        public static string SaveVariantData(int localeId, int? templateId, int variantId, bool isActive) => $"{ApiRoot}/contentcontainer/savevariantdata/{localeId}/{templateId}/{variantId}/{isActive}";

        //Get Content Container Locale Data
        public static string GetVariantLocaleData(int variantId) => $"{ApiRoot}/contentcontainer/GetVariantLocaleData/{variantId}";

        //Activate/Deactivate Variant
        public static string ActivateDeactivateVariant(bool isActivate) => $"{ApiRoot}/contentcontainer/activatedeactivatevariant/{isActivate}";

        //Publish Content Container
        public static string PublishContentContainer(string containerKey, string targetPublishState) => $"{ApiRoot}/contentcontainer/publishcontentcontainer/{containerKey}/{targetPublishState}";

        //Publish Content Container Variant
        public static string PublishContainerVariant(string containerKey, int containerProfileVariantId, string targetPublishState) => $"{ApiRoot}/contentcontainer/publishcontainervariant/{containerKey}/{containerProfileVariantId}/{targetPublishState}";

    }


}

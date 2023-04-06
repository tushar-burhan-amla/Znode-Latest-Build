namespace Znode.Engine.Api.Client.Endpoints
{
    public class AddonGroupEndPoint : BaseEndpoint
    {
        public static string GetAddonGroup() => $"{ApiRoot}/addongroup";

        public static string CreateAddonGroup() => $"{ApiRoot}/addongroup/create";

        public static string UpdateAddonGroup() => $"{ApiRoot}/addongroup/update";

        public static string GetAddonGroupList() => $"{ApiRoot}/addongroup/list";

        public static string DeleteAddonGroup() => $"{ApiRoot}/addongroup/delete";

        public static string AssociateAddonGroupProduct() => $"{ApiRoot}/addongroup/associateaddongroupproduct";

        public static string GetAssociatedAddonGroupProduct(int addonGroupId) => $"{ApiRoot}/addongroup/getassociatedaddongroupproducts/{addonGroupId}";

        public static string GetUnassociatedAddonGroupProduct(int addonGroupId) => $"{ApiRoot}/addongroup/getunassociatedaddongroupproducts/{addonGroupId}";

        public static string DeleteAddonGroupProductAssociation() => $"{ApiRoot}/addongroup/deleteaddongroupproductassociation";
    }
}

namespace Znode.Engine.Api.Client.Endpoints
{
    public class ProfileEndPoint : BaseEndpoint
    {
        #region Profile
        //Create profile endpoint.
        public static string Create() => $"{ApiRoot}/profile/create";

        //Get profile list endpoint.
        public static string GetProfileList() => $"{ApiRoot}/profile/list";

        //Delete profile endpoint.
        public static string Delete() => $"{ApiRoot}/profile/delete";

        //Update profile endpoint.
        public static string Update() => $"{ApiRoot}/profile/update";

        //Get profile by profile id endpoint.
        public static string GetProfileByProfileId(int profileId) => $"{ApiRoot}/profile/getprofile/{profileId}";
        #endregion

        #region Profile Catalog
        //Get profile catalog list endpoint.
        public static string GetProfileCatalogList() => $"{ApiRoot}/profile/profilecataloglist";

        //Delete associated catalog to profile endpoint.
        public static string DeleteAssociatedProfileCatalog(int profileId) => $"{ApiRoot}/profile/deleteassociatedprofilecatalog/{profileId}";

        //Associate Catalog to Profile endpoint.
        public static string AssociateCatalogToProfile() => $"{ApiRoot}/profile/associatecatalogtoprofile";
        #endregion
    }
}

namespace Znode.Engine.Api.Client.Endpoints
{
    public class SearchProfileEndpoint : BaseEndpoint
    {
        //gets list of search profiles
        public static string GetSearchProfileList() => $"{ApiRoot}/searchprofile/list";

        //create search profile
        public static string Create() => $"{ApiRoot}/searchprofile/create";

        public static string PublishSearchProfile(int searchProfileId) => $"{ApiRoot}/searchprofile/publishsearchprofile/{searchProfileId}";

        // Get the list of catalogs.
        public static string GetCatalogList() => $"{ApiRoot}/searchprofile/getcataloglist";

        //Get Search Profile on the basis of SearchProfile id Endpoint.
        public static string GetSearchProfile(int searchProfileId) => $"{ApiRoot}/searchprofile/{searchProfileId}";

        //updates search profile
        public static string UpdateSearchProfile() => $"{ApiRoot}/searchprofile/update";

        //deletes search profile
        public static string DeleteSearchProfile() => $"{ApiRoot}/searchprofile/delete";

        //gets profile details
        public static string ProfileDetails() => $"{ApiRoot}/searchprofile/getdetails";

        public static string GetSearchProfileProducts() => $"{ApiRoot}/search/getsearchprofileproduct";

        //Get Features list by query id.
        public static string GetFeaturesByQueryId(int queryId) => $"{ApiRoot}/searchprofile/featurelist/{queryId}";

        //Set default search profile
        public static string SetDefaultSearchProfile() => $"{ApiRoot}/searchprofile/setdefaultsearchprofile";

        #region Search Triggers
        //Gets list of search profile triggers.
        public static string GetSearchProfileTriggerList() => $"{ApiRoot}/searchprofiletriggers/getsearchprofiletriggerlist";

        //Create search profile triggers.
        public static string CreateSearchProfileTriggers() => $"{ApiRoot}/searchprofiletriggers/createsearchprofiletriggers";

        //Get search profile trigger on the basis of searchProfileTriggerId id Endpoint.
        public static string GetSearchProfileTriggers(int searchProfileTriggerId) => $"{ApiRoot}/searchprofiletriggers/getsearchprofiletriggers/{searchProfileTriggerId}";

        //Updates search profile triggers.
        public static string UpdateSearchProfileTriggers() => $"{ApiRoot}/searchprofiletriggers/updatesearchprofiletriggers";

        //Deletes search profile triggers.
        public static string DeleteSearchProfileTriggers() => $"{ApiRoot}/searchprofiletriggers/deletesearchprofiletriggers";
        #endregion

        #region Search Facets
        //gets list of search attributes
        public static string GetSearchAttributeList() => $"{ApiRoot}/searchattributes/getassociatedunassociatedcatalogattributes";

        //Associate UnAssociated search attributes to search profile.
        public static string AssociateAttributesToProfile() => $"{ApiRoot}/searchattributes/associateattributestoprofile";

        //UnAssociate search attributes from search profile.
        public static string UnAssociateAttributesFromProfile() => $"{ApiRoot}/searchattributes/unassociateattributesfromprofile";

        #endregion

        //get search profile portal list.
        public static string GetSearchProfilePortalList() => $"{ApiRoot}/searchprofileportals/list";

        //Associate portal to search profile.
        public static string AssociatePortalToSearchProfile() => $"{ApiRoot}/searchprofile/associateunassociateportaltosearchprofile";

        //gets searchable attributes list based on catalog Id
        public static string GetCatalogBasedAttributes() => $"{ApiRoot}/searchprofile/catalogbasedattributes";

        //gets unassociated portal list
        public static string GetUnAssociatedPortalList() => $"{ApiRoot}/searchprofile/unassociatedportallist";

        //Get field value list by catalog id.
        public static string GetfieldValuesList(int publishCatalogId, int searchProfileId) => $"{ApiRoot}/getfieldvalueslist/{publishCatalogId}/{searchProfileId}";

    }
}

namespace Znode.Engine.Api.Client.Endpoints
{
    //Configuration for endpoint of Attribute family.
    public class AttributeFamilyEndpoint : BaseEndpoint
    {
        public static string GetAttributeFamilyList() => $"{ApiRoot}/attributefamily/list";

        public static string Create() => $"{ApiRoot}/attributefamily/create";

        public static string GetAssignedAttributeGroups() => $"{ApiRoot}/attributefamily/getassignedattributegroups";

        public static string AssignAttributeGroups() => $"{ApiRoot}/attributefamily/associateattributegroup";

        public static string UnAssignAttributeGroups(int attributeFamilyId, int attributeGroupId) 
            => $"{ApiRoot}/attributefamily/unassociateattributegroup/{attributeFamilyId}/{attributeGroupId}";

        public static string Delete() => $"{ApiRoot}/attributefamily/delete";

        public static string GetAttributeFamily(int attributeFamilyId) => $"{ApiRoot}/attributefamily/getattributefamily/{attributeFamilyId}";

        public static string GetFamilyLocale(int attributeFamilyId) => $"{ApiRoot}/attributefamily/getfamilylocale/{attributeFamilyId}";

        public static string SaveLocales() => $"{ApiRoot}/attributefamily/savelocales";

        public static string GetUnAssignedAttributeGroups() => $"{ApiRoot}/attributefamily/getunassignedattributegroups";

        public static string GetAttributesByGroupIds() => $"{ApiRoot}/attributefamily/getattributesbygroupids";
    }
}

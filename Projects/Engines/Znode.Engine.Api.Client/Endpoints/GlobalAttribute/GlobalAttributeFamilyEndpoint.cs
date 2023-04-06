namespace Znode.Engine.Api.Client.Endpoints
{
    public class GlobalAttributeFamilyEndpoint : BaseEndpoint
    {

        public static string CreateAttributeFamily() => $"{ApiRoot}/globalattributefamily/create";

        public static string List() => $"{ApiRoot}/globalattributefamily/list";

        public static string UpdateAttributeFamily() => $"{ApiRoot}/globalattributefamily/update";

        public static string DeleteAttributeFamily() => $"{ApiRoot}/globalattributefamily/delete";

        public static string GetAttributeFamily(string familyCode) => $"{ApiRoot}/globalattributefamily/getattributefamily/{familyCode}";

        public static string GetAssignedAttributeGroups(string familyCode) => $"{ApiRoot}/globalattributefamily/getassignedattributegroups/{familyCode}";

        public static string  GetUnassignedAttributeGroups(string familyCode) => $"{ApiRoot}/globalattributefamily/getunassignedattributegroups/{familyCode}";

        public static string AssignAttributeGroups(string attributeGroupIds, string familyCode) => $"{ApiRoot}/globalattributefamily/assignattributegroups/{attributeGroupIds}/{familyCode}";

        public static string UnassignAttributeGroups(string groupCode, string familyCode) => $"{ApiRoot}/globalattributefamily/unassignAttributeGroups/{groupCode}/{familyCode}";

        public static string UpdateAttributeGroupDisplayOrder(string groupCode, string familyCode, int displayOrder) => $"{ApiRoot}/globalattributefamily/updateattributegroupdisplayorder/{groupCode}/{familyCode}/{displayOrder}";

        public static string GetGlobalAttributeFamilyLocales(string familyCode) => $"{ApiRoot}/globalattributefamily/getattributefamilylocale/{familyCode}";

        public static string IsFamilyCodeExist(string familyCode) => $"{ApiRoot}/globalattributefamily/isfamilycodeexist/{familyCode}";

    }
}

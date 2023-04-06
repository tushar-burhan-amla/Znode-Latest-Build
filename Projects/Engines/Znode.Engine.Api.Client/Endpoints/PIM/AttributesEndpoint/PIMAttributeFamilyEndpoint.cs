namespace Znode.Engine.Api.Client.Endpoints
{
    //Configuration for endpoint of Attribute family.
    public class PIMAttributeFamilyEndpoint : BaseEndpoint
    {
        //Get attribute family list.
        public static string GetAttributeFamilyList() => $"{ApiRoot}/pimattributefamily/list";

        //Create attribute family.
        public static string Create() => $"{ApiRoot}/pimattributefamily/create";

        //Get Assigned Attribute Groups.
        public static string GetAssignedAttributeGroups() => $"{ApiRoot}/pimattributefamily/getassignedattributegroups";

        //Associate Attribute Groups.
        public static string AssociateAttributeGroups() => $"{ApiRoot}/pimattributefamily/associateattributegroup";

        //UnAssociate Attribute Groups on the basis of attribute family id and attribute group id.
        public static string UnAssociateAttributeGroups(int attributeFamilyId, int attributeGroupId, bool isCategory)
            => $"{ApiRoot}/pimattributefamily/unassociateattributegroup/{attributeFamilyId}/{attributeGroupId}/{isCategory}";

        //Delete attribute family.
        public static string Delete() => $"{ApiRoot}/pimattributefamily/delete";

        //Get attribute family on the basis of attribute family id.
        public static string GetAttributeFamily(int attributeFamilyId) => $"{ApiRoot}/pimattributefamily/getattributefamily/{attributeFamilyId}";

        //Get UnAssigned Attribute Groups.
        public static string GetUnAssignedAttributeGroups() => $"{ApiRoot}/pimattributefamily/getunassignedattributegroups";

        //Get Family Locale by family Id.
        public static string GetFamilyLocale(int attributeFamilyId) => $"{ApiRoot}/pimattributefamily/getfamilylocale/{attributeFamilyId}";

        //Save locales for Attribute Family.
        public static string SaveLocales() => $"{ApiRoot}/pimattributefamily/savelocales";

        //Get attributes by Attribute group Id.
        public static string GetAttributesByGroupIds() => $"{ApiRoot}/pimattributefamily/getattributesbygroupids";

        //Update Attribute Group Display Order
        public static string UpdateAttributeGroupDisplayOrder() => $"{ApiRoot}/pimattributefamily/updateattributegroupdisplayorder";

        #region Attributes
        //Get asssigned attributes.
        public static string GetAssignedAttributes() => $"{ApiRoot}/pimattributefamily/getassignedattributes";

        //Get UnAssigned Attributes.
        public static string GetUnAssignedAttributes() => $"{ApiRoot}/pimattributefamily/getunassignedattributes";

        //Associate Attribute to Groups.
        public static string AssignAttributes() => $"{ApiRoot}/pimattributefamily/assignattributes";

        //Unassociate attribute from group.
        public static string UnAssignAttributes()
            => $"{ApiRoot}/pimattributefamily/unassignattributes"; 
        #endregion
    }
}

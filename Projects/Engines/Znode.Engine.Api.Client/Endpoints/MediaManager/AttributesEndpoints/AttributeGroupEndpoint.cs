namespace Znode.Engine.Api.Client.Endpoints
{
    public class AttributeGroupEndpoint : BaseEndpoint
    {
        //Get attribute group list endpoint
        public static string GetAttributeGroupList() => $"{ApiRoot}/attributegroups";

        //Get attribute group endpoint
        public static string Get(int attributeGroupId) => $"{ApiRoot}/attributegroup/{attributeGroupId}";

        //Create attribute group endpoint
        public static string Create() => $"{ApiRoot}/attributegroup";

        //Update attribute group endpoint
        public static string Update() => $"{ApiRoot}/attributegroup/update";

        //Delete attribute group endpoint
        public static string Delete() => $"{ApiRoot}/attributegroup/delete";

        //Assigns an attribute to attribute group.
        public static string AssignAttributes() => $"{ApiRoot}/assignattributes";

        //Gets associated attributes to an attribute group.
        public static string GetAssociatedAttributes() => $"{ApiRoot}/associatedattributes";

        //Deletes an associated attribute from an attribute group.
        public static string DeleteAssociatedAttribute(int attributeGroupMapperId) => $"{ApiRoot}/deleteassociatedattribute/{attributeGroupMapperId}";

        //Updates an attribute group mapper.
        public static string UpdateAttributeGroupMapper() => $"{ApiRoot}/updateattributegroupmapper";

        //Get Attribute Group Locales.
        public static string GetAttributeGroupLocales(int attributeGroupId) => $"{ApiRoot}/attributegroup/attributegrouplocales/{attributeGroupId}";

        //Get UnAssigned Attributes
        public static string GetUnAssignedAttributes(int attributeGroupId) => $"{ApiRoot}/attributegroup/getunassignedattributelist/{attributeGroupId}";
    }
}

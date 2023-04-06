namespace Znode.Engine.Api.Client.Endpoints
{
    public class GlobalAttributeEndpoint : BaseEndpoint
    {
        //Create attribute.
        public static string CreateAttribute() => $"{ApiRoot}/globalattribute/create";

        //Save Attribute Locale Values.
        public static string SaveLocales() => $"{ApiRoot}/globalattribute/savelocales";

        //Save attribute default values.
        public static string SaveDefaultValues() => $"{ApiRoot}/globalattribute/savedefaultvalues";

        // Get Input Validation By Attribute type and attribute Id.
        public static string GetInputValidations(int typeId, int attributeId = 0) => $"{ApiRoot}/globalattribute/inputvalidations/{typeId}/{attributeId}";

        //Get Attribute List.
        public static string GetAttributeList() => $"{ApiRoot}/globalattribute/list";

        //Get attribute by global attribute id.
        public static string GetAttribute(int id) => $"{ApiRoot}/globalattribute/{id}";

        //Update existing attribute.
        public static string UpdateAttribute() => $"{ApiRoot}/globalattribute/update";

        //Delete existing attribute(s).
        public static string DeleteAttribute() => $"{ApiRoot}/globalattribute/delete";

        //Get attribute locale value by attribute id.
        public static string GetAttributeLocale(int attributeId) => $"{ApiRoot}/globalattribute/getattributelocale/{attributeId}";

        //Get default values by attribute id.
        public static string GetDefaultValues(int attributeId) => $"{ApiRoot}/globalattribute/getdefaultvalues/{attributeId}";

        //Check attribute code already exist or not.
        public static string IsAttributeCodeExist(string attributeCode) => $"{ApiRoot}/globalattribute/isattributecodeexist/{attributeCode}";

        //Delete attribute's default values.
        public static string DeleteDefaultValues(int defaultvalueId) => $"{ApiRoot}/globalattribute/deletedefaultvalues/{defaultvalueId}";

        //Check values of attributes already exist or not.
        public static string IsAttributeValueUnique() => $"{ApiRoot}/globalattribute/isattributevalueunique";

        // Get entity list
        public static string GetGlobalEntity() => $"{ApiRoot}/globalattributeentity/getallentitylist";

        // Get assigned entity groups.
        public static string GetAssignedEntityAttributeGroups() => $"{ApiRoot}/globalattributeentity/getassignedentityattributegroups";

        // Get unassigned entity groups.
        public static string GetUnAssignedEntityAttributeGroups() => $"{ApiRoot}/globalattributeentity/getunassignedentityattributegroups";

        public static string AssociateAttributeEntityToGroups() => $"{ApiRoot}/globalattributeentity/associateattributeentitytogroups";

        //Un associate group from entity on the basis of entity Id and group Id.
        public static string UnAssociateEntityGroups(int entityId, int groupId) => $"{ApiRoot}/globalattributeentity/unassociateentitygroups/{entityId}/{groupId}";

        //Get default values by attribute id.
        public static string GetEntityAttributeDetails(int entityId, string entityType) => $"{ApiRoot}/globalattributeentity/getentityattributedetails/{entityId}/{entityType}";

        //Update Attribute Group Display Order
        public static string UpdateAttributeGroupDisplayOrder() => $"{ApiRoot}/globalattributeentity/updateattributegroupdisplayorder";

        public static string SaveEntityAttributeDetails() => $"{ApiRoot}/globalattributeentity/saveentityattributedetails";

        //Get publish global attributes.
        public static string GetGlobalEntityAttributes(int entityId, string entityType) => $"{ApiRoot}/globalattributeentity/getglobalentityattributes/{entityId}/{entityType}";

        //gets the global attributes based on the passed familyCode for setting the values for default container variant.
        public static string GetGlobalAttributesForDefaultVariantData(string familyCode, string entityType) => $"{ApiRoot}/globalattributeentity/getglobalattributesfordefaultvariantdata/{familyCode}/{entityType}";

        //Gets the global attributes based on the passed familyId for setting the values for default container variant.
        public static string GetGlobalAttributesForAssociatedVariant(int variantId, string entityType, int localeId = 0) => $"{ApiRoot}/globalattributeentity/getglobalattributesforassociatedvariant/{variantId}/{entityType}/{localeId}";

    }
}

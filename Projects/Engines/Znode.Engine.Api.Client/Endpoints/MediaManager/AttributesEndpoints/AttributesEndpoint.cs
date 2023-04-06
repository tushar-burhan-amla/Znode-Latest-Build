namespace Znode.Engine.Api.Client.Endpoints
{
    public class AttributesEndpoint : BaseEndpoint
    {
        //Create Attribute endpoint
        public static string Create() => $"{ApiRoot}/attributes/create";

        //Get attribute list endpoint
        public static string GetAttributeList() => $"{ApiRoot}/attributes/list";

        //Delete attribute endpoint
        public static string Delete() => $"{ApiRoot}/attributes/delete";

        //Update attribute endpoint
        public static string Update() => $"{ApiRoot}/attributes/update";

        // Get Input Validation By Attribute type and attribute Id
        public static string GetInputValidations(int typeId, int attributeId = 0) => $"{ApiRoot}/attributes/inputvalidations/{typeId}/{attributeId}";

        //Get attribute by attribute id endpoint
        public static string GetAttribute(int attributeId) => $"{ApiRoot}/attributes/{attributeId}";

        //Get attribute type list endpoint
        public static string GetAttributeTypeList() => $"{ApiRoot}/attributestype/list";

        //Get attributelocale by attribute id endpoint
        public static string GetAttributeLocale(int attributeId) => $"{ApiRoot}/attributelocale/{attributeId}";

        //Save Attribute Locale Values
        public static string SaveLocales() => $"{ApiRoot}/attributes/savelocales";

        //Save Attribute Default Values
        public static string SaveDefaultValues() => $"{ApiRoot}/attributes/savedefaultvalues";

        //Delete Attribute Default values
        public static string DeleteDefaultValues(int defaultvalueId) => $"{ApiRoot}/attributes/deletedefaultvalues/{defaultvalueId}";

        //Get Default Values By Attribute Id
        public static string GetDefaultValues(int attributeId) => $"{ApiRoot}/attributes/getdefaultvalues/{attributeId}";

        //Check attribute Code already exist or not
        public static string IsAttributeCodeExist(string attributeCode) => $"{ApiRoot}/attributes/isattributecodeexist/{attributeCode}";
    }
}

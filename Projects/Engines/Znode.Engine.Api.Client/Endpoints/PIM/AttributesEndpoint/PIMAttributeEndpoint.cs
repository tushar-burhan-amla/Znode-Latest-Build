namespace Znode.Engine.Api.Client.Endpoints
{
    public class PIMAttributeEndpoint : BaseEndpoint
    {
        //Get attribute type list endpoint
        public static string GetAttributeTypes(bool isCategory) => $"{ApiRoot}/pimattribute/types/{isCategory}";

        // Get Input Validation By Attribute type and attribute Id
        public static string GetInputValidations(int typeId, int attributeId = 0) => $"{ApiRoot}/pimattribute/inputvalidations/{typeId}/{attributeId}";

        //Get Attribute List
        public static string GetAttributes() => $"{ApiRoot}/pimattribute/list";

        //Get Attribute By Attribute Id
        public static string GetAttribute(int id) => $"{ApiRoot}/pimattribute/{id}";

        //Get Attribute Locale Value By attribute Id
        public static string GetAttributeLocale(int attributeId) => $"{ApiRoot}/pimattribute/getattributelocale/{attributeId}";

        //Get Default Values By Attribute Id
        public static string GetDefaultValues(int attributeId) => $"{ApiRoot}/pimattribute/getdefaultvalues/{attributeId}";

        //Create attribute
        public static string CreateAttribute() => $"{ApiRoot}/pimattribute/create";

        //Save Attribute Locale Values
        public static string SaveLocales() => $"{ApiRoot}/pimattribute/savelocales";

        //Save Attribute Default Values
        public static string SaveDefaultValues() => $"{ApiRoot}/pimattribute/savedefaultvalues";

        //Update existing Attribute
        public static string UpdateAttribute() => $"{ApiRoot}/pimattribute/update";

        //Delete Existing Attribute
        public static string DeleteAttribute() => $"{ApiRoot}/pimattribute/delete";

        //Get FrontEnd Properties
        public static string FrontEndProperties(int pimAttributeId) => $"{ApiRoot}/pimfrontproperties/{pimAttributeId}";

        //Delete Attribute Default values
        public static string DeleteDefaultValues(int defaultvalueId) => $"{ApiRoot}/pimattribute/deletedefaultvalues/{defaultvalueId}";

        //Check attribute Code already exist or not
        public static string IsAttributeCodeExist(string attributeCode, bool isCategory) => $"{ApiRoot}/pimattribute/isattributecodeexist/{attributeCode}/{isCategory}";

        //Check values of attributes already exist or not
        public static string IsAttributeValueUnique() => $"{ApiRoot}/pimattribute/isattributevalueunique";

       // Get attribute validation by attribute code.
        public static string GetAttributeValidationByCodes() => $"{ApiRoot}/pimattribute/getattributevalidationbycodes";

    }
}

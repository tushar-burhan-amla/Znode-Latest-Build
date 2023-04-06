namespace Znode.Engine.Api.Client.Endpoints
{
    public class TaxRuleTypeEndpoint : BaseEndpoint
    {
        //Get Tax RuleType List Endpoint
        public static string List() => $"{ApiRoot}/taxruletype/list";

        //Get Tax RuleType Endpoint
        public static string Get(int taxRuleTypeId) => $"{ApiRoot}/taxruletype/get/{taxRuleTypeId}";

        //Create Tax RuleType Endpoint
        public static string Create() => $"{ApiRoot}/taxruletype/create";

        //Update Tax RuleType Endpoint
        public static string Update() => $"{ApiRoot}/taxruletype/update";

        //Delete Tax RuleType Endpoint
        public static string Delete() => $"{ApiRoot}/taxruletype/delete";

        //Get Tax RuleType List which are Not In Database Endpoint
        public static string GetAllTaxRuleTypesNotInDatabase() => $"{ApiRoot}/taxruletype/getalltaxruletypesnotindatabase";

        //Enable/Disable bulky tax rule types Endpoint.
        public static string BulkEnableDisableTaxRuleTypes(bool isEnable) => $"{ApiRoot}/taxruletype/bulkenabledisabletaxruletypes/{isEnable}";

    }
}

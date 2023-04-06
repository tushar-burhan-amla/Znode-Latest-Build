namespace Znode.Engine.Api.Client.Endpoints
{
    public class TaxClassEndpoint : BaseEndpoint
    {
        //Get Tax Class List Endpoint
        public static string List() => $"{ApiRoot}/taxclass/list";

        //Get Tax Class Endpoint
        public static string Get(int taxClassId) => $"{ApiRoot}/taxclass/get/{taxClassId}";

        //Create Tax Class Endpoint
        public static string Create() => $"{ApiRoot}/taxclass/create";

        //Update Tax Class Endpoint
        public static string Update() => $"{ApiRoot}/taxclass/update";

        //Delete Tax Class Endpoint
        public static string Delete() => $"{ApiRoot}/taxclass/delete";

        #region Tax Class SKU.

        //Tax Class SKU List endpoint.
        public static string TaxClassSKUList() => $"{ApiRoot}/taxclass/sku/list";

        //Add Tax Class SKU endpoint.
        public static string AddTaxClassSKU() => $"{ApiRoot}/taxclass/sku/create";

        //Delete Tax Class SKU endpoint.
        public static string DeleteTaxClassSKU() => $"{ApiRoot}/taxclass/sku/delete";

        //Get Unassociated Product List Endpoint.
        public static string GetUnassociatedProductList() => $"{ApiRoot}/taxclass/sku/getunassociatedproductlist";

        #endregion Tax Class SKU.

        #region Tax Rule.

        //Tax Rule List endpoint.
        public static string TaxRuleList() => $"{ApiRoot}/taxrule/list";

        //Get Tax Rule endpoint.
        public static string GetTaxRule(int taxRuleId) => $"{ApiRoot}/taxrule/get/{taxRuleId}";

        //Add Tax Rule endpoint.
        public static string AddTaxRule() => $"{ApiRoot}/taxrule/create";

        //Update Tax Rule endpoint.
        public static string UpdateTaxRule() => $"{ApiRoot}/taxrule/update";

        //Delete Tax Rule endpoint.
        public static string DeleteTaxRule() => $"{ApiRoot}/taxrule/delete";

        #endregion Tax Rule.

        #region Avalara
        //test avalara connection endpoint.
        public static string TestAvalaraConnection() => $"{ApiRoot}/taxclass/testavalaraconnection";
        #endregion

    }
}
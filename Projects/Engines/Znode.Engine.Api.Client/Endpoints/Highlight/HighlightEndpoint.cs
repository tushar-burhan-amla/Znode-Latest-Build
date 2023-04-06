namespace Znode.Engine.Api.Client.Endpoints
{
    public class HighlightEndpoint : BaseEndpoint
    {
        //Get Highlight List Endpoint
        public static string List() => $"{ApiRoot}/highlight/list";

        //Create Highlight Endpoint.
        public static string Create() => $"{ApiRoot}/highlight/create";

        //Get Highlight on the basis of highlightId.
        public static string Get(int highlightId, int productId) => $"{ApiRoot}/gethighlight/{highlightId}/{productId}";

        //Get Highlight on the basis of highlightCode Endpoint.
        public static string GetByCode(string highlightCode) => $"{ApiRoot}/getHighlightByCode/{highlightCode}";

        //Update Highlight Endpoint.
        public static string Update() => $"{ApiRoot}/highlight/update";

        //Delete Highlight Endpoint.
        public static string Delete() => $"{ApiRoot}/highlight/delete";

        #region Highlight Type
        //Get Highlight Type List Endpoint.
        public static string GetHighlightTypeList() => $"{ApiRoot}/highlighttype/list";
        #endregion

        #region Highlight Product
        //Highlight Product List endpoint.
        public static string HighlightProductList() => $"{ApiRoot}/highlight/product/list";

        //Highlight Product List endpoint.
        public static string HighlightUnassociatedProductList() => $"{ApiRoot}/highlight/unassociatedproduct/list";

        //Associate Highlight Product endpoint.
        public static string AssociateAndUnAssociateProduct() => $"{ApiRoot}/highlight/assocaitehighlightproduct";

        //Get highlight code list endpoint
        public static string HighlightCodeList(string attributeCode) => $"{ApiRoot}/highlight/highlightcodelist/{attributeCode}";
        #endregion
    }
}

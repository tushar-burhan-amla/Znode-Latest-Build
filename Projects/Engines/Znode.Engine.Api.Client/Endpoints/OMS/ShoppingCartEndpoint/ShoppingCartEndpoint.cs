namespace Znode.Engine.Api.Client.Endpoints
{
    public class ShoppingCartEndpoint : BaseEndpoint
    {
        //Get Cookie details by cookie id.
        public static string GetShoppingCart() => $"{ApiRoot}/shoppingcarts/getshoppingcart";

        //Get Shopping cart count
        public static string GetCartCount() => $"{ApiRoot}/shoppingcarts/getcartcount";
        

        //Creates a new shopping cart.
        public static string Create() => $"{ApiRoot}/shoppingcarts";

        //Add product to cart.
        public static string AddToCartProduct() => $"{ApiRoot}/addtocartproduct";

        //Performs calculations for a shopping cart.
        public static string Calculate() => $"{ApiRoot}/shoppingcarts/calculate";

        //Endpoint to remove saved cart items by user id and cookie mapping id.
        public static string RemoveSavedCartItems() => $"{ApiRoot}/shoppingcarts/removeallcartitem";

        //Endpoint to get the list of shipping options
        public static string GetShippingEstimates(string zipCode) => $"{ApiRoot}/shoppingcarts/getshippingestimates/{zipCode}";

        //Endpoint to get oms line item Details
        public static string GetOmsLineItemDetails(int omsOrderId) => $"{ApiRoot}/shoppingcarts/getomslineitemdetails/{omsOrderId}";

        //Remove Saved Cart Line Item by omsSavedCartLineItemId
        public static string RemoveCartLineItem(int omsSavedCartLineItemId) => $"{ApiRoot}/shoppingcarts/removecartlineitem/{omsSavedCartLineItemId}";

        // Merge Cart after login
        public static string MergeGuestUsersCart() => $"{ApiRoot}/shoppingcarts/mergeguestuserscart";
    }
}

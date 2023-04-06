namespace Znode.Engine.Api.Client.Endpoints
{
    public class RMARequestItemEndpoint : BaseEndpoint
    {
        //Get rma request items.
        public static string List() => $"{ApiRoot}/rmarequestitems";

        //Get RMA Request Items For Gift Card
        public static string GetRMARequestItemsForGiftCard(string orderLineitems) => $"{ApiRoot}/rmarequestitems/getrmarequestitemsforgiftcard/{orderLineitems}";

        //Create RMA Request
        public static string Create() => $"{ApiRoot}/rmarequestitems/create";
    }
}

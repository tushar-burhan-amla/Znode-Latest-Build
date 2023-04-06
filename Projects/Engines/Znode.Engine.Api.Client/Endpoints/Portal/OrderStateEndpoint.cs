namespace Znode.Engine.Api.Client.Endpoints
{
    public class OrderStateEndpoint : BaseEndpoint
    {
        //Get OrderState List endpoint
        public static string List() => $"{ApiRoot}/orderstate/list";
    }
}

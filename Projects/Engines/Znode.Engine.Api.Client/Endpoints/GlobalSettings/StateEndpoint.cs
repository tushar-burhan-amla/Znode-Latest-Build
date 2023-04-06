namespace Znode.Engine.Api.Client.Endpoints
{
    public class StateEndpoint : BaseEndpoint
    {
        //Get state list endpoint
        public static string GetStateList() => $"{ApiRoot}/state/list";
    }
}

namespace Znode.Engine.Api.Client.Endpoints
{
    public class NavigationEndpoint : BaseEndpoint
    {
        public static string GetNavigationDetails() => $"{ApiRoot}/navigation/getnavigationdetails";
    }
}

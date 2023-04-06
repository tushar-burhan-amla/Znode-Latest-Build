namespace Znode.Engine.Api.Client.Endpoints
{
    public class CityEndpoint : BaseEndpoint
    {
        //Get city list endpoint
        public static string GetCityList() => $"{ApiRoot}/city/list";
    }
}

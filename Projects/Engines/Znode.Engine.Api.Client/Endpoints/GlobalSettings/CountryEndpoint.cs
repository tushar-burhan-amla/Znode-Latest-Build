namespace Znode.Engine.Api.Client.Endpoints
{
    public class CountryEndpoint : BaseEndpoint
    {
        //Get country list endpoint
        public static string GetCountryList() => $"{ApiRoot}/country/list";

        //Get country endpoint
        public static string GetCountry() => $"{ApiRoot}/country";

        //Update country endpoint
        public static string UpdateCountry() => $"{ApiRoot}/country/update";
    }
}

namespace Znode.Engine.Api.Client.Endpoints
{
    public class WeightUnitEndpoint : BaseEndpoint
    {
        //Get weightunit list endpoint
        public static string GetWeightUnitList() => $"{ApiRoot}/weightunit/list";

        //Update weightunit endpoint
        public static string UpdateWeightUnit() => $"{ApiRoot}/weightunit/update";
    }
}

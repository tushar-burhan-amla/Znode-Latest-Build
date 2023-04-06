namespace Znode.Engine.Api.Cache
{
    public interface IWeightUnitCache
    {
        /// <summary>
        /// Get a list of all Weight units.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetWeightUnits(string routeUri, string routeTemplate);
    }
}

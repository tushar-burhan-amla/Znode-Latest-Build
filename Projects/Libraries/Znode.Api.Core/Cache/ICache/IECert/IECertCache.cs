namespace Znode.Engine.Api.Cache
{
    public interface IECertCache
    {

        /// <summary>
        /// Get the list of all tax classes.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>list of all tax classes in string format.</returns>
        string GetECertificateList(string routeUri, string routeTemplate);

    }
}

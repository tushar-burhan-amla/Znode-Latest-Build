using Znode.Engine.Api.Models;

namespace Znode.Engine.Taxes
{
    public interface IAvataxClient
    {
        /// <summary>
        /// Method to test Avalara API connection.
        /// </summary>
        /// <param name="taxportalModel">TaxPortalModel</param>
        /// <returns></returns>
        string RESTTestConnection(TaxPortalModel taxportalModel);
    }
}

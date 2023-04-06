using System.Collections.Generic;

namespace Znode.Admin.Core.Agents
{
    public interface ICustomHeaders
    {
        /// <summary>
        /// Set Custom Header For API Request
        /// </summary>
        /// <returns>List OF Headers in Key Value Pair</returns>
        Dictionary<string, string> SetCustomHeaderOfClient();
    }
}

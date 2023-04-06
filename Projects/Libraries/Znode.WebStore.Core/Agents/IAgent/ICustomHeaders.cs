using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.WebStore.Core.Agents.IAgent
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

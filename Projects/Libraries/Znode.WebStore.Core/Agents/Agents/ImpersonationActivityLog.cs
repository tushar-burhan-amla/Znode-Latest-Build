using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.WebStore.Agents
{
    public class ImpersonationActivityLog : BaseAgent, IImpersonationActivityLog
    {
        public bool AddImpersonationEntry<T>(T entity)
        {
            ImpersonationActivityLog impersonationActivityLog = GetFromSession<ImpersonationActivityLog>(WebStoreConstants.ImpersonationSessionKey);
            if (impersonationActivityLog != null)
            {

            }
            return true;
        }
    }
}

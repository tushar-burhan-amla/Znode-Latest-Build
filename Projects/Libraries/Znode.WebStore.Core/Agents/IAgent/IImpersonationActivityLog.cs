using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.WebStore.Agents
{
    public interface IImpersonationActivityLog
    {
        
        bool AddImpersonationEntry<T>(T entity);
    }
}

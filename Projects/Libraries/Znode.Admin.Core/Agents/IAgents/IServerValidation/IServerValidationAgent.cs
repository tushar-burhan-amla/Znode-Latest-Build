using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.Agents
{
    public interface IServerValidationAgent
    {
        Dictionary<string,string> Validate(BindDataModel model);
    }
}

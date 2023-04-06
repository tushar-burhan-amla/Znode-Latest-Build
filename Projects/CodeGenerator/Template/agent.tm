using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using ZNode.Libraries.ECommerce.Utilities;
using ZNode.Libraries.Framework.Business;
using ZNode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    public class {FileName}Agent : BaseAgent, I{FileName}Agent
    {
        #region Private Variables
        private readonly I{FileName}Client _{fileName}Client;
        #endregion Private Variables

        #region Constructor
        /// <summary>
        /// Constructor for {FileName} agent.
        /// </summary>
        public {FileName}Agent(I{FileName}Client {fileName}Client)
        {
            _{fileName}Client = GetClient<I{FileName}Client>({fileName}Client);
        }
        #endregion Constructor


        #region Public Methods
		
        #endregion Public Methods


        #region Private Methods.

        #endregion Private Methods.
    }
}
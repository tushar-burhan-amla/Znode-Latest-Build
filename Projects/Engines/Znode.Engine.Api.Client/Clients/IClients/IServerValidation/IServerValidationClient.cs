using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface IServerValidationClient : IBaseClient
    {
        Dictionary<string, string> validateControl(ValidateServerModel model);
    }
}

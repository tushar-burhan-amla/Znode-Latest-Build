
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client.Clients
{
    public interface IDemoClient
    {
        UserModel GetAccount(int accountId, ExpandCollection expands);
    }
}

using Znode.Engine.Api.Client.Clients;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Agents
{

    public class DemoAgent : IDemoAgent
    {
        private IDemoClient _demoClient { get; } = new DemoClient();
       
        public virtual UserModel GetAccounts() => _demoClient.GetAccount(1, GetExpands());
        
        private ExpandCollection GetExpands()=> new ExpandCollection()
                {
                    ExpandKeys.Addresses,
                    ExpandKeys.Profiles,
                    ExpandKeys.WishLists,
                    ExpandKeys.Orders,
                    ExpandKeys.OrderLineItems,
                    ExpandKeys.User,
                    ExpandKeys.GiftCardHistory
                };        
    }
}
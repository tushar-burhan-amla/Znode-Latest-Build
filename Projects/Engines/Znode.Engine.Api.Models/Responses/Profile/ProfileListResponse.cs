using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class ProfileListResponse : BaseListResponse
    {
        public List<ProfileModel> Profiles { get; set; }
        public bool HasParentAccounts { get; set; }
        public string CustomerName { get; set; }
        public int AccountId { get; set; }
    }
}

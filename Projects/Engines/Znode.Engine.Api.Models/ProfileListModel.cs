using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ProfileListModel : BaseListModel
    {
        public ProfileListModel()
        {
            Profiles = new List<ProfileModel>();
        }
        public List<ProfileModel> Profiles { get; set; }
        public bool HasParentAccounts { get; set; }
        public string CustomerName { get; set; }
        public int AccountId { get; set; }
    }   
}       

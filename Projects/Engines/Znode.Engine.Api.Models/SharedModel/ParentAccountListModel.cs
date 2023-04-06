using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ParentAccountListModel : BaseListModel
    {
        public List<ParentAccountModel> ParentAccount { get; set; }

        public ParentAccountListModel()
        {
            ParentAccount = new List<ParentAccountModel>();
        }
    }
}

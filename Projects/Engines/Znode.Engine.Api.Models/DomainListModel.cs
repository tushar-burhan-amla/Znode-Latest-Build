using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class DomainListModel : BaseListModel
    {
        public List<DomainModel> Domains { get; set; }

        public DomainListModel()
        {
            Domains = new List<DomainModel>();
        }
    }
}

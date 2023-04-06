using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ECertificateListModel : BaseListModel
    {
        public List<ECertificateModel> ECertificates { get; set; }
    }
}

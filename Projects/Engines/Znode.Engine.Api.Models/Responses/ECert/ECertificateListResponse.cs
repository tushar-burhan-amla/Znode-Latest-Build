using System.Collections.Generic; 

namespace Znode.Engine.Api.Models.Responses
{
    public class ECertificateListResponse : BaseListResponse
    {
        public List<ECertificateModel> ECertificates { get; set; }
    }
}

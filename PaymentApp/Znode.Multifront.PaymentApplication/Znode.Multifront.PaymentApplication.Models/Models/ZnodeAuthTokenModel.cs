using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Multifront.PaymentApplication.Models.Models
{
    public class ZnodeAuthTokenModel : BaseModel
    {
        public int AuthTokenId { get; set; }
        public string AuthToken { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> TotalAttempt { get; set; }
    }
}

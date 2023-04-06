using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Libraries.Klaviyo
{
    public class IdentifyModel : BaseModel
    {
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string Image { get; set; }
        public List<string> Consent { get; set; }
    }
}

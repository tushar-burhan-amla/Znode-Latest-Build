using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("EmailOptInCustomer")]
    [HighlightedClass]
    public class EmailOptInCustomerModel : List<EmailOptInCustomerInfo>
    {
        [HighlightedMember]
        public EmailOptInCustomerModel()
        {

        }
    }
}

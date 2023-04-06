using DevExpress.DataAccess.ObjectBinding;
using System.Collections.Generic;
using System.ComponentModel;

namespace Znode.Libraries.DevExpress.Report
{
    [DisplayName("ServiceRequest")]
    [HighlightedClass]
    public class ServiceRequestModel : List<ServiceRequestInfo>
    {
        [HighlightedMember]
        public ServiceRequestModel()
        {

        }
    }
}

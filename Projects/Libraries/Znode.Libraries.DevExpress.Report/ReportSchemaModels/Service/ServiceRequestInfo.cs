using System;

namespace Znode.Libraries.DevExpress.Report
{
    public class ServiceRequestInfo
    {
        public int CaseRequestId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StatusName { get; set; }
        public string StoreName { get; set; }
    }
}

using System;

namespace Znode.Libraries.DevExpress.Report
{
    public class UsersInfo : BaseInfo
    {
        public DateTime RegistrationDate { get; set; }
        public string CustomerName { get; set; }
        public string StoreName { get; set; }
        public string Email { get; set; }
        public string CustomerStatus { get; set; }
    }
}

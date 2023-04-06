using System;

namespace Znode.Engine.Api.Models
{
    public class ReportUsersModel
    {
        public DateTime RegistrationDate { get; set; }
        public string CustomerName { get; set; }
        public string StoreName { get; set; }
        public string Email { get; set; }
        public string CustomerStatus { get; set; }
    }
}

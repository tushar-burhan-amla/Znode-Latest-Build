using System;

namespace Znode.Engine.Api.Models
{
    public class LicenceInfoModel : BaseModel
    {
        public string LicenseType { get; set; }
        public string SerialKey { get; set; }
        public DateTime InstallationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}

using System;

namespace Znode.Engine.Admin.ViewModels
{
    public class LicenceInfoViewModel: BaseViewModel
    {
        public string LicenseType { get; set; }
        public string SerialKey { get; set; }
        public DateTime InstallationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
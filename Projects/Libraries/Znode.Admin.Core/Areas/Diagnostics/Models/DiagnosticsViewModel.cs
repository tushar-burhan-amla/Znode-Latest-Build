namespace Znode.Engine.Admin.ViewModels
{
    public class DiagnosticsViewModel: BaseViewModel
    {
        public string ProductVersion { get; set; }

        public bool DatabaseStatus { get; set; }

        public bool PublicPermissions { get; set; }

        public bool LicenseStatus { get; set; }

        public string LicenseDescription { get; set; }

        public bool SMTPAccountStatus { get; set; }

        public string ExceptionSummary { get; set; }

        public string CaseNumber { get; set; }

        public string MongoStatus { get; set; }

        public string ElasticSearchStatus { get; set; }
    }
}

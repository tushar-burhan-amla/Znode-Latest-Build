using System;

namespace Znode.Engine.Api.Models
{
    public class DiagnosticsProductVersionModel : BaseModel
    {
        public int MultifrontId { get; set; }
        public string VersionName { get; set; }
        public string Descriptions { get; set; }
        public Nullable<int> MajorVersion { get; set; }
        public Nullable<int> MinorVersion { get; set; }
        public Nullable<int> BuildVersion { get; set; }
        public Nullable<int> LowerVersion { get; set; }
        public Nullable<int> PatchIndex { get; set; }
    }
}

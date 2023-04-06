namespace Znode.Engine.Api.Models
{
    public class FamilyExtensionModel : BaseModel
    {
        public string Extension { get; set; }
        public string MaxFileSize { get; set; }
        public string ValidationName { get; set; }
        public int MediaAttributeFamilyId { get; set; }
        public string FamilyCode { get; set; }
    }
}

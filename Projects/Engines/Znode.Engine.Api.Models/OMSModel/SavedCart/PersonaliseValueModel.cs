namespace Znode.Engine.Api.Models
{
    public class PersonaliseValueModel : BaseModel
    {
        public string PersonalizeCode { get; set; }
        public string PersonalizeValue { get; set; }
        public string DesignId { get; set; }
        public string ThumbnailURL { get; set; }
        public string PersonalizeName { get; set; }
        public int OmsSavedCartLineItemId { get; set; }
        public int GroupIdentifier { get; set; }
    }
}

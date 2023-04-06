namespace Znode.Engine.Api.Models.Responses
{
    public class PIMAttributeFamilyResponse : BaseResponse
    {
        public PIMAttributeFamilyModel PIMAttributeFamily { get; set; }

        public PIMFamilyDetailsModel PIMFamilyDetails { get; set; }
    }
}

namespace Znode.Engine.Api.Models.Responses
{
    public class EntityAttributeResponse : BaseResponse
    {
        public EntityAttributeModel EntityAttribute { get; set; }
        public bool IsSuccess { get; set; }
    }
}

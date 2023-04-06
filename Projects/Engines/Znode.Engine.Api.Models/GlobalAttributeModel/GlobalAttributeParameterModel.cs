namespace Znode.Engine.Api.Models
{
    public class GlobalAttributeParameterModel : BaseModel
    {
        public int Id { get; set; }
        public string AttributeCode { get; set; }
        public string AttributeName { get; set; }
    }
}
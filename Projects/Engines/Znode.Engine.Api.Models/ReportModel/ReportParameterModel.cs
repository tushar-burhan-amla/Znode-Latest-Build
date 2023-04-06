
namespace Znode.Engine.Api.Models
{
    public class ReportParameterModel : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Operator { get; set; }
        public string DataType { get; set; }
        public string Value { get; set; }
    }
}

namespace Znode.Engine.Api.Models
{
    public class EntityColumnModel :BaseModel
    {
        public string ObjectName { get; set; }
        public int ColumnId { get; set; }
        public string ColumnName { get; set; }
        public string DataType { get; set; }
    }
}

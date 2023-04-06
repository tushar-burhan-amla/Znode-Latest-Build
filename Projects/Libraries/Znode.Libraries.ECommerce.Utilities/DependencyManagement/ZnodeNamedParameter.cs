
namespace Znode.Libraries.ECommerce.Utilities
{
    public class ZnodeNamedParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public ZnodeNamedParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}

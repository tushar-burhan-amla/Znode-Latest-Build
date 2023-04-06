using System;

namespace Znode.Libraries.ECommerce.Utilities
{
    public class ZnodeTypedParameter
    {
        public Type Type { get; set; }
        public object Value { get; set; }

        public ZnodeTypedParameter(Type type, object value)
        {
            Type = type;
            Value = value;
        }
    }
}

using System;

namespace Znode.Libraries.ECommerce.Utilities
{
    public class RealTime : Attribute
    {
        public string Name { get; set; }

        public RealTime(string name)
        {
            Name = name;
        }
    }
}

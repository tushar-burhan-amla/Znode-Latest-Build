using System.Collections.Generic;

namespace Znode.Engine.Admin.Models
{
    public class BaseTree
    {
        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public bool Collapsed { get; set; }
        public string CssClassName { get; set; }
        public string  Path { get; set; }
        public List<BaseTree> Children { get; set; }  
    }
}
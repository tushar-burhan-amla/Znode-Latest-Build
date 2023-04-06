using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Znode.Engine.Admin.ViewModels
{
    public class DiagnosticViewModel : BaseViewModel
    {
        public string Category { get; set; }
        public string Item { get; set; }
        public bool Status { get; set; }        
    }
}

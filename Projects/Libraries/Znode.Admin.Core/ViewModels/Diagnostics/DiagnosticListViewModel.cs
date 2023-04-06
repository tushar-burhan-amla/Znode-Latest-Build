using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class DiagnosticListViewModel : BaseViewModel
    {
        public List<DiagnosticViewModel> DiagnosticsList { get; set; }
        public GridModel GridModel { get; set; }

    }
}

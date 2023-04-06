using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class GlobalAttributeFamilyListViewModel : BaseViewModel
    {
        public List<GlobalAttributeFamilyViewModel> AttributeFamilyList { get; set; }

        public GridModel GridModel { get; set; }
        public string GlobalEntity { get; set; }
        public int GlobalEntityId { get; set; }
    }
}

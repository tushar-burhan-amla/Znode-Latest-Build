using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class GlobalAttributeListViewModel : BaseViewModel
    {
        public List<GlobalAttributeViewModel> List { get; set; }
        public GridModel GridModel { get; set; }
        public string GlobalEntity { get; set; }
        public int GlobalEntityId { get; set; }
        public string EntityName { get; set; }
    }
}
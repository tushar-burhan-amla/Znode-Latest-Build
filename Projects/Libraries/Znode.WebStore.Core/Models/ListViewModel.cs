using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.WebStore.Models
{
    public class ListViewModel
    {
        public int[] AssignedId { get; set; }
        public int[] UnAssignedId { get; set; }
        public IEnumerable<SelectListItem> AssignedList { get; set; }
        public IEnumerable<SelectListItem> UnAssignedList { get; set; }
    }
}
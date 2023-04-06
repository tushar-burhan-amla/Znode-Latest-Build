using System.Collections.Generic;

namespace Znode.Engine.WebStore.Models
{
    public class DropdownListModel
    {
        public List<DropdownModel> Dropdown { get; set; }

        public string Name { get; set; }
        public string Label { get; set; }
        public int Id { get; set; }

        public DropdownListModel()
        {
            Dropdown = new List<DropdownModel>();
        }
    }
}
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class TaxClassSKUViewModel :BaseViewModel
    {
        public int TaxClassSKUId { get; set; }
        public int? TaxClassId { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public string SKUs { get; set; }
        public string ProductName { get; set; }
        
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ECertificateViewModel : BaseViewModel
    {
        public int ECertificateId { get; set; }
        public string IssuedByName { get; set; }
        public string IssuedById { get; set; }
        [Required(AllowEmptyStrings = false,ErrorMessageResourceType  = typeof(WebStore_Resources), ErrorMessageResourceName = ZnodeWebStore_Resources.ECertRequiredDataErrorMessage)]
        public string CertificateKey { get; set; }
        public string CertificateType { get; set; }
        public string IssuedCYMD { get; set; }
        public decimal IssuedAmt { get; set; }
        public decimal Balance { get; set; }
        public string BalanceWithCurrency { get; set; }
        public string LastUsedCYMD { get; set; }
        public decimal RedemptionApplied { get; set; }
        public decimal CurrentBalance { get; set; }
        public string PortalId { get; set; }
    }
}

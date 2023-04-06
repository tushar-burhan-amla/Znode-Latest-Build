using System;

namespace Znode.Engine.WebStore.ViewModels
{
    public class VoucherViewModel : BaseViewModel
    {
        public int VoucherId { get; set; }
        public int? UserId { get; set; }
        public int PortalId { get; set; }

        public string Name { get; set; }
        public string CardNumber { get; set; }

        public string Amount { get; set; }
        public string RemainingAmount { get; set; }

        public decimal? VoucherAmount { get; set; }
        public decimal? VoucherBalanceAmount { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? ExpirationDate { get; set; }

        public string CurrencyCode { get; set; }
        public string CultureCode { get; set; }
    }
}

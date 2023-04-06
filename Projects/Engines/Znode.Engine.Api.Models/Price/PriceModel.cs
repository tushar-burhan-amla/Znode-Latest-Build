using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class PriceModel : BaseModel
    {
        public int PriceListId { get; set; }
        [Required]
        public string ListCode { get; set; }
        [Required]
        public string ListName { get; set; }
        public int CurrencyId { get; set; }
        public int OldCurrencyId { get; set; }
        public int PortalId { get; set; }
        public int ProfileId { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string Currency { get; set; }
        public int? Precedence { get; set; }
        public int? AccountId { get; set; }
        public int? UserId { get; set; }
        public int PriceListProfileId { get; set; }
        public string UserName { get; set; }
        public string CurrencyName { get; set; }
        public string StoreName { get; set; }
        public string ProfileName { get; set; }
        public string AccountName { get; set; }
        public bool IsParentAccount { get; set; }
        public int PriceListUserId { get; set; }
        public int PriceListAccountId { get; set; }
        public int PriceListPortalId { get; set; }
        public int PortalProfileId { get; set; }
        public List<ImportPriceModel> ImportPriceList { get; set; }
        public int CultureId { get; set; }
        public string CultureName { get; set; }
        public int OldCultureId { get; set; }
        public string CultureCode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Agents;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Admin.Core
{
    public class AdminHelper : BaseAgent, IAdminHelper
    {
        //Gets the default value for assigning to InHandDate
        public virtual DateTime GetInHandDate()
        {
            const int postDateDays = 30;
            return DateTime.Today.Date.AddDays(postDateDays);
        }

        //Gets the Names and Descriptions of passed Enum Members and returns in ShippingConstraintsViewModel
        public virtual IList<ShippingConstraintsViewModel> GetEnumMembersNameAndDescription(Enum value)
        {
            return HelperUtility.GetNamesAndDescriptionsFromEnum(value).Select(s => new ShippingConstraintsViewModel
            {
                ShippingConstraintCode = s.Key,
                Description = s.Value
            }).ToList();
        }

        //Gets the dafault quote expiration date
        public virtual DateTime GetQuoteExpirationDate(int quoteExpiredInDays)
        {
            return quoteExpiredInDays > 0 ? DateTime.Now.AddDays(quoteExpiredInDays): DateTime.Today.Date.AddDays(30);
        }
    }
}
using System;
using System.Collections.Generic;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Admin.Core
{
    public interface IAdminHelper
    {
        //Gets the default value for assigning to InHandDate
        DateTime GetInHandDate();

        //Gets the Names and Descriptions of passed Enum Members and returns in ShippingConstraintsViewModel
        IList<ShippingConstraintsViewModel> GetEnumMembersNameAndDescription(Enum value);

        //Gets the default quote expiration date
        DateTime GetQuoteExpirationDate(int quoteExpiredInDays);
    }
}
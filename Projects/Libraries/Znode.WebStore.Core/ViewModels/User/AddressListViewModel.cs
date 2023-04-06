using System;
using System.Collections.Generic;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.ViewModels
{
    public class AddressListViewModel : BaseViewModel
    {
        public List<AddressViewModel> AddressList { get; set; }
        public string AccountName { get; set; }
        public int AccountId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public int UserId { get; set; }
        public bool HasParentAccounts { get; set; }

        public AddressViewModel BillingAddress { get; set; }
        public AddressViewModel ShippingAddress { get; set; }

        public int SelectedAddressId { get; set; }

        public AddressListViewModel()
        {
            BillingAddress = new AddressViewModel();
            ShippingAddress = new AddressViewModel();
        }

        public bool PermissionWithNullCheck
        {
            get
            {
                if (!string.Equals(this?.RoleName, ZnodeRoleEnum.Manager.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    && !string.Equals(this?.RoleName, ZnodeRoleEnum.User.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    return true;
                else
                    return false;

            }
        }

        public bool PermissionWithOutNullCheck
        {
            get
            {
                if (!string.Equals(this.RoleName, ZnodeRoleEnum.Manager.ToString(), StringComparison.CurrentCultureIgnoreCase)
                    && !string.Equals(this.RoleName, ZnodeRoleEnum.User.ToString(), StringComparison.CurrentCultureIgnoreCase))
                    return true;
                else
                    return false;
            }
        }
    }
}
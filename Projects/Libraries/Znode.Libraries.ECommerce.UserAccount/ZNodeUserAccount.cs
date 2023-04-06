using System;
using System.Linq;
using System.Web;
using ZNode.Libraries.Framework.Business;

namespace ZNode.Libraries.ECommerce.UserAccount
{
    /// <summary>
    /// Provides the functions for managing user accounts.
    /// </summary>
    [Serializable()]
    public class ZNodeUserAccount : ZNodeUserAccountBase
    {
        #region Private Variables

        //TODO Data Library to be added

        //private Address _ShippingAddress = new Address();
        //private Address _BillingAddress = new Address();

        #endregion

        #region Public Constructor
        /// <summary>
        /// Initializes a new instance of the ZNodeUserAccount class.
        /// </summary>
        //public ZNodeUserAccount()
        //{
        //    _Account = new Account();
        //    Addresses = new TList<Address>();
        //}

        //public ZNodeUserAccount(Account account)
        //{
        //    _Account = account;
        //    Addresses = new TList<Address>();

        //    //if (account.AddressCollection != null && account.AddressCollection.Any())
        //    if(account?.AddressCollection?.Any() ?? false)
        //    {
        //        Addresses = account.AddressCollection;

        //        // Get top 1 billing address.
        //        this.SetBillingAddress(Addresses.FirstOrDefault(address => address.IsDefaultBilling));

        //        // Get top 1 shipping address.
        //        this.SetShippingAddress(Addresses.FirstOrDefault(address => address.IsDefaultShipping));
        //    }
        }

        #endregion

        #region Public Properties

       // public TList<Address> Addresses { get; set; }

        //public bool EnableCustomerPricing
        //{
        //    get { return _Account.EnableCustomerPricing.GetValueOrDefault(false); }
        //    set { _Account.EnableCustomerPricing = value; }
        //}

        /// <summary>
        /// Gets or sets the EmailID
        /// </summary>
        //public string EmailID
        //{
        //    get { return this._Account.Email; }
        //    set { this._Account.Email = value; }
        //}

        /// <summary>
        /// Gets or sets the firstname
        /// </summary>
        //public string FirstName
        //{
        //    get { return this.BillingAddress.FirstName; }
        //    set { this._BillingAddress.FirstName = value; }
        //}

        ///// <summary>
        ///// Gets or sets Last Name
        ///// </summary>
        //public string LastName
        //{
        //    get { return this.BillingAddress.LastName; }
        //    set { this.BillingAddress.LastName = value; }
        //}

        ///// <summary>
        ///// Gets or sets Customn1
        ///// </summary>
        //public string Custom1
        //{
        //    get { return this._Account.Custom1; }
        //    set { this._Account.Custom1 = value; }
        //}

        ///// <summary>
        ///// Gets or sets the Custom2
        ///// </summary>
        //public string Custom2
        //{
        //    get { return this._Account.Custom2; }
        //    set { this._Account.Custom2 = value; }
        //}

        ///// <summary>
        ///// Gets or sets the custom3
        ///// </summary>
        //public string Custom3
        //{
        //    get { return this._Account.Custom3; }
        //    set { this._Account.Custom3 = value; }
        //}

        ///// <summary>
        ///// Gets or sets the account id
        ///// </summary>
        //public int AccountID
        //{
        //    get { return this._Account.AccountID; }
        //    set { this._Account.AccountID = value; }
        //}

        ///// <summary>
        ///// Gets or sets Parent Account ID
        ///// </summary>
        //public int ParentAccountID
        //{
        //    get { return (int)this._Account.ParentAccountID; }
        //    set { this._Account.ParentAccountID = value; }
        //}

        ///// <summary>
        ///// Gets or sets User ID
        ///// </summary>
        //public Guid? UserID
        //{
        //    get { return this._Account.UserID; }
        //    set { this._Account.UserID = value; }
        //}

        ///// <summary>
        ///// Gets or sets Profile ID
        ///// </summary>
        //public int ProfileID
        //{
        //    get { return (int)this._Account.ProfileID; }
        //    set { this._Account.ProfileID = value; }
        //}

        ///// <summary>
        ///// Gets or sets External Account Number
        ///// </summary>
        //public string ExternalAccountNo
        //{
        //    get { return this._Account.ExternalAccountNo; }
        //    set { this._Account.ExternalAccountNo = value; }
        //}

        ///// <summary>
        ///// Gets or sets the company name
        ///// </summary>
        //public string CompanyName
        //{
        //    get { return this._Account.CompanyName; }
        //    set { this._Account.CompanyName = value; }
        //}

        ///// <summary>
        ///// Gets or sets Account Type id
        ///// </summary>
        //public int AccountTypeID
        //{
        //    get { return (int)this._Account.AccountTypeID; }
        //    set { this._Account.AccountTypeID = value; }
        //}

        ///// <summary>
        ///// Gets or sets Account Profile Code
        ///// </summary>
        //public string AccountProfileCode
        //{
        //    get { return this._Account.AccountProfileCode; }
        //    set { this._Account.AccountProfileCode = value; }
        //}

        ///// <summary>
        ///// Gets or sets Sub Account Limit
        ///// </summary>
        //public int SubAccountLimit
        //{
        //    get { return (int)this._Account.SubAccountLimit; }
        //    set { this._Account.SubAccountLimit = value; }
        //}

        ///// <summary>
        ///// Gets or sets the billing address
        ///// </summary>
        //public Address BillingAddress
        //{
        //    get { return this._BillingAddress; }
        //    set { this._BillingAddress = value; }
        //}

        ///// <summary>
        ///// Gets or sets the shipping address
        ///// </summary>
        //public Address ShippingAddress
        //{
        //    get { return this._ShippingAddress; }
        //    set { this._ShippingAddress = value; }
        //}

        ///// <summary>
        ///// Gets or sets the description
        ///// </summary>
        //public string Description
        //{
        //    get { return this._Account.Description; }
        //    set { this._Account.Description = value; }
        //}

        ///// <summary>
        ///// Gets or sets the create user
        ///// </summary>
        //public string CreateUser
        //{
        //    get { return this._Account.CreateUser; }
        //    set { this._Account.CreateUser = value; }
        //}

        //public DateTime CreateDte
        //{
        //    get { return this._Account.CreateDte; }
        //    set { this._Account.CreateDte = value; }
        //}

        ///// <summary>
        ///// Gets or sets the update user
        ///// </summary>
        //public string UpdateUser
        //{
        //    get { return this._Account.UpdateUser; }
        //    set { this._Account.UpdateUser = value; }
        //}

        //public DateTime? UpdateDte
        //{
        //    get { return this._Account.UpdateDte; }
        //    set { this._Account.UpdateDte = value; }
        //}

        ///// <summary>
        ///// Gets or sets a value indicating whether active or not
        ///// </summary>
        //public bool ActiveInd
        //{
        //    get { return (bool)this._Account.ActiveInd; }
        //    set { this._Account.ActiveInd = value; }
        //}

        ///// <summary>
        ///// Gets or sets a value indicating whether email opt in or not.
        ///// </summary>
        //public bool EmailOptIn
        //{
        //    get { return (bool)_Account.EmailOptIn; }
        //    set { _Account.EmailOptIn = value; }
        //}

        ///// <summary>
        ///// Gets or sets the Source
        ///// </summary>
        //public string Source
        //{
        //    get { return _Account.Source; }
        //    set { _Account.Source = value; }
        //}

        ///// <summary>
        ///// Gets or sets the referral commission type
        ///// </summary>
        //public int? ReferralCommissionTypeId
        //{
        //    get { return _Account.ReferralCommissionTypeID; }
        //    set { _Account.ReferralCommissionTypeID = value; }
        //}

        ///// <summary>
        ///// Gets or sets Referral Commission
        ///// </summary>
        //public decimal ReferralCommission
        //{
        //    get { return (decimal)_Account.ReferralCommission; }
        //    set { _Account.ReferralCommission = value; }
        //}

        ///// <summary>
        ///// Gets or sets the taxid
        ///// </summary>
        //public string TaxId
        //{
        //    get { return _Account.TaxID; }
        //    set { _Account.TaxID = value; }
        //}

        ///// <summary>
        ///// Gets or sets the referral status
        ///// </summary>
        //public string ReferralStatus
        //{
        //    get { return _Account.ReferralStatus; }
        //    set { _Account.ReferralStatus = value; }
        //}

        ///// <summary>
        ///// Gets or sets the referral account id
        ///// </summary>
        //public int? ReferralAccountId
        //{
        //    get { return _Account.ReferralAccountID; }
        //    set { _Account.ReferralAccountID = value; }
        //}

        //public string Website
        //{
        //    get { return _Account.Website; }
        //    set { _Account.Website = value; }
        //}

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the current Account
        /// </summary>
        /// <returns>Returns the current account</returns>
  //      public static ZNodeUserAccount CurrentAccount()
  //      {
  //          ZNodeUserAccount userAcct;

  //          // get the user account from session
  //          userAcct = (ZNodeUserAccount)HttpContext.Current.Session[ZNodeSessionKeyType.UserAccount.ToString()];

  //          // Not in session
  //          if (Equals(userAcct , null))
  //          {
  //              userAcct = new ZNodeUserAccount();

  //              if (HttpContext.Current.User.Identity.IsAuthenticated)
  //              {
  //                  bool success = userAcct.GetByLogin(ZNodeConfigManager.SiteConfig.PortalID, HttpContext.Current.User.Identity.Name);

  //                  if (success)
  //                      userAcct.SetAccountAddress();

  //                  // Get account and set to session
  //                  HttpContext.Current.Session.Add(ZNodeSessionKeyType.UserAccount.ToString(), userAcct);

  //                  // Get Profile entity for logged in user profileId
  //                  ZNode.Libraries.DataAccess.Service.ProfileService profileService = new ZNode.Libraries.DataAccess.Service.ProfileService();
  //                  ZNode.Libraries.DataAccess.Entities.Profile profile = profileService.GetByProfileID(userAcct.ProfileID);

  //                  // Hold this profile object in the session state
  //                  HttpContext.Current.Session.Add("ProfileCache", profile);

  //                  return userAcct;
  //              }
  //              else
  //              {
  //                  // not authenticated so return null
  //                  return null;
  //              }
  //          }
  //          else
  //          {
  //              // Return the value from session
  //              if (!Equals(userAcct.BillingAddress , null) && string.IsNullOrEmpty(userAcct.BillingAddress.FirstName) &&
  //                  !Equals(userAcct.ShippingAddress , null) && string.IsNullOrEmpty(userAcct.ShippingAddress.FirstName))
  //              {
  //                  userAcct.SetAccountAddress();
  //              }

  //              return userAcct;
  //          }
  //      }

  //      /// <summary>
  //      /// Get the user by id
  //      /// </summary>
  //      /// <param name="UserId">User ID;s GUID</param>
  //      /// <returns>Returns the user details</returns>
  //      public Account GetByUserID(Guid UserId)
  //      {
  //          AccountService acctServ = new AccountService();
  //          TList<Account> accountList = acctServ.GetByUserID(UserId);

  //          if (!Equals(accountList , null))
  //          {
  //              AddressService addressService = new AddressService();
  //              TList<Address> addressList = addressService.GetByAccountID(accountList[0].AccountID);
  //              if (addressList.Count > 0)
  //              {
  //                  // Get top 1 billing address.
  //                  addressList.ApplyFilter(delegate (Address address) { return address.IsDefaultBilling == true; });

  //                  if (addressList.Count > 0)
  //                      this.BillingAddress = addressList[0];

  //                  // Get top 1 shipping address.
  //                  addressList.ApplyFilter(delegate (Address address) { return address.IsDefaultShipping == true; });

  //                  if (addressList.Count > 0)
  //                      this.ShippingAddress = addressList[0];
  //              }

  //              // Get account details.
  //              if (accountList.Count > 0)
  //                  return accountList[0];
  //          }

  //          return null;
  //      }

  //      /// <summary>
  //      /// Adds a new account record for the newly created user
  //      /// </summary>
  //      public new void AddUserAccount()
  //      {
  //          Profile _profile = null;

  //          // get default profile
  //          ProfileService ProfileService = new ProfileService();
  //          Domain currentDomain = ZNodeConfigManager.DomainConfig;

  //          PortalService portalService = new PortalService();
  //          Portal currentPortal = portalService.GetByPortalID(currentDomain.PortalID);
  //          _profile = ProfileService.GetByProfileID(currentPortal.DefaultAnonymousProfileID.GetValueOrDefault());

  //          // pre-set properties
  //          _Account.ActiveInd = true;
  //          _Account.CreateDte = System.DateTime.Now;
  //          _Account.UpdateDte = System.DateTime.Now;
  //          _Account.ProfileID = _profile.ProfileID;
  //          _Account.Email = this.EmailID;

  //          // Auto-assign Account number 
  //          if (!Equals(_profile.DefaultExternalAccountNo , null))
  //              this._Account.ExternalAccountNo = _profile.DefaultExternalAccountNo;

  //          this._Account.ParentAccountID = null;

  //          // add account
  //          AccountService acctServ = new AccountService();
  //          acctServ.Insert(_Account);
  //      }

  //      /// <summary>
  //      /// Get the account portal profile id
  //      /// </summary>
  //      /// <param name="AccountId">Account ID</param>
  //      /// <param name="PortalId">Portal ID for the account</param>
  //      /// <returns>Returns the profile ID</returns>
  //      public int GetAccountPortalProfileID(int AccountId, int PortalId)
  //      {
  //          // AccountProfile
  //          AccountHelper accountHelper = new AccountHelper();
  //          int profileID = accountHelper.GetCustomerProfile(AccountId, PortalId);

  //          return profileID;
  //      }

  //      /// <summary>
  //      /// Set the billing address in current user account session.
  //      /// </summary>
  //      /// <param name="addressId">Address Id</param>
		//public void SetBillingAddress(Address address)
  //      {
  //          if (!Equals(address , null))
  //          {
  //              this._BillingAddress.AddressID = address.AddressID;
  //              this._BillingAddress.FirstName = address.FirstName;
  //              this._BillingAddress.MiddleName = address.MiddleName;
  //              this._BillingAddress.LastName = address.LastName;
  //              this._BillingAddress.CompanyName = address.CompanyName;
  //              this._BillingAddress.Street = address.Street;
  //              this._BillingAddress.Street1 = address.Street1;
  //              this._BillingAddress.City = address.City;
  //              this._BillingAddress.StateCode = address.StateCode;
  //              this._BillingAddress.PostalCode = address.PostalCode;
  //              this._BillingAddress.CountryCode = address.CountryCode;
  //              this._BillingAddress.PhoneNumber = address.PhoneNumber;
  //              this._BillingAddress.IsDefaultBilling = address.IsDefaultBilling;
  //              this._BillingAddress.AccountID = address.AccountID;
  //              this._BillingAddress.IsDefaultShipping = address.IsDefaultShipping;
  //              this._BillingAddress.Name = address.Name;

  //              // Get the user account from session
  //              ZNodeUserAccount userAcct = (ZNodeUserAccount)HttpContext.Current.Session[ZNodeSessionKeyType.UserAccount.ToString()];

  //              if (!Equals(userAcct , null))
  //                  HttpContext.Current.Session.Add(ZNodeSessionKeyType.UserAccount.ToString(), userAcct);
  //          }
  //      }

  //      /// <summary>
  //      /// Set the shipping address in current user account session.
  //      /// </summary>
  //      /// <param name="addressId">Address Id</param>
		//public void SetShippingAddress(Address address)
  //      {
  //          if (!Equals(address , null))
  //          {
  //              this.ShippingAddress.AddressID = address.AddressID;
  //              this._ShippingAddress.FirstName = address.FirstName;
  //              this._ShippingAddress.MiddleName = address.MiddleName;
  //              this._ShippingAddress.LastName = address.LastName;
  //              this._ShippingAddress.CompanyName = address.CompanyName;
  //              this._ShippingAddress.Street = address.Street;
  //              this._ShippingAddress.Street1 = address.Street1;
  //              this._ShippingAddress.City = address.City;
  //              this._ShippingAddress.StateCode = address.StateCode;
  //              this._ShippingAddress.PostalCode = address.PostalCode;
  //              this._ShippingAddress.CountryCode = address.CountryCode;
  //              this._ShippingAddress.PhoneNumber = address.PhoneNumber;
  //              this._ShippingAddress.IsDefaultBilling = address.IsDefaultBilling;
  //              this._ShippingAddress.AccountID = address.AccountID;
  //              this._ShippingAddress.IsDefaultShipping = address.IsDefaultShipping;
  //              this._ShippingAddress.Name = address.Name;

  //              // Get the user account from session
  //              ZNodeUserAccount userAcct = (ZNodeUserAccount)HttpContext.Current.Session[ZNodeSessionKeyType.UserAccount.ToString()];

  //              if (!Equals(userAcct , null))
  //                  HttpContext.Current.Session.Add(ZNodeSessionKeyType.UserAccount.ToString(), userAcct);
  //          }
  //      }

  //      public bool CheckoutAddress()
  //      {
  //          bool isValid = false;

  //          if (!Equals(BillingAddress , null))
  //              isValid = CheckValidAddress(this.BillingAddress);

  //          if (!Equals(ShippingAddress , null))
  //              isValid = CheckValidAddress(this.ShippingAddress);

  //          return isValid;
  //      }

  //      /// <summary>
  //      /// Check valid address for useraccount
  //      /// </summary>
  //      /// <param name="address"></param>
  //      /// <returns>Returns boolean value true if address is valid and returns false otherwise</returns>
  //      public bool CheckValidAddress(ZNode.Libraries.DataAccess.Entities.Address address)
  //      {
  //          bool isValid = false;

  //          if (!Equals(address , null))
  //          {
  //              if (address.FirstName.Length > 0 && address.LastName.Length > 0 && address.Street.Length > 0 && address.City.Length > 0 && address.StateCode.Length > 0 && address.PostalCode.Length > 0 && !string.IsNullOrEmpty(address.PhoneNumber))
  //                  isValid = true;
  //          }

  //          return isValid;
  //      }
  //      #endregion

  //      #region Private Metods
  //      /// <summary>
  //      /// Set the account address
  //      /// </summary>
  //      public void SetAccountAddress()
  //      {
  //          Addresses = new AddressService().GetByAccountID(_Account.AccountID);
  //          if (Addresses.Any())
  //          {
  //              // Get top 1 billing address.
  //              this.SetBillingAddress(Addresses.FirstOrDefault(address => address.IsDefaultBilling));

  //              // Get top 1 shipping address.
  //              this.SetShippingAddress(Addresses.FirstOrDefault(address => address.IsDefaultShipping));
  //          }
  //      }
        #endregion
   }

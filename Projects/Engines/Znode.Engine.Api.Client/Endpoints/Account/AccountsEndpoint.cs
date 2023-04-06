namespace Znode.Engine.Api.Client.Endpoints
{
    //Configure the Endpoints used to create URLs for Account related APIs
    public class AccountsEndpoint : BaseEndpoint
    {
        #region Account
        //Get company account list.
        public static string GetAccountList() => $"{ApiRoot}/Account/list";

        //Get Company Account by .Account Id
        public static string GetAccount(int accountId) => $"{ApiRoot}/Account/{accountId}";

        //Get Company Account by Account Name
        public static string GetAccountByUserName(string accountName, int portalId) => $"{ApiRoot}/account/getaccountbyname/{accountName}/{portalId}";

        //Create Company Account
        public static string Create() => $"{ApiRoot}/Account/Create";

        //Update Company Account
        public static string Update() => $"{ApiRoot}/Account/Update";

        //Delete Company Account
        public static string Delete() => $"{ApiRoot}/Account/Delete";

        //Get Account by account Code
        public static string GetAccountByCode(int accountCode) => $"{ApiRoot}/account/getaccountbyaccountcode/{accountCode}";

        //Delete Account by account Code
        public static string DeleteAccountByCode(int accountCode) => $"{ApiRoot}/account/deleteaccountbyCode";

        #endregion

        #region Account Notes Endpoint
        //Create Account Note Endpoint.
        public static string CreateAccountNote() => $"{ApiRoot}/account/createaccountnote";

        //Get Account Note on the basis of Account Note id Endpoint.
        public static string GetAccountNote(int noteId) => $"{ApiRoot}/account/getaccountnote/{noteId}";

        //Update Account Note Endpoint.
        public static string UpdateAccountNote() => $"{ApiRoot}/account/updateaccountnote";

        //Get Account Note list Endpoint.
        public static string GetAccountNotes() => $"{ApiRoot}/account/getaccountnotes";

        //Delete Account Note Endpoint.
        public static string DeleteAccountNote() => $"{ApiRoot}/account/deleteaccountnote";
        #endregion

        #region Account Department Endpoint
        //Create Account Department Endpoint.
        public static string CreateAccountDepartment() => $"{ApiRoot}/account/createaccountdepartment";

        //Get Account Department on the basis of Account Department id Endpoint.
        public static string GetAccountDepartment(int departmentId) => $"{ApiRoot}/account/getaccountdepartment/{departmentId}";

        //Update Account Department Endpoint.
        public static string UpdateAccountDepartment() => $"{ApiRoot}/account/updateaccountdepartment";

        //Get Account Department list Endpoint.
        public static string GetAccountDepartments() => $"{ApiRoot}/account/getaccountdepartments";

        //Delete Account Department Endpoint.
        public static string DeleteAccountDepartment() => $"{ApiRoot}/account/deleteaccountdepartment";
        #endregion

        #region Address
        //Get address list.
        public static string GetAddressList() => $"{ApiRoot}/account/addresslist";

        //Create account address.
        public static string CreateAccountAddress() => $"{ApiRoot}/account/createaccountaddress";

        //Get account address.
        public static string GetAccountAddress() => $"{ApiRoot}/account/getaccountaddress";

        //Update account address.
        public static string UpdateAccountAddress() => $"{ApiRoot}/account/updateaccountaddress";

        //Delete account address.
        public static string DeleteAccountAddress() => $"{ApiRoot}/account/deleteaccountaddress";
        #endregion

        #region Associate Price
        //Endpoint to Associate Price List.
        public static string AssociatePriceList() => $"{ApiRoot}/account/associatepricelist";

        //Remove associated price lists from account endpoint.
        public static string UnAssociatePriceList() => $"{ApiRoot}/account/unassociatepricelist";

        //Get associated price list precedence data for Account endpoint.
        public static string GetAssociatedPriceListPrecedence() => $"{ApiRoot}/account/getassociatedpricelistprecedence";

        //Update associated price list precedence data for Account endpoint.
        public static string UpdateAssociatedPriceListPrecedence() => $"{ApiRoot}/account/updateassociatedpricelistprecedence";
        #endregion

        #region Account Order
        //Get user order list of account.
        public static string GetAccountUserOrderList(int accountId) => $"{ApiRoot}/account/getaccountuserorderlist/{accountId}";
        #endregion

        #region Account Profile

        //Get associated/Unassociated profile for account.
        public static string GetAssociatedUnAssociatedProfile() => $"{ApiRoot}/account/getassociatedunassociatedprofile";

        //Endpoint to Associate profile.
        public static string AssociateProfile() => $"{ApiRoot}/account/associateprofile";

        //Remove associated profiles from account.
        public static string UnAssociateProfile() => $"{ApiRoot}/account/unassociateprofile";
        #endregion

        #region Approver Level

        //Get approver level list
		public static string GetApproverLevelList() => $"{ApiRoot}/account/getapproverlevelList";

        //Create and update approver level
        public static string CreateApproverLevel() => $"{ApiRoot}/account/createapproverlevel";

        //Delete approval level list
        public static string DeleteApproverLevelList() => $"{ApiRoot}/account/deleteapproverlevel";
        
        //Get level list
        public static string GetLevelsList() => $"{ApiRoot}/account/getlevelslist";

        // Save permission setting.
        public static string SavePermissionSetting() => $"{ApiRoot}/account/savepermissionsetting";

        #endregion

        //Get parent account List
        public static string GetParentAccountList() => $"{ApiRoot}/account/getparentaccountlist";
    }
}

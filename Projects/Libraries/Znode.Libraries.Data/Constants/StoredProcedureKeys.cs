namespace Znode.Libraries.Data
{
    /// <summary>
    /// StoredProcedureKeys
    /// </summary>
    public struct StoredProcedureKeys
    {
        //Operatod
        public const string AND = "and";
        public const string OR = "or";
        public const string TildOperator = "";
        public const string TildEqualToOperator = "";

        //columns
        public const string ProductTypeId = "producttypeid";
        public const string ProductId = "productid";
        public const string PortalId = "portalId";
        public const string Portalid = "portalid";
        public const string AccountId = "accountid";
        public const string RoleName = "rolename";
        public const string CategoryId = "categoryid";
        public const string SkuId = "skuId";

        //Condition
        public const string RoleNameIsNull = "~~rolename~~ is null";
        public const string RoleNameIsNullWithAnd = " and ~~rolename~~ is null";
        public const string GetAccountId = "~~accountid~~={0}";
        public const string FranchiseRole = "FRANCHISE<br>";
        public const string VendorRole = "VENDOR<br>";
        public const string RejectionMessageFilterCondition = "~~PortalId~~={0} and ~~LocaleId~~={1}";
        //Order By 
        public const string sortKey = "sort";
        public const string sortDirKey = "sortDir";
        public const string defaultSortKey = "displayorder asc";
    }
}

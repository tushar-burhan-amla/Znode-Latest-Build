using System;

namespace Znode.Engine.Exceptions
{
    public static class ErrorCodes
    {
        public const Int32 ProfileNotPresent = 1000;
        public const Int32 MembershipError = 1001;
        public const Int32 UserNameUnavailable = 1002;
        public const Int32 LoginFailed = 1003;
        #region Znode Reset Password Link
        //Error Code for Account gets Locked
        public const Int32 AccountLocked = 1004;
        public const Int32 TwoAttemptsToAccountLocked = 1008;
        public const Int32 OneAttemptToAccountLocked = 1009;
        public const Int32 LockOutEnabled = 1010;

        //Error Codes for Reset Password Links
        public const Int32 ResetPasswordContinue = 2001;
        public const Int32 ResetPasswordLinkExpired = 2002;
        public const Int32 ResetPasswordTokenMismatch = 2003;
        public const Int32 ResetPasswordNoRecord = 2004;

        public const Int32 SEOUrlAlreadyExists = 2005;
        public const Int32 ErrorSendResetPasswordLink = 2006;
        public const Int32 ErrorResetPassword = 2007;

        public const Int32 DefaultBillingAddressNoSet = 3001;
        public const Int32 DefaultShippingNoSet = 3002;
        public const Int32 NoShippingFacility = 3003;

        public const Int32 IsAssociatedSupplier = 4001;
        #endregion

        public const Int32 CustomerAccountError = 1005;
        public const Int32 ExportError = 1006;
        public const Int32 ImportError = 1007;
        public const Int32 ZnodeEncryptionError = 5001;

        // Error codes for general exception
        public const Int32 NullModel = 1;
        public const Int32 AlreadyExist = 2;
        public const Int32 AtLeastSelectOne = 3;
        public const Int32 AssociationDeleteError = 4;
        public const Int32 InvalidData = 5;
        public const Int32 NotFound = 6;
        public const Int32 NotPermitted = 7;
        public const Int32 IdLessThanOne = 8;
        public const Int32 ExceptionalError = 9;
        public const Int32 RestrictSystemDefineDeletion = 10;
        public const Int32 SKUAlreadyExist = 11;
        public const Int32 InternalItemNotUpdated = 12;
        public const Int32 NotDeleteActiveRecord = 13;
        public const Int32 URLRedirectCreationFailed = 14;
        public const Int32 CreationFailed = 15;
        public const Int32 ProcessingFailed = 16;
        public const Int32 DuplicateQuantityError = 17;
        public const Int32 DefaultDataDeletionError = 18;
        public const Int32 DynamicReportSoapException = 19;
        public const Int32 DynamicReportGeneralException = 20;
        public const Int32 AssociationUpdateError = 21;
        public const int DuplicateSearchIndexName = 22;
        public const Int32 OutOfStockException = 23;
        public const Int32 MinAndMaxSelectedQuantityError = 24;
        public const Int32 EmailTemplateDoesNotExists = 25;
        public const Int32 AllowedTerritories = 26;
        public const Int32 SetDefaultDataError = 27;
        public const Int32 MongoAuthentication = 28;
        public const Int32 FileNotFound = 28;
        public const Int32 CategoryPublishError = 33;
        public const Int32 ProductPublishError = 34;
        public const Int32 ProductSeoPublishError = 35;
        public const Int32 CategorySeoPublishError = 36;

        public const Int32 IsUsed = 29;
        public const Int32 DuplicateProductKey = 30;
        public const Int32 WebAPIKeyNotFound = 31;
        public const Int32 UnAuthorized = 32;
        public const Int32 NonDefaultUrlDeleteError = 36;
        public const Int32 StoreNotPublished = 6001;
        public const Int32 InvalidCSV = 6002;
        public const Int32 DisabledCMSPageResults = 6003;

        #region Publish related error codes
        public const Int32 GenericExceptionDuringPublish = 7000;
        public const Int32 InvalidEntityPassedDuringPublish = 7001;
        public const Int32 EntityNotFoundDuringPublish = 7002;
        public const Int32 StoreNotPublishedForAssociatedEntity = 7003;
        public const Int32 SQLExceptionDuringPublish = 7004;
        public const Int32 EntityExceptionDuringPublish = 7005;
        public const Int32 MinMaxQtyError = 7008;
        public const Int32 UpdateCustomFieldError = 7009;
        #endregion

        #region Misconfiguration related error codes
        public const Int32 InvalidDomainConfiguration = 8001;
        public const Int32 InvalidSqlConfiguration = 8002;
        public const Int32 InvalidMongoConfiguration = 8003;
        public const Int32 InvalidElasticSearchConfiguration = 8004;

        public const Int32 InvalidZnodeLicense = 8888;
        #endregion

        #region Account Verifications Error Codes
        public const Int32 EmailVerification = 9000;
        public const Int32 AdminApproval = 9001;
        public const Int32 AdminApprovalLoginFail = 9002;
        #endregion

        public const Int32 NotOrderEligibleForReturn = 301;

        #region Payment error codes
        public const Int32 PaymentCaptureError = 37;
        public const Int32 PaymentRefundError = 38;
        #endregion

        public const Int32 ProductNotFound = 39;

        #region TradeCentric error codes
        public const Int32 ErrorUnAuthorized = 401;
        public const Int32 Success = 200;
        #endregion
    }
}

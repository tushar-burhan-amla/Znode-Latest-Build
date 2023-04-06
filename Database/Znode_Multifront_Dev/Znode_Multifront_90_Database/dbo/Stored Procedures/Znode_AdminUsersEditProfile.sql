CREATE PROCEDURE [dbo].[Znode_AdminUsersEditProfile]  
(  
	@LoggedInUserId int  
)  
AS  
/*  
Summary: This SP is used to get User Details  
EXEC Znode_AdminUsersEditProfile @LoggedInUserId = 1006  
*/  
BEGIN  
BEGIN TRY  
SET NOCOUNT ON;  
 
	SELECT UserId, AspNetUserId, FirstName, LastName, MiddleName, CustomerPaymentGUID,BudgetAmount,Email, PhoneNumber,EmailOptIn,  
		ReferralStatus, ReferralCommission, ReferralCommissionTypeId, IsActive, ExternalId, IsShowMessage, CreatedBy,  
		CreatedDate, ModifiedBy, ModifiedDate, Custom1, Custom2, Custom3, Custom4, Custom5, IsVerified,  
		MediaId, UserVerificationType, UserName,AccountId
	FROM ZnodeUser a  
	WHERE a.UserId = @LoggedInUserId  
   
END TRY  
BEGIN CATCH  
	DECLARE @ERROR_PROCEDURE VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_AdminUsersEditProfile @LoggedInUserId='+cast(@LoggedInUserId as varchar(10));  
	EXEC Znode_InsertProcedureErrorLog  
	@ProcedureName    = 'Znode_AdminUsersEditProfile',  
	@ErrorInProcedure = @ERROR_PROCEDURE,  
	@ErrorMessage     = @ErrorMessage,  
	@ErrorLine        = @ErrorLine,  
	@ErrorCall        = @ErrorCall;  
END CATCH;  
END;

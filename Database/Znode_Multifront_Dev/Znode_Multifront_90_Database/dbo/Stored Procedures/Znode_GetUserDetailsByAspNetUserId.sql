CREATE PROCEDURE [dbo].[Znode_GetUserDetailsByAspNetUserId]
(
	@AspNetUserId NVARCHAR(256),
	@PortalId INT
)
AS
BEGIN
BEGIN TRY
	SET NOCOUNT ON;
	Declare @UserId INT,@AcountId INT, @UserPortalStatus BIT

	SELECT @UserId = UserId,@AcountId=AccountId FROM ZnodeUser 
	WHERE AspNetUserId = @AspNetUserId
	
	--To get the user data
	SELECT * FROM ZnodeUser 
	where UserId = @UserId
	
	--To get the users addresses
	SELECT ZUA.UserAddressId,ZUA.UserId,ZA.AddressId,ZA.FirstName,ZA.LastName,ZA.DisplayName,ZA.CompanyName,ZA.Address1,ZA.Address2,ZA.Address3,
		   ZA.CountryName,ZA.StateName,ZA.CityName,ZA.PostalCode,ZA.PhoneNumber,ZA.Mobilenumber,ZA.AlternateMobileNumber,
		   ZA.FaxNumber,ZA.IsDefaultBilling,ZA.IsDefaultShipping,ZA.IsActive,ZA.ExternalId,ZA.IsShipping,ZA.IsBilling,ZA.EmailAddress 
	FROM ZnodeAddress ZA
	INNER JOIN ZnodeUserAddress ZUA ON ZUA.AddressId = ZA.AddressId 
	WHERE ZUA.UserId = @UserId
	Union All
	--To get the users account addresses
	SELECT ZUA.AccountAddressId,@UserId as UserId,ZA.AddressId,ZA.FirstName,ZA.LastName,ZA.DisplayName,ZA.CompanyName,ZA.Address1,ZA.Address2,ZA.Address3,
		   ZA.CountryName,ZA.StateName,ZA.CityName,ZA.PostalCode,ZA.PhoneNumber,ZA.Mobilenumber,ZA.AlternateMobileNumber,
		   ZA.FaxNumber,ZA.IsDefaultBilling,ZA.IsDefaultShipping,ZA.IsActive,ZA.ExternalId,ZA.IsShipping,ZA.IsBilling,ZA.EmailAddress 
	FROM ZnodeAddress ZA
	INNER JOIN ZnodeAccountAddress ZUA ON ZUA.AddressId = ZA.AddressId 
	WHERE ZUA.AccountId = @AcountId 
	
	--To get the users whishlists
	SELECT UserWishListId,UserId,SKU,WishListAddedDate,AddOnSKUs 
	FROM ZnodeUserWishList
	WHERE UserId = @UserId
	
	--To get the users profiles	
	SELECT ZP.ProfileId,ZP.ProfileName,ZP.ShowOnPartnerSignup,ZP.Weighting,ZP.TaxExempt,ZP.DefaultExternalAccountNo,ZP.ParentProfileId, ZUP.IsDefault
	FROM ZnodeProfile ZP
	INNER JOIN ZnodeUserProfile ZUP ON ZP.ProfileId = ZUP.ProfileId 
	WHERE ZUP.UserId = @UserId

	IF EXISTS(SELECT * FROM ZnodeUserPortal WHERE (PortalId = @PortalId OR PortalId IS NULL) AND UserId = @UserId)
		SET @UserPortalStatus = 1
	ELSE
		SET @UserPortalStatus = 0

	Select @UserPortalStatus as UserPortalStatus
END TRY 
BEGIN CATCH 

	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetUserDetailsByAspNetUserId @AspNetUserId = '+CAST(@AspNetUserId AS VARCHAR(200))+', @PortalId = '+CAST(@PortalId AS VARCHAR(200))+',@UserPortalStatus ='+CAST(@UserPortalStatus AS VARCHAR(10));
    EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_GetUserDetailsByAspNetUserId',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;

END CATCH 
END
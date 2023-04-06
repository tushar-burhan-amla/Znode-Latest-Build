CREATE PROCEDURE [dbo].[Znode_UpdateUserProfileBasedOnAccountProfile]
(
	@AccountId Int,
	@UserIds Varchar(2000),
	@Status bit = 0 out 
)
as
BEGIN
	SET NOCOUNT ON
	DECLARE @UserIds_Tbl TABLE(UserId int)
	DECLARE @GetDate DATETIME =dbo.Fn_GetDate()
	DECLARE @ProfileIdByAccountId INT;

	BEGIN TRY
	BEGIN TRAN
		INSERT INTO @UserIds_Tbl(UserId)
		SELECT Item FROM dbo.Split(@UserIds,',')

		--Getting Default Profile Id Based on Account Id
		SELECT @ProfileIdByAccountId = ProfileId FROM ZnodeAccountProfile WHERE AccountId = @AccountId and IsDefault = 1;

		 IF isnull(@UserIds,'0') <> '0' or  ISNULL(@UserIds,'') <> ''
		BEGIN
			
			DELETE FROM ZnodeUserProfile  WHERE EXISTS (SELECT TOP 1 1 FROM @UserIds_Tbl UIT
			WHERE UIT.UserId = ZnodeUserProfile.UserId);

			INSERT INTO ZnodeUserProfile(ProfileId,UserId,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT @ProfileIdByAccountId,UserId,1,@AccountId,@GetDate,@AccountId,@GetDate
			FROM @UserIds_Tbl;
		
		END
		IF ISNULL(@AccountId,0)<>0
		BEGIN
			DECLARE @UserRoleId NVARCHAR(128)=(SELECT TOP 1 Id FROM AspNetRoles WHERE Name='User');
			DECLARE @CustomerRoleId NVARCHAR(128)=(SELECT TOP 1 Id FROM AspNetRoles WHERE Name='Customer');

			UPDATE ANUR
			SET ANUR.RoleId=@UserRoleId
			FROM ZnodeUser ZU 
			INNER JOIN AspNetUsers ANU on ZU.AspNetUserId=ANU.Id
			INNER JOIN AspNetUserRoles ANUR ON ANU.Id=ANUR.UserId
			WHERE ZU.UserId IN (SELECT UserId FROM @UserIds_Tbl)
			AND ANUR.RoleId=@CustomerRoleId;
		END

		SET @Status = 1;
		SELECT 1 AS ID,@Status AS STATUS;    
	COMMIT TRAN
	END TRY
	BEGIN CATCH
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_UpdateUserProfileBasedOnAccountProfile @AccountId='+CAST(@AccountId AS VARCHAR(50))+',@UserIds='+CAST(@UserIds AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,@Status AS STATUS;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName    = 'Znode_UpdateUserProfileBasedOnAccountProfile',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage     = @ErrorMessage,
				@ErrorLine        = @ErrorLine,
				@ErrorCall        = @ErrorCall;
			ROLLBACK TRAN
	END CATCH
END

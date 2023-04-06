
CREATE PROCEDURE [dbo].[Znode_ReapirAdminUser](@IsCreateAdminUser bit, @IsUpdatePassword bit)
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Create Admin user and update password
	-- Unit Testing : 
	--------------------------------------------------------------------------------------

BEGIN
	BEGIN TRAN A;
	BEGIN TRY
		DECLARE @MessageDisplay nvarchar(100), @SSQL nvarchar(max),@AspNetZnodeUserId nvarchar(256),@ASPNetUsersId nvarchar(256),
		@PasswordHash nvarchar(max),@SecurityStamp nvarchar(max),@RoleId nvarchar(256),@IsAllowGlobalLevelUserCreation nvarchar(10),
		@GetDate datetime ,@UserId int,@AddressId int,@Status bit,@UserName  Nvarchar(512), @PortalId int ,@NewUserId int 
		
		SET  @UserName = 'admin@znode1.com'
		SET  @PortalId  = null 
		SET  @SecurityStamp = N'0wVYOZNK4g4kKz9wNs-UHw2'
		SET  @PasswordHash = N'APy4Tm1KbRG6oy7h3r85UDh/lCW4JeOi2O2Mfsb3OjkpWTp1YfucMAvvcmUqNaSOlA==';
		SELECT  @RoleId  = Id from AspNetRoles where   NAME = 'Admin'  
		SET  @GetDate = [dbo].[Fn_GetDate]()
		SET  @UserId = 2 
		SET @ASPNetUsersId = newid()

		--Select ANR.Name, ANZU.USername  ,ANU.* from AspNetZnodeUser ANZU INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
		--INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId	
		--INNER JOIN AspNetUserRoles ANUR ON ZU.AspNetUserId = ANUR.UserId
		--inner join AspNetRoles ANR ON ANUR.RoleId = ANUR.RoleId
		--where ANR.Name = 'Admin' and ANZU.Username = 'admin@znode.com'
		----where Isnull(ANZU.PortalId,0) = Isnull(@PortalId ,0)
		if @IsCreateAdminUser =1 
		Begin 
				IF NOT Exists (select TOP 1 1 from AspNetZnodeUser where UserName = @UserName ) 
				Begin
						DECLARE @InsertedAspNetZnodeUser TABLE (AspNetZnodeUserId nvarchar(256) ,UserName nvarchar(512),PortalId int )
						DECLARE @InsertedASPNetUsers TABLE (Id nvarchar(256) ,UserName nvarchar(512))
						DECLARE @InsertZnodeUser TABLE (UserId int)
				
						Insert into AspNetZnodeUser (AspNetZnodeUserId, UserName, PortalId)		
						OUTPUT INSERTED.AspNetZnodeUserId, INSERTED.UserName, INSERTED.PortalId	INTO  @InsertedAspNetZnodeUser 			 
						Select NEWID(),@UserName, @PortalId 
				 
						--where Not Exists (Select TOP 1 1  from AspNetZnodeUser ANZ where Isnull(ANZ.PortalId,0) = Isnull(@PortalId,0) AND ANZ.UserName = IC.UserName)
						
						INSERT INTO ASPNetUsers (Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,
						LockoutEndDateUtc,LockOutEnabled,AccessFailedCount,PasswordChangedDate,UserName)
						output inserted.Id, inserted.UserName into @InsertedASPNetUsers
						SELECT @ASPNetUsersId, NULL Email,0 ,@PasswordHash,@SecurityStamp,NULL PhoneNumber,0,0,NULL LockoutEndDateUtc,1 LockoutEnabled,
						0,@GetDate,AspNetZnodeUserId from @InsertedAspNetZnodeUser  
				
						INSERT INTO  ZnodeUser(AspNetUserId,FirstName,LastName,MiddleName,CustomerPaymentGUID,BudgetAmount,Email,PhoneNumber,EmailOptIn,
						IsActive,ExternalId, CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
						--OUTPUT Inserted.UserId into @InsertZnodeUser
						SELECT IANU.Id,'' FirstName,'' LastName,'' MiddleName,null CustomerPaymentGUID,null BudgetAmount,null Email,null PhoneNumber,0 EmailOptIn,1 IsActive,null ExternalId, @UserId,@Getdate,@UserId,@Getdate
						from @InsertedASPNetUsers IANU 

						SET @NewUserId = @@Identity 
				  	     
						INSERT INTO AspNetUserRoles (UserId,RoleId)  Select @ASPNetUsersId, @RoleID 

						INSERT INTO ZnodeUserPortal (UserId,PortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate) 
						SELECT @NewUserId, @PortalId , @UserId,@Getdate,@UserId,@Getdate --from @InsertZnodeUser

						Insert into ZnodeAddress (FirstName,LastName,DisplayName,Address1,Address2,Address3,CountryName,StateName,
						CityName,PostalCode,PhoneNumber,Mobilenumber,AlternateMobileNumber,FaxNumber,IsDefaultBilling,IsDefaultShipping,IsActive,
						ExternalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
						Select '' FirstName, '' LastName, '' DisplayName,'' Address1,'' Address2,'' Address3, '' CountryName, '' StateName,
						'' CityName,'' PostalCode,'' PhoneNumber, '' Mobilenumber, '' AlternateMobileNumber,'' FaxNumber, 1 IsDefaultBilling, 0 IsDefaultShipping,1 IsActive,
						'' ExternalId,@UserId CreatedBy,@Getdate CreatedDate,@UserId ModifiedBy,@Getdate ModifiedDate

						SET @AddressId = @@Identity
						Insert into ZnodeUserAddress(UserId,AddressId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
						SElect @AddressId , @NewUserId , @UserId, @Getdate, @UserId, @Getdate from @InsertZnodeUser

						SET @Status = 1;
						select @Status 
						Select 'New user is created by name  : ' + @UserName	+ ' and Password : admin12345'  
							
						--Select ANZU.PortalId ,ANR.Name, ANZU.USername  ,ANU.* from AspNetZnodeUser ANZU INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
						--INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId	
						--INNER JOIN AspNetUserRoles ANUR ON ZU.AspNetUserId = ANUR.UserId
						--inner join AspNetRoles ANR ON ANUR.RoleId = ANUR.RoleId
						--where ANR.Name = 'Admin' and ANZU.Username = 'admin@znode1.com'

				End
				Else 
				Begin
				   Select 'User Already Exists : ' + @UserName
				End
		END
		Else If @IsUpdatePassword =1 
		Begin

				--Select ANR.Name, ANZU.USername  ,ANU.* from AspNetZnodeUser ANZU 
				--INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
				--INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId	
				--INNER JOIN AspNetUserRoles ANUR ON ZU.AspNetUserId = ANUR.UserId
				--inner join AspNetRoles ANR ON ANUR.RoleId = ANUR.RoleId
				--where ANR.Name = 'Admin' and ANZU.Username = 'admin@znode.com' 
				--AND Isnull(ANZU.PortalId,0) = Isnull(@portalId,0)

				Update ANU SET    
						 ANU.[PasswordHash] = @PasswordHash
						,ANU.[SecurityStamp]= @SecurityStamp
				   	    ,ANU.LockoutEndDateUtc = NULL 
						,ANU.PasswordChangedDate= @Getdate
				--Select ANR.Name, ANZU.USername  ,ANU.* 
				FROM AspNetZnodeUser ANZU 
				INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
				INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId	
				INNER JOIN AspNetUserRoles ANUR ON ZU.AspNetUserId = ANUR.UserId
				inner join AspNetRoles ANR ON ANUR.RoleId = ANUR.RoleId
				where ANR.Name = 'Admin' and ANZU.Username = 'admin@znode.com' 
				AND Isnull(ANZU.PortalId,0) = Isnull(@portalId,0)

				 DELETE from ZnodePasswordLog where userid in 
				 (Select ANU.Id 
				FROM AspNetZnodeUser ANZU 
				INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
				INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId	
				INNER JOIN AspNetUserRoles ANUR ON ZU.AspNetUserId = ANUR.UserId
				inner join AspNetRoles ANR ON ANUR.RoleId = ANUR.RoleId
				where ANR.Name = 'Admin' and ANZU.Username = 'admin@znode.com' 
				AND Isnull(ANZU.PortalId,0) = Isnull(@portalId,0))
				-------Reset Password ( admin12345 )
				SET @Status = 1;
				select @Status 
				Select  'Updated Password : admin12345'
		END
		-- 'End'
		COMMIT TRAN A;
	END TRY
	BEGIN CATCH
	SET @Status = 0;
		SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE();
		ROLLBACK TRAN A;
	END CATCH;
END;
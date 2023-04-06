CREATE PROCEDURE [dbo].[Znode_ImportB2BCustomer](
	  @TableName nvarchar(100), @Status bit OUT, @UserId int, @ImportProcessLogId int, @NewGUId nvarchar(200), @LocaleId int= 0,@PortalId int ,@CsvColumnString nvarchar(max))
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Import SEO Details
	
	-- Unit Testing : 
	--------------------------------------------------------------------------------------

BEGIN
	BEGIN TRAN A;
	BEGIN TRY
		DECLARE @MessageDisplay nvarchar(100), @SSQL nvarchar(max),@AspNetZnodeUserId nvarchar(256),@ASPNetUsersId nvarchar(256),
		@PasswordHash nvarchar(max),@SecurityStamp nvarchar(max),@RoleId nvarchar(256),@IsAllowGlobalLevelUserCreation nvarchar(10),
		@AccountId int

		SET @SecurityStamp = '0wVYOZNK4g4kKz9wNs-UHw2'
		SET @PasswordHash = 'APy4Tm1KbRG6oy7h3r85UDh/lCW4JeOi2O2Mfsb3OjkpWTp1YfucMAvvcmUqNaSOlA==';
		SELECT  @RoleId  = Id FROM AspNetRoles WHERE   NAME = 'User'  
		
		SELECT @AccountId = AccountId FROM ZnodeUser ZU WHERE UserId = @UserId

		Select @IsAllowGlobalLevelUserCreation = FeatureValues from ZnodeGlobalsetting where FeatureName = 'AllowGlobalLevelUserCreation'

		DECLARE @GetDate datetime= dbo.Fn_GetDate();
		-- Retrive RoundOff Value from global setting 

		-- Three type of import required three table varible for product , category and brand
		CREATE TABLE #InsertCustomer 
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int, UserName nvarchar(512) ,FirstName	nvarchar(200),
			LastName nvarchar(200), BudgetAmount	numeric,Email	nvarchar(100),PhoneNumber	nvarchar(100),
		    EmailOptIn	bit	,ReferralStatus	nvarchar(40),IsActive	bit	,ExternalId	nvarchar(max), GUID NVARCHAR(400)
		);
		

		SET @SSQL = 'INSERT INTO #InsertCustomer( RowNumber,'+@CsvColumnString+',GUID ) 
					 SELECT RowNumber,' + @CsvColumnString + ',GUID FROM '+@TableName+'';

		EXEC sys.sp_sqlexec @SSQL
	
	    -- start Functional Validation 
		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
				SELECT '104', 'UserName', UserName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
				FROM #InsertCustomer AS ii
				WHERE ii.UserName not like '%_@_%_.__%' 
				
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
				SELECT '30', 'UserName', UserName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
				FROM #InsertCustomer AS ii
				WHERE ii.UserName in 
				(SELECT UserName  FROM #InsertCustomer group by UserName  having count(*) > 1 )

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
				SELECT '104', 'Email', Email, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
				FROM #InsertCustomer AS ii
				WHERE ii.Email not like '%_@_%_.__%'

		--Note : Content page import is not required 
		
		-- End Function Validation 	
		-----------------------------------------------
		--- Delete Invalid Data after functional validatin  

		DELETE FROM #InsertCustomer
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber is not null 
		);
		
		-- Update Record count in log 
        DECLARE @FailedRecordCount BIGINT
		DECLARE @SuccessRecordCount BIGINT
		SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;
		Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM #InsertCustomer
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount,
			TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))
		WHERE ImportProcessLogId = @ImportProcessLogId;
		-- End

		-- Insert Product Data 

		DECLARE @InsertedAspNetZnodeUser TABLE (AspNetZnodeUserId nvarchar(256) ,UserName nvarchar(512),PortalId int )
		DECLARE @InsertedASPNetUsers TABLE (Id nvarchar(256) ,UserName nvarchar(512))
		DECLARE @InsertZnodeUser TABLE (UserId int,AspNetUserId nvarchar(256) )

		UPDATE ANU SET 
		ANU.PhoneNumber	= IC.PhoneNumber
		from AspNetZnodeUser ANZU INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
		INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId	
		INNER JOIN #InsertCustomer IC ON ANZU.UserName = IC.UserName 
		where Isnull(ANZU.PortalId,0) = Isnull(@PortalId ,0)

		UPDATE ZU SET 
		ZU.FirstName	= IC.FirstName,
		ZU.LastName		= IC.LastName,
		--ZU.MiddleName	= IC.MiddleName,
		ZU.BudgetAmount = IC.BudgetAmount,
		ZU.Email		= IC.Email,
		ZU.PhoneNumber	= IC.PhoneNumber,
		ZU.EmailOptIn	= Isnull(IC.EmailOptIn,0),
		ZU.IsActive		= 1,--IC.IsActive,
		ZU.AccountId    = @AccountId
		--ZU.ExternalId = ExternalId
		from AspNetZnodeUser ANZU INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
		INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId	
		INNER JOIN #InsertCustomer IC ON ANZU.UserName = IC.UserName 
		where Isnull(ANZU.PortalId,0) = Isnull(@PortalId ,0)

		Insert into AspNetZnodeUser (AspNetZnodeUserId, UserName, PortalId)		
		OUTPUT INSERTED.AspNetZnodeUserId, INSERTED.UserName, INSERTED.PortalId	INTO  @InsertedAspNetZnodeUser 			 
		Select NEWID(),IC.UserName, @PortalId FROM #InsertCustomer IC 
		where Not Exists (Select TOP 1 1  from AspNetZnodeUser ANZ where Isnull(ANZ.PortalId,0) = Isnull(@PortalId,0) AND ANZ.UserName = IC.UserName)

		INSERT INTO ASPNetUsers (Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,
		LockoutEndDateUtc,LockOutEnabled,AccessFailedCount,PasswordChangedDate,UserName)
		output inserted.Id, inserted.UserName into @InsertedASPNetUsers
		SELECT NewId(), Email,0 ,@PasswordHash,@SecurityStamp,PhoneNumber,0,0,NULL LockoutEndDateUtc,A.IsActive As LockoutEnabled,
		0,@GetDate,AspNetZnodeUserId from #InsertCustomer A INNER JOIN @InsertedAspNetZnodeUser  B 
		ON A.UserName = B.UserName
				
		INSERT INTO  ZnodeUser(AccountId, AspNetUserId,FirstName,LastName,CustomerPaymentGUID,Email,PhoneNumber,EmailOptIn,
		IsActive,ExternalId, CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		OUTPUT Inserted.UserId, Inserted.AspNetUserId into @InsertZnodeUser
		SELECT @AccountId, IANU.Id AspNetUserId ,IC.FirstName,IC.LastName,null CustomerPaymentGUID,IC.Email
		,IC.PhoneNumber,Isnull(IC.EmailOptIn,0), 1 as IsActive,--IC.IsActive,
		IC.ExternalId, @UserId,@Getdate,@UserId,@Getdate
		FROM #InsertCustomer IC Inner join 
		@InsertedAspNetZnodeUser IANZU ON IC.UserName = IANZU.UserName  INNER JOIN 
		@InsertedASPNetUsers IANU ON IANZU.AspNetZnodeUserId = IANU.UserName 
				  	     
		INSERT INTO AspNetUserRoles (UserId,RoleId)  Select AspNetUserId, @RoleID from @InsertZnodeUser 
		INSERT INTO ZnodeUserPortal (UserId,PortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate) 
		SELECT UserId, @PortalId , @UserId,@Getdate,@UserId,@Getdate from @InsertZnodeUser
				
		Declare @ProfileId  int 
		select TOP 1 @ProfileId   =  ProfileId from ZnodePortalprofile where Portalid = @PortalId and IsDefaultRegistedProfile=1

		insert into ZnodeUserProfile (ProfileId,UserId,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT @ProfileId  , UserId, 1 , @UserId,@Getdate,@UserId,@Getdate from @InsertZnodeUser

		DECLARE @GlobalAttributeDetail TABLE
		( 
		GlobalAttributeId int, AttributeTypeName varchar(300), AttributeCode varchar(300), SourceColumnName nvarchar(600), IsRequired bit 
		);

		INSERT INTO @GlobalAttributeDetail ( GlobalAttributeId, AttributeTypeName, AttributeCode, SourceColumnName, IsRequired )
		SELECT ZGA.GlobalAttributeId, ZAT.AttributeTypeName, ZGA.AttributeCode, ZGA.AttributeCode SourceColumnName, ZGA.IsRequired
		from tempdb.sys.columns a
		INNER JOIN ZnodeGlobalAttribute ZGA ON a.Name = ZGA.AttributeCode
		INNER JOIN ZnodeAttributeType ZAT ON ZGA.AttributeTypeId = ZAT.AttributeTypeId
		WHERE  object_id = object_id(@TableName)

		DECLARE @AttributeTypeName NVARCHAR(10), @AttributeCode NVARCHAR(300), @GlobalAttributeId INT, @IsRequired BIT, @SourceColumnName NVARCHAR(600), @NewProductId INT, @PimAttributeValueId INT, @SQLQuery varchar(max); 

		DECLARE  @UserDetail TABLE (  UserId int, GlobalAttributeId int , AttributeValue varchar(max), LocaleId int, RowNumber int)
			
		DECLARE Cr_AttributeDetails CURSOR LOCAL FAST_FORWARD
		FOR SELECT GlobalAttributeId, AttributeTypeName, AttributeCode, SourceColumnName, IsRequired FROM @GlobalAttributeDetail  WHERE ISNULL(SourceColumnName, '') <> '';
		OPEN Cr_AttributeDetails;
		FETCH NEXT FROM Cr_AttributeDetails INTO @GlobalAttributeId, @AttributeTypeName, @AttributeCode, @SourceColumnName, @IsRequired;
		WHILE @@FETCH_STATUS = 0
		BEGIN

			SET @NewProductId = 0;
			SET @SQLQuery = 'SELECT ZU.UserId ,'''+CONVERT(VARCHAR(100), @GlobalAttributeId)+''' GlobalAttributeId , TN.'+@SourceColumnName+' as AttributeValue, '+CONVERT(VARCHAR(100), @LocaleId)+' LocaleId
							, RowNumber FROM '+@TableName+' TN
							INNER JOIN AspNetZnodeUser ANZU ON TN.UserName = ANZU.UserName
							INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
							INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId ';
				--print @SQLQuery	

			INSERT INTO @UserDetail(  UserId, GlobalAttributeId, AttributeValue, LocaleId, RowNumber )
			EXEC sys.sp_sqlexec @SQLQuery;
			FETCH NEXT FROM Cr_AttributeDetails INTO @GlobalAttributeId, @AttributeTypeName, @AttributeCode, @SourceColumnName, @IsRequired;
		END;
		CLOSE Cr_AttributeDetails;
		DEALLOCATE Cr_AttributeDetails;
			
		-----GLOBAL ATTRIBUTE USER INSERT
		INSERT INTO ZnodeUserGlobalAttributeValue ( UserId,	GlobalAttributeId,	GlobalAttributeDefaultValueId,	AttributeValue,	CreatedBy,	CreatedDate,	ModifiedBy,	ModifiedDate )
		SELECT UserId, GlobalAttributeId ,NULL, NULL, @UserId, @GetDate, @UserId , @GetDate
		FROM @UserDetail UD
		where isnull(AttributeValue,'') <> ''
		AND NOT EXISTS (select * from ZnodeUserGlobalAttributeValue GAV where UD.UserId = GAV.UserId and UD.GlobalAttributeId = GAV.GlobalAttributeId )
				
		UPDATE ZnodeUserGlobalAttributeValueLocale set AttributeValue = UD.AttributeValue, ModifiedBy = @UserId,	ModifiedDate = @GetDate, GlobalAttributeDefaultValueId = GADV.GlobalAttributeDefaultValueId
		FROM ZnodeUserGlobalAttributeValueLocale UGAVL
		INNER JOIN ZnodeUserGlobalAttributeValue UGAV on UGAV.UserGlobalAttributeValueId = UGAVL.UserGlobalAttributeValueId 
		INNER JOIN @UserDetail UD ON  UGAV.UserId = UD.UserId and UGAV.GlobalAttributeId = UD.GlobalAttributeId
		LEFT JOIN ZnodeGlobalAttributeDefaultValue GADV ON GADV.GlobalAttributeId = UD.GlobalAttributeId AND UD.AttributeValue = GADV.AttributeDefaultValueCode
				
		INSERT INTO ZnodeUserGlobalAttributeValueLocale( UserGlobalAttributeValueId,	LocaleId,	AttributeValue,	CreatedBy,	CreatedDate,	ModifiedBy,	ModifiedDate, GlobalAttributeDefaultValueId )
		SELECT UGAV.UserGlobalAttributeValueId, UD.LocaleId , UD.AttributeValue, @UserId, @GetDate, @UserId , @GetDate, GADV.GlobalAttributeDefaultValueId
		from ZnodeUserGlobalAttributeValue UGAV 
		INNER JOIN @UserDetail UD ON  UGAV.UserId = UD.UserId and UGAV.GlobalAttributeId = UD.GlobalAttributeId
		LEFT JOIN ZnodeGlobalAttributeDefaultValue GADV ON GADV.GlobalAttributeId = UD.GlobalAttributeId AND UD.AttributeValue = GADV.AttributeDefaultValueCode
		WHERE isnull(UD.AttributeValue,'') <> ''
		AND NOT EXISTS ( select * from ZnodeUserGlobalAttributeValueLocale UGAVL where UGAV.UserGlobalAttributeValueId = UGAVL.UserGlobalAttributeValueId)

		SET @GetDate = dbo.Fn_GetDate();
		--Updating the import process status
		UPDATE ZnodeImportProcessLog
		SET Status = CASE WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 4 )
							WHEN ISNULL(@FailedRecordCount,0) = 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 2 )
							WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) = 0 THEN dbo.Fn_GetImportStatus( 3 )
						END, 
			ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		COMMIT TRAN A;
	END TRY
	BEGIN CATCH
	ROLLBACK TRAN A;

		SET @Status = 0;
		SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE();

		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportB2BCustomer @TableName = '+CAST(@TableName AS VARCHAR(max)) +',@Status='+ CAST(@Status AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(max)) +',@PortalId='+CAST(@PortalId AS VARCHAR(max)) +',@CsvColumnString='+CAST(@CsvColumnString AS VARCHAR(max));


		---Import process updating fail due to database error
		UPDATE ZnodeImportProcessLog
		SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		---Loging error for Import process due to database error
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '93', '', '', @NewGUId,  @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId

		--Updating total and fail record count
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) WHERE ImportProcessLogId = @ImportProcessLogId) , SuccessRecordCount = 0 ,
		TotalProcessedRecords = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) Where ImportProcessLogId = @ImportProcessLogId)
		WHERE ImportProcessLogId = @ImportProcessLogId;

		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_ImportB2BCustomer',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
		
	END CATCH;
END;

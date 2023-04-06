CREATE PROCEDURE [dbo].[Znode_ImportCustomer]
(
	  @TableName NVARCHAR(100), 
	  @Status BIT OUT, 
	  @UserId INT, 
	  @ImportProcessLogId INT, 
	  @NewGUId NVARCHAR(200), 
	  @LocaleId INT= 0,
	  @PortalId INT ,
	  @CsvColumnString NVARCHAR(max)
)
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Import Customer Details
	
	-- Unit Testing : 
	--------------------------------------------------------------------------------------

BEGIN
	BEGIN TRAN A;
	BEGIN TRY

	    DECLARE @MessageDisplay NVARCHAR(100), @SSQL NVARCHAR(max),@AspNetZnodeUserId NVARCHAR(256),@ASPNetUsersId NVARCHAR(256),
			@PasswordHash NVARCHAR(max),@SecurityStamp NVARCHAR(max),@RoleId NVARCHAR(256),@IsAllowGlobalLevelUserCreation NVARCHAR(10)
			,@ProfileId INT, @FailedRecordCount BIGINT, @SuccessRecordCount BIGINT
		 
		SET @SecurityStamp = '0wVYOZNK4g4kKz9wNs-UHw2'
		SET @PasswordHash = 'APy4Tm1KbRG6oy7h3r85UDh/lCW4JeOi2O2Mfsb3OjkpWTp1YfucMAvvcmUqNaSOlA==';

		SELECT @RoleId = Id FROM AspNetRoles WHERE   NAME = 'User'  

		SELECT @IsAllowGlobalLevelUserCreation = FeatureValues FROM ZnodeGlobalsetting WHERE FeatureName = 'AllowGlobalLevelUserCreation'

		DECLARE @GetDate DATETIME= dbo.Fn_GetDate();
		-- Retrive RoundOff Value FROM global setting 
		Delete from ZnodeImportLog where ErrorDescription = '42' and ColumnName like '%AccountCode%' and  ImportProcessLogId= @ImportProcessLogId

		DECLARE @AttributeCode Nvarchar(max) = ''
		SELECT @AttributeCode=STRING_AGG(c.AttributeCode , ',')   
		FROM ZnodeGlobalFamilyGroupMapper a
		INNER JOIN ZnodeGlobalAttributeGroupMapper f ON (f.GlobalAttributeGroupId = a.GlobalAttributeGroupId)
		INNER JOIN ZnodeGlobalAttribute c ON (c.GlobalAttributeId = f.GlobalAttributeId)
		WHERE GlobalAttributeFamilyId = (SELECT TOP 1 GlobalAttributeFamilyId FROM ZnodeGlobalAttributeFamily where FamilyCode='User')
			AND NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ZnodeUser' AND COLUMN_NAME=c.AttributeCode)
			AND EXISTS (SELECT TOP 1 1 FROM tempdb.sys.columns WHERE Object_id = Object_id(@TableName) AND name = c.AttributeCode )
		
		-- Three type of import required three table varible for product , category and brand
		CREATE TABLE #InsertCustomer 
		( 
			RowId INT IDENTITY(1, 1) PRIMARY KEY, RowNumber INT, UserName NVARCHAR(512) ,FirstName	NVARCHAR(200),
			LastName NVARCHAR(200), BudgetAmount	NUMERIC,Email	NVARCHAR(100),PhoneNumber	NVARCHAR(100),
		    EmailOptIn	varchar(100)	,ReferralStatus	NVARCHAR(40),IsActive	varchar(100)	,ExternalId	NVARCHAR(max),CreatedDate DATETIME,
			ProfileName varchar(200),AccountCode NVARCHAR(100),DepartmentName varchar(300),RoleName NVARCHAR(256), GUID NVARCHAR(400)
			,Custom1 NVARCHAR(max),Custom2 NVARCHAR(max),Custom3 NVARCHAR(max),
			Custom4 NVARCHAR(max),Custom5 NVARCHAR(max), SMSOptIn varchar(100)
		);

		SET @SSQL = ' ALTER TABLE #InsertCustomer ADD '+REPLACE(TRIM(LTRIM(@AttributeCode)), ',',' NVARCHAR(max),' ) + CASE WHEN @AttributeCode = '' THEN '' ELSE ' NVARCHAR(max)' END 
		EXEC (@SSQL)

		SET @SSQL = 'INSERT INTO #InsertCustomer( RowNumber,' + @CsvColumnString + ',GUID)
						SELECT RowNumber,' + @CsvColumnString + ',GUID FROM '+ @TableName;

		EXEC sys.sp_sqlexec @SSQL;

		--If Account Code Exists then it will update as it is and if not exists then it will show error.
		UPDATE IC set AccountCode = ZA.AccountCode 
		FROM ZnodeUser a
		inner join ZnodeAccount ZA on (ZA.AccountId = a.AccountId)
		INNER JOIN #InsertCustomer IC on (IC.UserName = a.UserName)
		WHERE ISNULL(IC.AccountCode,'') = '' AND ISNULL(IC.RoleName,'') <> ''

		IF @CsvColumnString NOT LIKE '%AccountCode%'
		BEGIN
			--If Acount Code Exists then it will update as it is and if not exists then it will show error.
			UPDATE IC set AccountCode = ZA.AccountCode
			FROM ZnodeUser a
			INNER JOIN ZnodeAccount ZA on (ZA.AccountId = a.AccountId)
			INNER JOIN #InsertCustomer IC on (IC.UserName = a.UserName)
			WHERE ISNULL(IC.AccountCode,'') = '' 
		END
		
		SELECT TOP 1 @ProfileId   =  ProfileId FROM ZnodePortalprofile WHERE Portalid = @Portalid and IsDefaultRegistedProfile=1
		
		IF (ISNULL(@ProfileId ,0) = 0 ) 
		BEGIN
			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			SELECT '62', 'Default Portal Profile', '', @NewGUId, 1 , @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
							
			UPDATE ZnodeImportProcessLog
			SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
			WHERE ImportProcessLogId = @ImportProcessLogId;

			SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog 
			WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;
			SELECT @SuccessRecordCount = 0

			UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount , 
			TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))
			WHERE ImportProcessLogId = @ImportProcessLogId;

			DELETE FROM #InsertCustomer 
			SET @Status = 0;

			COMMIT TRAN A;
			RETURN 0 
		END
		
		IF @IsAllowGlobalLevelUserCreation = 'true'
		BEGIN
			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT '10', 'UserName', UserName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM #InsertCustomer AS ii  
			WHERE ltrim(rtrim(ii.UserName)) IN (SELECT UserName FROM AspNetZnodeUser);  
		END

        INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '104', 'UserName', UserName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertCustomer AS ii
		WHERE ii.UserName not like '%_@_%_.__%' 

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '35', 'UserName', UserName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertCustomer AS ii
		WHERE ii.UserName like '%;%'
				
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '30', 'UserName', UserName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertCustomer AS ii
		WHERE ltrim(rtrim(ii.UserName)) IN 
		(SELECT ltrim(rtrim(UserName))  FROM #InsertCustomer GROUP BY ltrim(rtrim(UserName))  having count(*) > 1 )

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '104', 'Email', Email, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertCustomer AS ii
		WHERE ii.Email not like '%_@_%_.__%' 

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
		SELECT '68', 'IsActive',  IsActive , @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
		FROM #InsertCustomer AS ii  
		WHERE ii.IsActive not in ('True','1','Yes','FALSE','0','No','')

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '77', 'AccountCode', ii.AccountCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertCustomer ii 
		WHERE ISNULL(LTRIM(RTRIM(ii.AccountCode)),'') !='' 
			AND NOT EXISTS(SELECT * FROM ZnodeAccount za INNER JOIN ZnodePortalAccount zpa on za.AccountId = zpa.AccountId
				WHERE  ISNULL(LTRIM(RTRIM(ii.AccountCode)),'') = za.AccountCode and zpa.PortalId = @PortalId )
			AND EXISTS(SELECT ISNULL(LTRIM(RTRIM(AccountCode)),'') FROM ZnodeAccount za1 WHERE ISNULL(LTRIM(RTRIM(ii.AccountCode)),'') = za1.AccountCode );

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '73', 'AccountCode', AccountCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertCustomer AS ii
		WHERE ISNULL(LTRIM(RTRIM(ii.AccountCode)),'') !='' AND ISNULL(LTRIM(RTRIM(ii.AccountCode)),'') NOT IN 
			(
				SELECT ISNULL(LTRIM(RTRIM(AccountCode)),'') FROM ZnodeAccount   
			);

        INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '75', 'RoleName', RoleName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertCustomer AS ii
		WHERE ISNULL(LTRIM(RTRIM(ii.AccountCode)),'') ='' AND ISNULL(LTRIM(RTRIM(RoleName)),'') <> '' 

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '74', 'RoleName', RoleName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertCustomer AS ii
		WHERE --ltrim(rtrim(ii.RoleName)) not IN ('User','Manager','Administrator') and ISNULL(LTRIM(RTRIM(RoleName)),'') <> '' and
			ISNULL(LTRIM(RTRIM(RoleName)),'') <> '' and not exists (SELECT top 1 1 FROM  AspNetRoles ANR WHERE name IN ('User','Manager','Administrator') AND  ANR.name =ii.RoleName)

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '76', 'DepartmentName', DepartmentName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
		FROM #InsertCustomer AS ii
		WHERE ISNULL(LTRIM(RTRIM(ii.DepartmentName)),'') <> ''
			AND NOT EXISTS(SELECT * FROM  ZnodeAccount ZA INNER JOIN ZnodeDepartment ZD on ZA.AccountId = ZD.AccountId
				WHERE ISNULL(LTRIM(RTRIM(ii.AccountCode)),'') = LTRIM(RTRIM(za.AccountCode))
				AND ISNULL(LTRIM(RTRIM(ii.DepartmentName)),'') = LTRIM(RTRIM(ZD.DepartmentName)))

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
		SELECT '68', 'EmailOptIn',  EmailOptIn , @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
		FROM #InsertCustomer AS ii  
		WHERE ii.EmailOptIn NOT IN ('True','1','Yes','FALSE','0','No','');

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
		SELECT '68', 'SMSOptIn',  SMSOptIn , @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
		FROM #InsertCustomer AS ii  
		WHERE ii.SMSOptIn NOT IN ('True','1','Yes','FALSE','0','No','');
	
		UPDATE ZIL
		SET ZIL.ColumnName =   ZIL.ColumnName + ' [ UserName - ' + ISNULL(UserName,'') + ' ] '
		FROM ZnodeImportLog ZIL 
		INNER JOIN #InsertCustomer IPA ON (ZIL.RowNumber = IPA.RowNumber)
		WHERE  ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL;

		--Note : Content page import is not required 
		
		-- END Function Validation 	
		-----------------------------------------------
		--- Delete Invalid Data after functional validatin  
		IF OBJECT_ID('tempdb..#GlobalAttributeData1') IS NOT NULL 
			DROP TABLE #GlobalAttributeData1;

		CREATE TABLE #GlobalAttributeData1 (RowNumber INT,Username NVARCHAR(500),FirstName NVARCHAR(100),LastName NVARCHAR(100),AttributeCode Nvarchar(300),AttributeValue Nvarchar(Max),Userid Int)

		SET @SSQL =' Select RowNumber,Username,FirstName,LastName,AttributeCode,AttributeValue
                    FROM #InsertCustomer 
					UNPIVOT
					( AttributeValue For AttributeCode IN ('+@AttributeCode+')
					) UNPIVOT8'
                
		INSERT INTO #GlobalAttributeData1(RowNumber,Username,FirstName,LastName,AttributeCode,AttributeValue)   
        EXEC sys.sp_sqlexec @SSQL;  

		ALTER TABLE #GlobalAttributeData1 ADD AttributetypeName VARCHAR(200) , ValidationName VARCHAR(600) , ValidationValue VARCHAR(600);
		
		UPDATE A
		SET A.AttributetypeName=C.AttributeTypeName, ValidationName = d.name  , ValidationValue =  e.Name
		FROM #GlobalAttributeData1 A
		INNER JOIN ZnodeGlobalAttribute B ON (B.AttributeCode=A.AttributeCode)
		INNER JOIN ZnodeAttributeType C ON (C.AttributeTypeId=B.AttributeTypeId)
		INNER JOIN ZnodeAttributeInputValidation d ON (d.AttributeTypeId = c.AttributeTypeId AND c.AttributeTypeName=d.ControlName)
		INNER JOIN ZnodeGlobalAttributeValidation e ON (e.InputValidationId = d.InputValidationId AND b.GlobalAttributeId = e.GlobalAttributeId);

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
		SELECT dbo.Fn_ValidateDataErrorCode(ii.AttributetypeName,ii.ValidationName ), ii.AttributeCode, ii.AttributeValue , @NewGUId, RowNumber, @UserId, @GetDate, 
			@UserId, @GetDate, @ImportProcessLogId  
		FROM #GlobalAttributeData1 AS ii
		WHERE dbo.Fn_ValidateData (ii.AttributetypeName,ii.AttributeValue,ii.ValidationName, ii.ValidationValue)=0
			AND ii.AttributeValue<>'';
		
		DELETE FROM #InsertCustomer
		WHERE RowNumber IN
		(
			SELECT DISTINCT RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId AND RowNumber IS NOT NULL
			--AND GUID = @NewGUID
		);

		-- Update Record count IN log 
        
		SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog 
		WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;

		SELECT @SuccessRecordCount = count(DISTINCT RowNumber) FROM #InsertCustomer

		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount , 
		TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))
		WHERE ImportProcessLogId = @ImportProcessLogId;
		-- END

		-- Insert Product Data 
				
		DECLARE @InsertedAspNetZnodeUser TABLE (AspNetZnodeUserId NVARCHAR(256) ,UserName NVARCHAR(512),PortalId INT )
		DECLARE @InsertedASPNetUsers TABLE (Id NVARCHAR(256) ,UserName NVARCHAR(512))
		DECLARE @InsertZnodeUser TABLE (UserId INT,AspNetUserId NVARCHAR(256),CreatedDate DATETIME )

		UPDATE ANU 
		SET ANU.PhoneNumber	= IC.PhoneNumber, 
			ANU.LockoutEndDateUtc = case when IC.IsActive IN('0','No','False') then @GetDate when IC.IsActive IN('1','Yes','True') then null else ANU.LockoutEndDateUtc END
		FROM AspNetZnodeUser ANZU 
		INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
		INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId	
		INNER JOIN #InsertCustomer IC ON ANZU.UserName = IC.UserName 
		WHERE CASE WHEN @IsAllowGlobalLevelUserCreation = 'true' THEN -1 ELSE Isnull(ANZU.PortalId,0) END = CASE WHEN @IsAllowGlobalLevelUserCreation = 'true' THEN -1 ELSE Isnull(@PortalId ,0) END

		UPDATE ZU SET 
		ZU.FirstName	= IC.FirstName,
		ZU.LastName		= IC.LastName,
		ZU.BudgetAmount = IC.BudgetAmount,
		ZU.Email		= IC.Email,
		ZU.PhoneNumber	= IC.PhoneNumber,
		ZU.EmailOptIn	= (CASE ISNULL(IC.EmailOptIn,0) WHEN 'Yes' THEN 1 WHEN 'No' THEN 0 ELSE CAST(ISNULL(IC.EmailOptIn,0) As BIT) END),
		ZU.IsActive		= (CASE ISNULL(IC.IsActive,0) WHEN 'Yes' THEN 1 WHEN 'No' THEN 0 ELSE CAST(ISNULL(IC.IsActive,0) As BIT) END),
		ZU.Custom1 = IC.Custom1,
		ZU.Custom2 = IC.Custom2,
		ZU.Custom3 = IC.Custom3,
		ZU.Custom4 = IC.Custom4,
		ZU.Custom5 = IC.Custom5,
		ZU.SMSOptIn	= (CASE ISNULL(IC.SMSOptIn,0) WHEN 'Yes' THEN 1 WHEN 'No' THEN 0 WHEN '' THEN ZU.SMSOptIn ELSE CAST(ISNULL(IC.SMSOptIn,0) As BIT) END)
		FROM AspNetZnodeUser ANZU INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
		INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId	
		INNER JOIN #InsertCustomer IC ON ANZU.UserName = IC.UserName 
		WHERE CASE WHEN @IsAllowGlobalLevelUserCreation = 'true' THEN -1 ELSE Isnull(ANZU.PortalId,0) END = CASE WHEN @IsAllowGlobalLevelUserCreation = 'true' THEN -1 ELSE Isnull(@PortalId ,0) END

		INSERT INTO AspNetZnodeUser (AspNetZnodeUserId, UserName, PortalId)		
		OUTPUT INSERTED.AspNetZnodeUserId, INSERTED.UserName, INSERTED.PortalId	INTO  @InsertedAspNetZnodeUser 			 
		SELECT NEWID(),IC.UserName, @PortalId FROM #InsertCustomer IC 
		WHERE NOT EXISTS (SELECT TOP 1 1  FROM AspNetZnodeUser ANZ 
			WHERE CASE WHEN @IsAllowGlobalLevelUserCreation = 'true' THEN -1 ELSE Isnull(ANZ.PortalId,0) END = CASE WHEN @IsAllowGlobalLevelUserCreation = 'true' THEN -1 ELSE Isnull(@PortalId ,0) END 
				AND ANZ.UserName = IC.UserName)

		INSERT INTO ASPNetUsers (Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,
		LockoutEndDateUtc,LockOutEnabled,AccessFailedCount,PasswordChangedDate,UserName)
		OUTPUT inserted.Id, inserted.UserName INTO @InsertedASPNetUsers
		SELECT NewId(), Email,0 ,@PasswordHash,@SecurityStamp,PhoneNumber,0,0,case when A.IsActive IN ('0','False','No') then @GetDate else null END LockoutEndDateUtc,1 LockoutEnabled,
			0,@GetDate,AspNetZnodeUserId 
		FROM #InsertCustomer A 
		INNER JOIN @InsertedAspNetZnodeUser B ON A.UserName = B.UserName
			
		INSERT INTO  ZnodeUser(AspNetUserId,FirstName,LastName,CustomerPaymentGUID,Email,PhoneNumber,EmailOptIn,
			IsActive,ExternalId, CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,UserName,Custom1,Custom2,Custom3,Custom4,Custom5,SMSOptIn)
		OUTPUT Inserted.UserId, Inserted.AspNetUserId,Inserted.CreatedDate INTO @InsertZnodeUser
		SELECT IANU.Id AspNetUserId ,IC.FirstName,IC.LastName,null CustomerPaymentGUID,IC.Email,IC.PhoneNumber,
			(CASE ISNULL(IC.EmailOptIn,0) WHEN 'Yes' THEN 1 WHEN 'No' THEN 0 ELSE CAST(ISNULL(IC.EmailOptIn,0) As BIT) END),
			(CASE ISNULL(IC.IsActive,0) WHEN 'Yes' THEN 1 WHEN 'No' THEN 0 ELSE CAST(ISNULL(IC.IsActive,0) As BIT) END),
			IC.ExternalId, @UserId,
			CASE WHEN IC.CreatedDate IS NULL OR IC.CreatedDate = '' THEN  @Getdate ELSE IC.CreatedDate END,
			@UserId,@Getdate,IC.UserName,IC.Custom1,IC.Custom2,IC.Custom3,IC.Custom4,IC.Custom5,
				(CASE ISNULL(IC.SMSOptIn,0) WHEN 'Yes' THEN 1 WHEN 'No' THEN 0 ELSE CAST(ISNULL(IC.SMSOptIn,0) As BIT) END)
		FROM #InsertCustomer IC 
		INNER JOIN @InsertedAspNetZnodeUser IANZU ON IC.UserName = IANZU.UserName
		INNER JOIN @InsertedASPNetUsers IANU ON IANZU.AspNetZnodeUserId = IANU.UserName 
	  	     
		INSERT INTO AspNetUserRoles (UserId,RoleId)  SELECT AspNetUserId, @RoleID FROM @InsertZnodeUser 
		INSERT INTO ZnodeUserPortal (UserId,PortalId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate) 
		SELECT UserId, @PortalId , @UserId, IZU.CreatedDate,@UserId,@Getdate 
		FROM @InsertZnodeUser IZU

		INSERT INTO ZnodeAccountUserPermission(UserId,AccountPermissionAccessId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT UserId, 4 , @UserId, @Getdate,@UserId,@Getdate 
		FROM @InsertZnodeUser IZU
		
		UPDATE a 
		SET a.Userid = b.UserId
		FROM #GlobalAttributeData1 a 
		INNER JOIN ZnodeUser b ON (b.username = a.Username)  
				
		UPDATE D
		SET D.AttributeValue=A.AttributeValue
		FROM #GlobalAttributeData1 A 
		INNER JOIN ZnodeGlobalAttribute C ON A.AttributeCode=C.AttributeCode
		INNER JOIN ZnodeUser ZU ON LTRIM(RTRIM(a.Username))=LTRIM(RTRIM(ZU.UserName)) 
		INNER JOIN ZnodeUserPortal ZUP on ZUP.UserId=ZU.UserId and ZUP.PortalId=@PortalId
		INNER JOIN ZnodeUserGlobalAttributeValue b ON c.GlobalAttributeId=b.GlobalAttributeId AND ZU.UserId=b.UserId
		INNER JOIN ZnodeUserGlobalAttributeValueLocale d on b.UserGlobalAttributeValueId=d.UserGlobalAttributeValueId

		INSERT INTO ZnodeUserGlobalAttributeValue (UserId,	GlobalAttributeId,	GlobalAttributeDefaultValueId,	AttributeValue,	CreatedBy,	CreatedDate,ModifiedBy,	ModifiedDate)
		SELECT g.Userid,ZGA.GlobalAttributeId,null,null,@UserId,@Getdate,@UserId,@Getdate
		FROM #GlobalAttributeData1 g 
		INNER JOIN ZnodeGlobalAttribute ZGA on ZGA.AttributeCode =g.AttributeCode
		INNER JOIN ZnodeUserPortal ZUP on ZUP.UserId=g.UserId and ZUP.PortalId=@PortalId
		WHERE NOT EXISTS (SELECT top 1 1 FROM ZnodeUserGlobalAttributeValue ZGAV WHERE ZGAV.Userid =g.Userid and ZGA.GlobalAttributeId=ZGAV.GlobalAttributeId)

		INSERT INTO ZnodeUserGlobalAttributeValuelocale(UserGlobalAttributeValueId,	LocaleId,	AttributeValue,	CreatedBy,	CreatedDate,	ModifiedBy,	ModifiedDate,	GlobalAttributeDefaultValueId,	MediaId,	MediaPath)
		SELECT UserGlobalAttributeValueId, @LocaleId,g.AttributeValue ,@UserId,@Getdate,@UserId,@Getdate,null,null,null
		FROM ZnodeUserGlobalAttributeValue ZUGAV 
		INNER JOIN #GlobalAttributeData1 g on ZUGAV.userid =g.userid
		INNER JOIN ZnodeUserPortal ZUP on ZUP.UserId=g.UserId and ZUP.PortalId=@PortalId
		INNER JOIN ZnodeGlobalAttribute ZGA on ZGA.AttributeCode =g.AttributeCode and ZGA.GlobalAttributeId = ZUGAV.GlobalAttributeId
		WHERE NOT EXISTS (SELECT Top 1 1 FROM ZnodeUserGlobalAttributeValuelocale z WHERE z.UserGlobalAttributeValueId = ZUGAV.UserGlobalAttributeValueId)

		---------------------------------------------------------------------------------

		DECLARE @Profile table (ProfileId INT)

		INSERT INTO ZnodeProfile (ProfileName,ShowOnPartnerSignup,Weighting,TaxExempt,DefaultExternalAccountNo,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,ParentProfileId)
		OUTPUT inserted.ProfileId INTO @Profile(ProfileId)
		SELECT Distinct ProfileName, 0, null,0, REPLACE(LTRIM(RTRIM(ProfileName)),' ','') as DefaultExternalAccountNo, @UserId,@Getdate, @UserId,@Getdate, null as ParentProfileId				
		FROM #InsertCustomer IC
		WHERE NOT EXISTS(SELECT * FROM ZnodeProfile ZP WHERE IC.ProfileName = ZP.ProfileName )
			AND ISNULL(ic.ProfileName,'') <> ''

		INSERT INTO ZnodePortalProfile (PortalId,	ProfileId,	IsDefaultAnonymousProfile,	IsDefaultRegistedProfile,	CreatedBy,	CreatedDate,	ModifiedBy,	ModifiedDate)
		SELECT @PortalId, ProfileId, 0 AS IsDefaultAnonymousProfile, 0 AS IsDefaultRegistedProfile, @UserId,@Getdate, @UserId,@Getdate
		FROM @Profile

		UPDATE ZnodeUserProfile 
		SET ProfileId = COALESCE(ZP.ProfileId,@ProfileId)
		FROM ZnodeUser a
		INNER JOIN ASPNetUsers b on (b.Id = a.AspNetUserId)
		INNER JOIN AspNetZnodeUser c on (c.AspNetZnodeUserId = b.UserName)
		INNER JOIN #InsertCustomer IC on (IC.UserName = c.UserName)
		INNER JOIN ZnodeUserProfile u ON u.UserId = a.UserId
		LEFT join ZnodeProfile ZP on IC.ProfileName = ZP.ProfileName
		--WHERE IC.ProfileName <> ''
				
		INSERT INTO ZnodeUserProfile (ProfileId,UserId,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT COALESCE(ZP.ProfileId,@ProfileId)  , a.UserId, 1 , @UserId,a.CreatedDate,@UserId,@Getdate 
		FROM ZnodeUser a
		INNER JOIN ASPNetUsers b on (b.Id = a.AspNetUserId)
		INNER JOIN AspNetZnodeUser c on (c.AspNetZnodeUserId = b.UserName)
		INNER JOIN #InsertCustomer IC on (IC.UserName = c.UserName)
		LEFT join ZnodeProfile ZP on IC.ProfileName = ZP.ProfileName
		WHERE NOT EXISTS (SELECT TOP  1 1 FROM ZnodeUserProfile u WHERE u.UserId = a.UserId )
			AND EXISTS(SELECT * FROM @InsertZnodeUser IZU WHERE A.UserId = IZU.UserId)

		--Updating RoleName if customer has changed the account and RoleName left blank then by default User role assigned to the customer
		UPDATE u 
		set u.RoleId = @RoleId
		FROM ZnodeUser a
		INNER JOIN ASPNetUsers b on (b.Id = a.AspNetUserId)
		INNER JOIN AspNetZnodeUser c on (c.AspNetZnodeUserId = b.UserName)
		INNER JOIN #InsertCustomer IC on (IC.UserName = c.UserName)
		INNER JOIN AspNetUserRoles u on u.UserId = b.Id
		WHERE ISNULL(IC.AccountCode,'') <> '' AND ISNULL(IC.RoleName,'') = ''
			AND EXISTS(SELECT * FROM ZnodeAccount ZA WHERE ISNULL(a.AccountId,0) = ZA.AccountId AND ZA.AccountCode <> IC.AccountCode)

		---to update accountid agaist user
		UPDATE ZU 
		SET ZU.AccountId = ZA.AccountId 
		FROM AspNetZnodeUser ANZU INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
		INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId	 
		INNER JOIN #InsertCustomer IC ON ANZU.UserName = IC.UserName
		INNER JOIN ZnodeAccount ZA ON ZA.AccountCode = IC.AccountCode
		WHERE ISNULL(ANZU.PortalId,0) = ISNULL(@PortalId ,0) AND ISNULL(IC.AccountCode,'') <> '' 
				
		UPDATE ZDU 
		set ZDU.DepartmentId = ZD.DepartmentId, 
			ModifiedBy = @UserId, 
			ModifiedDate = @Getdate
		FROM ZnodeUser a
		INNER JOIN ASPNetUsers b on (b.Id = a.AspNetUserId)
		INNER JOIN AspNetZnodeUser c on (c.AspNetZnodeUserId = b.UserName)
		INNER JOIN #InsertCustomer IC on (IC.UserName = c.UserName)
		INNER JOIN ZnodeDepartment ZD on IC.DepartmentName = ZD.DepartmentName
		INNER JOIN ZnodeDepartmentUser ZDU on ZDU.UserId = a.UserId
		WHERE ISNULL(IC.DepartmentName,'') <> ''

		INSERT INTO ZnodeDepartmentUser(UserId,DepartmentId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT a.UserId, ZD.DepartmentId, @UserId,a.CreatedDate,@UserId,@Getdate 
		FROM ZnodeUser a
		INNER JOIN ASPNetUsers b on (b.Id = a.AspNetUserId)
		INNER JOIN AspNetZnodeUser c on (c.AspNetZnodeUserId = b.UserName)
		INNER JOIN #InsertCustomer IC on (IC.UserName = c.UserName)
		INNER JOIN ZnodeDepartment ZD on IC.DepartmentName = ZD.DepartmentName
		WHERE NOT EXISTS (SELECT TOP  1 1 FROM ZnodeDepartmentUser u WHERE u.UserId = a.UserId)
			AND ISNULL(IC.DepartmentName,'') <> ''
		
		UPDATE u 
		SET u.RoleId = ZD.Id
		FROM ZnodeUser a
		INNER JOIN ASPNetUsers b on (b.Id = a.AspNetUserId)
		INNER JOIN AspNetZnodeUser c on (c.AspNetZnodeUserId = b.UserName)
		INNER JOIN #InsertCustomer IC on (IC.UserName = c.UserName)
		INNER JOIN AspNetRoles ZD on IC.RoleName = ZD.Name
		INNER JOIN AspNetUserRoles u on u.UserId = b.Id
		WHERE isnull(IC.RoleName,'') <> '' 
		
		--when RoleName and AccountCode is null in update file then If a value was available previously, then it should be removed (along with Department and Role Name) 
		--after the update process is complete.
		DELETE UR 
		FROM AspNetUserRoles UR 
		INNER JOIN ZnodeUser ZC	ON  ZC.AspNetUserId = UR.UserId
		WHERE EXISTS (SELECT * FROM #InsertCustomer IC WHERE IC.UserName = ZC.UserName AND (ISNULL(IC.RoleName,'') = '' AND ISNULL(IC.AccountCode,'') = ''))

		UPDATE ZnodeUser SET AccountId = NULL 
		WHERE EXISTS (SELECT * FROM #InsertCustomer IC WHERE IC.UserName = ZnodeUser.UserName AND ISNULL(IC.AccountCode,'') = '' AND ISNULL(IC.RoleName,'') = '' )		
		
		INSERT INTO AspNetUserRoles(UserId,RoleId)
		SELECT b.Id as ASPNetUserId, CASE WHEN ZD.Id IS NULL THEN @RoleId ELSE ZD.Id END as RoleId
		FROM ZnodeUser a
		INNER JOIN ASPNetUsers b on (b.Id = a.AspNetUserId)
		INNER JOIN AspNetZnodeUser c on (c.AspNetZnodeUserId = b.UserName)
		INNER JOIN #InsertCustomer IC on (IC.UserName = c.UserName)
		LEFT JOIN AspNetRoles ZD on IC.RoleName = ZD.Name
		WHERE NOT EXISTS (SELECT TOP  1 1 FROM AspNetUserRoles u WHERE u.UserId = b.Id)
			AND ISNULL(IC.AccountCode,'') <> ''
			
		--If no value was available previously then no value should be saved after the update process is complete.
		--If a value was available previously, then it should be removed after the update process is complete.
		DELETE DU 
		FROM ZnodeDepartmentUser DU
		INNER JOIN ZnodeUser ZU ON DU.UserId = ZU.UserId
		INNER JOIN ZnodeDepartment ZC ON  ZC.DepartmentId = DU.DepartmentId
		WHERE EXISTS (SELECT * FROM #InsertCustomer IC WHERE IC.UserName = ZU.UserName AND (ISNULL(IC.DepartmentName,'') = '' OR ISNULL(IC.ACCOUNTCODE,'') = ''))

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
		
		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportCustomer @TableName = '+CAST(@TableName AS VARCHAR(max))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@PortalId='+CAST(@PortalId AS VARCHAR(10))+',@CsvColumnString='+CAST(@CsvColumnString AS VARCHAR(max));
              	
		 ---Import process updating fail due to database error
		UPDATE ZnodeImportProcessLog
		SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		---Loging error for Import process due to database error
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '93', '', '', @NewGUId,  @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId

		--Updating total and fail record count
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount WITH (NOLOCK) WHERE ImportProcessLogId = @ImportProcessLogId) , SuccessRecordCount = 0 ,
			TotalProcessedRecords = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount WITH (NOLOCK) WHERE ImportProcessLogId = @ImportProcessLogId)
		WHERE ImportProcessLogId = @ImportProcessLogId;

        EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_ImportCustomer',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH;
END;
CREATE PROCEDURE [dbo].[Znode_ImportCustomerAddress]
(
	@TableName NVARCHAR(100), 
	@Status BIT OUT, 
	@UserId INT, 
	@ImportProcessLogId INT, 
	@NewGUId NVARCHAR(200), 
	@LocaleId INT= 0,
	@CsvColumnString NVARCHAR(MAX),
	@IsAccountAddress BIT = 0 
)
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Import Customer Address
	
	-- Unit Testing : 
	--------------------------------------------------------------------------------------
BEGIN
	BEGIN TRAN A;
	BEGIN TRY
		DECLARE @MessageDisplay NVARCHAR(100), @SSQL NVARCHAR(max),@IsAllowGlobalLevelUserCreation NVARCHAR(10)

		DECLARE @GETDATE DATETIME= dbo.Fn_GETDATE();
		-- Retrive Value FROM global setting 
		SELECT @IsAllowGlobalLevelUserCreation = FeatureValues FROM ZnodeGlobalsetting WHERE FeatureName = 'AllowGlobalLevelUserCreation'
		-- Three type of import required three table varible for product , category AND brand

		CREATE TABLE #InsertCustomerAddress 
		( 
			RowId INT IDENTITY(1, 1) PRIMARY KEY, RowNumber INT,UserName NVARCHAR(512),FirstName VARCHAR(300),
			LastName VARCHAR(300),DisplayName NVARCHAR(1200),Address1 VARCHAR(300),Address2 VARCHAR(300),
			CountryName VARCHAR(3000),StateName VARCHAR(3000),CityName VARCHAR(3000),PostalCode VARCHAR(50),
			PhoneNumber VARCHAR(50),IsDefaultBilling BIT,IsDefaultShipping BIT	,IsActive BIT,ExternalId NVARCHAR(2000),
			CompanyName NVARCHAR(2000), GUID NVARCHAR(400), PortalId INT
		);
	
		SET @SSQL = ' INSERT INTO #InsertCustomerAddress ( RowNumber, ' + @CsvColumnString + ' ,GUID )
		SELECT RowNumber,' + @CsvColumnString + ',GUID FROM '+ @TableName;
		EXEC sys.sp_sqlexec @SSQL;

		UPDATE ICA
		SET ICA.PortalId=ZUP.PortalId
		FROM #InsertCustomerAddress ICA
		INNER JOIN ZnodeUser ZU ON ICA.UserName=ZU.UserName
		INNER JOIN ZnodeUserPortal ZUP ON ZU.UserId=ZUP.UserId
		-- start Functional Validation 
		----------------------------------------------

		IF (@IsAccountAddress = 0)
		BEGIN
			IF @IsAllowGlobalLevelUserCreation = 'true'
			BEGIN
				INSERT INTO ZnodeImportLog
					(ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT '106', 'UserName', UserName, @NewGUId, RowNumber, @UserId, @GETDATE, @UserId, @GETDATE, @ImportProcessLogId
				FROM #InsertCustomerAddress AS ii
				WHERE ii.UserName NOT IN 
				(
					SELECT UserName FROM AspNetZnodeUser WHERE PortalId = ii.PortalId
				);
			END
			ELSE
			BEGIN
				INSERT INTO ZnodeImportLog
					(ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT '106', 'UserName', UserName, @NewGUId, RowNumber, @UserId, @GETDATE, @UserId, @GETDATE, @ImportProcessLogId
				FROM #InsertCustomerAddress AS ii
				WHERE ii.UserName NOT IN 
				(
					SELECT UserName FROM AspNetZnodeUser   
				);

				INSERT INTO ZnodeImportLog
					(ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId)
				SELECT '8', 'UserName', UserName, @NewGUId, RowNumber, @UserId, @GETDATE, @UserId, @GETDATE, @ImportProcessLogId
				FROM #InsertCustomerAddress AS ii
				WHERE ISNULL(ltrim(rtrim(ii.UserName)), '') = ''
			END
		END

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '8', 'StateName', StateName, @NewGUId, RowNumber, @UserId, @GETDATE, @UserId, @GETDATE, @ImportProcessLogId
		FROM #InsertCustomerAddress AS ii
		WHERE NOT EXISTS(SELECT * FROM ZnodeState ZS INNER JOIN ZnodeCountry ZC ON ZS.CountryCode = ZC.CountryCode
						WHERE ii.CountryName = ZC.CountryName AND ZS.StateName = ISNULL(LTRIM(RTRIM(ii.StateName)), ''))

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '8', 'CountryName', CountryName, @NewGUId, RowNumber, @UserId, @GETDATE, @UserId, @GETDATE, @ImportProcessLogId
		FROM #InsertCustomerAddress AS ii
		WHERE NOT EXISTS(SELECT * FROM ZnodeCountry ZC WHERE ZC.CountryName = ISNULL(LTRIM(RTRIM(ii.CountryName)), '') )

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '90', 'CountryName', CountryName, @NewGUId, RowNumber, @UserId, @GETDATE, @UserId, @GETDATE, @ImportProcessLogId
		FROM #InsertCustomerAddress AS ii
		WHERE ISNULL(ltrim(rtrim(ii.CountryName)), '') <> ''
			AND NOT EXISTS(SELECT * FROM ZnodePortalCountry ZPC INNER JOIN ZnodeCountry ZC ON ZPC.CountryCode = ZC.CountryCode
				WHERE PortalId = ii.PortalId AND LTRIM(RTRIM(ii.CountryName)) = LTRIM(RTRIM(ZC.CountryName)))
				AND ii.PortalId IS NOT NULL

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '134', 'CountryName', CountryName, @NewGUId, RowNumber, @UserId, @GETDATE, @UserId, @GETDATE, @ImportProcessLogId
		FROM #InsertCustomerAddress AS ii
		WHERE ISNULL(LTRIM(RTRIM(ii.CountryName)), '') <> ''
			AND NOT EXISTS(SELECT * FROM ZnodePortalCountry ZPC INNER JOIN ZnodeCountry ZC ON ZPC.CountryCode = ZC.CountryCode
				WHERE LTRIM(RTRIM(ii.CountryName)) = LTRIM(RTRIM(ZC.CountryName)))
			AND ii.PortalId IS NULL

		 -- error log when atleast db have 
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '63', 'IsDefaultBilling/IsDefaultShipping', IsDefaultBilling, @NewGUId, RowNumber, @UserId, @GETDATE, @UserId, @GETDATE, @ImportProcessLogId
		FROM #InsertCustomerAddress IC WHERE EXISTS
			(
				SELECT TOP 1 1  FROM AspNetZnodeUser ANZU 
				INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
				INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId	
				INNER JOIN ZnodeUserAddress ZUA ON ZUA.UserId = ZU.UserId
				INNER JOIN ZnodeAddress ZA ON ZUA.AddressId = ZA.AddressId
				WHERE ANZU.UserName = IC.UserName AND ZA.IsDefaultBilling =IC.IsDefaultBilling 
					AND ZA.IsDefaultShipping =IC.IsDefaultShipping
			)

		--Note : Content page import is not required 
		
		-- End Function Validation 	
		-----------------------------------------------
		UPDATE ZIL
		SET ZIL.ColumnName =   ZIL.ColumnName + ' [ UserName - ' + ISNULL(UserName,'') + ' ] '
		FROM ZnodeImportLog ZIL 
		INNER JOIN #InsertCustomerAddress IPA ON (ZIL.RowNumber = IPA.RowNumber)
		WHERE  ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL

		DELETE FROM #InsertCustomerAddress
		WHERE RowNumber IN
		(
			SELECT DISTINCT RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  AND RowNumber IS NOT NULL
		);

		UPDATE ZA SET ZA.IsDefaultBilling = 0
		FROM AspNetZnodeUser ANZU INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
		INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId	
		INNER JOIN ZnodeUserAddress ZUA ON ZUA.UserId = ZU.UserId
		INNER JOIN ZnodeAddress ZA ON ZUA.AddressId = ZA.AddressId
		INNER JOIN #InsertCustomerAddress IC ON ANZU.UserName = IC.UserName --and IC.IsDefaultBilling <> ZA.IsDefaultBilling 
		WHERE IC.IsDefaultBilling = 1 

		UPDATE ZA SET ZA.IsDefaultShipping = 0
		FROM AspNetZnodeUser ANZU INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
		INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId	
		INNER JOIN ZnodeUserAddress ZUA ON ZUA.UserId = ZU.UserId
		INNER JOIN ZnodeAddress ZA ON ZUA.AddressId = ZA.AddressId
		INNER JOIN #InsertCustomerAddress IC ON ANZU.UserName = IC.UserName
		WHERE IC.IsDefaultShipping = 1 

		--DELETE FROM ZnodeImportLog  WHERE ImportProcessLogId = @ImportProcessLogId AND  ErrorDescription = '63'
		-- UPDATE Record count in log 
        DECLARE @FailedRecordCount BIGINT
		DECLARE @SuccessRecordCount BIGINT

		SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;

		SELECT @SuccessRecordCount = count(DISTINCT RowNumber) FROM #InsertCustomerAddress

		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount ,
			TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))
		WHERE ImportProcessLogId = @ImportProcessLogId;
		-- End
	
		DELETE FROM #InsertCustomerAddress 
		WHERE NOT EXISTS
			(SELECT TOP 1 1 FROM 
				(
					SELECT ROW_NUMBER() OVER(PARTITION BY UserName,FirstName,LastName,DisplayName,Address1,Address2,CountryName,StateName,CityName,PostalCode	
						,PhoneNumber,IsDefaultBilling,IsDefaultShipping,ExternalId,CompanyName ORDER BY RowId DESC)	AS rw_nmbr,RowId
					FROM #InsertCustomerAddress
				) abc WHERE rw_nmbr =1 AND abc.RowId = #InsertCustomerAddress.RowId
			)

		----------UPDATE ZnodeAddress
		DECLARE @AddressColumnString VARCHAR(1000), @WhereConditionString VARCHAR(1000), @UpdateColumnString VARCHAR(1000)

		SELECT @AddressColumnString = COALESCE(@AddressColumnString + ',', '')+a.ColumnName --COALESCE(@CsvColumnString + ' AND ', '') +'ZA.'+ColumnName+' =  IC.'+ColumnName
		FROM ZnodeImportUpdatableColumns a
		INNER JOIN INFORMATION_SCHEMA.COLUMNS b ON a.ColumnName = b.COLUMN_NAME  
		INNER JOIN dbo.Split(@CsvColumnString,',')C ON b.COLUMN_NAME = c.Item
		WHERE b.TABLE_NAME = 'ZnodeAddress' 
		AND EXISTS(SELECT * FROM ZnodeImportHead IH WHERE a.ImportHeadId = IH.ImportHeadId AND IH.Name in ('CustomerAddress','ShippingAddress'))

		SELECT @UpdateColumnString = COALESCE(@UpdateColumnString + ' , ', '') +'ZA.'+a.COLUMN_NAME+' =  IC.'+a.COLUMN_NAME  
		FROM INFORMATION_SCHEMA.COLUMNS a
		INNER JOIN dbo.Split(@CsvColumnString,',')b ON a.COLUMN_NAME = b.Item
		WHERE NOT EXISTS (SELECT * FROM dbo.Split(@AddressColumnString,',') c WHERE a.COLUMN_NAME = c.Item )
		AND a.TABLE_NAME = 'ZnodeAddress'

		SELECT @WhereConditionString = COALESCE(@WhereConditionString + ' AND ', '') +'ZA.'+item+' =  IC.'+item FROM dbo.split(@AddressColumnString,',')
		SET @WhereConditionString = Replace(@WhereConditionString , 'ZA.Address2 =  IC.Address2', 'ISNULL(ZA.Address2 ,'''')=  ISNULL(IC.Address2 ,'''')') 
	
		UPDATE a SET a.StateName =c.StateCode  
		FROM #InsertCustomerAddress a 
		INNER JOIN ZnodeCountry b ON a.CountryName = b.CountryName 
		INNER JOIN ZnodeState c ON b.CountryCode = c.CountryCode AND C.StateName = a.StateName
		
		UPDATE a SET a.CountryName= b.CountryCode FROM  #InsertCustomerAddress a INNER JOIN ZnodeCountry b ON a.CountryName = b.CountryName 
		
		CREATE TABLE #InsertedUserAddress (AddressId  NVARCHAR(256), UserId NVARCHAR(max),UserName nvarchar(300)) 

		IF ( @IsAccountAddress = 1 )
		BEGIN
			UPDATE ZnodeAddress SET IsDefaultBilling = 0,  IsDefaultShipping = 0
			FROM AspNetZnodeUser ANZU 
			INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
			INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId	
			INNER JOIN ZnodeUserAddress ZUA ON ZUA.UserId = ZU.UserId
			INNER JOIN ZnodeAddress ZA ON ZUA.AddressId = ZA.AddressId
			INNER JOIN #InsertCustomerAddress IC ON ANZU.UserName = IC.UserName AND ZA.IsDefaultBilling =IC.IsDefaultBilling 
													AND ZA.IsDefaultShipping =IC.IsDefaultShipping

			SET @SSQL = '
				UPDATE ZA set ModifiedBy = '+CONVERT(VARCHAR(10), @UserId)+', ModifiedDate = '''+CONVERT(NVARCHAR(30),@GetDate,121)+''' '+CASE WHEN ISNULL(@UpdateColumnString,'') = '' THEN '' ELSE ','+@UpdateColumnString END+' 
				FROM ZnodeAddress ZA
				INNER JOIN #InsertCustomerAddress IC ON '+CASE WHEN ISNULL(@WhereConditionString,'') = '' THEN ' 1 = 0 ' ELSE @WhereConditionString END

			EXEC (@SSQL)

			SET @SSQL = '
				Insert into ZnodeAddress (FirstName,LastName,DisplayName,Address1,Address2,Address3,CountryName,
										StateName,CityName,PostalCode,PhoneNumber,
										IsDefaultBilling,IsDefaultShipping,IsActive,ExternalId,CompanyName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsShipping,IsBilling)		
				OUTPUT INSERTED.AddressId INTO  #InsertedUserAddress (AddressId) 			 
				SELECT IC.FirstName,IC.LastName,IC.DisplayName,IC.Address1,IC.Address2,null,IC.CountryName ,
				IC.StateName,
				,IC.CityName,IC.PostalCode,IC.PhoneNumber,
				isnull(IC.IsDefaultBilling,0),isnull(IC.IsDefaultShipping,0),isnull(IC.IsActive,0),IC.ExternalId,IC.CompanyName, '+CONVERT(VARCHAR(10), @UserId)+' , '''+CONVERT(NVARCHAR(30),@GetDate,121)+''' , '+CONVERT(VARCHAR(10), @UserId)+' ,'''+CONVERT(NVARCHAR(30),@GetDate,121)+''', 1, 1
				FROM  #InsertCustomerAddress IC
				WHERE NOT EXISTS(SELECT * FROM ZnodeAddress ZA WHERE '+CASE WHEN ISNULL(@WhereConditionString,'') = '' THEN ' 1 = 0 ' ELSE @WhereConditionString END +')'

			EXEC (@SSQL)

			DECLARE @AccountId INT
			SELECT @AccountId = AccountId FROM ZnodeUser WHERE UserId = @UserId
			INSERT INTO ZnodeAccountAddress ( AccountId, AddressId, CreatedBy, CreatedDate,	ModifiedBy,	ModifiedDate )
			SELECT @AccountId, Addressid ,  @UserId , @GETDATE, @UserId , @GETDATE FROM #InsertedUserAddress UA
			WHERE NOT EXISTS ( SELECT * FROM ZnodeAccountAddress AA WHERE AccountId = @AccountId AND AA.Addressid = UA.Addressid )
		END
		ELSE
		BEGIN
			UPDATE ZnodeAddress SET IsDefaultBilling = 0,  IsDefaultShipping = 0
			FROM AspNetZnodeUser ANZU INNER JOIN ASPNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName 
			INNER JOIN ZnodeUser ZU ON ANU.ID = ZU.AspNetUserId	
			INNER JOIN ZnodeUserAddress ZUA ON ZUA.UserId = ZU.UserId
			INNER JOIN ZnodeAddress ZA ON ZUA.AddressId = ZA.AddressId
			INNER JOIN #InsertCustomerAddress IC ON ANZU.UserName = IC.UserName AND ZA.IsDefaultBilling =IC.IsDefaultBilling 
			AND ZA.IsDefaultShipping =IC.IsDefaultShipping
					
			SET @SSQL = '
				UPDATE ZA set ModifiedBy = '+CONVERT(VARCHAR(10), @UserId)+',ModifiedDate = '''+CONVERT(NVARCHAR(30),@GetDate,121)+''', Address3 = IC.UserName '+
				CASE WHEN ISNULL(@UpdateColumnString,'') = '' THEN '' ELSE ','+@UpdateColumnString END+' 
				OUTPUT INSERTED.AddressId,INSERTED.Address3 INTO  #InsertedUserAddress (AddressId,UserName) 
				FROM ZnodeAddress ZA 
				INNER JOIN #InsertCustomerAddress IC ON '+CASE WHEN ISNULL(@WhereConditionString,'') = '' THEN ' 1 = 0 ' ELSE @WhereConditionString END
			
			EXEC (@SSQL)

			UPDATE a SET a.UserId = b.UserId
			FROM #InsertedUserAddress a
			INNER JOIN ZnodeUser b ON LTRIM(RTRIM(A.UserName)) = LTRIM(RTRIM(b.UserName))

			SET @SSQL = '
				Insert into ZnodeAddress (FirstName,LastName,DisplayName,Address1,Address2,Address3,CountryName,
											StateName,CityName,PostalCode,PhoneNumber,
											IsDefaultBilling,IsDefaultShipping,IsActive,ExternalId,CompanyName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsShipping,IsBilling)		
				OUTPUT INSERTED.AddressId, INSERTED.Address3 INTO  #InsertedUserAddress (AddressId, UserId ) 			 
				SELECT IC.FirstName,IC.LastName,IC.DisplayName,IC.Address1,IC.Address2,convert(nvarchar(100),ZU.UserId),IC.CountryName ,
				IC.StateName 
				,IC.CityName,IC.PostalCode,IC.PhoneNumber,
				isnull(IC.IsDefaultBilling,0),isnull(IC.IsDefaultShipping,0),isnull(IC.IsActive,0),IC.ExternalId,IC.CompanyName, '+CONVERT(VARCHAR(10), @UserId)+' , '''+CONVERT(NVARCHAR(30),@GetDate,121)+''' , '+CONVERT(VARCHAR(10), @UserId)+' ,'''+CONVERT(NVARCHAR(30),@GetDate,121)+''',1,1
				FROM #InsertCustomerAddress IC INNER JOIN ZnodeUser ZU ON ZU.UserName= IC.UserName
				WHERE NOT EXISTS(SELECT * FROM ZnodeAddress ZA WHERE '+CASE WHEN ISNULL(@WhereConditionString,'') = '' THEN ' 1 = 0 ' ELSE @WhereConditionString END +')'
			
			print @SSQL

			EXEC (@SSQL)
			INSERT INTO ZnodeUserAddress(UserId,AddressId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			SELECT CAST( UserId AS INT ) , Addressid , @UserId , @GETDATE, @UserId , @GETDATE FROM  #InsertedUserAddress
		END
				
		UPDATE ZA SET ZA.CountryName = ZC.CountryCode
		FROM ZnodeAddress ZA
		INNER JOIN ZnodeCountry ZC ON LTRIM(RTRIM(ZA.CountryName)) = LTRIM(RTRIM(ZC.CountryName))

		UPDATE ZA SET ZA.Address3 = null 
		FROM ZnodeAddress ZA INNER JOIN #InsertedUserAddress IUA ON ZA.AddressId = IUA.AddressId 

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

		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 
					'EXEC Znode_ImportCustomerAddress 
						@TableName = '+CAST(@TableName AS VARCHAR(max)) +',
						@Status='+ CAST(@Status AS VARCHAR(10))+',
						@UserId = '+CAST(@UserId AS VARCHAR(50))+',
						@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',
						@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',
						@LocaleId='+CAST(@LocaleId AS VARCHAR(max)) +',
						@CsvColumnString='+CAST(@CsvColumnString AS VARCHAR(max)) +',
						@IsAccountAddress='+CAST(@IsAccountAddress AS VARCHAR(max));

		---Import process updating fail due to database error
		UPDATE ZnodeImportProcessLog
		SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GETDATE
		WHERE ImportProcessLogId = @ImportProcessLogId;

		---Loging error for Import process due to database error
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '93', '', '', @NewGUId,  @UserId, @GETDATE, @UserId, @GETDATE, @ImportProcessLogId

		--Updating total AND fail record count
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) WHERE ImportProcessLogId = @ImportProcessLogId) , SuccessRecordCount = 0 ,
			TotalProcessedRecords = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount with (nolock) WHERE ImportProcessLogId = @ImportProcessLogId)
		WHERE ImportProcessLogId = @ImportProcessLogId;

		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_ImportCustomerAddress',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	END CATCH;
END;
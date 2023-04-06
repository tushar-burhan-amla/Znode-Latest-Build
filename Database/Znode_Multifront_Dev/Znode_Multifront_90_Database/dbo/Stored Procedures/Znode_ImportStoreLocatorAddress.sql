CREATE PROCEDURE [dbo].[Znode_ImportStoreLocatorAddress](
	  @TableName nvarchar(100), @Status bit OUT, @UserId int, @ImportProcessLogId int, @NewGUId nvarchar(200), @CsvColumnString nvarchar(max))
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Import SEO Details
	
	-- Unit Testing : 
	--------------------------------------------------------------------------------------

BEGIN
	BEGIN TRAN A;
	BEGIN TRY
		DECLARE @MessageDisplay nvarchar(100), @SSQL nvarchar(max),@IsAllowGlobalLevelUserCreation nvarchar(10)

		DECLARE @GetDate datetime= dbo.Fn_GetDate();
		-- Retrive Value from global setting 
		Select @IsAllowGlobalLevelUserCreation = FeatureValues from ZnodeGlobalsetting where FeatureName = 'AllowGlobalLevelUserCreation'
		-- Three type of import required three table varible for product , category and brand

		CREATE TABLE #InsertCustomerAddress 
		( 
			 RowId int IDENTITY(1, 1) PRIMARY KEY, RowNumber int,PortalCode	nvarchar(600),StoreLocatorName	nvarchar(600)
			,FirstName	varchar	(300),LastName	varchar	(300),DisplayName	nvarchar(1200),Address1	varchar	(300),Address2	varchar	(300)
			,CountryName	varchar	(3000),StateName	varchar	(3000),CityName	varchar	(3000),PostalCode	varchar	(50)
			,PhoneNumber	varchar	(50),
			IsDefaultBilling	bit 
			,IsDefaultShipping	bit	,IsActive	bit	,ExternalId	nvarchar(2000),CompanyName nvarchar(2000), GUID NVARCHAR(400),
			DisplayOrder int ,Latitude decimal, Longitude decimal
		);
		
		SET @SSQL = 'INSERT INTO #InsertCustomerAddress( RowNumber,' + @CsvColumnString + ' ,GUID )
		SELECT RowNumber,' + @CsvColumnString + ',GUID FROM '+ @TableName;
		--INSERT INTO @InsertCustomerAddress( RowNumber,PortalCode,StoreLocatorName,FirstName,LastName,DisplayName,Address1,Address2,CountryName,
		--									StateName,CityName,PostalCode,PhoneNumber,
		--									IsDefaultBilling,IsActive,IsDefaultShipping,ExternalId,CompanyName,DisplayOrder,Latitude,Longitude,GUID )
		--print @SSQL
		EXEC sys.sp_sqlexec @SSQL;

		-- start Functional Validation 
		-----------------------------------------------
		--,PortalCode	,StoreName,IsStoreActive

				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
				SELECT '110', 'PortalCode', PortalCode, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
				FROM #InsertCustomerAddress AS ii
				WHERE ii.PortalCode NOT IN 
				(
					SELECT StoreCode FROM ZnodePortal 
				);
			
				INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
				SELECT '8', 'StoreLocatorName', StoreLocatorName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
				FROM #InsertCustomerAddress AS ii
				WHERE ISnull(ltrim(rtrim(ii.StoreLocatorName)), '') = ''


		DELETE FROM #InsertCustomerAddress
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId  and RowNumber is not null 
			--AND GUID = @NewGUID
		);

		-- Update Record count in log 
        DECLARE @FailedRecordCount BIGINT
		DECLARE @SuccessRecordCount BIGINT
		SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;
		Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM #InsertCustomerAddress
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount 
		WHERE ImportProcessLogId = @ImportProcessLogId;
		-- End

				--DECLARE #InsertedUserAddress TABLE (AddressId  nvarchar(256), PortalId nvarchar(max), PortalCode nvarchar(max)) 
				CREATE TABLE #InsertedUserAddress (AddressId  nvarchar(256), PortalId nvarchar(max), PortalCode nvarchar(max)) 
				
				----------update ZnodeAddress				
				DECLARE @AddressColumnString VARCHAR(1000), @WhereConditionString VARCHAR(1000), @UpdateColumnString VARCHAR(1000)

				SELECT @AddressColumnString = COALESCE(@AddressColumnString + ',', '')+a.ColumnName --COALESCE(@CsvColumnString + ' and ', '') +'ZA.'+ColumnName+' =  IC.'+ColumnName
				FROM ZnodeImportUpdatableColumns a
				INNER JOIN INFORMATION_SCHEMA.COLUMNS b on a.ColumnName = b.COLUMN_NAME  
				INNER JOIN dbo.Split(@CsvColumnString,',')C on b.COLUMN_NAME = c.Item
				WHERE b.TABLE_NAME = 'ZnodeAddress' 
				AND EXISTS(SELECT * FROM ZnodeImportHead IH where a.ImportHeadId = IH.ImportHeadId and IH.Name = 'StoreLocator')
				
				SELECT @UpdateColumnString = COALESCE(@UpdateColumnString + ' , ', '') +'ZA.'+a.COLUMN_NAME+' =  IC.'+a.COLUMN_NAME  
				FROM INFORMATION_SCHEMA.COLUMNS a
				INNER JOIN dbo.Split(@CsvColumnString,',')b on a.COLUMN_NAME = b.Item
				WHERE NOT EXISTS (SELECT * FROM dbo.Split(@AddressColumnString,',') c WHERE a.COLUMN_NAME = c.Item )
				AND a.TABLE_NAME = 'ZnodeAddress'

				SELECT @WhereConditionString = COALESCE(@WhereConditionString + ' AND ', '') +'ZA.'+item+' =  IC.'+item from dbo.split(@AddressColumnString,',')

				SET @SSQL = '
						UPDATE ZA set ModifiedBy = '+CONVERT(VARCHAR(10), @UserId)+', ModifiedDate = '''+CONVERT(NVARCHAR(30),@GetDate,121)+''' '+CASE WHEN ISNULL(@UpdateColumnString,'') = '' THEN '' ELSE ','+@UpdateColumnString END+' 
						FROM ZnodeAddress ZA
						INNER JOIN #InsertCustomerAddress IC ON '+CASE WHEN ISNULL(@WhereConditionString,'') = '' THEN ' 1 = 0 ' ELSE @WhereConditionString END

				EXEC (@SSQL)
								
				SET @SSQL = '		
				Insert into ZnodeAddress (FirstName,LastName,DisplayName,Address1,Address2,Address3,CountryName,
										StateName,CityName,PostalCode,PhoneNumber,
										IsDefaultBilling,IsDefaultShipping,IsActive,ExternalId,CompanyName,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)		
				OUTPUT INSERTED.AddressId , inserted.Address3 INTO  #InsertedUserAddress (AddressId,PortalCode) 			 
				SELECT IC.FirstName,IC.LastName,IC.DisplayName,IC.Address1,IC.Address2, IC.PortalCode +  ''~'' + IC.StoreLocatorName,IC.CountryName,
				IC.StateName,IC.CityName,IC.PostalCode,IC.PhoneNumber,
				isnull(IC.IsDefaultBilling,0),isnull(IC.IsDefaultShipping,0),isnull(IC.IsActive,0),IC.ExternalId,IC.CompanyName, '+CONVERT(VARCHAR(10), @UserId)+' , '''+CONVERT(NVARCHAR(30),@GetDate,121)+''' , '+CONVERT(VARCHAR(10), @UserId)+' ,'''+CONVERT(NVARCHAR(30),@GetDate,121)+'''  
				FROM  #InsertCustomerAddress IC
				WHERE NOT EXISTS(SELECT * FROM ZnodeAddress ZA WHERE '+CASE WHEN ISNULL(@WhereConditionString,'') = '' THEN ' 1 = 0 ' ELSE @WhereConditionString END +')'
				--print @SSQL
				EXEC (@SSQL)
				
				DECLARE @AddressColumnString1 VARCHAR(1000), @WhereConditionString1 VARCHAR(1000), @UpdateColumnString1 VARCHAR(1000)

				SELECT @AddressColumnString1 = COALESCE(@AddressColumnString1 + ',', '')+a.ColumnName --COALESCE(@CsvColumnString + ' and ', '') +'ZA.'+ColumnName+' =  IC.'+ColumnName
				FROM ZnodeImportUpdatableColumns a
				INNER JOIN INFORMATION_SCHEMA.COLUMNS b on a.ColumnName = b.COLUMN_NAME  
				INNER JOIN dbo.Split(@CsvColumnString,',')C on b.COLUMN_NAME = c.Item
				WHERE b.TABLE_NAME = 'ZnodePortalAddress' 
				AND EXISTS(SELECT * FROM ZnodeImportHead IH where a.ImportHeadId = IH.ImportHeadId and IH.Name = 'StoreLocator')

				--print @AddressColumnString

				SELECT @UpdateColumnString1 = COALESCE(@UpdateColumnString1 + ' , ', '') +'ZA.'+a.COLUMN_NAME+' =  IC.'+a.COLUMN_NAME  
				FROM INFORMATION_SCHEMA.COLUMNS a
				INNER JOIN dbo.Split(@CsvColumnString,',')b on a.COLUMN_NAME = b.Item
				WHERE NOT EXISTS (SELECT * FROM dbo.Split(@AddressColumnString1,',') c WHERE a.COLUMN_NAME = c.Item )
				AND a.TABLE_NAME = 'ZnodePortalAddress'

				SELECT @WhereConditionString1 = COALESCE(@WhereConditionString1 + ' AND ', '') +'ZA.'+item+' =  IC.'+item from dbo.split(@AddressColumnString1,',')

				DECLARE @AccountId INT
				SELECT @AccountId = AccountId FROM ZnodeUser where UserId = @UserId

				SET @SSQL = '
						UPDATE ZA set ModifiedBy = '+CONVERT(VARCHAR(10), @UserId)+' ,ModifiedDate = '''+CONVERT(NVARCHAR(30),@GetDate,121)+''' '+CASE WHEN ISNULL(@UpdateColumnString1,'') = '' THEN '' ELSE ','+@UpdateColumnString1 END+' 
						FROM ZnodePortalAddress ZA
						INNER JOIN #InsertCustomerAddress IC ON '+CASE WHEN ISNULL(@WhereConditionString1,'') = '' THEN ' 1 = 0 ' ELSE @WhereConditionString1 END
				--print @SSQL		
				EXEC (@SSQL)

				SET @SSQL = '
				INSERT INTO ZnodePortalAddress ( PortalId,AddressId,MediaId,StoreName,DisplayOrder,Latitude,Longitude
				,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate )
				Select ZP.PortalId ,IUA.AddressId,NULL,IC.StoreLocatorName, IC.DisplayOrder,IC.Latitude,IC.Longitude 
				,'+CONVERT(VARCHAR(10), @UserId)+' , '''+CONVERT(NVARCHAR(30),@GetDate,121)+''' '+' , '+CONVERT(VARCHAR(10), @UserId)+' , '''+CONVERT(NVARCHAR(30),@GetDate,121)+'''    
				from #InsertCustomerAddress IC INNER JOIN #InsertedUserAddress IUA ON 
				IC.PortalCode + ''~'' + IC.StoreLocatorName  = IUA.PortalCode
				INNER JOIN ZnodePortal ZP on IC.PortalCode = ZP.Code
				WHERE NOT EXISTS ( SELECT * FROM ZnodePortalAddress ZA WHERE '+CASE WHEN ISNULL(@WhereConditionString1,'') = '' THEN ' 1 = 0 ' ELSE @WhereConditionString1 END +')'
				--print @SSQL
				EXEC (@SSQL)
				
				UPDATE ZA SET ZA.Address3 = null 
				From ZnodeAddress ZA INNER JOIN #InsertedUserAddress IUA ON ZA.AddressId = IUA.AddressId 

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

		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportStoreLocatorAddress @TableName = '+CAST(@TableName AS VARCHAR(max)) +',@Status='+ CAST(@Status AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',@CsvColumnString='+CAST(@CsvColumnString AS VARCHAR(max));

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
		@ProcedureName = 'Znode_ImportStoreLocatorAddress',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
		
	END CATCH;
END;

CREATE PROCEDURE [dbo].[Znode_ImportVoucher](
	  @TableName nvarchar(100), @Status bit OUT, @UserId int, @ImportProcessLogId int, @NewGUId nvarchar(200), @PimCatalogId int= 0)
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Import Attribute Code Name and their default input validation rule other 
	--			  flag will be inserted as default we need to modify front end
	
	-- Unit Testing: 

	--------------------------------------------------------------------------------------
BEGIN
	BEGIN TRAN Voucher;
	BEGIN TRY
		DECLARE @MessageDisplay nvarchar(100), @SSQL nvarchar(max);
		DECLARE @GetDate datetime= dbo.Fn_GetDate(), @LocaleId int  ;
		SELECT @LocaleId = DBO.Fn_GetDefaultLocaleId();
		-- Retrive RoundOff Value from global setting 

		DECLARE @InsertVoucherData TABLE
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY,RowNumber int, StoreCode varchar(400),VoucherName varchar(600), VoucherNumber varchar(600),
			VoucherAmount varchar(600),UserName varchar(600), ExpirationDate Datetime,IsActive varchar(100),RemainingAmount varchar(600),
			RestrictVoucherToACustomer varchar(100), StartDate datetime, GUID nvarchar(400)
		);
		
		SET @SSQL = 'Select RowNumber,StoreCode, VoucherName,VoucherNumber,VoucherAmount,UserName,ExpirationDate,
						IsActive,RemainingAmount,RestrictVoucherToACustomer,StartDate,GUID FROM '+@TableName;
		INSERT INTO @InsertVoucherData( RowNumber,StoreCode, VoucherName,VoucherNumber,VoucherAmount,UserName,ExpirationDate,
										IsActive,RemainingAmount,RestrictVoucherToACustomer,StartDate,GUID)
		EXEC sys.sp_sqlexec @SSQL;

		select ANZU.UserName, ANU.Id, ZU.UserId
		into #TempUserData
		from AspNetZnodeUser ANZU 
		inner join AspNetUsers ANU ON ANZU.AspNetZnodeUserId = ANU.UserName
		inner join ZnodeUser ZU ON ANU.Id = ZU.AspNetUserId

		select ZUP.PortalId, ZUP.UserId, ZP.StoreCode
		into #TempUserPortal
		from ZnodeUserPortal ZUP 
		INNER JOIN ZnodePortal ZP ON ZUP.PortalId = ZP.PortalId

		update @InsertVoucherData set VoucherNumber = [dbo].[Fn_RandomString](10)
		where isnull(ltrim(rtrim(VoucherNumber)),'') = ''

		-- Start Functional Validation 
		-----------------------------------------------
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '120', 'StoreCode', StoreCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertVoucherData AS ii
			   WHERE ii.StoreCode not in 
			   (
				   SELECT StoreCode FROM ZnodePortal 
			   );

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '65', 'ExpirationDate', ExpirationDate, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertVoucherData AS ii
			   WHERE (ii.ExpirationDate < ii.StartDate OR ii.ExpirationDate < @GetDate)

		--INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		--	   SELECT '65', 'ExpirationDate', ExpirationDate, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		--	   FROM @InsertVoucherData AS ii
		--	   WHERE ii.ExpirationDate < @GetDate

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '8', 'StartDate', StartDate, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertVoucherData AS ii
			   WHERE isnull(ii.StartDate,'') = ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '8', 'ExpirationDate', StartDate, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertVoucherData AS ii
			   WHERE isnull(ii.ExpirationDate,'') = ''
		
		--INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		--	   SELECT '67', 'UserName', UserName, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		--	   FROM @InsertVoucherData AS ii
		--	   WHERE isnull(ii.UserName,'') <> '' and not exists ( SELECT VoucherNumber FROM #TempUserData U where ii.UserName = U.UserName);
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '67', 'UserName', UserName, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertVoucherData AS ii			 
			   WHERE isnull(ii.UserName,'') <> '' 
			   and not exists ( SELECT * FROM #TempUserData U   
			   inner join  #TempUserPortal UP on U.UserId = UP.UserId where  ii.UserName = U.UserName and ii.StoreCode = UP.StoreCode)
			 

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '71', 'VoucherNumber', VoucherNumber, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM @InsertVoucherData AS ii
			   WHERE len(ltrim(rtrim(ii.VoucherNumber))) <> 10 
	
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT '68', 'IsActive', IsActive, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM @InsertVoucherData AS ii  
			WHERE ii.IsActive not in ('True','1','Yes','FALSE','0','No','')
		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT '5', 'ExpirationDate', ExpirationDate, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM @InsertVoucherData AS ii  
			WHERE isdate(ii.ExpirationDate) = 0

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT '5', 'StartDate', StartDate, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM @InsertVoucherData AS ii  
			WHERE isdate(ii.StartDate) = 0

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT '69', 'RemainingAmount', RemainingAmount, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM @InsertVoucherData AS ii  
			WHERE ii.VoucherAmount <> ii.RemainingAmount

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT '68', 'RestrictVoucherToACustomer', RestrictVoucherToACustomer, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM @InsertVoucherData AS ii  
			WHERE ii.RestrictVoucherToACustomer not in ('True','1','Yes','FALSE','0','No','')

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT distinct '2', 'VoucherAmount', VoucherAmount, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM @InsertVoucherData AS ii  
			CROSS APPLY ZnodeCulture b
			WHERE  VoucherAmount like '%[a-z]%' or VoucherAmount like '%'+b.Symbol+'%'
			and b.Symbol is not null 

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT distinct '2', 'RemainingAmount', RemainingAmount, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM @InsertVoucherData AS ii
			CROSS APPLY ZnodeCulture b
			WHERE  RemainingAmount like '%[a-z]%' or RemainingAmount like '%'+b.Symbol+'%'
			and b.Symbol is not null 


		UPDATE ZIL
			   SET ZIL.ColumnName =   ZIL.ColumnName + ' [ VoucherName - ' + ISNULL(VoucherName,'') + ' ] '
			   FROM ZnodeImportLog ZIL 
			   INNER JOIN @InsertVoucherData IPA ON (ZIL.RowNumber = IPA.RowNumber)
			   WHERE  ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL

		-- End Function Validation 	
		-----------------------------------------------
		-- Delete Invalid Data after functional validatin  
		DELETE FROM @InsertVoucherData
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
		Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM @InsertVoucherData
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount ,
		TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0)) 
		WHERE ImportProcessLogId = @ImportProcessLogId;
	
		UPDATE ZGC set ExpirationDate = ICD.ExpirationDate, UserId = ZU.UserId, ModifiedBy = @UserId, ModifiedDate = @GetDate, IsActive = ICD.IsActive,
				RemainingAmount = ICD.RemainingAmount, RestrictToCustomerAccount = ICD.RestrictVoucherToACustomer, Name = ICD.VoucherName, StartDate = ICD.StartDate
				
		from ZnodeGiftCard ZGC
		inner join @InsertVoucherData ICD ON ICD.VoucherNumber = ZGC.CardNumber
		inner join ZnodePortal ZP ON ICD.StoreCode = ZP.StoreCode and ZGC.PortalId = ZP.PortalId
		left join #TempUserData ZU ON ICD.UserName = ZU.UserName

		insert into ZnodeGiftCard(PortalId,Name,CardNumber,Amount,UserId,ExpirationDate,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsActive,RemainingAmount,RestrictToCustomerAccount,StartDate)
		select ZP.PortalId, ICD.VoucherName, ICD.VoucherNumber, ICD.VoucherAmount,ZU.UserId, ICD.ExpirationDate, @UserId, @Getdate, @UserId, @Getdate, ICD.IsActive, ICD.RemainingAmount, ICD.RestrictVoucherToACustomer, ICD.StartDate
		From @InsertVoucherData ICD 
		inner join ZnodePortal ZP ON ICD.StoreCode = ZP.StoreCode 
		left join #TempUserData ZU ON ICD.UserName = ZU.UserName
		where not exists(select * from ZnodeGiftCard ZGC where ICD.VoucherNumber = ZGC.CardNumber )

		SET @GetDate = dbo.Fn_GetDate();
		--Updating the import process status
		UPDATE ZnodeImportProcessLog
		SET Status = CASE WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 4 )
							WHEN ISNULL(@FailedRecordCount,0) = 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 2 )
							WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) = 0 THEN dbo.Fn_GetImportStatus( 3 )
						END, 
			ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		COMMIT TRAN Voucher;
	END TRY
	BEGIN CATCH
	ROLLBACK TRAN Voucher

		SET @Status = 0;
		SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE();

		 DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportVoucher @TableName = '+CAST(@TableName AS VARCHAR(max)) +',@Status='+ CAST(@Status AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',@PimCatalogId='+CAST(@PimCatalogId AS VARCHAR(max));

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
		@ProcedureName = 'Znode_ImportVoucher',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	END CATCH;
END;
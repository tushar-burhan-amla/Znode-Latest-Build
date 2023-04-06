CREATE PROCEDURE [dbo].[Znode_ImportBrands](
	  @TableName			NVARCHAR(100), 
	  @Status				BIT OUT, 
	  @UserId				INT, 
	  @ImportProcessLogId	INT, 
	  @NewGUId				NVARCHAR(200),
	  @LocaleId	            INT= 1,
	  @CsvColumnString		NVARCHAR(max)
	  )
AS
	--------------------------------------------------------------------------------------
	-- Summary :  Made a provision to import Brand details.
	
	-- Unit Testing: 

	--------------------------------------------------------------------------------------
BEGIN
	BEGIN TRAN Brands;
	BEGIN TRY
		DECLARE @MessageDisplay nvarchar(100), @SSQL nvarchar(max);
		DECLARE @GetDate datetime= dbo.Fn_GetDate();
		IF isnull(@LocaleId,0)=0
		BEGIN
			SELECT @LocaleId = DBO.Fn_GetDefaultLocaleId();
		END
		-- Retrive RoundOff Value from global setting 

		Create TABLE #InsertBrandData 
		( 
			RowId int IDENTITY(1, 1) PRIMARY KEY,RowNumber int, BrandCode varchar(max),IsActive varchar(max), BrandDescription varchar(max) default null,
			SEOKeyword varchar(max) default null,SEODescription varchar(max) default null, BrandLogo varchar(max) default null,SEOTitle varchar(max) default null,
			SEOFriendlyPageName varchar(max) default null,URLKey varchar(max) default null, Custom1 varchar(max) default null,Custom2 varchar(max) default null,
			Custom3 varchar(max) default null,Custom4 varchar(max) default null,Custom5 varchar(max) default null, GUID nvarchar(400)
		);	
			
		SET @SSQL = 'INSERT INTO #InsertBrandData( RowNumber,' + @CsvColumnString + ',GUID)
		             SELECT RowNumber,' + @CsvColumnString + ',GUID FROM '+ @TableName;		
		EXEC sys.sp_sqlexec @SSQL;

		UPDATE #InsertBrandData SET IsActive=CASE  WHEN IsActive IN ('Yes', 'True','1') THEN 1 WHEN IsActive IN ('No', 'False','0') THEN  0 END
		
		SELECT BrandCode 
		INTO #DuplicateBrandData 
		FROM #InsertBrandData 
		Group BY BrandCode  having count(*) > 1
		
		-- Start Functional Validation 
		-----------------------------------------------
		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '53', 'BrandCode', BrandCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM #InsertBrandData AS ii
			   WHERE exists(SELECT * FROM #DuplicateBrandData bd where bd.BrandCode=ii.BrandCode)
			   And not exists(select * from  ZnodeBrandDetails zbd 
							  INNER JOIN #DuplicateBrandData bd on bd.BrandCode=zbd.BrandCode) 

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '8', 'BrandCode', BrandCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM #InsertBrandData AS ii
			   WHERE isnull(BrandCode,'')=''			   
			  

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '121', 'BrandCode', BrandCode, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM #InsertBrandData AS ii
			   WHERE  not exists 
				   (
						select * from ZnodePimAttributeDefaultValue zpadv
						inner join ZnodePimAttribute zpa on zpa.PimAttributeId=zpadv.PimAttributeId
						where AttributeCode='Brand' and AttributeDefaultValueCode=ii.BrandCode
				   );

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT '8', 'IsActive', IsActive, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM #InsertBrandData AS ii  
			WHERE isnull(ii.IsActive,'')=''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT '68', 'IsActive', IsActive, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM #InsertBrandData AS ii  
			WHERE isnull(ii.IsActive,'') not in ('True','1','Yes','FALSE','0','No')
			
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '82', 'SEOKeyword', SEOKeyword, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM #InsertBrandData AS ii
			  WHERE len(ltrim(rtrim(ii.SEOKeyword))) > 300 and isnull(ii.SEOKeyword,'')<>''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '82', 'SEODescription', SEODescription, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM #InsertBrandData AS ii
			   WHERE len(ltrim(rtrim(ii.SEODescription))) > 300 and isnull(ii.SEODescription,'')<>''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '45', 'BrandLogo', BrandLogo, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM #InsertBrandData AS ii			 
			   WHERE isnull(BrandLogo,'')<>'' AND reverse(left(reverse(ii.BrandLogo),charindex('.',reverse(ii.BrandLogo)))) 
			   not in (select ValidationName from View_FamilyExtensions where FamilyCode='Image')
		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '45', 'BrandLogo', BrandLogo, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM #InsertBrandData AS ii			 
			   WHERE isnull(BrandLogo,'')<>'' and not exists (select * from ZnodeMedia where FileName=ii.BrandLogo)

  
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '81', 'SEOTitle', SEOTitle, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM #InsertBrandData AS ii
			   WHERE len(ltrim(rtrim(ii.SEOTitle))) > 200 and isnull(ii.SEOTitle,'')<>''		
		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT '102', 'SEOFriendlyPageName', SEOFriendlyPageName, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM #InsertBrandData AS ii  
			WHERE (isnull(ii.SEOFriendlyPageName,'') LIKE '%[^a-zA-Z0-9]%' and  isnull(ii.SEOFriendlyPageName,'') NOT LIKE '%[_-]%' )

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )  
			SELECT '81', 'URLKey', URLKey, @NewGUId, RowNumber, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId  
			FROM #InsertBrandData AS ii  
			WHERE len(ltrim(rtrim(ii.URLKey))) > 200 and isnull(ii.URLKey,'')<>''			


		UPDATE ZIL
			   SET ZIL.ColumnName =   ZIL.ColumnName + ' [ Brand - ' + ISNULL(BrandCode,'') + ' ] '
			   FROM ZnodeImportLog ZIL 
			   INNER JOIN #InsertBrandData IPA ON (ZIL.RowNumber = IPA.RowNumber)
			   WHERE  ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL

		-- End Function Validation 	
		-----------------------------------------------
		-- Delete Invalid Data after functional validatin  
		DELETE FROM #InsertBrandData
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
		Select @SuccessRecordCount = count(DISTINCT RowNumber) FROM #InsertBrandData
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount ,
		TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0)) 
		WHERE ImportProcessLogId = @ImportProcessLogId;
		
		--<Begin Data Updation>
		CREATE TABLE #InsertedBrands (BrandId INT,BrandCode nvarchar(100),CMSSEODetailId int ) 
		
		UPDATE zbd set WebsiteLink=isnull(ibd.URLKey,WebsiteLink),IsActive=isnull(ibd.IsActive,zbd.IsActive),ModifiedBy=@UserId,
					   ModifiedDate=@GetDate,Custom1=isnull(ibd.Custom1,zbd.custom1),Custom2=isnull(ibd.Custom2,zbd.custom2),Custom3=isnull(ibd.Custom3,zbd.custom3),Custom4=isnull(ibd.Custom4,zbd.custom4),
					   Custom5=ISNULL(ibd.Custom5, zbd.Custom5)      
		OUTPUT INSERTED.BrandId ,INSERTED.BrandCode INTO  #InsertedBrands (BrandId, BrandCode)
		from ZnodeBrandDetails zbd
		inner join #InsertBrandData IBD on ibd.BrandCode=zbd.BrandCode
		where ibd.RowNumber=(select max(RowNumber) from #InsertBrandData a where a.BrandCode=zbd.BrandCode)

		UPDATE zbdl set zbdl.BrandId=zbd.BrandId,zbdl.Description=isnull(ibd.BrandDescription,zbdl.Description),SEOFriendlyPageName=isnull(IBD.SEOFriendlyPageName,zbdl.SEOFriendlyPageName),
		LocaleId=@LocaleId,ModifiedBy=@UserId,ModifiedDate=@GetDate,BrandName=isnull(ibd.BrandCode,zbdl.BrandName)
		FROM ZnodeBrandDetailLocale zbdl
		inner join ZnodeBrandDetails zbd on zbd.BrandId=zbdl.BrandId
		inner join #InsertBrandData IBD on ibd.BrandCode=zbd.BrandCode
		inner join #InsertedBrands ib on ib.BrandId=zbdl.BrandId
		where ibd.RowNumber=(select max(RowNumber) from #InsertBrandData a where a.BrandCode=zbd.BrandCode)

		UPDATE ZCD set SEOId=ib.BrandId,SEOUrl=isnull(ibd.SEOFriendlyPageName,zcd.SEOUrl),ModifiedBy=@UserId,ModifiedDate=@GetDate,
					   SEOCode=isnull(ibd.BrandCode,zcd.SEOCode)
		OUTPUT INSERTED.SEOId ,INSERTED.SEOCode, INSERTED.CMSSEODetailId INTO  #InsertedBrands (BrandId, BrandCode,CMSSEODetailId) 
		FROM ZnodeCMSSEODetail ZCD		
		inner join #InsertedBrands ib on ib.BrandId=ZCD.SEOId
		inner join #InsertBrandData IBD on ibd.BrandCode=zcd.SEOCode
		where exists (select null from ZnodeBrandDetailLocale zbdl where zbdl.BrandId=ZCD.SEOId)
		and ibd.RowNumber=(select max(RowNumber) from #InsertBrandData a where a.BrandCode=zcd.SEOCode)
		
		--<Delete Records Having Null Value>
		Delete from #InsertedBrands where isnull(CMSSEODetailId,0)=0
		--</Delete Records Having Null Value>
		
		UPDATE zcdl set CMSSEODetailId =isnull(ib.CMSSEODetailId,zcdl.CMSSEODetailId),LocaleId=@LocaleId,SEOTitle=isnull(ibd.SEOTitle,zcdl.SEOTitle),
						SEODescription=isnull(ibd.SEODescription,zcdl.SEODescription),SEOKeywords=isnull(ibd.SEOKeyword,zcdl.SEOKeywords),ModifiedBy=@UserId,ModifiedDate=@GetDate
		FROM ZnodeCMSSEODetailLocale zcdl
		inner join #InsertedBrands ib on ib.CMSSEODetailId=zcdl.CMSSEODetailId
		inner join #InsertBrandData IBD on ibd.BrandCode=ib.BrandCode
		and ibd.RowNumber=(select max(RowNumber) from #InsertBrandData a 
		inner join ZnodeBrandDetails zcd on  a.BrandCode=zcd.BrandCode)

		--</End Data Updation>

		--<Begin Data Insert>
		insert into ZnodeBrandDetails(BrandCode,MediaId,WebsiteLink,DisplayOrder,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Custom1,Custom2,Custom3,
										Custom4,Custom5)
		OUTPUT INSERTED.BrandId ,INSERTED.BrandCode INTO  #InsertedBrands (BrandId, BrandCode) 		
		Select ibd.BrandCode,(select top 1 MediaId from ZnodeMedia where FileName=ibd.BrandLogo order by CreatedDate desc),ibd.URLKey,0,ibd.IsActive,@UserId,@GetDate,@UserId,@GetDate,ibd.Custom1,ibd.Custom2,ibd.Custom3,ibd.Custom4,ibd.Custom5
		from #InsertBrandData IBD	
		WHERE NOT EXISTS(SELECT * FROM ZnodeBrandDetails ZBD WHERE ZBD.BrandCode = IBD.BrandCode )		

		insert into ZnodeBrandDetailLocale (BrandId,Description,SEOFriendlyPageName,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,BrandName)
		select ac.BrandId,ibdl.BrandDescription,ibdl.SEOFriendlyPageName,@LocaleId,@UserId,@GetDate,@UserId,@GetDate,ibdl.BrandCode 
		from #InsertBrandData IBDL
		INNER JOIN #InsertedBrands ac on ac.BrandCode=ibdl.BrandCode
		WHERE NOT EXISTS(SELECT * FROM ZnodeBrandDetailLocale a 
							INNER JOIN ZnodeBrandDetails b on  b.BrandCode=ibdl.BrandCode AND a.BrandId=b.BrandId )	

		insert into ZnodeCMSSEODetail (CMSSEOTypeId,SEOId,SEOUrl,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,SEOCode,PublishStateId)
		OUTPUT INSERTED.SEOId ,INSERTED.SEOCode, INSERTED.CMSSEODetailId INTO  #InsertedBrands (BrandId, BrandCode,CMSSEODetailId)
		select (select CMSSEOTypeId  from ZnodeCMSSEOType where Name = 'Brand'),ac.BrandId,ibdl.SEOFriendlyPageName,@UserId,@GetDate,@UserId,@GetDate,ibdl.BrandCode,'2'
		from #InsertBrandData IBDL
		INNER JOIN #InsertedBrands ac on ac.BrandCode=ibdl.BrandCode
		WHERE NOT EXISTS(SELECT * FROM ZnodeCMSSEODetail a 
							INNER JOIN ZnodeBrandDetails b on b.BrandCode=ibdl.BrandCode 
							where a.SEOCode=ibdl.BrandCode
							AND a.CMSSEOTypeId=(select CMSSEOTypeId  from ZnodeCMSSEOType where Name = 'Brand')  )
		
		--<Delete Records Having Null Value>
		Delete from #InsertedBrands where isnull(CMSSEODetailId,0)=0
		--</Delete Records Having Null Value>

		insert into ZnodeCMSSEODetailLocale (CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		select ac.CMSSEODetailId ,@LocaleId,ibdl.SEOTitle,ibdl.SEODescription,ibdl.SEOKeyword,@UserId,@GetDate,@UserId,@GetDate
		from #InsertBrandData IBDL
		INNER JOIN #InsertedBrands ac on ac.BrandCode=ibdl.BrandCode
		WHERE NOT EXISTS(SELECT * FROM ZnodeCMSSEODetailLocale a 
							INNER JOIN ZnodeCMSSEODetail b on a.CMSSEODetailId=b.CMSSEODetailId and b.SEOCode=ibdl.BrandCode
							INNER JOIN ZnodeBrandDetails c on c.BrandCode=ibdl.BrandCode 
							AND b.CMSSEOTypeId=(select CMSSEOTypeId  from ZnodeCMSSEOType where Name = 'Brand')  )		
        --<End Data Insert>

		SET @GetDate = dbo.Fn_GetDate();

		--Updating the import process status
		UPDATE ZnodeImportProcessLog
		SET Status = CASE WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 4 )
							WHEN ISNULL(@FailedRecordCount,0) = 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 2 )
							WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) = 0 THEN dbo.Fn_GetImportStatus( 3 )
						END, 
			ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		COMMIT TRAN Brands;
	END TRY
	BEGIN CATCH
	ROLLBACK TRAN Brands

		SET @Status = 0;
		SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE();

		 DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportBrands @TableName = '+CAST(@TableName AS VARCHAR(max)) +',
		 @Status='+ CAST(@Status AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(200))+',
		 @CsvColumnString='+CAST(@CsvColumnString AS VARCHAR(max)) ;

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
		@ProcedureName = 'Znode_ImportBrands',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
	END CATCH;
END;
go
CREATE PROCEDURE [dbo].[Znode_ImportPriceList]
(
	@TableName nvarchar(100),
	@Status bit OUT, 
	@UserId int, 
	@ImportProcessLogId int,
	@NewGUId nvarchar(200),
	@PriceListId int )
AS 
	/*
	----Summary:  Import RetailPrice List 
	----		  Input XML data extracted in table format (table variable name:  #InsertPriceForValidation) by using  @xml.nodes 
	----		  Validate data column wise and store error log into @ErrorLogForInsertPrice table 
	----          Remove wrong data from table #InsertPriceForValidation and inserted correct data into @InsertPrice table for 
	----		  further processing (Importing to target database )
	---- Version 1 : Required Validation 
	---- UomName should not be null 
	---- Data for this RetailPrice list is already available  
	---- Version 2 : Required Validation 
	---- If UomName will be null then insert first record from UomTable and If UomName is wrong then raise error
	---- SKU with retailprice data is available with price list id will insert 
	---- multiple SKU with retail price is available then updated last sku details to price table and price tier table for respective price list
	----1. Import functionality should be provided only for single price list (Validate - Pending) 
	----  Tier price : TierStartQuantity should not between TierStartQuantity and TierEndQuantity for already existing SKU 
	----  In case of update details for SKU if any kind of price value will null then avoid it to update on existing value. 
	----2. From XML only SKU and RetailPrice is mandatory
	----3. SKUActivation date sholud be less than SKUExpriration date
	----4. Activation date sholud be less than Expiration date
	----5. If Tier RetailPrice has values and TierSartQuantity /TierEndQuantity or both has null value then it should not get updated/created.
	----6. ActivationDate and ExpirationDate value for tier price will be SKUActivationDate SKUExprirationDate 
	--- Change History : 
	--Remove column which is used to store range of qunatity by single column Quantity from table ZnodeTierProduct 
	--Manditory Retail price in Znodepricetable 
	-- SKUActivationfrom date and to date will used for tier price will store in single table ZnodePrice
	--Unit Testing   
	
*/
BEGIN
	BEGIN TRAN A;
	BEGIN TRY
	    DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
		
		IF OBJECT_ID('#InsertPriceForValidation', 'U') IS NOT NULL 
			DROP TABLE #InsertPriceForValidation
		ELSE 
			CREATE TABLE #InsertPriceForValidation 
			(SKU varchar(300) NULL, TierStartQuantity varchar(300) NULL, RetailPrice varchar(300) NULL, SalesPrice varchar(300) NULL, TierPrice varchar(300) NULL, SKUActivationDate varchar(300) NULL, SKUExpirationDate varchar(300) NULL,
			Custom1 varchar(300) NULL, Custom2 varchar(300) NULL, Custom3 varchar(300) NULL,CostPrice varchar(100), RowNumber varchar(300) NULL)

		IF OBJECT_ID('#InsertPrice', 'U') IS NOT NULL 
			DROP TABLE #InsertPrice
		ELSE 
			CREATE TABLE #InsertPrice 
			( 
				SKU varchar(300), TierStartQuantity numeric(28, 6) NULL, RetailPrice numeric(28, 6) NULL, SalesPrice numeric(28, 6) NULL, TierPrice numeric(28, 6) NULL, SKUActivationDate varchar(300) NULL, SKUExpirationDate varchar(300) NULL,
				Custom1 varchar(300) NULL, Custom2 varchar(300) NULL, Custom3 varchar(300) NULL,CostPrice numeric(28, 6), RowNumber varchar(300)
			);
	
	
		DECLARE @SKU TABLE
		( 
				SKU nvarchar(300)
		);
		INSERT INTO @SKU
			   SELECT b.AttributeValue
			   FROM ZnodePimAttributeValue AS a
					INNER JOIN
					ZnodePimAttributeValueLocale AS b
					ON a.PimAttributeId = dbo.Fn_GetProductSKUAttributeId() AND 
					   a.PimAttributeValueId = b.PimAttributeValueId;


		DECLARE @RoundOffValue int, @MessageDisplay nvarchar(100); 
		-- Retrive RoundOff Value from global setting 

		SELECT @RoundOffValue = FeatureValues FROM ZnodeGlobalSetting WHERE FeatureName = 'PriceRoundOff';
	
		--@MessageDisplay will use to display validate message for input inventory value  

		DECLARE @sSql nvarchar(max);
		SET @sSql = ' Select @MessageDisplay_new = Convert(Numeric(28, '+CONVERT(nvarchar(200), @RoundOffValue)+'), 999999.000000000 ) ';
		EXEC SP_EXecutesql @sSql, N'@MessageDisplay_new NVARCHAR(100) OUT', @MessageDisplay_new = @MessageDisplay OUT;
		

		SET @SSQL = 'Select SKU,TierStartQuantity ,RetailPrice,SalesPrice,TierPrice,SKUActivationDate ,SKUExpirationDate ,
		 Custom1, Custom2, Custom3,CostPrice, RowNumber FROM '+@TableName;
		INSERT INTO #InsertPriceForValidation( SKU, TierStartQuantity, RetailPrice, SalesPrice, TierPrice, SKUActivationDate, SKUExpirationDate,
		 Custom1, Custom2, Custom3,CostPrice, RowNumber )
		EXEC sys.sp_sqlexec @SSQL;

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
				SELECT '113', 'TierPrice', TierPrice, @NewGUId, RowNumber , @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
				FROM #InsertPriceForValidation
				WHERE  CONVERT(varchar(100), TierPrice) ='' AND CONVERT(varchar(100), TierStartQuantity) <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
				SELECT '114', 'TierStartQuantity', TierStartQuantity, @NewGUId, RowNumber , @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
				FROM #InsertPriceForValidation
				WHERE  CONVERT(varchar(100), TierStartQuantity) ='' AND CONVERT(varchar(100), TierPrice) <> ''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
				SELECT '2', 'TierPrice', TierPrice, @NewGUId, RowNumber , @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
				FROM #InsertPriceForValidation
				WHERE (isnumeric(TierPrice)=0  
				or exists(select * from ZnodeCulture where Symbol is not null and TierPrice like '%'+Symbol+'%')) and ISNULL(TierPrice,'')<>''
		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
				SELECT '2', 'SalesPrice', SalesPrice, @NewGUId, RowNumber , @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
				FROM #InsertPriceForValidation
				WHERE (isnumeric(SalesPrice)=0	or exists(select * from ZnodeCulture where Symbol is not null and SalesPrice like '%'+Symbol+'%'))
				and ISNULL(SalesPrice,'')<>''

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
				SELECT '2', 'RetailPrice', RetailPrice, @NewGUId, RowNumber , @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
				FROM #InsertPriceForValidation
				WHERE (isnumeric(RetailPrice)=0 or exists(select * from ZnodeCulture where Symbol is not null and RetailPrice like '%'+Symbol+'%')) and ISNULL(RetailPrice,'')<>''
		
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
				SELECT '2', 'CostPrice', CostPrice, @NewGUId, RowNumber , @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId
				FROM #InsertPriceForValidation
				WHERE (isnumeric(CostPrice)=0	or exists(select * from ZnodeCulture where Symbol is not null and CostPrice like '%'+Symbol+'%'))
				and ISNULL(CostPrice,'')<>''

			INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
	        SELECT '53', 'TierStartQuantity', TierStartQuantity, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
	        FROM #InsertPriceForValidation 
			   WHERE TierStartQuantity IN
			   (
				   select TierStartQuantity from #InsertPriceForValidation
					group by TierStartQuantity
					having count(1)>1
			   ) AND ISNULL(TierStartQuantity,'') <> ''

		UPDATE ZIL
			   SET ZIL.ColumnName =   ZIL.ColumnName + ' [ SKU - ' + ISNULL(SKU,'') + ' ] '
			   FROM ZnodeImportLog ZIL 
			   INNER JOIN #InsertPriceForValidation IPA ON (ZIL.RowNumber = IPA.RowNumber)
			   WHERE  ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL
			   			  	
	    --- Delete Invalid Data after functional validation 
		DELETE FROM #InsertPriceForValidation
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId AND 
				  Guid = @NewGUId
		);
		-- 1)  Validation for SKU is pending Proper data not found and 
		--Discussion still open for Publish version where we create SKU and use the SKU code for validation 
		--------------------------------------------------------------------------------------
		--2)  Start Data Type Validation for XML Data  
		--------------------------------------------------------------------------------------			
		---------------------------------------------------------------------------------------
		---------If UOM will blank then retrive top -- Finctionality pending 
		---Validate 
		
		INSERT INTO #InsertPrice( SKU, TierStartQuantity, RetailPrice, SalesPrice, TierPrice, SKUActivationDate, SKUExpirationDate,
		 Custom1, Custom2, Custom3,CostPrice, RowNumber )
			   SELECT SKU,
					  CASE
					  WHEN CONVERT(Varchar(100),TierStartQuantity) = '' THEN 0
					  ELSE CONVERT(numeric(28, 6), TierStartQuantity)
					  END, CONVERT(numeric(28, 6), RetailPrice),
															  CASE
															  WHEN SalesPrice = '' THEN NULL
															  ELSE CONVERT(numeric(28, 6), SalesPrice)
															  END,
															  CASE
															  WHEN TierPrice = '' THEN NULL
															  ELSE CONVERT(numeric(28, 6), TierPrice)
															  END, SKUActivationDate, SKUExpirationDate,
															   Custom1, Custom2, Custom3,
															   CASE
															  WHEN CostPrice = '' THEN NULL
															  ELSE CONVERT(numeric(28, 6), CostPrice)
															  END, RowNumber
			   FROM #InsertPriceForValidation;
			 
		--------------------------------------------------------------------------------------
		--- start Functional Validation 
		--------------------------------------------------------------------------------------
		--- Verify SKU is present or not 

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '98', 'SKU', SKU, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertPrice AS ii
		WHERE ii.SKU NOT IN
		(
			SELECT SKU
			FROM @SKU
		);

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '39', 'SKUActivationDate', SKUActivationDate, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM #InsertPrice AS IP
			   WHERE cast(SKUActivationDate as datetime) > cast(SKUExpirationDate as datetime) AND 
					 ISNULL(SKUExpirationDate, '') <> '';
					 
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '108', 'TierStartQuantity', TierStartQuantity, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM #InsertPriceForValidation
			   WHERE( TierPrice IS NULL OR TierPrice = '0') AND  TierStartQuantity  = '';
			  
			  
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '109', 'TierPrice', TierPrice, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM #InsertPriceForValidation WHERE( TierPrice IS NULL OR  TierPrice = '') AND TierStartQuantity  <> 0;

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '26', 'TierStartQuantity', TierStartQuantity, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM #InsertPriceForValidation IPV
			   WHERE TierStartQuantity = ''  
				AND	( TierPrice <> ''  OR TierPrice IS NULL ) 

			  
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
			   SELECT '26', 'TierStartQuantity', TierStartQuantity, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
			   FROM #InsertPriceForValidation IPV
			   WHERE TierStartQuantity <> '' AND 
			    ISNULL(CAST(TierStartQuantity AS numeric(28, 6)), 0) <= 0 
				AND	( TierPrice <> ''  OR TierPrice IS NULL ) 
		
		
				  
		UPDATE ZIL
			   SET ZIL.ColumnName =   ZIL.ColumnName + ' [ SKU - ' + ISNULL(SKU,'') + ' ] '
			   FROM ZnodeImportLog ZIL 
			   INNER JOIN #InsertPrice IPA ON (ZIL.RowNumber = IPA.RowNumber)
			   WHERE  ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL

			 
 	
		-- End Function Validation 	
		---------------------------
		--- Delete Invalid Data after functional validation 
		DELETE FROM #InsertPrice
		WHERE RowNumber IN
		(
			SELECT DISTINCT 
				   RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId AND 
				  Guid = @NewGUId
		);
		
		DECLARE @FailedRecordCount BIGINT, @SuccessRecordCount BIGINT 
	
		SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND  ImportProcessLogId = @ImportProcessLogId;

		SELECT @SuccessRecordCount = COUNT(DISTINCT ROWNUMBER) FROM #InsertPrice WHERE 	ROWNUMBER IS NOT NULL ;

		UPDATE ZnodeImportProcessLog SET FailedRecordcount = @FailedRecordCount , SuccessRecordCount = @SuccessRecordCount,
		TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0)) 
		WHERE ImportProcessLogId = @ImportProcessLogId;

		UPDATE ZP
				SET ZP.SalesPrice = IP.SalesPrice, ZP.RetailPrice = CASE
				WHEN CONVERT(varchar(100), ISNULL(IP.RetailPrice, '')) <> '' THEN IP.RetailPrice
				END, ZP.ActivationDate = CASE
				WHEN ISNULL(IP.SKUActivationDate, '') <> '' THEN IP.SKUActivationDate
				ELSE NULL
				END, ZP.ExpirationDate = CASE
				WHEN ISNULL(IP.SKUExpirationDate, '') <> '' THEN IP.SKUExpirationDate
				ELSE NULL
				END, ZP.ModifiedBy = @UserId, ZP.ModifiedDate = @GetDate,
				ZP.CostPrice =IP.CostPrice
		FROM #InsertPrice IP INNER JOIN ZnodePrice ZP ON ZP.PriceListId = @PriceListId AND  ZP.SKU = IP.SKU  
			 --Retrive last record from price list of specific SKU ListCode and Name 									
		WHERE IP.RowNumber IN
		(
			SELECT MAX(IPI.RowNumber) FROM #InsertPrice AS IPI WHERE IPI.SKU = IP.SKU 
		);
		INSERT INTO ZnodePrice( PriceListId, SKU, SalesPrice, RetailPrice, ActivationDate, ExpirationDate, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate,CostPrice )
			   SELECT @PriceListId, IP.SKU, IP.SalesPrice, IP.RetailPrice,
																						   CASE
																						   WHEN ISNULL(IP.SKUActivationDate, '') = '' THEN NULL
																						   ELSE IP.SKUActivationDate
																						   END,
																						   CASE
																						   WHEN ISNULL(IP.SKUExpirationDate, '') = '' THEN NULL
																						   ELSE IP.SKUExpirationDate
																						   END, @UserId, @GetDate, @UserId, @GetDate,IP.CostPrice
			   FROM #InsertPrice AS IP
			   WHERE NOT EXISTS
			   (
				   SELECT TOP 1 1
				   FROM ZnodePrice
				   WHERE ZnodePrice.PriceListId = @PriceListId AND 
						 ZnodePrice.SKU = IP.SKU 
			   ) AND 
					 IP.RowNumber IN
			   (
					SELECT MAX(IPI.RowNumber)
					FROM #InsertPrice AS IPI
					WHERE IPI.SKU = IP.SKU 
			   );

			 

		IF EXISTS
		(
			SELECT TOP 1 1
			FROM #InsertPrice
			WHERE CONVERT(varchar(100), TierStartQuantity) <> '' AND 
				  (CONVERT(varchar(100), TierPrice) <> '' OR CONVERT (varchar(100), TierPrice) IS NOT NULL)
		)
		BEGIN
		
			UPDATE ZPT
			  SET ZPT.Price = IP.TierPrice, ZPT.ModifiedBy = @UserId, ZPT.ModifiedDate = @GetDate,
			  ZPT.Custom1 = IP.Custom1,ZPT.Custom2 = IP.Custom2, ZPT.Custom3 = IP.Custom3 
			FROM #InsertPrice IP INNER JOIN ZnodePriceTier ZPT ON ZPT.PriceListId = @PriceListId AND  ZPT.SKU = IP.SKU AND ZPT.Quantity = IP.TierStartQuantity 
		    --Retrive last record from price list of specific SKU ListCode and Name 
			WHERE IP.RowNumber IN
			(
				SELECT MAX(IPI.RowNumber) FROM #InsertPrice AS IPI WHERE IPI.SKU = IP.SKU AND IPI.TierStartQuantity = IP.TierStartQuantity 
			);

			INSERT INTO ZnodePriceTier( PriceListId, SKU, Price, Quantity, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Custom1, Custom2, Custom3 )
				   SELECT @PriceListId, IP.SKU, IP.TierPrice, IP.TierStartQuantity,  @UserId, @GetDate, @UserId, @GetDate, Custom1, Custom2, Custom3
				   FROM #InsertPrice AS IP 
				   WHERE NOT EXISTS
				   (
					   SELECT TOP 1 1 FROM ZnodePriceTier WHERE ZnodePriceTier.PriceListId = @PriceListId AND  ZnodePriceTier.SKU = IP.SKU AND 
							 ZnodePriceTier.Quantity = IP.TierStartQuantity
				   ) AND  IP.RowNumber IN
				   (
					   SELECT MAX(IPI.RowNumber) FROM #InsertPrice AS IPI WHERE IPI.SKU = IP.SKU AND  IPI.TierStartQuantity = IP.TierStartQuantity
				   );
		END;  

		SET @Status = 1;
		
		SET @GetDate = dbo.Fn_GetDate();
		--Updating the import process status
		UPDATE ZnodeImportProcessLog
		SET Status = CASE WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 4 )
							WHEN ISNULL(@FailedRecordCount,0) = 0 AND ISNULL(@SuccessRecordCount,0) > 0 THEN dbo.Fn_GetImportStatus( 2 )
							WHEN ISNULL(@FailedRecordCount,0) > 0 AND ISNULL(@SuccessRecordCount,0) = 0 THEN dbo.Fn_GetImportStatus( 3 )
						END, 
			ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		-- COMMIT TRAN ImportProducts;
		COMMIT TRAN A;
	END TRY
	BEGIN CATCH
	ROLLBACK TRAN A;

		SET @Status = 0;
		SELECT ERROR_LINE(), ERROR_MESSAGE(), ERROR_PROCEDURE();

		DECLARE @Error_procedure VARCHAR(8000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportPriceList @TableName = '+CAST(@TableName AS VARCHAR(max)) +',@Status='+ CAST(@Status AS VARCHAR(10))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',@PriceListId='+CAST(@PriceListId AS VARCHAR(max));
		
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
		@ProcedureName = 'Znode_ImportPriceList',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;

	END CATCH;
END;
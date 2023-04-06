CREATE PROCEDURE [dbo].[Znode_ImportAssociateProducts]
(
	@TableName			NVARCHAR(100),
	@Status				BIT OUT,
	@UserId				INT,
	@ImportProcessLogId INT,
	@NewGUId			NVARCHAR(200),
	@PimCatalogId		INT= 0
)
AS
--------------------------------------------------------------------------------------
-- Summary : Import Product Association 

-- Unit Testing : 
--BEGIN TRANSACTION;
--UPDATE ZnodeGlobalSetting SET FeatureValues = '5' WHERE FeatureName = 'InventoryRoundOff'
--DECLARE @status INT;
--EXEC [Znode_ImportInventory] @InventoryXML = '<ArrayOfImportInventoryModel>
--<ImportInventoryModel>
--<SKU>S1002</SKU>
--<Quantity>999998.33</Quantity>
--<ReOrderLevel>10</ReOrderLevel>
--<RowNumber>1</RowNumber>
--<ListCode>TestInventory</ListCode>
--<ListName>TestInventory</ListName>
--</ImportInventoryModel>
--</ArrayOfImportInventoryModel>' , @status = @status OUT , @UserId = 2;
--SELECT @status;
--ROLLBACK TRANSACTION;
--------------------------------------------------------------------------------------
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
	BEGIN TRAN A;
		DECLARE @MessageDisplay NVARCHAR(100), @SSQL NVARCHAR(MAX);
		DECLARE @GetDate DATETIME= dbo.Fn_GetDate();

		IF OBJECT_ID('TEMPDB..#InsertProductAssociation') IS NOT NULL 
			DROP TABLE #InsertProductAssociation;

		IF OBJECT_ID('TEMPDB..#InsertProduct') IS NOT NULL 
			DROP TABLE #InsertProduct;

		IF OBJECT_ID('TEMPDB..#SKU') IS NOT NULL 
			DROP TABLE #SKU;

		IF OBJECT_ID('TEMPDB..#InsertProductAssociation_Parent_type') IS NOT NULL 
			DROP TABLE #InsertProductAssociation_Parent_type;

		IF OBJECT_ID('TEMPDB..#ParentChildSKU') IS NOT NULL 
			DROP TABLE #ParentChildSKU;
		-- Retrive RoundOff Value FROM global setting 

		CREATE TABLE #InsertProductAssociation 
		( 
			RowId INT IDENTITY(1, 1) PRIMARY KEY, RowNumber INT, ParentSKU VARCHAR(300), ChildSKU VARCHAR(200), DisplayOrder INT,
				IsDefault VARCHAR(10), GUID NVARCHAR(400),BundleQuantity VARCHAR(10) NULL
		);

		CREATE TABLE #InsertProductAssociation_Parent_type
		( 
			RowId INT PRIMARY KEY, RowNumber INT, ParentSKU VARCHAR(300), ChildSKU VARCHAR(200), DisplayOrder INT,IsDefault VARCHAR(10),
				GUID NVARCHAR(400),BundleQuantity VARCHAR(10) NULL,PT_ParentProductId VARCHAR(300), PT_ProductType NVARCHAR(100)
		);
		
		CREATE TABLE #InsertProduct 
		( 
			RowId INT IDENTITY(1, 1) PRIMARY KEY, RowNumber INT, ParentProductId VARCHAR(300), ChildProductId VARCHAR(200), DisplayOrder INT,
				IsDefault VARCHAR(10), GUID NVARCHAR(400), ProductType NVARCHAR(100),BundleQuantity VARCHAR(10) NULL
		);

		DECLARE @CategoryAttributId INT, @InventoryListId INT;

		SET @SSQL = 'SELECT RowNumber,ParentSKU,ChildSKU,DisplayOrder,IsDefault,GUID,Quantity FROM '+@TableName;
		INSERT INTO #InsertProductAssociation( RowNumber, ParentSKU,ChildSKU,DisplayOrder,IsDefault, GUID, BundleQuantity)
		EXEC sys.sp_sqlexec @SSQL;

		SELECT ChildSKU As ParentChildSKU
		INTO #ParentChildSKU
		FROM #InsertProductAssociation
		UNION 
		SELECT ParentSku As ParentChildSKU
		FROM #InsertProductAssociation

		--@MessageDisplay will use to display validate message for input inventory value
		CREATE TABLE #SKU
		(
			SKU NVARCHAR(300), PimProductId INT
		);

		INSERT INTO #SKU
		SELECT SKU, PimProductId
		FROM ZnodePimProduct WITH (NOLOCK)
		WHERE SKU IN (SELECT ParentChildSKU FROM #ParentChildSKU);

		DECLARE @ProductType TABLE
		( 
			ProductType NVARCHAR(100), PimProductId INT
		);

		INSERT INTO @ProductType
		SELECT ProductType, PimProductId
		FROM ZnodePimProduct WITH (NOLOCK)
		WHERE ProductType IN ('GroupedProduct','BundleProduct','ConfigurableProduct')
			AND SKU IN (SELECT ParentChildSKU FROM #ParentChildSKU);

		INSERT INTO #InsertProductAssociation_Parent_type
		SELECT IPAC.*,SKUParent.PimProductId, PT.ProductType
		FROM #InsertProductAssociation AS IPAC
		INNER JOIN #SKU AS SKUParent ON IPAC.ParentSKU = SKUParent.SKU 
		INNER JOIN @ProductType PT on PT.PimProductId = SKUParent.PimProductId;

		-- start Functional Validation 
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '84', 'IsDefault', IsDefault, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAssociation_Parent_type AS ii
		WHERE ISNULL(ii.IsDefault,'') = '' AND ii.PT_ProductType ='ConfigurableProduct';

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '68', 'IsDefault', IsDefault, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAssociation_Parent_type AS ii
		WHERE ISNULL(ii.IsDefault,'') not in ('true','1','false','0') AND ISNULL(ii.IsDefault,'') <> '' AND ii.PT_ProductType ='ConfigurableProduct';

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '68', 'IsDefault', IsDefault, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAssociation_Parent_type AS ii
		WHERE ISNULL(ii.IsDefault,'') not in ('true','1','false','0') AND ISNULL(ii.IsDefault,'') <> '' AND ii.PT_ProductType ='BundleProduct';

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '98', 'ChildSKU', ChildSKU, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAssociation_Parent_type AS ii
		WHERE NOT EXISTS( SELECT SKU FROM #SKU SKU WHERE ii.ChildSKU = SKU.SKU);

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '53', 'ParentSKU / ChildSKU', ParentSKU+' / '+ChildSKU, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAssociation AS ii
		WHERE ii.ParentSKU IN
		(
			SELECT ParentSKU FROM #InsertProductAssociation_Parent_type
			GROUP BY ParentSKU,ChildSKU
			HAVING COUNT(1)>1
		);

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '49', 'ParentSKU', ParentSKU , @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAssociation AS ii
		WHERE NOT EXISTS
			( SELECT SKU FROM #SKU SKU INNER JOIN @ProductType PT ON SKU.PimProductId = PT.PimProductId WHERE ii.ParentSKU = SKU.SKU )
			AND EXISTS (SELECT SKU FROM #SKU SKU WHERE ii.ParentSKU = SKU.SKU);

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '94', 'ParentSKU', ParentSKU , @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAssociation AS ii
		WHERE NOT EXISTS (SELECT SKU FROM #SKU SKU WHERE ii.ParentSKU = SKU.SKU);


		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '51', 'ChildSKU', ChildSKU, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAssociation AS ii
		WHERE EXISTS 
			(SELECT SKU FROM #SKU SKU INNER JOIN @ProductType PT ON SKU.PimProductId = PT.PimProductId AND ii.ChildSKU = SKU.SKU)
			AND EXISTS (SELECT SKU FROM #SKU SKU WHERE ii.ChildSKU = SKU.SKU);
			
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '94', 'ChildSKU', ChildSKU, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAssociation AS ii
		WHERE NOT EXISTS (SELECT SKU FROM #SKU SKU WHERE ii.ChildSKU = SKU.SKU);

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '101', 'ParentSKU', 'Configure Attribute Missing: '+ Convert(NVARCHAR(400),ISNULL(ParentSKU,'')), @NewGUId, RowNumber, 2, @GetDate, 2, 
			@GetDate, @ImportProcessLogId
		FROM #InsertProductAssociation_Parent_type AS ii 
		INNER JOIN #SKU PS ON ii.ParentSKU = PS.SKU 
		INNER JOIN @ProductType PT ON PS.PimProductId = PT.PimProductId AND PT.ProductType IN ('ConfigurableProduct')
		WHERE NOT EXISTS (SELECT PimProductId FROM ZnodePimConfigureProductAttribute d WHERE PS.PimProductId = d.PimProductId);
		-- END Function Validation 	

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '16', 'DisplayOrder', DisplayOrder, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAssociation_Parent_type AS ii
		WHERE (ISNUMERIC(ii.DisplayOrder)=0 OR ii.DisplayOrder < 0 OR ii.DisplayOrder > 99999);

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '26', 'BundleQuantity', BundleQuantity, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAssociation_Parent_type AS ii
		WHERE ((ISNULL(ii.BundleQuantity,0) < 1 ) AND ii.PT_ProductType ='BundleProduct' ); --or ii.DisplayOrder = 0

		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, RowNumber, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '2', 'BundleQuantity', BundleQuantity, @NewGUId, RowNumber, 2, @GetDate, 2, @GetDate, @ImportProcessLogId
		FROM #InsertProductAssociation_Parent_type AS ii
		WHERE (ii.BundleQuantity ='' AND ii.PT_ProductType ='BundleProduct'); --or ii.DisplayOrder = 0

		UPDATE ZIL
		SET ZIL.ColumnName = ZIL.ColumnName + ' [ SKU - ' + ISNULL(ParentSKU,'') + ' ] '
		FROM ZnodeImportLog ZIL 
		INNER JOIN #InsertProductAssociation_Parent_type IPA ON (ZIL.RowNumber = IPA.RowNumber)
		WHERE ZIL.ImportProcessLogId = @ImportProcessLogId AND ZIL.RowNumber IS NOT NULL;

		--- Delete Invalid Data after functional validation
		DELETE FROM #InsertProductAssociation_Parent_type
		WHERE RowNumber IN
		(
			SELECT DISTINCT RowNumber
			FROM ZnodeImportLog
			WHERE ImportProcessLogId = @ImportProcessLogId AND GUID = @NewGUID
		);

		SELECT RowNumber, SKUParent.PimProductId SKUParentId,
			( SELECT TOP 1 SKUChild.PimProductId FROM #SKU AS SKUChild WHERE SKUChild.SKU = IPAC.ChildSKU ) SKUChildId,
			CASE WHEN ISNULL(DisplayOrder,'')= '' AND IPAC.PT_ProductType ='ConfigurableProduct' THEN 99 ELSE DisplayOrder END DisplayOrder ,
				CASE WHEN (ISNULL(IsDefault,'')= '' OR IsDefault =0 OR IsDefault > 999) AND IPAC.PT_ProductType !='ConfigurableProduct' THEN '0' 
					ELSE IsDefault END IsDefault, IPAC.PT_ProductType
			, BundleQuantity
		FROM #InsertProductAssociation_Parent_type AS IPAC 
		INNER JOIN #SKU AS SKUParent ON IPAC.ParentSKU = SKUParent.SKU;

		INSERT INTO #InsertProduct (RowNumber, ParentProductId , ChildProductId , DisplayOrder, IsDefault, ProductType, BundleQuantity)
		SELECT RowNumber , SKUParent.PimProductId SKUParentId,
				( SELECT TOP 1 SKUChild.PimProductId FROM #SKU AS SKUChild WHERE SKUChild.SKU = IPAC.ChildSKU ) SKUChildId,
				CASE WHEN ISNULL(DisplayOrder,'')= '' AND IPAC.PT_ProductType ='ConfigurableProduct' THEN 99 ELSE DisplayOrder END DisplayOrder ,
					CASE WHEN (ISNULL(IsDefault,'')= '' OR IsDefault =0 OR IsDefault > 999) AND IPAC.PT_ProductType !='ConfigurableProduct' THEN '0' 
						ELSE IsDefault END IsDefault, IPAC.PT_ProductType
				, BundleQuantity
		FROM #InsertProductAssociation_Parent_type AS IPAC INNER JOIN #SKU AS SKUParent ON IPAC.ParentSKU = SKUParent.SKU;

		-- UPDATE Record count in log 
		DECLARE @FailedRecordCount BIGINT, @SuccessRecordCount BIGINT

		SELECT @FailedRecordCount = COUNT(DISTINCT RowNumber) FROM ZnodeImportLog WHERE RowNumber IS NOT NULL AND ImportProcessLogId = @ImportProcessLogId;

		SELECT @SuccessRecordCount = COUNT(DISTINCT RowNumber) FROM #InsertProduct;

		UPDATE ZnodeImportProcessLog 
		SET FailedRecordcount = @FailedRecordCount , 
			SuccessRecordCount = @SuccessRecordCount, 
			TotalProcessedRecords = (ISNULL(@FailedRecordCount,0) + ISNULL(@SuccessRecordCount,0))
		WHERE ImportProcessLogId = @ImportProcessLogId;
		-- END

		UPDATE #InsertProduct
		SET IsDefault =0
		WHERE RowNumber < (SELECT MAX(RowNumber) FROM #InsertProduct WHERE IsDefault = '1' OR IsDefault = 'True' AND ProductType ='ConfigurableProduct')
			AND ProductType ='ConfigurableProduct';

		UPDATE ZnodePimProductTypeAssociation
		SET IsDefault =0
		WHERE EXISTS (SELECT top 1 1 FROM #InsertProduct IP WHERE IsDefault = '1' OR IsDefault = 'true' AND PimParentProductId = IP.ParentProductId 
			AND ProductType ='ConfigurableProduct');

		UPDATE B 
		SET b.ModifiedDate = @GetDate, b.ModifiedBy = @UserId, b.DisplayOrder = CASE WHEN ISNULL(a.DisplayOrder,0)<>0 THEN a.DisplayOrder ELSE b.DisplayOrder END
			,b.IsDefault = A.IsDefault,b.BundleQuantity= CASE WHEN A.Producttype = 'BundleProduct' THEN CASE WHEN ISNULL(a.BundleQuantity,'') ='' THEN
				CASE WHEN b.BundleQuantity >=1 THEN b.BundleQuantity ELSE 1 END ELSE cast (a.BundleQuantity as INT) END ELSE NULL END
		FROM #InsertProduct A
		INNER JOIN ZnodePimProductTypeAssociation B ON a.ParentProductId = b.PimParentProductId AND a.ChildProductId = b.PimProductId;

		INSERT INTO ZnodePimProductTypeAssociation 
			(PimParentProductId, PimProductId, DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, IsDefault,BundleQuantity) 
		SELECT ParentProductId , ChildProductId , CASE WHEN ISNULL(DisplayOrder,0)=0 THEN 1 ELSE DisplayOrder END, @UserId, @GetDate, @UserId, 
			@GetDate, IsDefault ,CASE WHEN Producttype = 'BundleProduct' THEN CASE WHEN ISNULL(BundleQuantity,'') ='' THEN 1 
				ELSE CAST (BundleQuantity as INT) END ELSE NULL END
		FROM #InsertProduct
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimProductTypeAssociation WHERE PimParentProductId = #InsertProduct.ParentProductId
			AND PimProductId = #InsertProduct.ChildProductId);

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
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportAssociateProducts
					@TableName = '+CAST(@TableName AS VARCHAR(MAX)) +',
					@Status='+ CAST(@Status AS VARCHAR(10))+',
					@UserId = '+CAST(@UserId AS VARCHAR(50))+',
					@ImportProcessLogId='+CAST(@ImportProcessLogId AS VARCHAR(10))+',
					@NewGUId='+CAST(@NewGUId AS VARCHAR(200))+',
					@PimCatalogId='+CAST(@PimCatalogId AS VARCHAR(MAX));

		---Import process updating fail due to database error
		UPDATE ZnodeImportProcessLog
		SET Status = dbo.Fn_GetImportStatus( 3 ), ProcessCompletedDate = @GetDate
		WHERE ImportProcessLogId = @ImportProcessLogId;

		---Loging error for Import process due to database error
		INSERT INTO ZnodeImportLog( ErrorDescription, ColumnName, Data, GUID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, ImportProcessLogId )
		SELECT '93', '', '', @NewGUId, @UserId, @GetDate, @UserId, @GetDate, @ImportProcessLogId

		--Updating total and fail record count
		UPDATE ZnodeImportProcessLog SET FailedRecordcount = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount WITH (NOLOCK) WHERE ImportProcessLogId = @ImportProcessLogId) , SuccessRecordCount = 0 ,
		TotalProcessedRecords = (SELECT TOP 1 RowsCount FROM Znode_ImportCsvRowCount WITH (NOLOCK) WHERE ImportProcessLogId = @ImportProcessLogId)
		WHERE ImportProcessLogId = @ImportProcessLogId;

		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_ImportAssociateProducts',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	
	END CATCH;
END;
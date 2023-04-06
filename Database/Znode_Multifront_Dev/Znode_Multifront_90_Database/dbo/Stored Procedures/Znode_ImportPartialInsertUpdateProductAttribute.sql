CREATE PROCEDURE [dbo].[Znode_ImportPartialInsertUpdateProductAttribute] 
(
	@PimProductDetail	PIMPRODUCTDETAIL READONLY,
	@UserId				INT ,
	@status				BIT OUT,
	@IsNotReturnOutput	BIT = 0,
	@CopyPimProductId	INT = 0
)
AS
/*
	Summary : To Insert / Update bulk Product with multiple attribute values 
	Update Logic: 
*/
BEGIN 
	BEGIN TRY 
	SET NOCOUNT ON 

	--DECLARE @LocationDataPimAttribute INT

	--SET @LocationDataPimAttribute =(
	--SELECT TOP 1 PimAttributeId FROM dbo.ZnodePimAttribute where AttributeCode ='LocationData')

	DECLARE @DefaultLocaleId INT= dbo.fn_getdefaultLocaleId()
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
	DECLARE @PublishStateIdForDraft INT = [dbo].[Fn_GetPublishStateIdForDraftState]()
	DECLARE @PublishStateIdForNotPublished INT = [dbo].[Fn_GetPublishStateIdForForNotPublishedState]()
	DECLARE @TBL_DefaultAttributeId TABLE (PimAttributeId INT PRIMARY KEY , AttributeCode VARCHAR(600))
	DECLARE @TBL_MediaAttributeId TABLE (PimAttributeId INT PRIMARY KEY, AttributeCode VARCHAR(600))
	DECLARE @TBL_TextAreaAttributeId TABLE (PimAttributeId INT PRIMARY KEY , AttributeCode VARCHAR(600))
	 
	INSERT INTO @TBL_DefaultAttributeId (PimAttributeId,AttributeCode)
	SELECT PimAttributeId,AttributeCode FROM [dbo].[Fn_GetDefaultAttributeId] ()
			 
	INSERT INTO @TBL_MediaAttributeId (PimAttributeId,AttributeCode)
	SELECT PimAttributeId,AttributeCode FROM [dbo].[Fn_GetProductMediaAttributeId]()

	INSERT INTO @TBL_TextAreaAttributeId (PimAttributeId ,AttributeCode)
	SELECT PimAttributeId, AttributeCode FROM [dbo].[Fn_GetTextAreaAttributeId]()

	SELECT TBLPA.* INTO #Temp_Product FROM @PimProductDetail TBLPA

	DELETE FROM #Temp_Product WHERE RTRIM(LTRIM(AttributeValue)) = '';

	UPDATE TP
	SET TP.PimAttributeFamilyId=PP.PimAttributeFamilyId
	FROM #Temp_Product TP
	INNER JOIN ZnodePimProduct PP ON TP.PimProductId=PP.PimProductId;

	DECLARE @SQL VARCHAR(MAX);
	IF NOT EXISTS (SELECT TOP 1 1  FROM tempdb.INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME='RowNumber' AND TABLE_NAME='#Temp_Product')
	BEGIN
		SET @SQL='ALTER TABLE #Temp_Product ADD RowNumber BIGINT IDENTITY(1,1);'

		EXEC (@SQL);
	END

	UPDATE TP 
	SET TP.PimAttributeValueId = ZPAV.PimAttributeValueId
	FROM #Temp_Product TP
	INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimAttributeId = TP.PimAttributeId AND ZPAV.PimProductId = TP.PimProductId )
	 
	UPDATE ZPAVL
	SET AttributeValue = TP.AttributeValue
	,ModifiedDate = @GetDate
	,ModifiedBy = @UserId
	FROM ZnodePimAttributevalueLocale ZPAVL 
	INNER JOIN #Temp_Product TP ON (TP.PimAttributeValueId = ZPAVL.PimAttributeValueId AND TP.LocaleId = ZPAVL.LocaleId)

	UPDATE ZPAVL SET AttributeValue = TP.AttributeValue
	,ModifiedDate = @GetDate
	,ModifiedBy = @UserId
	FROM ZnodePimProductAttributeTextAreaValue ZPAVL 
	INNER JOIN #Temp_Product TP ON (TP.PimAttributeValueId = ZPAVL.PimAttributeValueId AND TP.LocaleId = ZPAVL.LocaleId)

	--- Update default value 
	SELECT n.PimAttributeValueId, n.LocaleId,h.PimAttributeDefaultValueId
	INTO #temp_DefaultValue 
	FROM #Temp_Product N 
	CROSS APPLY STRING_SPLIT(n.AttributeValue,',') g 
	INNER JOIN ZnodePimAttributeDefaultValue h ON (h.AttributeDefaultValueCode = g.value)
	WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeDefaultValue T WHERE T.PimAttributeValueId = n.PimAttributeValueId)
	 
	DELETE YU FROM ZnodePimProductAttributeDefaultValue YU 
	WHERE EXISTS (SELECT TOP 1 1 FROM #temp_DefaultValue TY WHERE 
	YU.PimAttributeDefaultValueId <> TY.PimAttributeDefaultValueId 
	AND YU.PimAttributeValueId = TY.PimAttributeValueId )

	--End
	INSERT INTO ZnodePimProductAttributeDefaultValue (PimAttributeValueId,PimAttributeDefaultValueId,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
	SELECT YU.PimAttributeValueId, PimAttributeDefaultValueId,LocaleId,@UserId,@GetDate,@UserId,@GetDate 
	FROM #temp_DefaultValue YU 
	WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeDefaultValue TY WHERE YU.PimAttributeDefaultValueId <> TY.PimAttributeDefaultValueId AND YU.PimAttributeValueId = TY.PimAttributeValueId )

	-- Update product media 
	SELECT n.PimAttributeValueId, n.LocaleId,h.MediaId, h.Path As MediaPath
	INTO #temp_MediaValue
	FROM #Temp_Product N 
	CROSS APPLY STRING_SPLIT(n.AttributeValue,',') g 
	INNER JOIN ZnodeMedia h ON (h.MediaId = CAST(g.value as int))
	WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeMedia T WHERE T.PimAttributeValueId = n.PimAttributeValueId)
	 
	DELETE YU FROM ZnodePimProductAttributeMedia YU 
	WHERE EXISTS (SELECT TOP 1 1 FROM #temp_MediaValue TY WHERE YU.MediaId <> TY.MediaId 
	AND YU.PimAttributeValueId = TY.PimAttributeValueId )

	INSERT INTO ZnodePimProductAttributeMedia (PimAttributeValueId,MediaPath,MediaId,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
	SELECT YU.PimAttributeValueId, MediaPath,MediaId,LocaleId,@UserId,@GetDate,@UserId,@GetDate 
	FROM #temp_MediaValue YU 
	WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeMedia TY WHERE YU.MediaId <> TY.MediaId AND YU.PimAttributeValueId = TY.PimAttributeValueId )
	 
	UPDATE ZnodePimProduct SET ModifiedBy = @UserId ,ModifiedDate = @GetDate ,PublishStateId = @PublishStateIdForDraft	
	WHERE EXISTS (SELECT TOP 1 1 FROM #Temp_Product TY WHERE TY.PimProductId = ZnodePimProduct.PimProductId)
	-- Insert For New Product and Attribute 
	 
	--Update A SET A.PimAttributeFamilyId = B.PimAttributeFamilyId from ZnodePimProduct A Inner join #Temp_Product B on A.PimProductId = B.PimProductId 
	--where ProductType <> 'ConfigurableProduct'
	 
	--Update C SET C.PimAttributeFamilyId = B.PimAttributeFamilyId from ZnodePimProduct A Inner join #Temp_Product B on A.PimProductId = B.PimProductId 
	--Inner join ZnodePimattributeValue C on a.PimProductId = C.PimProductId
	--where ProductType <> 'ConfigurableProduct'
	 	 
	DECLARE @TBL_PimProductId TABLE (PimProductId INT , SKU VARCHAR(1000), RowNumber INT)
	DECLARE @TBL_PimAttributeValue TABLE (PimAttributeValueId INT , PimProductId INT ,PimAttributeId INT )

	--BEGIN TRAN 

	MERGE INTO ZnodePimProduct TARGET USING (SELECT PimAttributeFamilyId,AttributeValue SKU , RowNumber 
	FROM #Temp_Product WHERE ProductAttributeCode = 'SKU' AND PimAttributeValueId IS NULL 
	GROUP BY AttributeValue,PimAttributeFamilyId,RowNumber ) SOURCE ON 1=0 
	WHEN NOT MATCHED THEN INSERT 
	(PimAttributeFamilyId,ExternalId,IsProductPublish,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PublishStateId) 
	VALUES (Source.PimAttributeFamilyId,NULL ,NULL, @UserId , @Getdate,@userId , @getdate, @PublishStateIdForNotPublished )
	OUTPUT Inserted.PimProductId , Source.SKU, Source.RowNumber INTO @TBL_PimProductId;

	INSERT INTO @TBL_PimProductId (PimProductId, SKU, RowNumber)
	SELECT PimProductId, NULL, RowNumber
	FROM #Temp_Product TP
	WHERE PimProductId IS NOT NULL
		AND	NOT EXISTS (SELECT 1 FROM @TBL_PimProductId PPI WHERE PPI.PimProductId = TP.PimProductId AND PPI.RowNumber = TP.RowNumber)
	GROUP BY PimProductId,RowNumber;

	UPDATE P
	SET P.SKU=PP.SKU
	FROM @TBL_PimProductId P
	INNER JOIN ZnodePimProduct PP ON P.PimProductId=PP.PimProductId;

	INSERT INTO ZnodePimAttributeValue 
		(PimAttributeFamilyId,PimProductId,PimAttributeId,PimAttributeDefaultValueId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
	OUTPUT Inserted.PimAttributeValueId , inserted.PimProductId,inserted.PimAttributeId INTO @TBL_PimAttributeValue
	SELECT PimAttributeFamilyId,CASE WHEN TYU.PimProductId IS NULL THEN TBL.PimProductId ELSE TYU.PimProductId END ,PimAttributeId,NULL PimAttributeDefaultValueId,NULL AttributeValue,@UserId , @Getdate,@userId , @getdate
	FROM #Temp_Product TBL 
	LEFT JOIN @TBL_PimProductId TYU ON (TYU.RowNumber = TBL.RowNumber ) 
	WHERE TBL.PimAttributeValueId IS NULL 
	AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttributeValue RT WHERE RT.PimProductId = CASE WHEN TYU.PimProductId IS NULL THEN TBL.PimProductId ELSE TYU.PimProductId END AND RT.PimAttributeId = TBL.PimAttributeId)
	--AND TBL.PimAttributeId<> @LocationDataPimAttribute

	UPDATE TP SET TP.PimAttributeValueId = ZPAV.PimAttributeValueId
	FROM #Temp_Product TP
	INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimAttributeId = TP.PimAttributeId AND ZPAV.PimProductId = TP.PimProductId )

	SELECT distinct CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END PimAttributeValueId , LocaleId , AttributeValue ,Row_number()Over(PARTITION BY CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END, LocaleId Order BY CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END, LocaleId ) RowId
	INTO #temp_AttributeValueLocale 
	FROM #Temp_Product TBL 
	LEFT JOIN @TBL_PimProductId TYU ON (TYU.RowNumber = TBL.RowNumber ) 
	LEFT JOIN @TBL_PimAttributeValue TBPV ON (TBPV.PimProductId = TYU.PimProductId AND TBPV.PimAttributeId = TBL.PimAttributeId)
	WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimAttributeValueLocale TYU WHERE TYU.PimAttributeValueId = CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END AND TYU.LocaleId = TBL.LocaleId)
		AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_DefaultAttributeId TY WHERE TY.PimAttributeId = TBL.PimAttributeId )
		AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_MediaAttributeId TY WHERE TY.PimAttributeId = TBL.PimAttributeId )
		AND NOT EXISTS (SELECT TOP 1 1 FROM @TBL_TextAreaAttributeId TY WHERE TY.PimAttributeId = TBL.PimAttributeId )
		AND CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END IS NOT NULL 

	DELETE FROM #temp_AttributeValueLocale WHERE RowId > 1 

	INSERT INTO ZnodePimAttributeValueLocale (PimAttributeValueId,LocaleId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
	SELECT PimAttributeValueId , LocaleId , AttributeValue , @UserId , @Getdate,@userId , @getdate
	FROM #temp_AttributeValueLocale TBPV

	SELECT distinct CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END PimAttributeValueId, LocaleId , AttributeValue 
		,ROW_NUMBER() OVER (PARTITION BY CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END, LocaleId Order BY CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END, LocaleId ) RowId
	INTO #temp_AttributeValuetextareaLocale
	FROM #Temp_Product TBL 
	LEFT JOIN @TBL_PimProductId TYU ON (TYU.RowNumber = TBL.RowNumber ) 
	LEFT JOIN @TBL_PimAttributeValue TBPV ON (TBPV.PimProductId = TYU.PimProductId AND TBPV.PimAttributeId = TBL.PimAttributeId)
	WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeTextAreaValue TYU WHERE TYU.PimAttributeValueId = CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END AND TYU.LocaleId = TBL.LocaleId)
	-- AND TBL.PimAttributeId<> @LocationDataPimAttribute
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_TextAreaAttributeId TY WHERE TY.PimAttributeId = TBL.PimAttributeId ) 
		AND CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END IS NOT NULL 

	DELETE FROM #temp_AttributeValuetextareaLocale WHERE RowId > 1 

	DELETE ZPVL FROM dbo.ZnodePimAttributeValue ZPV WITH(NOLOCK) 
	INNER JOIN ZnodePimProductAttributeTextAreaValue ZPVL ON ZPV.PimAttributeValueId=ZPVL.PimAttributeValueId
	INNER JOIN ZnodePimAttribute ZPT WITH(NOLOCK) ON ZPT.PimAttributeId=ZPV.PimAttributeId
	INNER JOIN ZnodeAttributeType ZT WITH(NOLOCK) ON ZT.AttributeTypeId=ZPT.AttributeTypeId
	--WHERE ZPV.PimAttributeId=@LocationDataPimAttribute
	WHERE EXISTS (SELECT * FROM #temp_AttributeValuetextareaLocale WHERE PimAttributeValueId=ZPVL.PimAttributeValueId)

	DELETE ZPV FROM dbo.ZnodePimAttributeValue ZPV
	INNER JOIN ZnodePimProductAttributeTextAreaValue ZPVL WITH(NOLOCK) ON ZPV.PimAttributeValueId=ZPVL.PimAttributeValueId
	INNER JOIN ZnodePimAttribute ZPT WITH(NOLOCK) ON ZPT.PimAttributeId=ZPV.PimAttributeId
	INNER JOIN ZnodeAttributeType ZT WITH(NOLOCK) ON ZT.AttributeTypeId=ZPT.AttributeTypeId
	--WHERE ZPV.PimAttributeId=@LocationDataPimAttribute
	WHERE EXISTS (SELECT * FROM #temp_AttributeValuetextareaLocale WHERE PimAttributeValueId=ZPVL.PimAttributeValueId)

	INSERT INTO ZnodePimProductAttributeTextAreaValue(PimAttributeValueId,LocaleId,AttributeValue,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
	SELECT PimAttributeValueId, LocaleId , AttributeValue , @UserId , @Getdate,@userId , @getdate
	FROM #temp_AttributeValuetextareaLocale 
	--COMMIT TRAN 

	-- Default attributevalue 
	SELECT CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END PimAttributeValueId, TBL.LocaleId,h.PimAttributeDefaultValueId
	INTO #temp_DefaultValue_1 
	FROM #Temp_Product TBL 
	LEFT JOIN @TBL_PimProductId TYU ON (TYU.RowNumber = TBL.RowNumber ) 
	LEFT JOIN @TBL_PimAttributeValue TBPV ON (TBPV.PimProductId = TYU.PimProductId AND TBPV.PimAttributeId = TBL.PimAttributeId)
	CROSS APPLY STRING_SPLIT(TBL.AttributeValue,',') g 
	INNER JOIN ZnodePimAttributeDefaultValue h ON (h.AttributeDefaultValueCode = g.value AND h.PimAttributeId = TBL.pimAttributeId )
	WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeDefaultValue T WHERE T.PimAttributeValueId = CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END)
		AND EXISTS (SELECT TOP 1 1 FROM @TBL_DefaultAttributeId TY WHERE TY.PimAttributeId = TBL.PimAttributeId ) 
		AND CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END IS NOT NULL 
	--SELECT * FROM #temp_DefaultValue_1

	INSERT INTO ZnodePimProductAttributeDefaultValue (PimAttributeValueId,PimAttributeDefaultValueId,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
	SELECT YU.PimAttributeValueId, PimAttributeDefaultValueId,LocaleId,@UserId,@GetDate,@UserId,@GetDate 
	FROM #temp_DefaultValue_1 YU 
	WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeDefaultValue TY WHERE YU.PimAttributeDefaultValueId = TY.PimAttributeDefaultValueId AND YU.PimAttributeValueId = TY.PimAttributeValueId )
		AND EXISTS (SELECT TOP 1 1 FROM ZnodePimAttributeValue X where YU.PimAttributeValueId = X.PimAttributeValueId )
	 
	SELECT CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END PimAttributeValueId, TBL.LocaleId,h.MediaId
		,h.Path As MediaPath
	INTO #temp_MediaValue_1
	FROM #Temp_Product TBL 
	LEFT JOIN @TBL_PimProductId TYU ON (TYU.RowNumber = TBL.RowNumber ) 
	LEFT JOIN @TBL_PimAttributeValue TBPV ON (TBPV.PimProductId = TYU.PimProductId AND TBPV.PimAttributeId = TBL.PimAttributeId)
	CROSS APPLY STRING_SPLIT(TBL.AttributeValue,',') g 
	INNER JOIN ZnodeMedia h ON (h.MediaId = cast(g.value as int))
	WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeMedia T WHERE T.PimAttributeValueId = CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END)
	AND EXISTS (SELECT TOP 1 1 FROM @TBL_MediaAttributeId TY WHERE TY.PimAttributeId = TBL.PimAttributeId ) 
	AND CASE WHEN TBPV.PimAttributeValueId IS NULL THEN TBL.PimAttributeValueId ELSE TBPV.PimAttributeValueId END IS NOT NULL 

	INSERT INTO ZnodePimProductAttributeMedia (PimAttributeValueId,MediaPath,MediaId,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
	SELECT YU.PimAttributeValueId,MediaPath, MediaId,LocaleId,@UserId,@GetDate,@UserId,@GetDate 
	FROM #temp_MediaValue_1 YU 
	WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimProductAttributeMedia TY WHERE YU.MediaId = TY.MediaId AND YU.PimAttributeValueId = TY.PimAttributeValueId )


	IF EXISTS (SELECT TOP 1 1 FROM @PimProductDetail P INNER JOIN ZnodePimAttribute PA ON P.PimAttributeId=PA.PimAttributeId
			INNER JOIN ZnodeAttributeType c ON PA.AttributeTypeId=c.AttributeTypeId 
			WHERE c.AttributeTypeName = 'link' AND ISNULL(P.AttributeValue,'')<>'')
	BEGIN
		DECLARE @PimAttributeId INT =(SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'SKU');

		SELECT ZPAV.PimProductId, ZPAVL.AttributeValue as SKU
		INTO #ProductSKU
		FROM #Temp_Product tt
		INNER JOIN ZnodePimAttributeValue ZPAV ON ZPAV.PimProductId=tt.PimProductId AND ZPAV.PimAttributeId=tt.PimAttributeId
		INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
		WHERE ZPAV.PimAttributeId=@PimAttributeId;	

		--Inserting the link attribute product association
		INSERT INTO ZnodePimLinkProductDetail (PimParentProductId,PimProductId,PimAttributeId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,DisplayOrder)
		SELECT a.PimProductId, f.PimProductId, b.PimAttributeId,@UserId,@GetDate,@UserId,@GetDate,1 displayOrder
		FROM @PimProductDetail a 
		CROSS APPLY DBO.SPLIT(a.AttributeValue,',') s
		INNER JOIN ZnodePimAttribute b ON (b.PimAttributeId = a.PimAttributeId)
		INNER JOIN ZnodeAttributeType c ON (c.AttributeTypeId = b.AttributeTypeId)
		INNER JOIN #ProductSKU f ON (f.SKU = s.Item)
		WHERE c.AttributeTypeName = 'link'
			AND a.PimProductId != f.PimProductId
			AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimLinkProductDetail ER WHERE ER.PimParentProductId = a.PimProductId
					AND ER.PimProductId = f.PimProductId AND ER.PimAttributeId = b.PimAttributeId);
	END 

	INSERT INTO ZnodePimConfigureProductAttribute (PimProductId,PimAttributeId,PimFamilyId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
	SELECT TYU.PimProductId , g.value,TBL.ConfigureFamilyIds,@UserId,@GetDate,@UserId,@GetDate 
	FROM #Temp_Product TBL
	INNER JOIN @TBL_PimProductId TYU ON (TYU.RowNumber = TBL.RowNumber )
	CROSS APPLY STRING_SPLIT(TBL.ConfigureAttributeIds,',') g
	WHERE TBL.AttributeValue = 'ConfigurableProduct' AND TBL.PimAttributeId = (SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode='ProductType')
		AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePimConfigureProductAttribute TYU WHERE TYU.PimProductId = TBL.PimProductId AND TYU.PimAttributeId = g.value)
	GROUP BY TYU.PimProductId , g.value,TBL.ConfigureFamilyIds

	DELETE FROM ZnodePimProductAttributeDefaultValue 
	WHERE PimAttributeValueId IN (SELECT PimAttributeValueId FROM ZnodePimAttributeValue NT
	WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimConfigureProductAttribute TY WHERE NT.PimAttributeId = TY.PimAttributeId 
		AND NT.PimProductId =TY.PimProductId ))

	DELETE NT FROM ZnodePimAttributeValue NT
	WHERE EXISTS (SELECT TOP 1 1 FROM ZnodePimConfigureProductAttribute TY WHERE NT.PimAttributeId = TY.PimAttributeId 
		AND NT.PimProductId =TY.PimProductId )

	UPDATE P
	SET P.ProductAttributeCode = PA.AttributeCode
	FROM ZnodePimAttribute PA
	INNER JOIN #Temp_Product P ON PA.PimAttributeId=P.PimAttributeId
	WHERE PA.IsCategory = 0;

	DECLARE @LocaleId INT = (SELECT TOP 1 LocaleId FROM #Temp_Product);
	IF @LocaleId = @DefaultLocaleId
	BEGIN
		DECLARE @sqlt NVARCHAR(MAX) = ''
		DECLARE @AttributeCodeAtt VARCHAR(600) , @PimAttributeIdAttr INT

		DECLARE Cur_AttributeDataUpdate CURSOR FOR 

		SELECT b.AttributeCode , PimAttributeId 
		FROM INFORMATION_SCHEMA.COLUMNS a 
		INNER JOIN ZnodePimAttribute b ON (a.COLUMN_NAME = b.AttributeCode )
		WHERE TABLE_NAME = 'ZnodePimProduct'
			AND IsCategory = 0 
			AND IsShowOnGrid = 1 
			AND EXISTS (SELECT TOP 1 1 FROM #Temp_Product n WHERE n.ProductAttributeCode = b.AttributeCode)

		OPEN Cur_AttributeDataUpdate 
		FETCH NEXT FROM Cur_AttributeDataUpdate INTO @AttributeCodeAtt,@PimAttributeIdAttr 
		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @sqlt = 'UPDATE a
			SET '+@AttributeCodeAtt+'= AttributeValue 
			FROM ZnodePimProduct a 
			INNER JOIN #Temp_Product m ON (m.PimProductId = a.pimProductId ) 
			WHERE m.ProductAttributeCode = '''+@AttributeCodeAtt+'''
			'

			PRINT (@sqlt)
			EXEC (@sqlt)

			FETCH NEXT FROM Cur_AttributeDataUpdate 
			INTO @AttributeCodeAtt,@PimAttributeIdAttr 
		END

		CLOSE Cur_AttributeDataUpdate
		DEALLOCATE Cur_AttributeDataUpdate
	END

	SELECT 1 Id , CAST(1 AS BIT ) Status

	END TRY 
	BEGIN CATCH 
		SELECT 1 Id , CAST(0 AS BIT ) Status

		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), 
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_ImportPartialInsertUpdateProductAttribute @Status='+CAST(@Status AS VARCHAR(50));
		
		SELECT 0 AS ID,	CAST(0 AS BIT) AS Status,ERROR_MESSAGE();
		
		SET @Status = 0;
		
		EXEC Znode_InsertProcedureErrorLog
			@ProcedureName = 'Znode_ImportPartialInsertUpdateProductAttribute',
			@ErrorInProcedure = @Error_procedure,
			@ErrorMessage = @ErrorMessage,
			@ErrorLine = @ErrorLine,
			@ErrorCall = @ErrorCall;
	END CATCH 
END
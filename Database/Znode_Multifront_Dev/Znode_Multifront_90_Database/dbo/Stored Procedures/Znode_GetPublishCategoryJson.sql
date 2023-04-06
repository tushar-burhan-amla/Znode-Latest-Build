CREATE PROCEDURE [dbo].[Znode_GetPublishCategoryJson]
(   
	@PublishCatalogId INT,
	@UserId           INT,
	@Status           BIT = 0 OUT,
	@IsDebug          BIT = 0,
	@VersionIdString  VARCHAR(2000)= '',
	@LocaleId         TransferID READONLY,
	@RevisionState    VARCHAR(50) = ''
)
AS 
/*
Summary:Publish category with their respective products and details 
The result is fetched in xml form   
Unit Testing   
Begin transaction 
SELECT * FROM ZnodePIMAttribute 
SELECT * FROM ZnodePublishCatalog 
SELECT * FROM ZnodePublishCategory WHERE publishCAtegoryID = 167 


EXEC [Znode_GetPublishCategory] @PublishCatalogId = 3,@VersionId = 0 ,@UserId =2 ,@IsDebug = 1 
     


Rollback Transaction 
*/
BEGIN
BEGIN TRAN GetPublishCategory;
BEGIN TRY
SET NOCOUNT ON;
	DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
	DECLARE @LocaleIdIn INT= 0, @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId(), @Counter INT= 1, @MaxId INT= 0, @CategoryIdCount INT;
	DECLARE @IsActive BIT= [dbo].[Fn_GetIsActiveTrue]();
	DECLARE @AttributeIds VARCHAR(MAX)= '', @PimCategoryIds VARCHAR(MAX)= '', @DeletedPublishCategoryIds VARCHAR(MAX)= '', @DeletedPublishProductIds VARCHAR(MAX);
	--get the pim catalog id 
	DECLARE @PimCatalogId INT=(SELECT PimCatalogId FROM ZnodePublishcatalog WHERE PublishCatalogId = @PublishCatalogId); 

	DECLARE @TBL_AttributeIds TABLE
	(
		PimAttributeId       INT,
		ParentPimAttributeId INT,
		AttributeTypeId      INT,
		AttributeCode        VARCHAR(600),
		IsRequired           BIT,
		IsLocalizable        BIT,
		IsFilterable         BIT,
		IsSystemDefined      BIT,
		IsConfigurable       BIT,
		IsPersonalizable     BIT,
		DisplayOrder         INT,
		HelpDescription      VARCHAR(MAX),
		IsCategory           BIT,
		IsHidden             BIT,
		CreatedDate          DATETIME,
		ModifiedDate         DATETIME,
		AttributeName        NVARCHAR(MAX),
		AttributeTypeName    VARCHAR(300)
	);
	DECLARE @TBL_AttributeDefault TABLE
	(
		PimAttributeId            INT,
		AttributeDefaultValueCode VARCHAR(100),
		IsEditable                BIT,
		AttributeDefaultValue     NVARCHAR(MAX)
		,DisplayOrder   INT
	);
	DECLARE @TBL_AttributeValue TABLE
	(
		PimCategoryAttributeValueId INT,
		PimCategoryId               INT,
		CategoryValue               NVARCHAR(MAX),
		AttributeCode               VARCHAR(300),
		PimAttributeId              INT
	);
	DECLARE @TBL_LocaleIds TABLE
	(
		RowId     INT IDENTITY(1, 1),
		LocaleId  INT,
		IsDefault BIT
	);
	DECLARE @TBL_PimCategoryIds TABLE
	(
		PimCategoryId       INT,
		PimParentCategoryId INT,
		DisplayOrder        INT,
		ActivationDate      DATETIME,
		ExpirationDate      DATETIME,
		CategoryName        NVARCHAR(MAX),
		ProfileId           VARCHAR(MAX),
		IsActive            BIT,
		PimCategoryHierarchyId INT,
		ParentPimCategoryHierarchyId INT ,
		CategoryCode  NVARCHAR(MAX)             
	);


	DECLARE @TBL_PublishPimCategoryIds TABLE
	(
		PublishCategoryId       INT,
		PimCategoryId           INT,
		PublishProductId        varchar(max),
		PublishParentCategoryId INT ,
		PimCategoryHierarchyId INT ,parentPimCategoryHierarchyId INT,
		RowIndex INT
	);
	DECLARE @TBL_DeletedPublishCategoryIds TABLE
	(
		PublishCategoryId INT,
		PublishProductId  INT
	);
	DECLARE @TBL_CategoryXml TABLE
	(
		PublishCategoryId INT,
		CategoryXml       XML,
		LocaleId          INT
	);
	INSERT INTO @TBL_LocaleIds
	(
		LocaleId,
		IsDefault
	)
	-- here collect all locale ids
	SELECT LocaleId,IsDefault FROM ZnodeLocale MT WHERE IsActive = @IsActive
	AND (EXISTS (SELECT TOP 1 1  FROM @LocaleId RT WHERE RT.Id = MT.LocaleId )
	OR NOT EXISTS (SELECT TOP 1 1 FROM @LocaleId ));

	IF OBJECT_ID('tempdb..#VesionIds') is not null
		DROP TABLE #VesionIds
  				 
	SELECT PV.* INTO #VesionIds FROM ZnodePublishVersionEntity PV Inner join Split(@VersionIdString,',') S ON PV.VersionId = S.Item
		

	if object_id('tempdb..#CategoryData')is not null
		DROP TABLE #CategoryData

	------------for CategoryCode update
	SELECT ZPCAL.CategoryValue as CategoryCode,MAX(ZPC.PublishCategoryId) as PublishCategoryId ,ZPCA.PimCategoryId, ZPoC.PortalId
	INTO #CategoryData
	FROM ZnodePimCategoryAttributeValue ZPCA
	INNER JOIN ZnodePimCategoryAttributeValueLocale ZPCAL on ZPCA.PimCategoryAttributeValueId = ZPCAL.PimCategoryAttributeValueId
	INNER JOIN ZnodePimAttribute ZPA ON ZPCA.PimAttributeId = ZPA.PimAttributeId
	INNER JOIN ZnodePublishCategory ZPC on ZPCA.PimCategoryId = ZPC.PimCategoryId
	INNER JOIN ZnodePortalCatalog ZPoC on ZPC.PublishCatalogId = ZPoC.PublishCatalogId
	WHERE ZPA.AttributeCode = 'CategoryCode' AND ZPC.PublishCatalogId = @PublishCatalogId
	AND EXISTS(SELECT * FROM ZnodeCMSWidgetCategory ZCWC WHERE ZPC.PublishCategoryId = ZCWC.PublishCategoryId )
	GROUP BY ZPCAL.CategoryValue, ZPCA.PimCategoryId, ZPoC.PortalId

	UPDATE ZCWC SET ZCWC.CategoryCode = CD.CategoryCode
	FROM ZnodeCMSWidgetCategory ZCWC
	INNER JOIN #CategoryData CD ON ZCWC.PublishCategoryId = CD.PublishCategoryId and ZCWC.CMSMappingId = CD.PortalId
	WHERE ZCWC.TypeOFMapping = 'PortalMapping'
	----------

	INSERT INTO @TBL_PimCategoryIds(PimCategoryId,PimParentCategoryId,DisplayOrder,ActivationDate,ExpirationDate,IsActive,PimCategoryHierarchyId,ParentPimCategoryHierarchyId)
	SELECT DISTINCT ZPCH.PimCategoryId,ZPCH2.PimCategoryId  PimParentCategoryId,ZPCH.DisplayOrder,ZPCH.ActivationDate,ZPCH.ExpirationDate,ZPCH.IsActive ,ZPCH.PimCategoryHierarchyId,ZPCH.ParentPimCategoryHierarchyId
	FROM ZnodePimCategoryHierarchy AS ZPCH 
	LEFT JOIN ZnodePimCategoryHierarchy AS ZPCH2 ON (ZPCH2.PimCategoryHierarchyId = ZPCH. ParentPimCategoryHierarchyId ) 
	WHERE ZPCH.PimCatalogId = @PimCatalogId; 
	
	-- here is find the deleted publish category id on basis of publish catalog
	INSERT INTO @TBL_DeletedPublishCategoryIds(PublishCategoryId,PublishProductId)
	SELECT ZPC.PublishCategoryId,ZPCP.PublishProductId 
	FROM ZnodePublishCategoryProduct ZPCP
	INNER JOIN ZnodePublishCategory AS ZPC ON(ZPCP.PublishCategoryId = ZPC.PublishCategoryId AND ZPCP.PublishCatalogId = ZPC.PublishCatalogId)                                                  
	INNER JOIN ZnodePublishProduct ZPP ON(zpp.PublishProductId = zpcp.PublishProductId AND zpp.PublishCatalogId = zpcp.PublishCatalogId)
	INNER JOIN ZnodePublishCatalog ZPCC ON(ZPCC.PublishCatalogId = ZPCP.PublishCatalogId)
	WHERE ZPC.PublishCatalogId = @PublishCataLogId 
	AND NOT EXISTS(SELECT TOP 1 1 FROM ZnodePimCategoryHierarchy AS TBPC WHERE TBPC.PimCategoryId = ZPC.PimCategoryId AND TBPC.PimCategoryHierarchyId = ZPC.PimCategoryHierarchyId
	AND TBPC.PimCatalogId = ZPCC.PimCatalogId);

	-- here is find the deleted publish category id on basis of publish catalog
	SET @DeletedPublishCategoryIds = ISNULL(SUBSTRING((SELECT ','+CAST(PublishCategoryId AS VARCHAR(50)) FROM @TBL_DeletedPublishCategoryIds AS ZPC
				GROUP BY ZPC.PublishCategoryId FOR XML PATH('') ), 2, 4000), '');
	-- here is find the deleted publish category id on basis of publish catalog
	SET @DeletedPublishProductIds = '';
	
	EXEC Znode_DeletePublishCatalog @PublishCatalogIds = @PublishCatalogId,@PublishCategoryIds = @DeletedPublishCategoryIds,@PublishProductIds = @DeletedPublishProductIds; 
		
	MERGE INTO ZnodePublishCategory TARGET USING  @TBL_PimCategoryIds SOURCE ON
	(
		TARGET.PimCategoryId = SOURCE.PimCategoryId 
		AND TARGET.PublishCatalogId = @PublishCataLogId 
		AND TARGET.PimCategoryHierarchyId = SOURCE.PimCategoryHierarchyId
	)
	WHEN MATCHED THEN UPDATE SET TARGET.PimParentCategoryId = SOURCE.PimParentCategoryId,TARGET.CreatedBy = @UserId,TARGET.CreatedDate = @GetDate,
	TARGET.ModifiedBy = @UserId,TARGET.ModifiedDate = @GetDate,PimCategoryHierarchyId = SOURCE.PimCategoryHierarchyId,ParentPimCategoryHierarchyId=SOURCE.ParentPimCategoryHierarchyId
	WHEN NOT MATCHED THEN INSERT(PimCategoryId,PublishCatalogId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PimCategoryHierarchyId,ParentPimCategoryHierarchyId) 
	VALUES(SOURCE.PimCategoryId,@PublishCatalogId,@UserId,@GetDate,@UserId,@GetDate,SOURCE.PimCategoryHierarchyId,SOURCE.ParentPimCategoryHierarchyId)
	OUTPUT INSERTED.PublishCategoryId,INSERTED.PimCategoryId,INSERTED.PimCategoryHierarchyId,INSERTED.parentPimCategoryHierarchyId INTO @TBL_PublishPimCategoryIds(PublishCategoryId,PimCategoryId,PimCategoryHierarchyId,parentPimCategoryHierarchyId);
     
	-- here update the publish parent category id
	UPDATE ZPC SET [PimParentCategoryId] =TBPC.[PimCategoryId] 
	FROM ZnodePublishCategory ZPC
	INNER JOIN ZnodePublishCategory TBPC ON(ZPC.parentPimCategoryHierarchyId = TBPC.PimCategoryHierarchyId  ) 
	WHERE ZPC.PublishCatalogId =@PublishCatalogId
	AND ZPC.ParentPimCategoryHierarchyId IS NOT NULL
	AND TBPC.PublishCatalogId =@PublishCatalogId;

	UPDATE a
	SET  a.PublishParentCategoryId = b.PublishCategoryId
	FROM ZnodePublishCategory a 
	INNER JOIN ZnodePublishCategory b   ON (a.parentpimCategoryHierarchyId = b.pimCategoryHierarchyId)
	WHERE a.parentpimCategoryHierarchyId IS NOT NULL 
	AND a.PublishCatalogId =@PublishCatalogId
	AND b.PublishCatalogId =@PublishCatalogId

	UPDATE a set a.PublishParentCategoryId = NULL
	FROM ZnodePublishCategory a 
	WHERE a.parentpimCategoryHierarchyId IS NULL AND PimParentCategoryId IS NULL
	AND a.PublishCatalogId = @PublishCatalogId AND a.PublishParentCategoryId IS NOT NULL

	SET @MaxId =(SELECT MAX(RowId)FROM @TBL_LocaleIds);
	DECLARE @TransferID TRANSFERID 
	INSERT INTO @TransferID 
	SELECT DISTINCT  PimCategoryId
	FROM @TBL_PublishPimCategoryIds 

	SET @PimCategoryIds = SUBSTRING((SELECT ','+CAST(PimCategoryId AS VARCHAR(50)) FROM @TBL_PublishPimCategoryIds FOR XML PATH('')), 2, 4000);
		
	WHILE @Counter <= @MaxId -- Loop on Locale id 
	BEGIN
		SET @LocaleIdIn =(SELECT LocaleId FROM @TBL_LocaleIds WHERE RowId = @Counter);
                   
		SET @AttributeIds = SUBSTRING((SELECT ','+CAST(ZPCAV.PimAttributeId AS VARCHAR(50)) FROM ZnodePimCategoryAttributeValue ZPCAV 
				WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_PimCategoryIds TBPC WHERE TBPC.PimCategoryId = ZPCAV.PimCategoryId) GROUP BY ZPCAV.PimAttributeId FOR XML PATH('')), 2, 4000);
                
		SET @CategoryIdCount =(SELECT COUNT(1) FROM @TBL_PimCategoryIds);

		INSERT INTO @TBL_AttributeIds (PimAttributeId,ParentPimAttributeId,AttributeTypeId,AttributeCode,IsRequired,IsLocalizable,IsFilterable,IsSystemDefined,
		IsConfigurable,IsPersonalizable,DisplayOrder,HelpDescription,IsCategory,IsHidden,CreatedDate,ModifiedDate,AttributeName,AttributeTypeName)
		EXEC [Znode_GetPimAttributesDetails] @AttributeIds,@LocaleIdIn;

		INSERT INTO @TBL_AttributeDefault (PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder)
		EXEC [dbo].[Znode_GetAttributeDefaultValueLocale] @AttributeIds,@LocaleIdIn;

		INSERT INTO @TBL_AttributeValue (PimCategoryAttributeValueId,PimCategoryId,CategoryValue,AttributeCode,PimAttributeId)
		EXEC [dbo].[Znode_GetCategoryAttributeValueId] @TransferID,@AttributeIds,@LocaleIdIn;


		;WITH Cte_UpdateDefaultAttributeValue AS 
		(
			SELECT TBAV.PimCategoryId,TBAV.PimAttributeId,SUBSTRING((SELECT ','+AttributeDefaultValue FROM @TBL_AttributeDefault TBD WHERE TBAV.PimAttributeId = TBD.PimAttributeId
			AND EXISTS(SELECT TOP 1 1 FROM Split(TBAV.CategoryValue, ',') SP WHERE SP.Item = TBD.AttributeDefaultValueCode)FOR XML PATH('')), 2, 4000) DefaultCategoryAttributeValue
			FROM @TBL_AttributeValue TBAV WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_AttributeDefault TBAD WHERE TBAD.PimAttributeId = TBAV.PimAttributeId)
		)	 
		-- update the default value with locale 
		UPDATE TBAV SET CategoryValue = CTUDFAV.DefaultCategoryAttributeValue FROM @TBL_AttributeValue TBAV 
		INNER JOIN Cte_UpdateDefaultAttributeValue CTUDFAV ON(CTUDFAV.PimCategoryId = TBAV.PimCategoryId AND CTUDFAV.PimAttributeId = TBAV.PimAttributeId)
		WHERE CategoryValue IS NULL ;
					 
		-- here is update the media path  
		;WITH Cte_productMedia AS 
		(
			SELECT TBA.PimCategoryId,TBA.PimAttributeId,
			Isnull(
			(STUFF((SELECT ','+zm.PATH FROM ZnodeMedia ZM WHERE EXISTS
			(SELECT TOP 1 1 FROM dbo.split(TBA.CategoryValue, ',') SP
			WHERE SP.Item = CAST(Zm.MediaId AS VARCHAR(50)))FOR XML PATH(''),Type).value('.', 'varchar(max)'), 1, 1,'')) ,'no-image.png')
			CategoryValue
			FROM @TBL_AttributeValue TBA WHERE EXISTS(SELECT TOP 1 1 FROM [dbo].[Fn_GetProductMediaAttributeId]() FNMA WHERE FNMA.PImAttributeId = TBA.PimATtributeId)
		)
		UPDATE TBAV SET CategoryValue = CTCM.CategoryValue 
		FROM @TBL_AttributeValue TBAV 
		INNER JOIN Cte_productMedia CTCM ON(CTCM.PimCategoryId = TBAV.PimCategoryId
		AND CTCM.PimAttributeId = TBAV.PimAttributeId);
					 					
		;WITH Cte_PublishProductIds AS 
		(
			SELECT TBPC.PublishcategoryId,SUBSTRING((SELECT ','+CAST(PublishProductId AS VARCHAR(50))
			FROM ZnodePublishCategoryProduct ZPCP 
			WHERE ZPCP.PublishCategoryId = TBPC.publishCategoryId
			AND ZPCP.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId
			AND ZPCP.PublishCatalogId = @PublishCatalogId FOR XML PATH('')), 2, 8000) PublishProductId ,PimCategoryHierarchyId
			FROM @TBL_PublishPimCategoryIds TBPC
		)
		UPDATE TBPPC SET PublishProductId = CTPP.PublishProductId FROM @TBL_PublishPimCategoryIds TBPPC INNER JOIN Cte_PublishProductIds CTPP ON(TBPPC.PublishCategoryId = CTPP.PublishCategoryId 
		AND TBPPC.PimCategoryHierarchyId = CTPP.PimCategoryHierarchyId);

		;WITH Cte_CategoryProfile AS 
		(
			SELECT PimCategoryId,ZPCC.PimCategoryHierarchyId,
			SUBSTRING(( SELECT ','+CAST(ProfileId AS VARCHAR(50)) 
			FROM ZnodeProfile ZPC 
			INNER JOIN ZnodePimCategoryHierarchy ZPRCC ON(ZPRCC.PimCategoryHierarchyId = ZPCC.PimCategoryHierarchyId
			AND ZPRCC.PimCatalogId = ZPC.PimCatalogId) 
			WHERE ZPC.PimCatalogId = ZPCC.PimCatalogId FOR XML PATH('')), 2, 4000) ProfileIds
			FROM ZnodePimCategoryHierarchy ZPCC 
			WHERE EXISTS(SELECT TOP 1 1 FROM @TBL_PimCategoryIds TBPC 
			WHERE TBPC.PimCategoryId = ZPCC.PimCategoryId AND ZPCC.PimCatalogId = @PimCatalogId 
			AND ZPCC.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId)
		)                          
		UPDATE TBPC SET TBPC.ProfileId = CTCP.ProfileIds FROM @TBL_PimCategoryIds TBPC 
		LEFT JOIN Cte_CategoryProfile CTCP ON(CTCP.PimCategoryId = TBPC.PimCategoryId AND CTCP.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId );
                         
				     
		UPDATE TBPC SET TBPC.CategoryName = TBAV.CategoryValue FROM @TBL_PimCategoryIds TBPC INNER JOIN @TBL_AttributeValue TBAV ON(TBAV.PimCategoryId = TBPC.PimCategoryId
		AND EXISTS(SELECT TOP 1 1 FROM [dbo].[Fn_GetCategoryNameAttribute]() FNGCNA WHERE FNGCNA.PimAttributeId = TBAV.PimAttributeId));


		UPDATE TBPC SET TBPC.CategoryCode = TBAV.CategoryValue FROM @TBL_PimCategoryIds TBPC INNER JOIN @TBL_AttributeValue TBAV ON(TBAV.PimCategoryId = TBPC.PimCategoryId
		AND EXISTS(SELECT TOP 1 1 FROM dbo.Fn_GetCategoryCodeAttribute() FNGCNA WHERE FNGCNA.PimAttributeId = TBAV.PimAttributeId))

		-- here update the publish category details 
		;WITH Cte_UpdateCategoryDetails AS 
		(
			SELECT TBC.PimCategoryId,PublishCategoryId,CategoryName, TBPPC.PimCategoryHierarchyId,CategoryCode
			FROM @TBL_PimCategoryIds TBC
			INNER JOIN @TBL_PublishPimCategoryIds TBPPC ON(TBC.PimCategoryId = TBPPC.PimCategoryId AND TBC.PimCategoryHierarchyId = TBPPC.PimCategoryHierarchyId)
		)						
		MERGE INTO ZnodePublishCategoryDetail TARGET USING Cte_UpdateCategoryDetails SOURCE ON(TARGET.PublishCategoryId = SOURCE.PublishCategoryId
		AND TARGET.LocaleId = @LocaleIdIn)
		WHEN MATCHED THEN UPDATE SET PublishCategoryId = SOURCE.PublishcategoryId,PublishCategoryName = SOURCE.CategoryName,LocaleId = @LocaleIdIn,ModifiedBy = @userId,ModifiedDate = @GetDate,CategoryCode=SOURCE.CategoryCode
		WHEN NOT MATCHED THEN INSERT(PublishCategoryId,PublishCategoryName,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,CategoryCode) VALUES
		(SOURCE.PublishCategoryId,SOURCE.CategoryName,@LocaleIdIn,@userId,@GetDate,@userId,@GetDate,SOURCE.CategoryCode);

		IF OBJECT_ID('tempdb..#Index') is not null
		BEGIN 
			DROP TABLE #Index
		END 

		CREATE TABLE #Index (RowIndex int ,PimCategoryId int , PimCategoryHierarchyId  int,ParentPimCategoryHierarchyId int )		
		INSERT INTO  #Index ( RowIndex ,PimCategoryId , PimCategoryHierarchyId,ParentPimCategoryHierarchyId)
		SELECT CAST(Row_number() OVER (Partition By TBL.PimCategoryId Order by ISNULL(TBL.PimCategoryId,0) desc) AS VARCHAR(100))
		,ZPC.PimCategoryId, ZPC.PimCategoryHierarchyId, ZPC.ParentPimCategoryHierarchyId
		FROM @TBL_PublishPimCategoryIds TBL
		INNER JOIN ZnodePublishCategory ZPC ON (TBL.PimCategoryId = ZPC.PimCategoryId AND TBL.PimCategoryHierarchyId = ZPC.PimCategoryHierarchyId)
		WHERE ZPC.PublishCatalogId = @PublishCatalogId

		UPDATE TBP SET  TBP.[RowIndex]=  IDX.RowIndex 
		FROM @TBL_PublishPimCategoryIds TBP INNER JOIN #Index IDX ON (IDX.PimCategoryId = TBP.PimCategoryId AND IDX.PimCategoryHierarchyId = TBP.PimCategoryHierarchyId)  
					
		IF OBJECT_ID('Tempdb..#CategoryEntity') IS NOT NULL
			DROP TABLE #CategoryEntity
						
					
		SELECT ISNULL(TYU.VersionId,'') VersionId,TBPC.PublishCategoryId ZnodeCategoryId,
		ISNULL(CategoryName, '') Name,ISNULL(CategoryCode,'') CategoryCode,
		ZPC.PublishCatalogId, ZPC.CatalogName ,
		Isnull('[' + THR.PublishParentCategoryId + ']',NULL) TempZnodeParentCategoryIds,
		ISNULL('[' + TBPC.PublishProductId + ']', NULL)  TempProductIds,
		@LocaleIdIn LocaleId,TBC.IsActive,ISNULL(DisplayOrder, '0') DisplayOrder,
		ISNULL(
		(
			SELECT TBA.AttributeCode,TBA.AttributeName,ISNULL(IsUseInSearch, 0) IsUseInSearch,
			ISNULL(IsHtmlTags, 0) IsHtmlTags,ISNULL(IsComparable, 0) IsComparable,
			TBAV.CategoryValue AttributeValues,TBA.AttributeTypeName 
			FROM @TBL_AttributeValue TBAV
			INNER JOIN @TBL_AttributeIds TBA ON(TBAV.PimAttributeId = TBA.PimAttributeId) 
			LEFT JOIN ZnodePimFrontendProperties ZPFP ON(ZPFP.PimAttributeId = TBA.PimAttributeId)
			WHERE TBC.PimCategoryId = TBAV.PimCategoryId  
			FOR JSON PATH) 
		, '[]')  as Attributes,
		ActivationDate,ExpirationDate,ISNULL(TBPC.RowIndex,1) CategoryIndex
		--,ProfileId TempProfileIds
		INTO #CategoryEntity
		FROM @TBL_PublishPimCategoryIds TBPC 
		INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId= @PublishCatalogId)
		LEFT JOIN #VesionIds TYU ON (TYU.ZnodeCatalogId = @PublishCatalogId AND TYU.LocaleId = @LocaleIdIn)
		INNER JOIN ZnodePublishCAtegory THR ON (THR.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId AND THR.PimCategoryId = TBPC.PimCategoryId AND THR.PublishCatalogId= @PublishCatalogId )
		INNER JOIN @TBL_PimCategoryIds TBC ON(TBC.PimCategoryId = TBPC.PimCategoryId AND TBC.PimCategoryHierarchyId = TBPC.PimCategoryHierarchyId) 
					
		UPDATE PCE SET PCE.ElasticSearchEvent = 2 FROM ZnodePublishCategoryEntity PCE
		WHERE NOT EXISTS(SELECT * FROM #CategoryEntity CE WHERE CE.VersionId = PCE.VersionId AND PCE.ZnodeCategoryId = CE.ZnodeCategoryId
		AND PCE.LocaleId = CE.LocaleId AND PCE.ZnodeCatalogId = CE.PublishCatalogId )
		AND PCE.LocaleId = @LocaleIdIn
		AND PCE.ZnodeCatalogId = @PublishCatalogId
		AND EXISTS(SELECT * FROM #VesionIds V WHERE PCE.VersionId = V.VersionId)

		UPDATE PCE SET PCE.Name = CE.Name, PCE.ZnodeParentCategoryIds = CE.TempZnodeParentCategoryIds, PCE.ProductIds = CE.TempProductIds,
			PCE.IsActive = CE.IsActive,PCE.DisplayOrder = CE.DisplayOrder,PCE.Attributes = CE.Attributes,
			PCE.ActivationDate = CE.ActivationDate,PCE.ExpirationDate = CE.ExpirationDate,PCE.CategoryIndex = CE.CategoryIndex, PCE.ElasticSearchEvent = 1
		FROM ZnodePublishCategoryEntity PCE
		INNER JOIN #CategoryEntity CE ON CE.VersionId = PCE.VersionId AND PCE.ZnodeCategoryId = CE.ZnodeCategoryId
		AND PCE.LocaleId = CE.LocaleId AND PCE.ZnodeCatalogId = CE.PublishCatalogId
		AND PCE.LocaleId = @LocaleIdIn
		AND EXISTS(SELECT * FROM #VesionIds V WHERE PCE.VersionId = V.VersionId)

		INSERT INTO ZnodePublishCategoryEntity
		(
			VersionId,ZnodeCategoryId,
			Name,CategoryCode,
			ZnodeCatalogId,CatalogName,ZnodeParentCategoryIds,
			ProductIds,LocaleId,IsActive,DisplayOrder,
			Attributes,
			ActivationDate,ExpirationDate,CategoryIndex,ElasticSearchEvent
		)
		SELECT VersionId,ZnodeCategoryId,
		Name,CategoryCode,
		PublishCatalogId,CatalogName,TempZnodeParentCategoryIds,
		TempProductIds,LocaleId,IsActive,DisplayOrder,
		Attributes,
		ActivationDate,ExpirationDate,CategoryIndex, 1 as ElasticSearchEvent
		FROM #CategoryEntity CE
		WHERE NOT EXISTS(SELECT * FROM ZnodePublishCategoryEntity PCE WHERE CE.VersionId = PCE.VersionId AND PCE.ZnodeCategoryId = CE.ZnodeCategoryId
		AND PCE.LocaleId = CE.LocaleId AND PCE.ZnodeCatalogId = CE.PublishCatalogId )
		AND CE.LocaleId = @LocaleIdIn
					
		DELETE FROM @TBL_AttributeIds;
		DELETE FROM @TBL_AttributeDefault;
		DELETE FROM @TBL_AttributeValue;
		SET @Counter = @Counter + 1;
	END;
	
	UPDATE ZnodePublishCatalogLog 
	SET PublishCategoryId = (SELECT COunt(DISTINCT PimCategoryId ) 
	FROM @TBL_PublishPimCategoryIds WHERE PublishCatalogId =@PublishCatalogId), 
	IsCategoryPublished = 1 
	WHERE PublishStateId = DBO.Fn_GetPublishStateIdForProcessing()

	------------for CategoryPublishId update
	SELECT ZPCAL.CategoryValue as CategoryCode,MAX(ZPC.PublishCategoryId) as PublishCategoryId ,ZPCA.PimCategoryId,ZPoC.PortalId
	INTO #CategoryData1
	FROM ZnodePimCategoryAttributeValue ZPCA
	INNER JOIN ZnodePimCategoryAttributeValueLocale ZPCAL on ZPCA.PimCategoryAttributeValueId = ZPCAL.PimCategoryAttributeValueId
	INNER JOIN ZnodePimAttribute ZPA ON ZPCA.PimAttributeId = ZPA.PimAttributeId
	INNER JOIN ZnodePublishCategory ZPC on ZPCA.PimCategoryId = ZPC.PimCategoryId
	INNER JOIN ZnodePortalCatalog ZPoC on ZPC.PublishCatalogId = ZPoC.PublishCatalogId
	WHERE ZPA.AttributeCode = 'CategoryCode' AND ZPC.PublishCatalogId = @PublishCatalogId
	GROUP BY ZPCAL.CategoryValue, ZPCA.PimCategoryId, ZPoC.PortalId

	UPDATE ZCWC SET ZCWC.PublishCategoryId = CD.PublishCategoryId
	FROM ZnodeCMSWidgetCategory ZCWC
	INNER JOIN #CategoryData1 CD ON ZCWC.CategoryCode = CD.CategoryCode and ZCWC.CMSMappingId = CD.PortalId
	WHERE ZCWC.TypeOFMapping = 'PortalMapping'

	IF (@RevisionState = 'PREVIEW')
		UPDATE ZnodePimCategory SET PublishStateId = Dbo.Fn_GetPublishStateIdForPreview() WHERE pimCategoryId IN (SELECT PimCategoryId FROM @TBL_PublishPimCategoryIds)
	Else 
		UPDATE ZnodePimCategory SET PublishStateId = Dbo.Fn_GetPublishStateIdForPublish() WHERE pimCategoryId IN (SELECT PimCategoryId FROM @TBL_PublishPimCategoryIds)

COMMIT TRAN GetPublishCategory;
SET @Status =1 
END TRY
BEGIN CATCH
	SELECT ERROR_MESSAGE();
	DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishCategory @PublishCatalogId = '+CAST(@PublishCatalogId AS VARCHAR(50))+',@UserId ='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(50));
	SET @Status = 0;
	ROLLBACK TRAN GetPublishCategory;
	EXEC Znode_InsertProcedureErrorLog
	@ProcedureName = 'Znode_GetPublishCategoryJson',
	@ErrorInProcedure = @Error_procedure,
	@ErrorMessage = @ErrorMessage,
	@ErrorLine = @ErrorLine,
	@ErrorCall = @ErrorCall;
END CATCH;
END;
CREATE PROCEDURE [dbo].[Znode_InsertUpdatePimCatalogProductDetailJson] 
(
	@PublishCatalogId INT = 0, 
	@LocaleId TransferId READONLY, 
	@UserId INT = 0,
	@IsDraftProductsOnly BIT = 1
)
AS 

--declare @LocaleId TransferId
--INSERT INTO @LocaleId
--SELECT 1
--exec [Znode_InsertUpdatePimCatalogProductDetailJson_New] @PublishCatalogId=8,@LocaleId=@LocaleId,@UserId=2,@IsDraftProductsOnly=1
--select * from ZnodePublishCatalog
--declare @LocaleId TransferId
--INSERT INTO @LocaleId
--SELECT 1
----union 
----SELECT 4
----union 
----SELECT 2
--exec [Znode_POC_InsertUpdatePimCatalogProductDetail] @PublishCatalogId=3,@LocaleId=@LocaleId,@UserId=2
BEGIN 
BEGIN TRY 

  SET NOCOUNT ON 
	   IF OBJECT_ID('tempdb..##AttributeValueLocale') IS NOT NULL
		DROP TABLE ##AttributeValueLocale

       DECLARE @LocaleId_In INT = 0 , @DefaultLocaleId INT = dbo.FN_GETDefaultLocaleId()
			   ,@GetDate DATETIME = dbo.fn_GetDate(),@IsActiveAttributeId int , @SKUAttributeId int
	   DECLARE @PimMediaAttributeId INT = dbo.Fn_GetProductImageAttributeId()		   

	   select @IsActiveAttributeId = PimAttributeId from ZnodePimAttribute where AttributeCode = 'IsActive'
	   select @SKUAttributeId = PimAttributeId from ZnodePimAttribute where AttributeCode = 'SKU'

	   CREATE TABLE #PimDefaultValueLocale  (PimAttributeDefaultJsonId INT  PRIMARY KEY ,PimAttributeDefaultValueId INT ,LocaleId INT, DefaultValueJson	nvarchar(max) )

	   CREATE TABLE ##AttributeValueLocale  (Id int Identity,  PimProductId INT, AttributeCode Varchar(300), AttributeValue varchar(max), AttributeEntity varchar(max), LocaleId int )
	   
	   --Creating indexes on temp table
	   CREATE INDEX Idx_#AttributeValueLocale_1 ON ##AttributeValueLocale(PimProductId,AttributeCode,LocaleId)
	   CREATE INDEX Idx_#AttributeValueLocale_2 ON [dbo].[##AttributeValueLocale] ([AttributeCode])
	   INCLUDE ([PimProductId],[AttributeValue],[LocaleId])

	   CREATE TABLE #ZnodePublishCategoryProduct
	   (
			PublishProductId INT,  PimAttributeId INT, PublishCatalogId INT, PimCategoryHierarchyId INT,PublishCategoryId INT, 
			PimAttributeValueId INT, CatalogName NVARCHAR(600),PimProductId INT,AttributeCode VARCHAR(600)
		)
		
		--CREATE INDEX #ZnodePublishCategoryProduct_2 ON #ZnodePublishCategoryProduct(PimAttributeId)
		--CREATE INDEX #ZnodePublishCategoryProduct_3 ON #ZnodePublishCategoryProduct(PimProductId,AttributeCode)
	   --SELECT PimProductId, PimAttributeId, PimAttributeValueId, ModifiedDate INTO ZnodePimAttributeValue FROM ZnodePimAttributeValue
	   --CREATE INDEX ZnodePimAttributeValue_PimAttributeValueId ON ZnodePimAttributeValue(PimAttributeValueId)
	   CREATE TABLE #ModifiedProducts1 (PublishProductId INT, PimCategoryHierarchyId INT,PimProductId INT)

	    --Fetching product SKU record which are active
	    Select ZPAV.PimProductId , ZPAVL.AttributeValue ,ZPAVL.LocaleId
		INTO  #VariantSKU 
		From ZnodePimAttributeValue ZPAV 
		INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON (ZPAV.PimAttributevalueid = ZPAVL.PimAttributeValueId )  
		Where ZPAV.PimAttributeId = @SKUAttributeId
		and exists(Select ZPAV.PimProductId From ZnodePimAttributeValue ZPAV1 INNER JOIN ZnodePimAttributeValueLocale ZPAVL1
				ON (ZPAV1.PimAttributevalueid = ZPAVL1.PimAttributeValueId AND ZPAVL1.AttributeValue = 'True')
				 Where ZPAV1.PimAttributeId = @IsActiveAttributeId and ZPAV.PimProductId = ZPAV1.PimProductId)
		AND EXISTS(SELECT * FROM @LocaleId L WHERE ZPAVL.LocaleId = L.Id)

		CREATE INDEX Idx_#VariantSKU ON #VariantSKU(PimProductId) INCLUDE(AttributeValue)

		--Getting last successful publish datetime
	    DECLARE @PublishModifiedDate Datetime
		SET @PublishModifiedDate =
		(SELECT TOP 1 ModifiedDate 
		FROM ZnodePublishCatalogLog 
		WHERE PublishCatalogId = @PublishCatalogId AND PublishStateId = (SELECT TOP 1 PublishStateId FROM ZnodePublishState WHERE StateName = 'Publish')
		ORDER BY ModifiedDate DESC)

		--SELECT ZPP1.PimProductId , ZPCPD.PublishProductId, ZPCPD.PublishCatalogId,PCL.ModifiedDate
		--INTO #ProductAttributeXML
		--FROM ZnodePublishProduct ZPP1
		--INNER JOIN ZnodePublishCatalogProductDetail ZPCPD ON ZPP1.PublishProductId = ZPCPD.PublishProductId AND ZPCPD.PublishCatalogId = ZPP1.PublishCatalogId 
		--CROSS APPLY #ZnodePublishCatalogLog PCL 
		--WHERE ZPCPD.PublishCatalogId =  @PublishCatalogId 

		--Getting publish product data with different category hierarchy
		SELECT ZPCC.PublishProductId , ZPCC.PublishCatalogId, ZPPC.PimCategoryHierarchyId, ZPPC.PublishCategoryId
		INTO #PublishCategoryProductData
		FROM ZnodePublishCategoryProduct ZPCC
		INNER JOIN ZnodePublishCategory ZPPC ON (ISNULL(ZPPC.PimCategoryHierarchyId,0) = ISNULL(ZPCC.PimCategoryHierarchyId,0) AND ZPPC.PublishCategoryId = ZPCC.PublishCategoryId)
		WHERE ZPCC.PublishCatalogId = @PublishCatalogId

		IF @IsDraftProductsOnly = 1
		BEGIN
			--All product of catalog updaing to draft as new locale is active
			IF (SELECT COUNT(*) FROM @LocaleId) <> (SELECT COUNT(DISTINCT LocaleId) FROM ZnodePublishProductEntity WHERE ZnodeCatalogId = @PublishCatalogId)
			BEGIN
				DECLARE @PublishStateIdForDraftState INT = dbo.Fn_GetPublishStateIdForDraftState()
				UPDATE ZPP SET PublishStateId = @PublishStateIdForDraftState
				FROM ZnodePimProduct ZPP
				WHERE EXISTS(SELECT * FROM ZnodePublishProduct ZPP1 WHERE ZPP.PimProductId = ZPP1.PimProductId AND ZPP1.PublishCatalogId = @PublishCatalogId)

			END

			---------- Products Attribute modified
			SELECT DISTINCT ZPP.PublishProductId,ZPP.PublishCatalogId, ZPP.PimProductId--,  ZPCC.PimCategoryHierarchyId 
			INTO #ModifiedProducts
			FROM ZnodePublishProduct  ZPP
			INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
			WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
			AND EXISTS(SELECT * FROM ZnodePimAttributeValue ZPAV
				INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)  WHERE (ZPAV.PimProductId = ZPP.PimProductId )
				AND EXISTS(SELECT * FROM ZnodePimFamilyGroupMapper ZPFGM WHERE (ZPFGM.PimAttributeFamilyId = ZPPI.PimAttributeFamilyId AND ZPFGM.PimAttributeId = ZPAV.PimAttributeId))
				AND ZPAV.ModifiedDate > @PublishModifiedDate)

			----------Attributes modified so considering all products associated to those attributes
			INSERT INTO #ModifiedProducts
			SELECT DISTINCT ZPP.PublishProductId,ZPP.PublishCatalogId, ZPP.PimProductId--,  ZPCC.PimCategoryHierarchyId 
			FROM ZnodePublishProduct  ZPP
			INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
			WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
			AND EXISTS(SELECT * FROM ZnodePimAttributeValue ZPAV
				INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)  WHERE (ZPAV.PimProductId = ZPP.PimProductId )
				AND EXISTS(SELECT * FROM ZnodePimFamilyGroupMapper ZPFGM WHERE (ZPFGM.PimAttributeFamilyId = ZPPI.PimAttributeFamilyId AND ZPFGM.PimAttributeId = ZPAV.PimAttributeId))
				AND ZPA.ModifiedDate > @PublishModifiedDate)
			
			-------- Products not published  
			INSERT INTO #ModifiedProducts
			SELECT ZPP.PublishProductId,ZPP.PublishCatalogId,ZPP.PimProductId
			FROM ZnodePublishProduct  ZPP
			INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
			WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
			AND EXISTS(SELECT * FROM ZnodePimFamilyGroupMapper ZPFGM WHERE (ZPFGM.PimAttributeFamilyId = ZPPI.PimAttributeFamilyId ))--AND ZPFGM.PimAttributeId = ZPAV.PimAttributeId))
			AND EXISTS(SELECT * FROM ZnodePimProduct ZPP1 INNER JOIN ZnodePublishState ZPS ON ZPP1.PublishStateId = ZPS.PublishStateId
						WHERE StateName <> 'Publish' AND ZPP.PimProductId = ZPP1.PimProductId )	
			AND NOT EXISTS(SELECT * FROM #ModifiedProducts X WHERE ZPP.PublishProductId = X.PublishProductId)
			
			-------- Products associated to catalog or category or modified catalog category products
			INSERT INTO #ModifiedProducts		
			SELECT ZPP.PublishProductId,ZPP.PublishCatalogId,ZPP.PimProductId
			FROM ZnodePublishProduct  ZPP
			INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
			INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId)
			WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
			AND EXISTS(SELECT * FROM ZnodePimFamilyGroupMapper ZPFGM WHERE (ZPFGM.PimAttributeFamilyId = ZPPI.PimAttributeFamilyId ))--AND ZPFGM.PimAttributeId = ZPAV.PimAttributeId))
			AND EXISTS(SELECT * FROM ZnodePimCategoryProduct ZPCC1 WHERE  ZPP.PimProductId = ZPCC1.PimProductId  
				AND ZPCC1.ModifiedDate>@PublishModifiedDate)
			AND NOT EXISTS(SELECT * FROM #ModifiedProducts X WHERE ZPP.PublishProductId = X.PublishProductId)

			---------- Link Product modified
			INSERT INTO #ModifiedProducts	
			SELECT ZPP.PublishProductId,ZPP.PublishCatalogId,ZPP.PimProductId
			FROM ZnodePublishProduct  ZPP
			INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
			WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
			AND EXISTS(SELECT * FROM ZnodePimLinkProductDetail ZPAV WHERE (ZPAV.PimParentProductId = ZPP.PimProductId ) 
				AND  ZPAV.ModifiedDate> @PublishModifiedDate )
			AND NOT EXISTS(SELECT * FROM #ModifiedProducts X WHERE ZPP.PublishProductId = X.PublishProductId)

							
			------Associated child Products (varients, Group) not published	
			INSERT INTO #ModifiedProducts	
			SELECT ZPP.PublishProductId,ZPP.PublishCatalogId,ZPP.PimProductId
			FROM ZnodePublishProduct  ZPP
			INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
			WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
			AND EXISTS(SELECT * FROM ZnodePimProductTypeAssociation ZPAV WHERE (ZPAV.PimProductId = ZPP.PimProductId ) 
				AND EXISTS(SELECT * FROM ZnodePimProduct ZPP1 INNER JOIN ZnodePublishState ZPS ON ZPP1.PublishStateId = ZPS.PublishStateId
						WHERE StateName <> 'Publish' AND ZPAV.PimProductId = ZPP1.PimProductId ))
			AND NOT EXISTS(SELECT * FROM #ModifiedProducts X WHERE ZPP.PublishProductId = X.PublishProductId)
			
			--------Link child Products (Bundle) not published 	
			INSERT INTO #ModifiedProducts
			SELECT ZPP.PublishProductId,ZPP.PublishCatalogId,ZPP.PimProductId
			FROM ZnodePublishProduct  ZPP
			INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
			WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
			AND EXISTS(SELECT * FROM ZnodePimLinkProductDetail ZPAV WHERE (ZPAV.PimProductId = ZPP.PimProductId )
				AND EXISTS(SELECT * FROM ZnodePimProduct ZPP1 INNER JOIN ZnodePublishState ZPS ON ZPP1.PublishStateId = ZPS.PublishStateId
						WHERE StateName <> 'Publish' AND ZPAV.PimProductId = ZPP1.PimProductId ))
			AND NOT EXISTS(SELECT * FROM #ModifiedProducts X WHERE ZPP.PublishProductId = X.PublishProductId)

			----Getting products of newly added category hierarchy 
			INSERT INTO #ModifiedProducts		
			SELECT ZPP.PublishProductId,ZPP.PublishCatalogId,ZPP.PimProductId
			FROM ZnodePublishProduct  ZPP
			INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
			INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId)
			WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
			AND EXISTS(SELECT * FROM ZnodePimFamilyGroupMapper ZPFGM WHERE (ZPFGM.PimAttributeFamilyId = ZPPI.PimAttributeFamilyId ))--AND ZPFGM.PimAttributeId = ZPAV.PimAttributeId))
			AND EXISTS(SELECT * FROM ZnodePimCategoryProduct ZPCC1  
					INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCC1.PimCategoryId = ZPCH.PimCategoryId AND ZPC.PimCatalogId = ZPCH.PimCatalogId  
					WHERE ZPP.PimProductId = ZPCC1.PimProductId 
					AND ZPCH.ModifiedDate > @PublishModifiedDate)
			AND NOT EXISTS(SELECT * FROM #ModifiedProducts X WHERE ZPP.PublishProductId = X.PublishProductId)
			
			SELECT ZnodeProductId, ZnodeCatalogId
			INTO #TempPublishProductEntity
			FROM ZnodePublishProductEntity
			WHERE ZnodeCatalogId = @PublishCatalogId

			IF EXISTS(SELECT * FROM #TempPublishProductEntity)
			BEGIN
				CREATE INDEX Id_#TempPublishProductEntity ON #TempPublishProductEntity(ZnodeProductId,ZnodeCatalogId)
				--Getting product data which are associated with catalog and not present in product entity table
				INSERT INTO #ModifiedProducts		
				SELECT ZPP.PublishProductId,ZPP.PublishCatalogId,ZPP.PimProductId
				FROM ZnodePublishProduct ZPP
				INNER JOIN ZnodePublishCatalogProductDetail ZPCPD ON ZPP.PublishProductId = ZPCPD.PublishProductId AND ZPP.PublishCatalogId = ZPCPD.PublishCatalogId
				WHERE NOT EXISTS(SELECT * FROM #TempPublishProductEntity b where b.ZnodeProductId= ZPCPD.PublishProductId AND B.ZnodeCatalogId = ZPCPD.PublishCatalogId )
				AND NOT EXISTS(SELECT * FROM #ModifiedProducts X WHERE ZPP.PublishProductId = X.PublishProductId)
			END

			--Fetching pimcategoryhierarchyid for published product
			INSERT INTO #ModifiedProducts1(PublishProductId, PimCategoryHierarchyId,PimProductId)
			SELECT DISTINCT ZPP.PublishProductId, ZPCC.PimCategoryHierarchyId,ZPP.PimProductId
			FROM #ModifiedProducts ZPP
			LEFT JOIN #PublishCategoryProductData ZPCC  ON (ZPP.PublishProductId = ZPCC.PublishProductId AND ZPCC.PublishCatalogId = ZPP.PublishCatalogId )
			
			---------------------Category associated to catalog or category or modified catalog
			SELECT ZPCH.PimCategoryId, ZPC1.PublishCategoryId, ZPCH.PimCategoryHierarchyId
			INTO #ModifiedCategory1
			FROM ZnodePimCategoryHierarchy ZPCH 
			INNER JOIN ZnodePublishCategory ZPC1 ON ZPCH.PimCategoryId = ZPC1.PimCategoryId 
			WHERE ZPC1.PublishCatalogId =  @PublishCatalogId 
			AND EXISTS (SELECT TOP 1 1 FROM ZnodePublishCatalogProductDetail BTM  
			WHERE BTM.PublishCatalogId = ZPC1.PublishCatalogId AND (BTM.ModifiedDate < ZPCH.ModifiedDate )   )
			AND NOT EXISTS(SELECT * FROM #ModifiedProducts1 MP WHERE  ISNULL(ZPCH.PimCategoryHierarchyId,0) = ISNULL(MP.PimCategoryHierarchyId,0))

			--INSERT INTO #ModifiedProducts1(PublishProductId, PimCategoryHierarchyId,PimProductId)	
			--SELECT BTM.PublishProductId, BTM.PimCategoryHierarchyId, ZPP.PimProductId
			--FROM ZnodePublishCatalogProductDetail BTM  
			--INNER JOIN ZnodePublishProduct ZPP ON BTM.PublishProductId = ZPP.PublishProductId AND BTM.PublishCatalogId = ZPP.PublishCatalogId
			--WHERE BTM.PublishCatalogId = @PublishCatalogId AND ISNULL(BTM.IsNotPublish,0) = 1
			
			IF EXISTS(SELECT TOP 1 1 FROM #ModifiedCategory1 )
			BEGIN
				-------- Category associated to catalog or category or modified catalog
				INSERT INTO #ModifiedProducts1		
				SELECT ZPP.PublishProductId,  ZPCC.PimCategoryHierarchyId,ZPP.PimProductId 
				FROM ZnodePublishProduct  ZPP
				INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
				INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId)
				INNER JOIN ZnodePimCategoryProduct ZPCC1 ON  ZPP.PimProductId = ZPCC1.PimProductId 
				INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCC1.PimCategoryId = ZPCH.PimCategoryId AND ZPC.PimCatalogId = ZPCH.PimCatalogId 
				LEFT JOIN #PublishCategoryProductData ZPCC  ON (ZPP.PublishProductId = ZPCC.PublishProductId AND ZPCC.PublishCatalogId = ZPP.PublishCatalogId)
				WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
				AND EXISTS(SELECT * FROM ZnodePimFamilyGroupMapper ZPFGM WHERE (ZPFGM.PimAttributeFamilyId = ZPPI.PimAttributeFamilyId ))
				AND EXISTS (SELECT TOP 1 1 FROM #ModifiedCategory1 BTM WHERE BTM.PimCategoryHierarchyId = ZPCH.PimCategoryHierarchyId  )
				AND NOT EXISTS(SELECT * FROM #ModifiedProducts1 X WHERE ZPP.PublishProductId = X.PublishProductId)
				------------------
			END
			
			--CREATE INDEX #ModifiedProducts1_PublishProductId ON #ModifiedProducts1(PublishProductId,PimCategoryHierarchyId)	
			--Getting all products of catalog for publish which are modified after last publish
			INSERT INTO #ZnodePublishCategoryProduct (PublishProductId,  PimAttributeId, PublishCatalogId , PimCategoryHierarchyId , PublishCategoryId,
					   PimAttributeValueId, CatalogName ,PimProductId ,AttributeCode)
			SELECT ZPP.PublishProductId,  ZPAV.PimAttributeId, ZPP.PublishCatalogId , ZPCC.PimCategoryHierarchyId , ZPCC.PublishCategoryId
					,ZPAV.PimAttributeValueId, ZPC.CatalogName--,CASE WHEN ZPCC.PublishProductId IS NULL THEN 1 ELSE  dense_rank()Over(ORDER BY ZPCC.PimCategoryHierarchyId,ZPCC.PublishProductId) END  ProductIndex 	
					,ZPP.PimProductId ,ZPA.AttributeCode				
			FROM ZnodePublishProduct  ZPP
			INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
			INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimProductId = ZPP.PimProductId )
			INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)
			INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId)
			LEFT JOIN #PublishCategoryProductData ZPCC  ON (ZPP.PublishProductId = ZPCC.PublishProductId AND ZPCC.PublishCatalogId = ZPP.PublishCatalogId)
			WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
			AND EXISTS(SELECT * FROM ZnodePimFamilyGroupMapper ZPFGM WHERE (ZPFGM.PimAttributeFamilyId = ZPPI.PimAttributeFamilyId AND ZPFGM.PimAttributeId = ZPAV.PimAttributeId))
			AND EXISTS (SELECT * FROM #ModifiedProducts1 MP WHERE ZPP.PublishProductId = MP.PublishProductId  ) 

		END
		ELSE 
		BEGIN	
			--Getting all products of catalog for publish first time 
			INSERT INTO #ZnodePublishCategoryProduct(PublishProductId,  PimAttributeId, PublishCatalogId , PimCategoryHierarchyId , PublishCategoryId,
				   PimAttributeValueId, CatalogName ,PimProductId ,AttributeCode)
			SELECT ZPP.PublishProductId,  ZPAV.PimAttributeId, ZPP.PublishCatalogId , ZPCC.PimCategoryHierarchyId , ZPCC.PublishCategoryId,
				   ZPAV.PimAttributeValueId, ZPC.CatalogName ,ZPP.PimProductId ,ZPA.AttributeCode				
			FROM ZnodePublishProduct  ZPP
			INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
			INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimProductId = ZPP.PimProductId )
			INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)
			INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId)
			LEFT JOIN #PublishCategoryProductData ZPCC  ON (ZPP.PublishProductId = ZPCC.PublishProductId AND ZPCC.PublishCatalogId = ZPP.PublishCatalogId)
			WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
			AND EXISTS(SELECT * FROM ZnodePimFamilyGroupMapper ZPFGM WHERE (ZPFGM.PimAttributeFamilyId = ZPPI.PimAttributeFamilyId AND ZPFGM.PimAttributeId = ZPAV.PimAttributeId))
			--AND EXISTS(SELECT * FROM ZnodePimCategoryProduct ZPCP Inner Join ZnodePimCategoryHierarchy ZPCH ON ZPCP.PimCategoryId = ZPCH.PimCategoryId
			--WHERE ZPCP.PimProductId = ZPP.PimProductId AND ZPCH.PimCatalogId = ZPC.PimCatalogId)
		
			--Fetching pimcategoryhierarchyid
			INSERT INTO #ModifiedProducts1(PublishProductId, PimCategoryHierarchyId,PimProductId)
			SELECT DISTINCT PublishProductId, PimCategoryHierarchyId,PimProductId
			FROM #ZnodePublishCategoryProduct ZPCC 
			
		END
		
		CREATE INDEX #ModifiedProducts1_PimProductId ON #ModifiedProducts1(PimProductId)

		SELECT ZPAV.PimProductId, ZPP.PublishProductId, ZPAV.LocaleId
		into #ProductLocaleWise
		FROM #VariantSKU ZPAV
		INNER JOIN ZnodePublishProduct ZPP on ZPAV.PimProductId = ZPP.PimProductId
		WHERE EXISTS(SELECT * FROM @LocaleId L WHERE ZPAV.LocaleId = L.Id)
		AND ZPP.PublishCatalogId = @PublishCatalogId

		IF (SELECT COUNT(*) FROM @LocaleId) > 1
		BEGIN
			INSERT INTO #ProductLocaleWise(PimProductId,PublishProductId,LocaleId)
			SELECT a.PimProductId,a.PublishProductId,b.Id
			FROM #ProductLocaleWise a
			CROSS APPLY @LocaleId b 
			WHERE LocaleId = @DefaultLocaleId
			AND b.Id <> @DefaultLocaleId
			AND NOT EXISTS(select * from #ProductLocaleWise c where a.PimProductId = c.PimProductId and c.LocaleId = b.Id)
		END

		------------
		Select Distinct PimProductId,PublishProductId, PublishCatalogId ,PublishCategoryId,CatalogName,PimCategoryHierarchyId
	    into #ZnodePublishCategoryProductForValidation from #ZnodePublishCategoryProduct

		
		CREATE INDEX #ZnodePublishCategoryProductForValidation_PimProductId ON #ZnodePublishCategoryProductForValidation(PimProductId)

		CREATE INDEX IDX_#ZnodePublishCategoryProduct_1 ON #ZnodePublishCategoryProduct(PimProductId)
		
		CREATE INDEX IDX_#ZnodePublishCategoryProduct_3 ON #ZnodePublishCategoryProduct(PublishCategoryId)

		--
		

		------Getting All Link Product Details
		SELECT ZPLPD.PimParentProductId, ZPLPD.PimProductId, ZPLPD.PimAttributeId, ZPAV.AttributeValue as SKU
		INTO #LinkProduct
		FROM ZnodePimLinkProductDetail ZPLPD 
		INNER JOIN #VariantSKU ZPAV ON (ZPAV.PimProductId = ZPLPD.PimProductId)
		WHERE EXISTS(SELECT * FROM #ModifiedProducts1 X WHERE X.PimProductId = ZPLPD.PimParentProductId)
		
		CREATE INDEX #LinkProduct_1 ON #LinkProduct(PimParentProductId,PimAttributeId)
		
		----Getting products link product value entity
	     INSERT INTO ##AttributeValueLocale ( PimProductId, AttributeCode, AttributeValue, AttributeEntity, LocaleId )
	     SELECT ZPLP.PimParentProductId ,ZPAX.AttributeCode, '' AttributeValue , 
		 JSON_MODIFY( JSON_Modify(ZPAX.AttributeJson , '$.AttributeValues' , 
		 ISNULL(SUBSTRING ( (SELECT ','+cast( LP.SKU as varchar(600))
							FROM #LinkProduct LP
							WHERE LP.PimParentProductId = ZPLP.PimParentProductId 
							AND LP.PimAttributeId = ZPLP.PimAttributeId FOR XML PATH('')),2,8000),'') ),'$.SelectValues',Json_Query('[]'))   

							, ZPAX.LocaleId
		 FROM ZnodePimLinkProductDetail ZPLP
		 INNER JOIN ZnodePimAttributeJSON ZPAX ON (ZPAX.PimAttributeId = ZPLP.PimAttributeId )
		 WHERE EXISTS(SELECT * FROM #ZnodePublishCategoryProductForValidation PPCP  WHERE (ZPLP.PimParentProductId = PPCP.PimProductId ))
		 AND EXISTS(SELECT * FROM @LocaleId L WHERE ZPAX.LocaleId = L.Id)
		 GROUP BY ZPLP.PimParentProductId ,ZPAX.AttributeCode , ZPAX.AttributeJSON,ZPAX.LocaleId,ZPAX.AttributeCode,ZPLP.PimAttributeId
		
		----Getting product attribute value entity 
		SELECT PPCP.PimProductId , ZPA.AttributeCode,
		ISNULL(ZPAVL.AttributeValue,'') AttributeValue,
		ZPAVL.LocaleId,ZPA.PimAttributeId---,ZPAX.AttributeJSON
		into #ZnodePimAttributeValueLocale
		FROM ZnodePimAttributeValue PPCP
		INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = PPCP.PimAttributeId)
		INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON (PPCP.PimAttributeValueId =ZPAVL.PimAttributeValueId)
		WHERE EXISTS(SELECT * FROM #ZnodePublishCategoryProductForValidation PPCP1  WHERE PPCP1.PimProductId = PPCP.PimProductId)
		AND NOT EXISTS(select * from ZnodePimConfigureProductAttribute UOP where PPCP.PimProductId = UOP.PimProductId AND PPCP.PimAttributeId = UOP.PimAttributeId)

		create index #ZnodePimAttributeValueLocale_1 on #ZnodePimAttributeValueLocale(PimProductId,AttributeCode,LocaleId)
		create index #ZnodePimAttributeValueLocale_2 on #ZnodePimAttributeValueLocale(PimAttributeId)
		
		SELECT ZPAVL.PimProductId , ZPAVL.AttributeCode,ZPAVL.AttributeValue ,
				JSON_MODIFY(
				JSON_MODIFY (Json_Query( ZPAX.AttributeJSON  ) , '$.AttributeValues' ,  ISNULL(ZPAVL.AttributeValue,'') )    
				,'$.SelectValues',Json_Query('[]'))   
				AS 'AttributeEntity'
				, 
				ZPAVL.LocaleId
		INTO #TempValueLocale
		FROM #ZnodePimAttributeValueLocale ZPAVL  
		INNER JOIN ZnodePimAttributeJSON ZPAX ON (ZPAX.PimAttributeId = ZPAVL.PimAttributeId and ZPAX.LocaleId = ZPAVL.LocaleId)
		WHERE EXISTS(Select Id from @LocaleId x where ZPAVL.LocaleId = x.Id) 
		AND NOT EXISTS(select * from ##AttributeValueLocale AVL where ZPAVL.PimProductId = AVL.PimProductId and ZPAVL.AttributeCode = AVL.AttributeCode and ZPAVL.LocaleId = AVL.LocaleId )
		
		
		----Creating products attribute wise json of attribute value entity
		INSERT INTO ##AttributeValueLocale ( PimProductId, AttributeCode, AttributeValue, AttributeEntity, LocaleId )
		SELECT PimProductId,AttributeCode,AttributeValue,AttributeEntity,LocaleId
		FROM #TempValueLocale

		 IF (SELECT COUNT(*) from @LocaleId) > 1
		 BEGIN
				----Getting product attribute value entity getting for other locale with default attribute json
				  INSERT INTO ##AttributeValueLocale ( PimProductId, AttributeCode, AttributeValue, AttributeEntity, LocaleId )
				  SELECT ZPAVL.PimProductId , ZPAVL.AttributeCode,ZPAVL.AttributeValue ,
							JSON_MODIFY(
							JSON_MODIFY (Json_Query( ZPAX.AttributeJSON  ) , '$.AttributeValues' ,  ISNULL(ZPAVL.AttributeValue,'') )    
							,'$.SelectValues',Json_Query('[]'))   
							AS 'AttributeEntity', 
						 ZPAVL.LocaleId
				  FROM #ZnodePimAttributeValueLocale ZPAVL 
				  INNER JOIN ZnodePimAttributeJSON ZPAX ON (ZPAX.PimAttributeId = ZPAVL.PimAttributeId AND ZPAX.LocaleId = @DefaultLocaleId)
				  WHERE ZPAX.LocaleId = @DefaultLocaleId AND ZPAVL.LocaleId <> @DefaultLocaleId 
				  AND EXISTS(SELECT * FROM @LocaleId L WHERE ZPAVL.LocaleId = L.Id) 
				  AND NOT EXISTS(SELECT * FROM ##AttributeValueLocale AVL where ZPAVL.PimProductId = AVL.PimProductId and ZPAVL.AttributeCode = AVL.AttributeCode and ZPAVL.LocaleId = AVL.LocaleId )
		END
			
		IF OBJECT_ID('TEMPDB..#ZnodePublishCatalogProductDetail') IS NOT NULL
			DROP TABLE #ZnodePublishCatalogProductDetail

		IF OBJECT_ID('TEMPDB..#ZnodePublishCatalogProductDetail1') IS NOT NULL
			DROP TABLE #ZnodePublishCatalogProductDetail1

		IF OBJECT_ID('TEMPDB..#TBL_ProductRequiredAttribute') IS NOT NULL
			DROP TABLE #TBL_ProductRequiredAttribute
		  
		CREATE TABLE #TBL_ProductRequiredAttribute (PimProductId INT,SKU VARCHAR(600),ProductName VARCHAR(600), IsActive VARCHAR(10), LocaleId INT)

		--Getting disctinct products locale wise
		INSERT INTO #TBL_ProductRequiredAttribute(PimProductId, LocaleId)
		SELECT DISTINCT PimProductId, LocaleId FROM ##AttributeValueLocale

		--Updating SKU
		UPDATE #TBL_ProductRequiredAttribute 
		SET SKU = b.AttributeValue
		FROM #TBL_ProductRequiredAttribute a
		INNER JOIN ##AttributeValueLocale b ON a.PimproductId = b.PimProductId AND a.LocaleId = b.LocaleId
		WHERE b.AttributeCode = 'SKU'

		--Updating ProductName
		UPDATE #TBL_ProductRequiredAttribute 
		SET ProductName = b.AttributeValue
		FROM #TBL_ProductRequiredAttribute a
		INNER JOIN ##AttributeValueLocale b ON a.PimproductId = b.PimProductId AND a.LocaleId = b.LocaleId
		WHERE b.AttributeCode = 'ProductName'

		--Updating IsActive
		UPDATE #TBL_ProductRequiredAttribute 
		SET IsActive = b.AttributeValue
		FROM #TBL_ProductRequiredAttribute a
		INNER JOIN ##AttributeValueLocale b ON a.PimproductId = b.PimProductId AND a.LocaleId = b.LocaleId
		WHERE b.AttributeCode = 'IsActive'

		  --CREATE INDEX IDX_#TBL_ProductRequiredAttribute_PimProductId ON #TBL_ProductRequiredAttribute(PimProductId)	  
		  SELECT ZPI.PublishProductId, ZPI.PublishCatalogId ,TYU.PublishCategoryId,ZPI.CatalogName,ISNULL(ZPI.PimCategoryHierarchyId,0) PimCategoryHierarchyId
					,TPAR.SKU,TPAR.ProductName,TPAR.IsActive,TYU.PublishCategoryName CategoryName,ISNULL(TYU.LocaleId,TPAR.LocaleId) as LocaleId
		   INTO #ZnodePublishCatalogProductDetail
		   FROM #ZnodePublishCategoryProduct ZPI
		   INNER JOIN #TBL_ProductRequiredAttribute TPAR ON (TPAR.PimProductId = ZPI.PimProductId )
		   LEFT JOIN ZnodePublishCategoryDetail TYU ON (TYU.PublishCategoryId = ZPI.PublishCategoryId AND TPAR.LocaleId = TYU.LocaleId )
		   GROUP BY PublishProductId, PublishCatalogId ,TYU.PublishCategoryId,CatalogName,PimCategoryHierarchyId
					,SKU,ProductName,TPAR.IsActive,PublishCategoryName, TYU.LocaleId, TPAR.LocaleId
  
			CREATE INDEX IDX_#ZnodePublishCatalogProductDetail ON #ZnodePublishCatalogProductDetail(PublishProductId,PublishCatalogId,PimCategoryHierarchyId,LocaleId)

			--Creating product index
			SELECT PublishProductId,PublishCatalogId,PimCategoryHierarchyId,SKU,ProductName,CategoryName, CatalogName, LocaleId ,IsActive
			      ,CASE WHEN PublishProductId IS NULL THEN 1 ELSE Row_Number()Over(Partition by PublishCatalogId,PublishProductId,LocaleId ORDER BY PublishProductId,PimCategoryHierarchyId,LocaleId) END  ProductIndex
			INTO #ZnodePublishCatalogProductDetail1
			FROM #ZnodePublishCatalogProductDetail Temp
			WHERE EXISTS(SELECT * FROM #ProductLocaleWise PL WHERE Temp.PublishProductId = PL.PublishProductId)
			  
			--Getting locale wise product which are not present for other locale (other than default locale)  
			INSERT INTO #ZnodePublishCatalogProductDetail1 (PublishProductId,PublishCatalogId,PimCategoryHierarchyId,SKU,ProductName,CategoryName, CatalogName, LocaleId ,IsActive,ProductIndex)
			SELECT PublishProductId,PublishCatalogId,PimCategoryHierarchyId,SKU,ProductName,CategoryName, CatalogName, b.Id ,IsActive,ProductIndex
			FROM #ZnodePublishCatalogProductDetail1 a
			CROSS APPLY @LocaleId b 
			WHERE NOT EXISTS(SELECT * FROM #ZnodePublishCatalogProductDetail1 c WHERE a.PublishProductId = c.PublishProductId AND b.Id = c.LocaleId  )
			AND a.LocaleId = @DefaultLocaleId 
			AND a.PublishCatalogId = @PublishCatalogId

			----Deleting product data which are not present
			--DELETE ZPCPD 
			--FROM ZnodePublishCatalogProductDetail ZPCPD
			--WHERE NOT EXISTS(SELECT * FROM #ProductLocaleWise ZPCPD1 WHERE ZPCPD.PublishProductId = ZPCPD1.PublishProductId 
			--                 AND ZPCPD.LocaleId = ZPCPD1.LocaleId )  
			--AND ZPCPD.PublishCatalogId = @PublishCatalogId
			
			--Deleting product data which are not present in category hierarchy
			DELETE ZPCPD 
			FROM ZnodePublishCatalogProductDetail ZPCPD
			WHERE EXISTS(SELECT * FROM #ZnodePublishCatalogProductDetail ZPCPD1 WHERE 
						ZPCPD.PublishProductId = ZPCPD1.PublishProductId 
			            AND ZPCPD.LocaleId = ZPCPD1.LocaleId 
						AND ISNULL(ZPCPD1.PimCategoryHierarchyId,0)  <> 0 ) AND ZPCPD.PimCategoryHierarchyId =0
			AND ZPCPD.PublishCatalogId = @PublishCatalogId
			
			create index #ZnodePublishCatalogProductDetail1_1 on #ZnodePublishCatalogProductDetail1(SKU,LocaleId)
			create index #ZnodePublishCatalogProductDetail1_2 on #ZnodePublishCatalogProductDetail1(PublishProductId,PublishCatalogId,PimCategoryHierarchyId,LocaleId)
			----Update data ZnodePublishCatalogProductDetail 
			UPDATE TARGET
			SET  TARGET.ProductIndex	=SOURCE.ProductIndex
				,TARGET.SKU = SOURCE.SKU
				,TARGET.ModifiedBy		= @UserId	
				,TARGET.ModifiedDate	= @GetDate
			FROM ZnodePublishCatalogProductDetail TARGET
			INNER JOIN #ZnodePublishCatalogProductDetail1 SOURCE
			ON (
		        SOURCE.PublishProductId = TARGET.PublishProductId
				AND SOURCE.PublishCatalogId = TARGET.PublishCatalogId 
				AND ISNULL(SOURCE.PimCategoryHierarchyId,0) = ISNULL(TARGET.PimCategoryHierarchyId,0)
				AND SOURCE.LocaleId = TARGET.LocaleId --@LocaleId_In
				)
			WHERE TARGET.PublishCatalogId = @PublishCatalogId

			----Update data ZnodePublishCatalogProductDetail 
			UPDATE TARGET
			SET  
				TARGET.ProductName		=SOURCE.ProductName
				,TARGET.CatalogName		=SOURCE.CatalogName
				,TARGET.IsActive		=case when SOURCE.IsActive in ('0','false') then 0 else 1 end 
				,TARGET.ModifiedBy		= @UserId	
				,TARGET.ModifiedDate	= @GetDate
			FROM ZnodePublishCatalogProductDetail TARGET
			INNER JOIN #ZnodePublishCatalogProductDetail1 SOURCE
			ON (
		        TARGET.SKU = SOURCE.SKU
				AND SOURCE.LocaleId = TARGET.LocaleId --@LocaleId_In
				)
			WHERE TARGET.PublishCatalogId = @PublishCatalogId

			
		----Update data ZnodePublishCatalogProductDetail 
		UPDATE TARGET
		SET  
			TARGET.CategoryName	=SOURCE.CategoryName
			,TARGET.ModifiedBy		= @UserId	
			,TARGET.ModifiedDate	= @GetDate
		FROM ZnodePublishCatalogProductDetail TARGET
		INNER JOIN #ZnodePublishCatalogProductDetail1 SOURCE
		ON (
		    TARGET.SKU = SOURCE.SKU
			AND SOURCE.LocaleId = TARGET.LocaleId 
			AND ISNULL(SOURCE.PimCategoryHierarchyId,0) = ISNULL(TARGET.PimCategoryHierarchyId,0)
			)
		WHERE ISNULL(TARGET.PimCategoryHierarchyId,0) <> 0
		AND TARGET.PublishCatalogId = @PublishCatalogId

		----Insert data ZnodePublishCatalogProductDetail 
		INSERT INTO ZnodePublishCatalogProductDetail
			( PublishProductId,PublishCatalogId,PimCategoryHierarchyId,SKU,ProductName,CategoryName, CatalogName,
				LocaleId ,IsActive,ProductIndex,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate )
		SELECT SOURCE.PublishProductId ,SOURCE.PublishCatalogId ,SOURCE.PimCategoryHierarchyId ,SOURCE.SKU ,SOURCE.ProductName
		,SOURCE.CategoryName ,SOURCE.CatalogName ,SOURCE.LocaleId ,SOURCE.IsActive ,SOURCE.ProductIndex ,@UserId ,@GetDate ,@UserId ,@GetDate
		FROM #ZnodePublishCatalogProductDetail1 SOURCE
		WHERE NOT EXISTS(SELECT * FROM ZnodePublishCatalogProductDetail TARGET WHERE SOURCE.PublishProductId = TARGET.PublishProductId
						AND SOURCE.PublishCatalogId = TARGET.PublishCatalogId 
						AND SOURCE.PimCategoryHierarchyId = TARGET.PimCategoryHierarchyId 
						AND TARGET.LocaleId = SOURCE.LocaleId )
		AND SOURCE.PublishCatalogId = @PublishCatalogId
				
		----Inserting product data for other locale		  
		INSERT INTO ZnodePublishCatalogProductDetail (PublishProductId,PublishCatalogId,PimCategoryHierarchyId,SKU,ProductName,CategoryName, CatalogName,
				LocaleId ,IsActive,ProductIndex,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		SELECT PublishProductId,PublishCatalogId,PimCategoryHierarchyId,SKU,ProductName,CategoryName, CatalogName,
				b.Id ,IsActive,ProductIndex,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
		FROM ZnodePublishCatalogProductDetail a
		CROSS APPLY @LocaleId b 
		WHERE NOT EXISTS(SELECT * FROM ZnodePublishCatalogProductDetail c WHERE a.PublishProductId = c.PublishProductId AND b.Id = c.LocaleId  )
		AND a.LocaleId = @DefaultLocaleId
		AND a.PublishCatalogId = @PublishCatalogId
		
		DELETE ZPCPD FROM ZnodePublishCatalogProductDetail ZPCPD
		INNER JOIN ZnodePublishProduct ZPD ON ZPCPD.PublishProductId = ZPD.PublishProductId AND ZPCPD.PublishCatalogId = ZPD.PublishCatalogId
		INNER JOIN ZnodePublishCatalog ZPC ON ZPCPD.PublishCatalogId = ZPC.PublishCatalogId
		WHERE NOT EXISTS(SELECT * FROM ZnodePimCategoryProduct ZPCC 
			    INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCC.PimCategoryId = ZPCH.PimCategoryId WHERE ZPD.PimProductId = ZPCC.PimProductId AND ZPC.PimCatalogId = ZPCH.PimCatalogId AND ZPCPD.PimCategoryHierarchyId = ZPCH.PimCategoryHierarchyId)
		AND ZPCPD.PimCategoryHierarchyId <> 0
		AND ZPCPD.PublishCatalogId = @PublishCatalogId
		
		----Updating the SKU into table ZnodePublishCatalogProductDetail
		UPDATE ZnodePublishCatalogProductDetail 
		SET SKU = b.AttributeValue
		FROM ZnodePublishCatalogProductDetail a
		INNER JOIN ZnodePublishProduct ZPP ON a.PublishProductId = ZPP.PublishProductId
		INNER JOIN ##AttributeValueLocale b ON ZPP.PimproductId = b.PimProductId AND a.LocaleId = b.LocaleId
		WHERE b.AttributeCode = 'SKU'
		AND a.PublishCatalogId = @PublishCatalogId

		----Updating the ProductName into table ZnodePublishCatalogProductDetail
		UPDATE ZnodePublishCatalogProductDetail 
		SET ProductName = b.AttributeValue
		FROM ZnodePublishCatalogProductDetail a
		INNER JOIN ZnodePublishProduct ZPP ON a.PublishProductId = ZPP.PublishProductId
		INNER JOIN ##AttributeValueLocale b ON ZPP.PimproductId = b.PimProductId AND a.LocaleId = b.LocaleId
		WHERE b.AttributeCode = 'ProductName'
		AND a.PublishCatalogId = @PublishCatalogId

		----Updating the IsActive into table ZnodePublishCatalogProductDetail
		UPDATE ZnodePublishCatalogProductDetail 
		SET IsActive = b.AttributeValue
		FROM ZnodePublishCatalogProductDetail a
		INNER JOIN ZnodePublishProduct ZPP ON a.PublishProductId = ZPP.PublishProductId
		INNER JOIN ##AttributeValueLocale b ON ZPP.PimproductId = b.PimProductId AND a.LocaleId = b.LocaleId
		WHERE b.AttributeCode = 'IsActive'
		AND a.PublishCatalogId = @PublishCatalogId
		
		---- To clean Older Hierarchies associations.
		DELETE ZPCPD
		FROM ZnodePublishCatalogProductDetail ZPCPD
		WHERE NOT EXISTS (select top 1 1 from ZnodePimCategoryHierarchy ZPCH where (ZPCPD.PimCategoryHierarchyId = ZPCH.PimCategoryHierarchyId))
		AND ISNULL(ZPCPD.PimCategoryHierarchyId,0) <>0
		
		SELECT a.PimProductId,  a.PimAttributeId, a.PimAttributeValueId, b.PimAttributeDefaultValueId, b.LocaleId
		INTO #PimProductAttributeDefaultValue
		FROM ZnodePimAttributeValue a 
		INNER JOIN ZnodePimProductAttributeDefaultValue b ON a.PimAttributeValueId = b.PimAttributeValueId 
		WHERE EXISTS(SELECT * FROM #ZnodePublishCategoryProductForValidation X WHERE X.PimProductId = a.PimProductId)
		
		CREATE INDEX Idx_#PimProductAttributeDefaultValue ON #PimProductAttributeDefaultValue (PimProductId,PimAttributeId)
		--CREATE INDEX Idx_#PimProductAttributeDefaultValue_1 ON #PimProductAttributeDefaultValue (PimAttributeDefaultValueId,LocaleId)

		SELECT  AA.DefaultValueJson , ZPADV.PimAttributeValueId, AA.LocaleId 
		INTO #PimAttributeDefaultXML
		FROM ZnodePimAttributeDefaultJSON AA 
		INNER JOIN #PimProductAttributeDefaultValue ZPADV ON ( ZPADV.PimAttributeDefaultValueId = AA.PimAttributeDefaultValueId AND AA.LocaleId = ZPADV.LocaleId)
		
		--create index #PimAttributeDefaultXML on #PimAttributeDefaultXML(PimAttributeValueId,LocaleId)
		--To get the data localwise
		IF (SELECT COUNT(*) FROM @LocaleId)>1
		BEGIN
			INSERT INTO #PimAttributeDefaultXML (DefaultValueJson, PimAttributeValueId, LocaleId)
			SELECT A.DefaultValueJson , A.PimAttributeValueId,b.id 
			FROM #PimAttributeDefaultXML A CROSS APPLY  @LocaleId b  
			WHERE NOT EXISTS ( SELECT * FROM #PimAttributeDefaultXML c WHERE a.PimAttributeValueId  = c.PimAttributeValueId AND c.LocaleId = b.Id )
			AND b.Id <> @DefaultLocaleId
		END
		
		------Getting child facets for merging   
		SELECT DISTINCT ZPPADV.PimAttributeDefaultValueId, ZPAV_Parent.PimAttributeValueId, ZPPADV.LocaleId
		INTO #PimChildProductFacets
		FROM ZnodePimAttributeValue ZPAV_Parent
		INNER JOIN ZnodePimProductTypeAssociation ZPPTA ON ZPAV_Parent.PimProductId = ZPPTA.PimParentProductId
		INNER JOIN #PimProductAttributeDefaultValue ZPPADV ON ZPPTA.PimProductId = ZPPADV.PimProductId AND ZPAV_Parent.PimAttributeId = ZPPADV.PimAttributeId
		WHERE EXISTS(SELECT * FROM ZnodePimFrontendProperties ZPFP WHERE ZPAV_Parent.PimAttributeId = ZPFP.PimAttributeId AND ZPFP.IsFacets = 1)
		AND EXISTS(SELECT * FROM #ZnodePublishCategoryProduct ZPPC WHERE ZPAV_Parent.PimProductId = ZPPC.PimProductId )
		AND NOT EXISTS(SELECT * FROM #PimProductAttributeDefaultValue ZPPADV1 WHERE ZPAV_Parent.PimAttributeValueId = ZPPADV1.PimAttributeValueId 
		                AND ZPPADV1.PimAttributeDefaultValueId = ZPPADV.PimAttributeDefaultValueId )

		----Merging childs facet attribute Default value XML for parent
		INSERT INTO #PimAttributeDefaultXML (DefaultValueJson, PimAttributeValueId, LocaleId)
		SELECT ZPADX.DefaultValueJson, ZPPADV.PimAttributeValueId, ZPPADV.LocaleId
		FROM #PimChildProductFacets ZPPADV		  
		INNER JOIN ZnodePimAttributeDefaultJSON ZPADX ON ( ZPPADV.PimAttributeDefaultValueId = ZPADX.PimAttributeDefaultValueId AND ZPPADV.LocaleId = ZPADX.LocaleId)
		
		CREATE INDEX Idx_#PimDefaultValueLocale ON #PimDefaultValueLocale(PimAttributeDefaultJsonId,LocaleId)

		CREATE INDEX Idx_#PimAttributeDefaultXML ON #PimAttributeDefaultXML(PimAttributeValueId,LocaleId)
		INCLUDE (DefaultValueJson)

		CREATE INDEX #ZnodePublishCategoryProduct_2 ON #ZnodePublishCategoryProduct(PimProductId,PimAttributeId)
		--CREATE INDEX #ZnodePublishCategoryProduct_3 ON #ZnodePublishCategoryProduct(PimProductId,AttributeCode)
		
		SELECT PPCP.PimProductId, PPCP.AttributeCode,PPCP.PimAttributeValueId, PPCP.PimAttributeId,ZPAX.AttributeJson,ZPAX.LocaleId
		INTO #ZnodeProductattributeDefaultData
		FROM #ZnodePublishCategoryProduct PPCP
		INNER JOIN ZnodePimAttributeJSON ZPAX ON (ZPAX.PimAttributeId = PPCP.PimAttributeId)
		WHERE EXISTS(SELECT * FROM #PimProductAttributeDefaultValue a  WHERE PPCP.PimProductId = a.PimProductId AND PPCP.PimAttributeId = a.PimAttributeId )
		AND EXISTS(SELECT * FROM @LocaleId L WHERE ZPAX.LocaleId = L.Id)
		AND NOT EXISTS(SELECT * FROM ZnodePimConfigureProductAttribute UOP WHERE PPCP.PimProductId = UOP.PimProductId AND PPCP.PimAttributeId = UOP.PimAttributeId )
		--AND NOT EXISTS(SELECT * FROM ##AttributeValueLocale AVL WHERE PPCP.PimProductId = AVL.PimProductId AND PPCP.AttributeCode = AVL.AttributeCode AND ZPAX.LocaleId = AVL.LocaleId )
		 
	
		--CREATE INDEX IDX_#ZnodeProductattributeDefaultData_1 ON #ZnodeProductattributeDefaultData(PimProductId,PimAttributeId)
		CREATE INDEX IDX_#ZnodeProductattributeDefaultData_2 ON #ZnodeProductattributeDefaultData(PimProductId,AttributeCode,LocaleId)
		CREATE INDEX IDX_#ZnodeProductattributeDefaultData_3 ON #ZnodeProductattributeDefaultData(PimAttributeValueId,LocaleId)
		
		SELECT PPCP.PimProductId, PPCP.AttributeCode,'' AttributeValue,
		JSON_MODIFY (JSON_MODIFY (PPCP.AttributeJson,'$.AttributeValues',''), '$.SelectValues',
			
				ISNULL((SELECT 
							ISNULL(JSON_VALUE(DefaultValueJson, '$.Code'),'') Code 
							,ISNULL(JSON_VALUE(DefaultValueJson, '$.LocaleId'),0) LocaleId
							,ISNULL(JSON_VALUE(DefaultValueJson, '$.Value'),'') Value
							,ISNULL(JSON_VALUE(DefaultValueJson, '$.AttributeDefaultValue'),'') AttributeDefaultValue
							,ISNULL(JSON_VALUE(DefaultValueJson, '$.DisplayOrder'),0) DisplayOrder
							,ISNULL(JSON_VALUE(DefaultValueJson, '$.IsEditable'),'false') IsEditable
							,ISNULL(JSON_VALUE(DefaultValueJson, '$.SwatchText'),'') SwatchText
							,ISNULL(JSON_VALUE(DefaultValueJson, '$.Path'),'') Path
					FROM #PimAttributeDefaultXML aa
					WHERE (aa.PimAttributeValueId = PPCP.PimAttributeValueId AND AA.LocaleId = PPCP.LocaleId ) For JSON Auto 
				),'[]') 
				) 
			 AttributeEntity 
		 , PPCP.LocaleId
		 INTO #DefaultData
		 FROM #ZnodeProductattributeDefaultData PPCP 
		 WHERE NOT EXISTS(SELECT * FROM ##AttributeValueLocale AVL WHERE PPCP.PimProductId = AVL.PimProductId AND PPCP.AttributeCode = AVL.AttributeCode AND PPCP.LocaleId = AVL.LocaleId )
		
		----Getting default attribute value entity
		INSERT INTO ##AttributeValueLocale		
		SELECT PimProductId,AttributeCode,'' AttributeValue,AttributeEntity,LocaleId
		FROM #DefaultData
		
		 
		--Getting text attribute value entity		
		SELECT PPCP.PimProductId , ZPA.AttributeCode,ISNULL(ZPAVL.AttributeValue,'') AttributeValue,ZPAVL.LocaleId,ZPA.PimAttributeId
		INTO #TempTxtArea
		FROM ZnodePimAttributeValue PPCP
		INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = PPCP.PimAttributeId)
		INNER JOIN ZnodePimProductAttributeTextAreaValue ZPAVL ON (PPCP.PimAttributeValueId =ZPAVL.PimAttributeValueId)
		WHERE EXISTS (Select * from @LocaleId x where ZPAVL.LocaleId = x.Id) AND
		EXISTS(SELECT * FROM #ZnodePublishCategoryProductForValidation PPCP1  WHERE PPCP1.PimProductId = PPCP.PimProductId)
		
		CREATE INDEX Idx_#TempTxtArea_1 ON #TempTxtArea(PimAttributeId,LocaleId)
		CREATE INDEX Idx_#TempTxtArea_2 ON #TempTxtArea(PimProductId,AttributeCode,LocaleId)

		--Getting tex area attribute value entity		
		SELECT ZPAVL.PimProductId , ZPAVL.AttributeCode,'' AttributeValue ,
			JSON_MODIFY (JSON_MODIFY (Json_Query( ZPAX.AttributeJSON  ) , '$.AttributeValues' ,  ISNULL(ZPAVL.AttributeValue,'') ) ,'$.SelectValues',Json_Query('[]'))
		    AS 'AttributeEntity', 
		 ZPAVL.LocaleId
		INTO #TempTxtAreaEntity
		FROM #TempTxtArea ZPAVL 
		INNER JOIN ZnodePimAttributeJSON ZPAX ON (ZPAX.PimAttributeId = ZPAVL.PimAttributeId AND ZPAVL.LocaleId = ZPAX.LocaleId)

		IF (SELECT COUNT(*) FROM @LocaleId) > 1
		BEGIN
			----Getting product attribute value entity getting for other locale with text area attribute json
			INSERT INTO #TempTxtAreaEntity(PimProductId,AttributeCode,AttributeValue,AttributeEntity,LocaleId)
			SELECT ZPAVL.PimProductId , ZPAVL.AttributeCode,'' AttributeValue ,
				JSON_MODIFY (JSON_MODIFY (Json_Query( ZPAX.AttributeJSON  ) , '$.AttributeValues' ,  ISNULL(ZPAVL.AttributeValue,'') ) ,'$.SelectValues',Json_Query('[]'))
				AS 'AttributeEntity', 
			 ZPAVL.LocaleId
			FROM #TempTxtArea ZPAVL 
			INNER JOIN ZnodePimAttributeJSON ZPAX ON (ZPAX.PimAttributeId = ZPAVL.PimAttributeId AND ZPAX.LocaleId = @DefaultLocaleId)
			WHERE Exists (SELECT TOP 1 1 FROM @LocaleId X WHERE X.Id =  ZPAVL.LocaleId )
			AND ZPAX.LocaleId = @DefaultLocaleId AND ZPAVL.LocaleId <> @DefaultLocaleId
			AND NOT EXISTS(SELECT * FROM #TempTxtAreaEntity ZPAVL1 WHERE ZPAVL.PimProductId = ZPAVL1.PimProductId AND ZPAVL.AttributeCode = ZPAVL1.AttributeCode AND ZPAVL.LocaleId = ZPAVL1.LocaleId)
		END
		
		INSERT INTO ##AttributeValueLocale ( PimProductId, AttributeCode, AttributeValue, AttributeEntity, LocaleId )
		Select PimProductId , AttributeCode,'' AttributeValue ,AttributeEntity, LocaleId 
		FROM #TempTxtAreaEntity

		 ----Getting custom field value entity
		 INSERT INTO ##AttributeValueLocale ( PimProductId, AttributeCode, AttributeValue, AttributeEntity, LocaleId )
 		 SELECT ZPCFX.PimProductId , ZPCFX.CustomCode, '' AttributeValue ,
			JSON_MODIFY (Json_Query( ZPCFX.CustomeFiledJson) ,'$.SelectValues',Json_Query('[]')) AttributeEntity, 
			ZPCFX.LocaleId
		 FROM ZnodePimCustomeFieldJSON ZPCFX 
		 WHERE EXISTS (Select * from @LocaleId x where ZPCFX.LocaleId = x.Id) 
		 AND EXISTS(SELECT * FROM #ZnodePublishCategoryProduct PPCP WHERE (PPCP.PimProductId = ZPCFX.PimProductId ))
		 AND NOT EXISTS(SELECT * FROM ##AttributeValueLocale AVL WHERE ZPCFX.PimProductId = AVL.PimProductId AND ZPCFX.CustomCode = AVL.AttributeCode AND ZPCFX.LocaleId = AVL.LocaleId )
		 GROUP BY ZPCFX.PimProductId , ZPCFX.CustomCode, ZPCFX.CustomeFiledJson , ZPCFX.LocaleId
		 
		 --Getting Media attribute value entity		
		SELECT PPCP.PimProductId , ZPA.AttributeCode,ISNULL(ZPAVL.MediaPath,'') AttributeValue,ZPAVL.LocaleId,ZPA.PimAttributeId,PPCP.PimAttributeValueId
		INTO #TempMedia
		FROM ZnodePimAttributeValue PPCP
		INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = PPCP.PimAttributeId)
		INNER JOIN ZnodePimProductAttributeMedia ZPAVL ON (PPCP.PimAttributeValueId =ZPAVL.PimAttributeValueId)
		WHERE EXISTS (SELECT * FROM @LocaleId x where ZPAVL.LocaleId = x.Id) AND
		EXISTS(SELECT * FROM #ZnodePublishCategoryProductForValidation PPCP1  WHERE PPCP1.PimProductId = PPCP.PimProductId)
		
		CREATE INDEX #TempMedia_1 ON #TempMedia(PimAttributeValueId,LocaleId)
		CREATE INDEX #TempMedia_2 ON #TempMedia(PimAttributeId)
		CREATE INDEX #TempMedia_3 ON #TempMedia(PimProductId,AttributeCode,LocaleId)

		 ----Getting image attribute value entity
		 INSERT INTO ##AttributeValueLocale ( PimProductId, AttributeCode, AttributeValue, AttributeEntity, LocaleId )
		 SELECT PPCP.PimProductId, PPCP.AttributeCode,'' AttributeValue,
		 JSON_MODIFY (JSON_MODIFY (Json_Query( ZPAX.AttributeJSON  ) , '$.AttributeValues',  
		 ISNULL((SELECT stuff( (SELECT ','+ZPPAM.AttributeValue FROM #TempMedia ZPPAM WHERE (ZPPAM.PimAttributeValueId = PPCP.PimAttributeValueId AND PPCP.LocaleId = ZPAX.LocaleId )
				 FOR XML PATH(''),Type).value('.', 'varchar(max)'), 1, 1, '')
				 
				 ),'') ) ,'$.SelectValues',Json_Query('[]'))   
				 AS 'AttributeEntity', 
				 ZPAX.LocaleId
		 FROM #TempMedia PPCP 
		 INNER JOIN ZnodePimAttributeJSON ZPAX ON (ZPAX.PimAttributeId = PPCP.PimAttributeId AND ZPAX.LocaleId = PPCP.LocaleId)
		 WHERE Exists (SELECT TOP 1 1 FROM @LocaleId X WHERE X.Id =  ZPAX.LocaleId )  
		 AND NOT EXISTS(SELECT * FROM ##AttributeValueLocale AVL WHERE PPCP.PimProductId = AVL.PimProductId AND PPCP.AttributeCode = AVL.AttributeCode AND ZPAX.LocaleId = PPCP.LocaleId )
		 AND NOT EXISTS(SELECT * FROM ZnodePimConfigureProductAttribute UOP WHERE ZPAX.PimAttributeId = UOP.PimAttributeId AND PPCP.PimProductId = UOP.PimProductId )
	
		 IF (SELECT COUNT(*) FROM @LocaleId) > 1
		 BEGIN
				----Getting product attribute value entity getting for other locale with default attribute json
				INSERT INTO ##AttributeValueLocale ( PimProductId, AttributeCode, AttributeValue, AttributeEntity, LocaleId )
				 SELECT PPCP.PimProductId, PPCP.AttributeCode,'' AttributeValue,
				 JSON_MODIFY (JSON_MODIFY (Json_Query( ZPAX.AttributeJSON  ) , '$.AttributeValues',  
				 ISNULL((SELECT stuff( (SELECT ','+ZPPAM.AttributeValue FROM #TempMedia ZPPAM WHERE (ZPPAM.PimAttributeValueId = PPCP.PimAttributeValueId AND PPCP.LocaleId = ZPAX.LocaleId )
						 FOR XML PATH(''),Type).value('.', 'varchar(max)'), 1, 1, '')
				 
						 ),'') ) ,'$.SelectValues',Json_Query('[]'))   
						 AS 'AttributeEntity', 
						 PPCP.LocaleId
				 FROM #TempMedia PPCP 
				 INNER JOIN ZnodePimAttributeJSON ZPAX ON (ZPAX.PimAttributeId = PPCP.PimAttributeId AND ZPAX.LocaleId = @DefaultLocaleId)
				 WHERE ZPAX.LocaleId = @DefaultLocaleId AND PPCP.LocaleId <> @DefaultLocaleId 
				 AND Exists (SELECT TOP 1 1 FROM @LocaleId X WHERE X.Id =  PPCP.LocaleId )  
				 AND NOT EXISTS(SELECT * FROM ##AttributeValueLocale AVL WHERE PPCP.PimProductId = AVL.PimProductId AND PPCP.AttributeCode = AVL.AttributeCode AND ZPAX.LocaleId = AVL.LocaleId )
				 AND NOT EXISTS(SELECT * FROM ZnodePimConfigureProductAttribute UOP WHERE ZPAX.PimAttributeId = UOP.PimAttributeId AND PPCP.PimProductId = UOP.PimProductId )
		END
		 -------------configurable attribute 		 
		INSERT INTO ##AttributeValueLocale ( PimProductId, AttributeCode, AttributeValue, AttributeEntity, LocaleId )
		SELECT DISTINCT UOP.PimProductId,c.AttributeCode,'' AttributeValue ,--'<Attributes><AttributeEntity>'+
		JSON_MODIFY (Isnull(JSON_MODIFY (c.AttributeJson,'$.AttributeValues',''),'')  ,'$.SelectValues',
			Isnull((SELECT DISTINCT 
							Isnull(JSON_VALUE(AA.DefaultValueJson, '$.Code'),'') Code 
							,Isnull(JSON_VALUE(AA.DefaultValueJson, '$.LocaleId'),0) LocaleId
							,Isnull(JSON_VALUE(AA.DefaultValueJson, '$.Value'),'') Value
							,Isnull(JSON_VALUE(AA.DefaultValueJson, '$.AttributeDefaultValue'),'') AttributeDefaultValue
							,Isnull(JSON_VALUE(AA.DefaultValueJson, '$.DisplayOrder'),0) DisplayOrder
							,Isnull(JSON_VALUE(AA.DefaultValueJson, '$.IsEditable'),'false') IsEditable
							,Isnull(JSON_VALUE(AA.DefaultValueJson, '$.SwatchText'),'') SwatchText
							,Isnull(JSON_VALUE(AA.DefaultValueJson, '$.Path'),'') Path 
							,ISNULL(ZPA.DisplayOrder,0)  AS VariantDisplayOrder 
							,ISNULL(ZPAVL_SKU.AttributeValue,'')   AS VariantSKU 
							,Isnull(ZPAV12.AttributeValue,'') AS VariantImagePath 
						 FROM ZnodePimAttributeDefaultJSON AA 
						 INNER JOIN #PimProductAttributeDefaultValue ZPADV ON ( ZPADV.PimAttributeDefaultValueId = AA.PimAttributeDefaultValueId )
						 INNER JOIN ZnodePimProductTypeAssociation YUP ON (YUP.PimProductId = ZPADV.PimProductId)
						 INNER JOIN #VariantSKU ZPAVL_SKU On YUP.PimProductId = ZPAVL_SKU.PimProductId
						 LEFT JOIN #TempMedia ZPAV12 ON (ZPAV12.PimProductId= YUP.PimProductId  AND ZPAV12.PimAttributeId = @PimMediaAttributeId ) 
						 LEFT JOIN ZnodePimAttribute ZPA ON (ZPA.PimattributeId = ZPADV.PimAttributeId)
						 WHERE (YUP.PimParentProductId  = UOP.PimProductId AND ZPADV.pimAttributeId = UOP.PimAttributeId )
		FOR JSON auto),'[]')) SelectValuesEntity ,
		c.LocaleId
		FROM ZnodePimConfigureProductAttribute UOP 
		INNER JOIN ZnodePimAttributeJSON c   ON (c.PimAttributeId = UOP.PimAttributeId )
		WHERE Exists (Select TOP 1 1 from @LocaleId X where X.Id = C.LocaleId )  
		AND EXISTS(SELECT * FROM #ZnodePublishCategoryProductForValidation PPCP1 where UOP.PimProductId = PPCP1.PimProductId )

		---Insert product attributewise data for other locale of default locale as local ise attribute data not present
		IF (SELECT COUNT(*) FROM @LocaleId) > 1
		BEGIN
			DECLARE @cur_Id int
			DECLARE cur_LocaleId CURSOR LOCAL FAST_FORWARD
			FOR SELECT Id FROM @LocaleId WHERE Id <> @DefaultLocaleId;

			OPEN cur_LocaleId;
			FETCH NEXT FROM cur_LocaleId INTO  @cur_Id;

			WHILE @@FETCH_STATUS = 0
			BEGIN 
				INSERT INTO ##AttributeValueLocale (PimProductId, AttributeCode, AttributeValue, AttributeEntity, LocaleId)
				SELECT A.PimProductId, A.AttributeCode, A.AttributeValue, A.AttributeEntity, b.Id
				FROM ##AttributeValueLocale a
				CROSS APPLY @LocaleId b 
				WHERE NOT EXISTS(SELECT * FROM ##AttributeValueLocale c WHERE a.PimProductId = c.PimProductId AND b.Id = c.LocaleId AND a.AttributeCode = c.AttributeCode )
				AND a.LocaleId = @DefaultLocaleId AND b.Id = @cur_Id

				FETCH NEXT FROM cur_LocaleId INTO  @cur_Id;
			END;
			CLOSE cur_LocaleId;
			DEALLOCATE cur_LocaleId;
		END
		-------------configurable attribute 
		--select * from #AttributeValueLocale

		--CREATE INDEX IDX_#AttributeValueLocale ON #AttributeValueLocale(PimProductId,AttributeCode,LocaleId)
		--CREATE INDEX IDX_#AttributeValueLocale_Id ON #AttributeValueLocale(ID)
		 	
		--DELETE ZPPAX FROM ZnodePublishProductAttributeJson ZPPAX
		--WHERE exists (SELECT * FROM #AttributeValueLocale AVL WHERE ZPPAX.PimProductId = AVL.PimProductId AND AVL.LocaleId = ZPPAX.LocaleId )
		--AND NOT EXISTS(SELECT * FROM #AttributeValueLocale AVL WHERE ZPPAX.PimProductId = AVL.PimProductId AND AVL.LocaleId = ZPPAX.LocaleId AND ZPPAX.AttributeCode = AVL.AttributeCode )

		--DECLARE @MaxCount INT, @MinRow INT, @MaxRow INT, @Rows numeric(10,2);
		--SELECT @MaxCount = COUNT(*) FROM #AttributeValueLocale;

		--SELECT @Rows = 200000
        
		--SELECT @MaxCount = CEILING(@MaxCount / @Rows);

		--IF OBJECT_ID('tempdb..#Temp_ImportLoop') IS NOT NULL
  --          DROP TABLE #Temp_ImportLoop;
        
		------ To get the min AND max rows for import in loop
		--;WITH cte AS 
		--(
		--	SELECT RowId = 1, 
		--		   MinRow = 1, 
  --                 MaxRow = cast(@Rows as int)
  --          UNION ALL
  --          SELECT RowId + 1, 
  --                 MinRow + cast(@Rows as int), 
  --                 MaxRow + cast(@Rows as int)
  --          FROM cte
  --          WHERE RowId + 1 <= @MaxCount
		--)
  --      SELECT RowId, MinRow, MaxRow
  --      INTO #Temp_ImportLoop
  --      FROM cte
		--OPTION (maxrecursion 0);

		----Cursor for rows wise execution in bulk
		--DECLARE cur_BulkData CURSOR LOCAL FAST_FORWARD
  --      FOR SELECT MinRow, MaxRow FROM #Temp_ImportLoop
		--WHERE EXISTS(SELECT * FROM #AttributeValueLocale);

  --      OPEN cur_BulkData;
  --      FETCH NEXT FROM cur_BulkData INTO  @MinRow, @MaxRow;

  --      WHILE @@FETCH_STATUS = 0
  --      BEGIN
	 --        UPDATE ZnodePublishProductAttributeJson SET IsUpdateLocaleWise = 0 WHERE ISNULL(IsUpdateLocaleWise,0) = 1
		--	  ----Update Product Attribute XML
		--	 UPDATE ZPPAX SET ZPPAX.Attributes = AVL.AttributeEntity, ZPPAX.ModifiedBy = @UserId, ZPPAX.ModifiedDate = @GetDate
		--	        , ZPPAX.IsUpdateLocaleWise = 0
		--	 FROM ZnodePublishProductAttributeJson ZPPAX 
		--	 INNER JOIN #AttributeValueLocale AVL ON ZPPAX.PimProductId = AVL.PimProductId AND AVL.LocaleId = ZPPAX.LocaleId AND ZPPAX.AttributeCode = AVL.AttributeCode 
		--	 WHERE  AVL.Id BETWEEN @MinRow AND @MaxRow AND AVL.AttributeEntity is not null
		 
		--	 ----Insert Product Attribute XML
		--	 INSERT INTO ZnodePublishProductAttributeJson(PimProductId,LocaleId,AttributeCode,Attributes,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
		--	 SELECT AVL.PimProductId, AVL.LocaleId, AVL.AttributeCode, cast(AVL.AttributeEntity as varchar(max)), @UserId CreatedBy, @GetDate CreatedDate, @UserId ModifiedBy, @GetDate ModifiedDate
		--	 FROM #AttributeValueLocale AVL
		--	 WHERE NOT EXISTS(SELECT * FROM ZnodePublishProductAttributeJson ZPPAX WHERE AVL.PimProductId = ZPPAX.PimProductId AND  AVL.LocaleId = ZPPAX.LocaleId AND AVL.AttributeCode = ZPPAX.AttributeCode )
		--	 AND  AVL.Id BETWEEN @MinRow AND @MaxRow AND AVL.AttributeEntity is not null
		--	 GROUP BY AVL.PimProductId, AVL.AttributeEntity, AVL.LocaleId, AVL.AttributeCode

		--	 FETCH NEXT FROM cur_BulkData INTO  @MinRow, @MaxRow;
  --      END;
		--CLOSE cur_BulkData;
		--DEALLOCATE cur_BulkData;

		--DELETE ZPPAX
		--FROM ZnodePublishProductAttributeJson ZPPAX
		--WHERE LocaleId <> @DefaultLocaleId
		--AND exists( SELECT * FROM ZnodePublishProductAttributeJson ZPPAX1 WHERE ZPPAX.AttributeCode = ZPPAX1.AttributeCode AND ZPPAX.PimProductId = ZPPAX1.PimProductId )
		--AND NOT EXISTS(SELECT * FROM #ProductLocaleWise AVL WHERE AVL.PimProductId = ZPPAX.PimProductId AND  AVL.LocaleId = ZPPAX.LocaleId )
		
		
		--DELETE  ZPPAX
		--FROM ZnodePublishProductAttributeJson ZPPAX
		--WHERE NOT EXISTS(SELECT * FROM #ProductLocaleWise ZLW WHERE ZPPAX.PimProductId = ZLW.PimProductId 
		--	                AND ZPPAX.LocaleId = ZLW.LocaleId )

		--SELECT PimProductId,Attributes Attributes,AttributeCode
		--INTO #ZnodePublishProductAttributeJson
		--FROM ZnodePublishProductAttributeJson 
		--WHERE LocaleId = @DefaultLocaleId

		--INSERT INTO ZnodePublishProductAttributeJson (PimProductId,Attributes,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,AttributeCode)
		--SELECT PimProductId,Attributes,b.id,@UserId,@GetDate,@UserId,@GetDate,AttributeCode
		--FROM #ZnodePublishProductAttributeJson a
		--CROSS APPLY @LocaleId b 
		--WHERE NOT EXISTS(SELECT * FROM ZnodePublishProductAttributeJson c WHERE a.PimProductId = c.PimProductId AND b.Id = c.LocaleId AND a.AttributeCode = c.AttributeCode )
		--AND b.Id <> @DefaultLocaleId
					  
END TRY
BEGIN CATCH
select ERROR_MESSAGE()
    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertUpdatePimCatalogProductDetailJson @PublishCatalogId = '+CAST(@PublishCatalogId AS VARCHAR(200))+',@UserId='+CAST(@UserId AS VARCHAR(200));


    EXEC Znode_InsertProcedureErrorLog
        @ProcedureName = 'Znode_InsertUpdatePimCatalogProductDetailJson',
        @ErrorInProcedure = @Error_procedure,
        @ErrorMessage = @ErrorMessage,
        @ErrorLine = @ErrorLine,
        @ErrorCall = @ErrorCall;
            
            
END CATCH;
END
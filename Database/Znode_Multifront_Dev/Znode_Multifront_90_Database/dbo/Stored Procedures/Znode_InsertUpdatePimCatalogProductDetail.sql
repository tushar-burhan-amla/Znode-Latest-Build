CREATE PROCEDURE [dbo].[Znode_InsertUpdatePimCatalogProductDetail] 
(
  @PublishCatalogId INT = 0 
  ,@LocaleId TransferId READONLY 
  ,@UserId INT = 0   
)
AS 
--declare @LocaleId TransferId
--insert into @LocaleId
--select 1
--union 
--select 4
--exec [Znode_InsertUpdatePimCatalogProductDetail] @PublishCatalogId=4,@LocaleId=@LocaleId,@UserId=2
BEGIN 
 BEGIN TRY 

  SET NOCOUNT ON 
       DECLARE @LocaleId_In INT = 0 , @DefaultLocaleId INT = dbo.FN_GETDefaultLocaleId()
			   ,@GetDate DATETIME = dbo.fn_GetDate()
	   DECLARE @PimMediaAttributeId INT = dbo.Fn_GetProductImageAttributeId()		   

	   CREATE TABLE #PimDefaultValueLocale  (PimAttributeDefaultXMLId INT  PRIMARY KEY ,PimAttributeDefaultValueId INT ,LocaleId INT, DefaultValueXML	nvarchar(max) )

	   CREATE TABLE #AttributeValueLocale  ( ID Int Identity Primary Key,PimProductId int, AttributeCode Varchar(300), AttributeValue varchar(max), AttributeEntity varchar(max), LocaleId int )

	    SELECT ZPAV.PimProductId, ZPP.PublishProductId, ZPAVL.LocaleId
		into #ProductLocaleWise
		FROM ZnodePimAttributeValue ZPAV 
		INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
		inner join ZnodePublishProduct ZPP on ZPAV.PimProductId = ZPP.PimProductId
		WHERE ZPAV.PimAttributeId = (select PimAttributeId from ZnodePimAttribute where AttributeCode = 'SKU')

		SELECT BTM.PimProductId , ZPCPD.PublishProductId, ZPCPD.PublishCatalogId,BTM.ModifiedDate
		into #ProductAttributeXML
		FROM ZnodePublishProductAttributeXML BTM 
		inner join ZnodePublishProduct ZPP1 ON BTM.PimProductId = ZPP1.PimProductId
		inner join ZnodePublishCatalogProductDetail ZPCPD ON ZPP1.PublishProductId = ZPCPD.PublishProductId AND ZPCPD.PublishCatalogId = ZPP1.PublishCatalogId 
		WHERE ZPCPD.PublishCatalogId =  @PublishCatalogId 

	    -------- Products Attribute modified 
		SELECT DISTINCT ZPP.PublishProductId,  ZPCC.PimCategoryHierarchyId 
		Into #ModifiedProducts
		FROM ZnodePublishProduct  ZPP
		INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
		INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimProductId = ZPP.PimProductId )
		INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)
		INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategoryProduct ZPCC  ON (ZPP.PublishProductId = ZPCC.PublishProductId AND ZPCC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategory ZPPC ON (isnull(ZPPC.PimCategoryHierarchyId,0) = isnull(ZPCC.PimCategoryHierarchyId,0) AND ZPPC.PublishCategoryId = ZPCC.PublishCategoryId)
		WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
		AND EXISTS(SELECT * FROM ZnodePimFamilyGroupMapper ZPFGM WHERE (ZPFGM.PimAttributeFamilyId = ZPPI.PimAttributeFamilyId AND ZPFGM.PimAttributeId = ZPAV.PimAttributeId))
		AND EXISTS (SELECT TOP 1 1 FROM #ProductAttributeXML BTM WHERE BTM.PimProductId = ZPP.PimProductId AND BTM.PublishCatalogId = ZPP.PublishCatalogId
						AND (BTM.ModifiedDate < ZPAV.ModifiedDate OR BTM.ModifiedDate < ZPA.ModifiedDate)   ) 
		
		-------- Products not published  
		Insert Into #ModifiedProducts
		SELECT ZPP.PublishProductId,  ZPCC.PimCategoryHierarchyId 
		FROM ZnodePublishProduct  ZPP
		INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
		INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategoryProduct ZPCC  ON (ZPP.PublishProductId = ZPCC.PublishProductId AND ZPCC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategory ZPPC ON (isnull(ZPPC.PimCategoryHierarchyId,0) = isnull(ZPCC.PimCategoryHierarchyId,0) AND ZPPC.PublishCategoryId = ZPCC.PublishCategoryId)
		WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
		AND EXISTS(SELECT * FROM ZnodePimFamilyGroupMapper ZPFGM WHERE (ZPFGM.PimAttributeFamilyId = ZPPI.PimAttributeFamilyId ))--AND ZPFGM.PimAttributeId = ZPAV.PimAttributeId))
		AND exists(select * from ZnodePimProduct ZPP1 INNER JOIN ZnodePublishState ZPS ON ZPP1.PublishStateId = ZPS.PublishStateId
					where StateName <> 'Publish' and ZPP.PimProductId = ZPP1.PimProductId )	
			
		-------- Products associated to catalog or category or modified catalog category products
		Insert Into #ModifiedProducts		
		SELECT ZPP.PublishProductId,  ZPCC.PimCategoryHierarchyId 
		FROM ZnodePublishProduct  ZPP
		INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
		INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId)
		INNER JOIN ZnodePimCategoryProduct ZPCC1 ON  ZPP.PimProductId = ZPCC1.PimProductId 
		INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCC1.PimCategoryId = ZPCH.PimCategoryId and ZPC.PimCatalogId = ZPCH.PimCatalogId 
		LEFT  JOIN ZnodePublishCategoryProduct ZPCC  ON (ZPP.PublishProductId = ZPCC.PublishProductId AND ZPCC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategory ZPPC ON (isnull(ZPPC.PimCategoryHierarchyId,0) = isnull(ZPCC.PimCategoryHierarchyId,0) AND ZPPC.PublishCategoryId = ZPCC.PublishCategoryId)
		WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
		AND EXISTS(SELECT * FROM ZnodePimFamilyGroupMapper ZPFGM WHERE (ZPFGM.PimAttributeFamilyId = ZPPI.PimAttributeFamilyId ))--AND ZPFGM.PimAttributeId = ZPAV.PimAttributeId))
		AND EXISTS (SELECT TOP 1 1 FROM #ProductAttributeXML BTM WHERE BTM.PimProductId = ZPCC1.PimProductId AND BTM.PublishCatalogId = ZPP.PublishCatalogId
						AND (BTM.ModifiedDate < ZPCC1.ModifiedDate )   )	 

		-------- Link Product modified 
		Insert Into #ModifiedProducts	
		SELECT ZPP.PublishProductId,  ZPCC.PimCategoryHierarchyId 
		FROM ZnodePublishProduct  ZPP
		INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
		INNER JOIN ZnodePimLinkProductDetail ZPAV ON (ZPAV.PimParentProductId = ZPP.PimProductId )
		INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategoryProduct ZPCC  ON (ZPP.PublishProductId = ZPCC.PublishProductId AND ZPCC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategory ZPPC ON (isnull(ZPPC.PimCategoryHierarchyId,0) = isnull(ZPCC.PimCategoryHierarchyId,0) AND ZPPC.PublishCategoryId = ZPCC.PublishCategoryId)
		WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
		--AND EXISTS(SELECT * FROM ZnodePimFamilyGroupMapper ZPFGM WHERE (ZPFGM.PimAttributeFamilyId = ZPPI.PimAttributeFamilyId AND ZPFGM.PimAttributeId = ZPAV.PimAttributeId))
		AND EXISTS (SELECT TOP 1 1 FROM #ProductAttributeXML BTM WHERE BTM.PimProductId = ZPP.PimProductId AND BTM.PublishCatalogId = ZPP.PublishCatalogId
						AND (BTM.ModifiedDate < ZPAV.ModifiedDate)   ) 

		--------Associated child Products (varients, Group) not published	
		Insert Into #ModifiedProducts	
		SELECT ZPP.PublishProductId,  ZPCC.PimCategoryHierarchyId 
		FROM ZnodePublishProduct  ZPP
		INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
		INNER JOIN ZnodePimProductTypeAssociation ZPAV ON (ZPAV.PimProductId = ZPP.PimProductId )
		INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategoryProduct ZPCC  ON (ZPP.PublishProductId = ZPCC.PublishProductId AND ZPCC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategory ZPPC ON (isnull(ZPPC.PimCategoryHierarchyId,0) = isnull(ZPCC.PimCategoryHierarchyId,0) AND ZPPC.PublishCategoryId = ZPCC.PublishCategoryId)
		WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
		AND exists(select * from ZnodePimProduct ZPP1 INNER JOIN ZnodePublishState ZPS ON ZPP1.PublishStateId = ZPS.PublishStateId
					where StateName <> 'Publish' and ZPAV.PimProductId = ZPP1.PimProductId )

		--------Link child Products (Bundle) not published 	
		Insert Into #ModifiedProducts
		SELECT ZPP.PublishProductId,  ZPCC.PimCategoryHierarchyId 
		FROM ZnodePublishProduct  ZPP
		INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
		INNER JOIN ZnodePimLinkProductDetail ZPAV ON (ZPAV.PimProductId = ZPP.PimProductId )
		INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategoryProduct ZPCC  ON (ZPP.PublishProductId = ZPCC.PublishProductId AND ZPCC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategory ZPPC ON (isnull(ZPPC.PimCategoryHierarchyId,0) = isnull(ZPCC.PimCategoryHierarchyId,0) AND ZPPC.PublishCategoryId = ZPCC.PublishCategoryId)
		WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
		AND exists(select * from ZnodePimProduct ZPP1 INNER JOIN ZnodePublishState ZPS ON ZPP1.PublishStateId = ZPS.PublishStateId
					where StateName <> 'Publish' and ZPAV.PimProductId = ZPP1.PimProductId )
		
		--------Associated child Products (varients, Group) attribute modified 
		Insert Into #ModifiedProducts	
		SELECT ZPP.PublishProductId,  ZPCC.PimCategoryHierarchyId 
		FROM ZnodePublishProduct  ZPP
		INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
		INNER JOIN ZnodePimProductTypeAssociation ZPAV ON (ZPAV.PimProductId = ZPP.PimProductId )
		INNER JOIN ZnodePimAttributeValue ZPAV1 ON (ZPAV1.PimProductId = ZPAV.PimProductId )
		INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategoryProduct ZPCC  ON (ZPP.PublishProductId = ZPCC.PublishProductId AND ZPCC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategory ZPPC ON (isnull(ZPPC.PimCategoryHierarchyId,0) = isnull(ZPCC.PimCategoryHierarchyId,0) AND ZPPC.PublishCategoryId = ZPCC.PublishCategoryId)
		WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
		AND EXISTS (SELECT TOP 1 1 FROM #ProductAttributeXML BTM WHERE BTM.PimProductId = ZPAV.PimProductId AND BTM.PublishCatalogId = ZPP.PublishCatalogId
						AND (BTM.ModifiedDate < ZPAV1.ModifiedDate)   )
		
		

		---------------------Category associated to catalog or category or modified catalog
		SELECT ZPCH.PimCategoryId, ZPC1.PublishCategoryId, ZPCH.PimCategoryHierarchyId
		into #ModifiedCategory
		FROM ZnodePimCategoryHierarchy ZPCH 
		INNER JOIN ZnodePublishCategory ZPC1 ON ZPCH.PimCategoryId = ZPC1.PimCategoryId 
        WHERE ZPC1.PublishCatalogId =  @PublishCatalogId 
		AND EXISTS (SELECT TOP 1 1 FROM ZnodePublishCatalogProductDetail BTM  
		WHERE BTM.PublishCatalogId = ZPC1.PublishCatalogId AND (BTM.ModifiedDate < ZPCH.ModifiedDate )   )
		and not exists(select * from #ModifiedProducts MP where  isnull(ZPCH.PimCategoryHierarchyId,0) = isnull(MP.PimCategoryHierarchyId,0))

		-------- Category associated to catalog or category or modified catalog
		Insert Into #ModifiedProducts		
		SELECT ZPP.PublishProductId,  ZPCC.PimCategoryHierarchyId 
		FROM ZnodePublishProduct  ZPP
		INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
		INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId)
		INNER JOIN ZnodePimCategoryProduct ZPCC1 ON  ZPP.PimProductId = ZPCC1.PimProductId 
		INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCC1.PimCategoryId = ZPCH.PimCategoryId and ZPC.PimCatalogId = ZPCH.PimCatalogId 
		LEFT  JOIN ZnodePublishCategoryProduct ZPCC  ON (ZPP.PublishProductId = ZPCC.PublishProductId AND ZPCC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategory ZPPC ON (isnull(ZPPC.PimCategoryHierarchyId,0) = isnull(ZPCC.PimCategoryHierarchyId,0) AND ZPPC.PublishCategoryId = ZPCC.PublishCategoryId)
		WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
		AND EXISTS(SELECT * FROM ZnodePimFamilyGroupMapper ZPFGM WHERE (ZPFGM.PimAttributeFamilyId = ZPPI.PimAttributeFamilyId ))
		AND EXISTS (SELECT TOP 1 1 FROM #ModifiedCategory BTM where BTM.PimCategoryHierarchyId = ZPCH.PimCategoryHierarchyId  ) 
		------------------

		--Getting all products of catalog for publish first time  
		SELECT ZPP.PublishProductId,  ZPAV.PimAttributeId, ZPP.PublishCatalogId , ZPCC.PimCategoryHierarchyId , ZPCC.PublishCategoryId,
		       ZPAV.PimAttributeValueId, ZPC.CatalogName ,ZPP.PimProductId ,ZPA.AttributeCode				
		into #ZnodePublishCategoryProduct
		FROM ZnodePublishProduct  ZPP
		INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
		INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimProductId = ZPP.PimProductId )
		INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)
		INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategoryProduct ZPCC  ON (ZPP.PublishProductId = ZPCC.PublishProductId AND ZPCC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategory ZPPC ON (isnull(ZPPC.PimCategoryHierarchyId,0) = isnull(ZPCC.PimCategoryHierarchyId,0) AND ZPPC.PublishCategoryId = ZPCC.PublishCategoryId)
		WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
		AND EXISTS(SELECT * FROM ZnodePimFamilyGroupMapper ZPFGM WHERE (ZPFGM.PimAttributeFamilyId = ZPPI.PimAttributeFamilyId AND ZPFGM.PimAttributeId = ZPAV.PimAttributeId))
		AND NOT EXISTS (SELECT TOP 1 1 FROM #ProductAttributeXML BTM WHERE BTM.PimProductId = ZPP.PimProductId AND BTM.PublishCatalogId = ZPP.PublishCatalogId)

		--Getting all products of catalog for publish which are modified after last publish
		INSERT INTO #ZnodePublishCategoryProduct 
		SELECT ZPP.PublishProductId,  ZPAV.PimAttributeId, ZPP.PublishCatalogId , ZPCC.PimCategoryHierarchyId , ZPCC.PublishCategoryId
			   ,ZPAV.PimAttributeValueId, ZPC.CatalogName--,CASE WHEN ZPCC.PublishProductId IS NULL THEN 1 ELSE  dense_rank()Over(ORDER BY ZPCC.PimCategoryHierarchyId,ZPCC.PublishProductId) END  ProductIndex 	
			   ,ZPP.PimProductId ,ZPA.AttributeCode				
		FROM ZnodePublishProduct  ZPP
		INNER JOIN ZnodePimProduct ZPPI ON (ZPPI.PimProductId = ZPP.PimProductId)
		INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimProductId = ZPP.PimProductId )
		INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = ZPAV.PimAttributeId)
		INNER JOIN ZnodePublishCatalog ZPC ON (ZPC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategoryProduct ZPCC  ON (ZPP.PublishProductId = ZPCC.PublishProductId AND ZPCC.PublishCatalogId = ZPP.PublishCatalogId)
		LEFT  JOIN ZnodePublishCategory ZPPC ON (ZPPC.PimCategoryHierarchyId = ZPCC.PimCategoryHierarchyId AND ZPPC.PublishCategoryId = ZPCC.PublishCategoryId)
		WHERE ZPP.PublishCatalogId =  @PublishCatalogId 
		AND EXISTS(SELECT * FROM ZnodePimFamilyGroupMapper ZPFGM WHERE (ZPFGM.PimAttributeFamilyId = ZPPI.PimAttributeFamilyId AND ZPFGM.PimAttributeId = ZPAV.PimAttributeId))
		AND EXISTS (SELECT * from #ModifiedProducts MP where ZPP.PublishProductId = MP.PublishProductId AND isnull(ZPCC.PimCategoryHierarchyId,0) = isnull(MP.PimCategoryHierarchyId,0)) 


		CREATE INDEX IDX_#ZnodePublishCategoryProduct_PimProductId ON #ZnodePublishCategoryProduct(PimProductId)
		CREATE INDEX IDX_#ZnodePublishCategoryProduct_PublishCategoryId ON #ZnodePublishCategoryProduct(PublishCategoryId)

		CREATE INDEX IDX_#ZnodePublishCategoryProduct_PimAttributeValueId ON #ZnodePublishCategoryProduct(PimAttributeValueId)
		CREATE INDEX IDX_#ZnodePublishCategoryProduct_PimAttributeId ON #ZnodePublishCategoryProduct(PimAttributeId)
		 
		------Getting All Link Product Details
		Select ZPLPD.PimParentProductId, ZPLPD.PimProductId, ZPLPD.PimAttributeId, ZPAVL.AttributeValue as SKU
		into #LinkProduct
		FROM ZnodePimLinkProductDetail ZPLPD 
		INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimProductId = ZPLPD.PimProductId)
		INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON ZPAV.PimAttributeValueId = ZPAVL.PimAttributeValueId
		WHERE exists(select * from ZnodePimAttribute ZPA where ZPA.PimAttributeId = ZPAV.PimAttributeId and ZPA.AttributeCode = 'SKU')
		
		 ----Getting products link product value entity
	     INSERT INTO #AttributeValueLocale ( PimProductId, AttributeCode, AttributeValue, AttributeEntity, LocaleId )
	     SELECT ZPLP.PimParentProductId ,ZPAX.AttributeCode, '' AttributeValue , ZPAX.AttributeXML+'<AttributeValues>' + 
		 stuff( (SELECT ','+cast( LP.SKU as varchar(600))
							FROM #LinkProduct LP
							WHERE LP.PimParentProductId = ZPLP.PimParentProductId 
							AND LP.PimAttributeId = ZPLP.PimAttributeId
							FOR XML PATH ('')), 1, 1, '')  +'</AttributeValues>', ZPAX.LocaleId
		 FROM ZnodePimLinkProductDetail ZPLP
		 INNER JOIN ZnodePimAttributeXML ZPAX ON (ZPAX.PimAttributeId = ZPLP.PimAttributeId )
		 WHERE EXISTS(SELECT * FROM #ZnodePublishCategoryProduct PPCP  WHERE (ZPLP.PimParentProductId = PPCP.PimProductId ))
		 GROUP BY ZPLP.PimParentProductId ,ZPAX.AttributeCode , ZPAX.AttributeXML,ZPAX.LocaleId,ZPAX.AttributeCode,ZPLP.PimAttributeId


		  ----Getting product attribute value entity
	      INSERT INTO #AttributeValueLocale ( PimProductId, AttributeCode, AttributeValue, AttributeEntity, LocaleId )
		  SELECT PPCP.PimProductId , ZPA.AttributeCode,ZPAVL.AttributeValue ,
		         ZPAX.AttributeXML + '<AttributeValues>'+(select ''+ISNULL(ZPAVL.AttributeValue,'') FOR XML PATH (''))+'</AttributeValues>'  AttributeEntity, ZPAVL.LocaleId
		  FROM ZnodePimAttributeValue PPCP
		  INNER JOIN ZnodePimAttribute ZPA ON (ZPA.PimAttributeId = PPCP.PimAttributeId)
		  INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON (PPCP.PimAttributeValueId =ZPAVL.PimAttributeValueId)
		  INNER JOIN ZnodePimAttributeXML ZPAX ON (ZPAX.PimAttributeId = ZPA.PimAttributeId and ZPAX.LocaleId = ZPAVL.LocaleId)
		  WHERE EXISTS(SELECT * FROM #ZnodePublishCategoryProduct PPCP1  WHERE PPCP1.PimProductId = PPCP.PimProductId)--(PPCP1.PimAttributeValueId =PPCP.PimAttributeValueId) AND (ZPA.PimAttributeId = PPCP1.PimAttributeId))
		  AND not exists(select * from #AttributeValueLocale AVL where PPCP.PimProductId = AVL.PimProductId and ZPA.AttributeCode = AVL.AttributeCode and ZPAVL.LocaleId = AVL.LocaleId )
		  and not exists(select * from ZnodePimConfigureProductAttribute UOP where ZPAX.PimAttributeId = UOP.PimAttributeId and PPCP.PimProductId = UOP.PimProductId )
		 
		  IF OBJECT_ID('TEMPDB..#ZnodePublishCatalogProductDetail') IS NOT NULL
			DROP TABLE #ZnodePublishCatalogProductDetail

		  IF OBJECT_ID('TEMPDB..#ZnodePublishCatalogProductDetail1') IS NOT NULL
			DROP TABLE #ZnodePublishCatalogProductDetail1

		  IF OBJECT_ID('TEMPDB..#TBL_ProductRequiredAttribute') IS NOT NULL
			DROP TABLE #TBL_ProductRequiredAttribute
		  			
		  
		create table #TBL_ProductRequiredAttribute (PimProductId int,SKU varchar(600),ProductName varchar(600), IsActive varchar(10), LocaleId INT)

		insert into #TBL_ProductRequiredAttribute(PimProductId, LocaleId)
		select distinct PimProductId, LocaleId from #AttributeValueLocale

		update #TBL_ProductRequiredAttribute 
		set SKU = b.AttributeValue
		from #TBL_ProductRequiredAttribute a
		inner join #AttributeValueLocale b on a.PimproductId = b.PimProductId AND a.LocaleId = b.LocaleId
		where b.AttributeCode = 'SKU'

		update #TBL_ProductRequiredAttribute 
		set ProductName = b.AttributeValue
		from #TBL_ProductRequiredAttribute a
		inner join #AttributeValueLocale b on a.PimproductId = b.PimProductId AND a.LocaleId = b.LocaleId
		where b.AttributeCode = 'ProductName'

		update #TBL_ProductRequiredAttribute 
		set IsActive = b.AttributeValue
		from #TBL_ProductRequiredAttribute a
		inner join #AttributeValueLocale b on a.PimproductId = b.PimProductId AND a.LocaleId = b.LocaleId
		where b.AttributeCode = 'IsActive'

		  CREATE INDEX IDX_#TBL_ProductRequiredAttribute_PimProductId ON #TBL_ProductRequiredAttribute(PimProductId)

		  SELECT ZPI.PublishProductId, ZPI.PublishCatalogId ,TYU.PublishCategoryId,ZPI.CatalogName,ISNULL(ZPI.PimCategoryHierarchyId,0) PimCategoryHierarchyId
					,TPAR.SKU,TPAR.ProductName,TPAR.IsActive,TYU.PublishCategoryName CategoryName,TPAR.LocaleId
		   into #ZnodePublishCatalogProductDetail
		   FROM #ZnodePublishCategoryProduct ZPI
		   INNER JOIN #TBL_ProductRequiredAttribute TPAR ON (TPAR.PimProductId = ZPI.PimProductId )
		   LEFT JOIN ZnodePublishCategoryDetail TYU ON (TYU.PublishCategoryId = ZPI.PublishCategoryId)
		   GROUP BY PublishProductId, PublishCatalogId ,TYU.PublishCategoryId,CatalogName,PimCategoryHierarchyId
					,SKU,ProductName,TPAR.IsActive,PublishCategoryName, TPAR.LocaleId  

						
			CREATE INDEX IDX_#ZnodePublishCatalogProductDetail ON #ZnodePublishCatalogProductDetail(PublishProductId,PublishCatalogId,PimCategoryHierarchyId,LocaleId)

			SELECT PublishProductId,PublishCatalogId,PimCategoryHierarchyId,SKU,ProductName,CategoryName, CatalogName, LocaleId ,IsActive
			      ,CASE WHEN PublishProductId IS NULL THEN 1 ELSE Row_Number()Over(Partition by PublishProductId ORDER BY PublishProductId,PimCategoryHierarchyId) END  ProductIndex
			INTO #ZnodePublishCatalogProductDetail1
			from #ZnodePublishCatalogProductDetail


			insert into #ZnodePublishCatalogProductDetail1 (PublishProductId,PublishCatalogId,PimCategoryHierarchyId,SKU,ProductName,CategoryName, CatalogName, LocaleId ,IsActive,ProductIndex)
			select PublishProductId,PublishCatalogId,PimCategoryHierarchyId,SKU,ProductName,CategoryName, CatalogName, b.Id ,IsActive,ProductIndex
			from #ZnodePublishCatalogProductDetail1 a
			cross apply @LocaleId b 
			where not exists(select * from #ZnodePublishCatalogProductDetail1 c where a.PublishProductId = c.PublishProductId and b.Id = c.LocaleId  )
			and a.LocaleId = @DefaultLocaleId 

			DELETE ZPCPD 
			from ZnodePublishCatalogProductDetail ZPCPD
			WHERE not exists(select * from #ProductLocaleWise ZPCPD1 where ZPCPD.PublishProductId = ZPCPD1.PublishProductId 
			                 and ZPCPD.LocaleId = ZPCPD1.LocaleId )  

			----Update data ZnodePublishCatalogProductDetail 
			UPDATE TARGET
			SET  TARGET.ProductIndex	=SOURCE.ProductIndex
				,TARGET.ModifiedBy		= @UserId	
				,TARGET.ModifiedDate	= @GetDate
			from ZnodePublishCatalogProductDetail TARGET
			INNER JOIN #ZnodePublishCatalogProductDetail1 SOURCE
			ON (
		        SOURCE.PublishProductId = TARGET.PublishProductId
				AND SOURCE.PublishCatalogId = TARGET.PublishCatalogId 
				AND isnull(SOURCE.PimCategoryHierarchyId,0) = isnull(TARGET.PimCategoryHierarchyId,0)
				AND SOURCE.LocaleId = TARGET.LocaleId --@LocaleId_In
				)

			----Update data ZnodePublishCatalogProductDetail 
			UPDATE TARGET
			SET  
				TARGET.ProductName		=SOURCE.ProductName
				,TARGET.CategoryName	=SOURCE.CategoryName
				,TARGET.CatalogName		=SOURCE.CatalogName
				,TARGET.IsActive		=case when SOURCE.IsActive in ('0','false') then 0 else 1 end 
				,TARGET.ModifiedBy		= @UserId	
				,TARGET.ModifiedDate	= @GetDate
			from ZnodePublishCatalogProductDetail TARGET
			INNER JOIN #ZnodePublishCatalogProductDetail1 SOURCE
			ON (
		        TARGET.SKU = SOURCE.SKU
				AND SOURCE.LocaleId = TARGET.LocaleId --@LocaleId_In
				)

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
					
			----		  
		   insert into ZnodePublishCatalogProductDetail (PublishProductId,PublishCatalogId,PimCategoryHierarchyId,SKU,ProductName,CategoryName, CatalogName,
				  LocaleId ,IsActive,ProductIndex,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			select PublishProductId,PublishCatalogId,PimCategoryHierarchyId,SKU,ProductName,CategoryName, CatalogName,
				  b.Id ,IsActive,ProductIndex,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
			from ZnodePublishCatalogProductDetail a
			cross apply @LocaleId b 
			where not exists(select * from ZnodePublishCatalogProductDetail c where a.PublishProductId = c.PublishProductId and b.Id = c.LocaleId  )
			and a.LocaleId = @DefaultLocaleId

			DELETE ZPCPD from ZnodePublishCatalogProductDetail ZPCPD
			inner join ZnodePublishProduct ZPD on ZPCPD.PublishProductId = ZPD.PublishProductId and ZPCPD.PublishCatalogId = ZPD.PublishCatalogId
			inner join ZnodePublishCatalog ZPC on ZPCPD.PublishCatalogId = ZPC.PublishCatalogId
			where not exists(select * from ZnodePimCategoryProduct ZPCC 
			      inner join ZnodePimCategoryHierarchy ZPCH ON ZPCC.PimCategoryId = ZPCH.PimCategoryId where ZPD.PimProductId = ZPCC.PimProductId and ZPC.PimCatalogId = ZPCH.PimCatalogId and ZPCPD.PimCategoryHierarchyId = ZPCH.PimCategoryHierarchyId)
			and ZPCPD.PimCategoryHierarchyId <> 0

		update ZnodePublishCatalogProductDetail 
		set SKU = b.AttributeValue
		from ZnodePublishCatalogProductDetail a
		inner join ZnodePublishProduct ZPP on a.PublishProductId = ZPP.PublishProductId
		inner join #AttributeValueLocale b on ZPP.PimproductId = b.PimProductId AND a.LocaleId = b.LocaleId
		where b.AttributeCode = 'SKU'

		update ZnodePublishCatalogProductDetail 
		set ProductName = b.AttributeValue
		from ZnodePublishCatalogProductDetail a
		inner join ZnodePublishProduct ZPP on a.PublishProductId = ZPP.PublishProductId
		inner join #AttributeValueLocale b on ZPP.PimproductId = b.PimProductId AND a.LocaleId = b.LocaleId
		where b.AttributeCode = 'ProductName'

		update ZnodePublishCatalogProductDetail 
		set IsActive = b.AttributeValue
		from ZnodePublishCatalogProductDetail a
		inner join ZnodePublishProduct ZPP on a.PublishProductId = ZPP.PublishProductId
		inner join #AttributeValueLocale b on ZPP.PimproductId = b.PimProductId AND a.LocaleId = b.LocaleId
		where b.AttributeCode = 'IsActive'

	   -- FETCH NEXT FROM CR_Locale_id INTO @LocaleId_In
	   --END    
	   
	   --CLOSE CR_Locale_id  
	   --DEALLOCATE CR_Locale_id 

		  select a.PimProductId,  a.PimAttributeId
		  into #PimProductAttributeDefaultValue
		  from ZnodePimAttributeValue a 
		  Inner join ZnodePimProductAttributeDefaultValue b on a.PimAttributeValueId = b.PimAttributeValueId 

		  create index Idx_#PimProductAttributeDefaultValue on #PimProductAttributeDefaultValue (PimProductId,PimAttributeId)

		  INSERT INTO #PimDefaultValueLocale
		  SELECT PimAttributeDefaultXMLId,PimAttributeDefaultValueId,LocaleId ,DefaultValueXML
		  FROM ZnodePimAttributeDefaultXML

		  SELECT  AA.DefaultValueXML , ZPADV.PimAttributeValueId, AA.LocaleId 
		  into #PimAttributeDefaultXML
		  FROM ZnodePimAttributeDefaultXML AA 
		  INNER JOIN #PimDefaultValueLocale GH ON (GH.PimAttributeDefaultXMLId = AA.PimAttributeDefaultXMLId AND AA.LocaleId = GH.LocaleId)
		  INNER JOIN ZnodePimProductAttributeDefaultValue ZPADV ON ( ZPADV.PimAttributeDefaultValueId = AA.PimAttributeDefaultValueId AND AA.LocaleId = ZPADV.LocaleId)

		  ----Getting child facets for merging		  
		  Select distinct ZPPADV.PimAttributeDefaultValueId, ZPAV_Parent.PimAttributeValueId, ZPPADV.LocaleId
		  Into #PimChildProductFacets
		  from ZnodePimAttributeValue ZPAV_Parent
		  inner join ZnodePimProductTypeAssociation ZPPTA ON ZPAV_Parent.PimProductId = ZPPTA.PimParentProductId
		  inner join ZnodePimAttributeValue ZPAV_Child ON ZPPTA.PimProductId = ZPAV_Child.PimProductId AND ZPAV_Parent.PimAttributeId = ZPAV_Child.PimAttributeId
		  inner join ZnodePimProductAttributeDefaultValue ZPPADV ON ZPAV_Child.PimAttributeValueId = ZPPADV.PimAttributeValueId 
		  where exists(select * from ZnodePimFrontendProperties ZPFP where ZPAV_Parent.PimAttributeId = ZPFP.PimAttributeId and ZPFP.IsFacets = 1)
		  and exists(select * from #ZnodePublishCategoryProduct ZPPC where ZPAV_Parent.PimProductId = ZPPC.PimProductId )
		  and not exists(select * from ZnodePimProductAttributeDefaultValue ZPPADV1 where ZPAV_Parent.PimAttributeValueId = ZPPADV1.PimAttributeValueId 
		                 and ZPPADV1.PimAttributeDefaultValueId = ZPPADV.PimAttributeDefaultValueId )

		  ----Merging childs facet attribute Default value XML for parent
		  insert into #PimAttributeDefaultXML (DefaultValueXML, PimAttributeValueId, LocaleId)
		  select ZPADX.DefaultValueXML, ZPPADV.PimAttributeValueId, ZPPADV.LocaleId
		  from #PimChildProductFacets ZPPADV		  
		  inner join ZnodePimAttributeDefaultXML ZPADX ON ( ZPPADV.PimAttributeDefaultValueId = ZPADX.PimAttributeDefaultValueId AND ZPPADV.LocaleId = ZPADX.LocaleId)

		  CREATE INDEX Idx_#PimDefaultValueLocale ON #PimDefaultValueLocale(PimAttributeDefaultXMLId,LocaleId)

		  CREATE INDEX Idx_#PimAttributeDefaultXML ON #PimAttributeDefaultXML(PimAttributeValueId,LocaleId)
		  INCLUDE (DefaultValueXML)

		  ----Getting default attribute value entity
		 INSERT INTO #AttributeValueLocale
		 SELECT PPCP.PimProductId, PPCP.AttributeCode,'' AttributeValue,ZPAX.AttributeXML+'<AttributeValues></AttributeValues>'
			    +CAST(( SELECT  cast(DefaultValueXML as xml) SelectValues  FROM #PimAttributeDefaultXML aa
				 WHERE (aa.PimAttributeValueId = PPCP.PimAttributeValueId and AA.LocaleId = ZPAX.LocaleId)
				 FOR XML PATH('') , TYPE ) AS NVARCHAR(max))  AttributeEntity , ZPAX.LocaleId
		 FROM #ZnodePublishCategoryProduct PPCP 
		 INNER JOIN ZnodePimAttributeXML ZPAX ON (ZPAX.PimAttributeId = PPCP.PimAttributeId)
		 where not exists(select * from #AttributeValueLocale AVL where PPCP.PimProductId = AVL.PimProductId and PPCP.AttributeCode = AVL.AttributeCode and ZPAX.LocaleId = AVL.LocaleId )
		and exists(select * from #PimProductAttributeDefaultValue a  where PPCP.PimProductId = a.PimProductId and ZPAX.PimAttributeId = a.PimAttributeId )
		 and exists(select * from ZnodePimAttributeValue a Inner join ZnodePimProductAttributeDefaultValue b on a.PimAttributeValueId = b.PimAttributeValueId 
		            and PPCP.PimProductId = a.PimProductId and ZPAX.PimAttributeId = a.PimAttributeId )
		and not exists(select * from ZnodePimConfigureProductAttribute UOP where ZPAX.PimAttributeId = UOP.PimAttributeId and PPCP.PimProductId = UOP.PimProductId )

		 ----Getting text attribute value entity
		 INSERT INTO #AttributeValueLocale ( PimProductId, AttributeCode, AttributeValue, AttributeEntity, LocaleId )
		 SELECT PPCP.PimProductId , ZPA.AttributeCode,ZPAVL.AttributeValue ,ZPAX.AttributeXML + '<AttributeValues>'+(SELECT ISNULL(ZPAVL.AttributeValue,'') FOR XML PATH (''))+'</AttributeValues>'  AttributeEntity, ZPAVL.LocaleId
		 FROM ZnodePimAttributeValue PPCP
		 INNER JOIN ZnodePimProductAttributeTextAreaValue ZPAVL ON (PPCP.PimAttributeValueId =ZPAVL.PimAttributeValueId)
		 INNER JOIN ZnodePimAttributeXML ZPAX ON (ZPAX.PimAttributeId = PPCP.PimAttributeId AND ZPAX.LocaleId = ZPAVL.LocaleId)
		 INNER JOIN ZnodePimAttribute ZPA on PPCP.PimAttributeId = ZPA.PimAttributeId
	     where exists(select * from #ZnodePublishCategoryProduct PPCP1 WHERE PPCP1.PimProductId = PPCP.PimProductId) --(PPCP1.PimAttributeValueId =ZPAVL.PimAttributeValueId) and (ZPAX.PimAttributeId = PPCP1.PimAttributeId))
		 and not exists(select * from #AttributeValueLocale AVL where PPCP.PimProductId = AVL.PimProductId and ZPA.AttributeCode = AVL.AttributeCode and ZPAVL.LocaleId = AVL.LocaleId )
		 group by PPCP.PimProductId , ZPA.AttributeCode,ZPAVL.AttributeValue ,ZPAX.AttributeXML,ZPAVL.AttributeValue, ZPAVL.LocaleId

		 ----Getting custome field value entity
		 INSERT INTO #AttributeValueLocale ( PimProductId, AttributeCode, AttributeValue, AttributeEntity, LocaleId )
	     SELECT ZPCFX.PimProductId , ZPCFX.CustomCode, '' AttributeValue ,ZPCFX.CustomeFiledXML  AttributeEntity, ZPCFX.LocaleId
		 FROM ZnodePimCustomeFieldXML ZPCFX 
		 where exists(select * from #ZnodePublishCategoryProduct PPCP where (PPCP.PimProductId = ZPCFX.PimProductId ))
		 and not exists(select * from #AttributeValueLocale AVL where ZPCFX.PimProductId = AVL.PimProductId and ZPCFX.CustomCode = AVL.AttributeCode and ZPCFX.LocaleId = AVL.LocaleId )
		 group by ZPCFX.PimProductId , ZPCFX.CustomCode, ZPCFX.CustomeFiledXML , ZPCFX.LocaleId

		  ----Getting image attribute value entity
		 INSERT INTO #AttributeValueLocale ( PimProductId, AttributeCode, AttributeValue, AttributeEntity, LocaleId )
		 SELECT PPCP.PimProductId, ZPA.AttributeCode,'' AttributeValue,ZPAX.AttributeXML+'<AttributeValues>'
			    +stuff( (SELECT ','+ZPPAM.MediaPath FROM ZnodePimProductAttributeMedia ZPPAM WHERE (ZPPAM.PimAttributeValueId = PPCP.PimAttributeValueId)
				 FOR XML PATH('')), 1, 1, '') +'</AttributeValues>' , ZPAX.LocaleId
		 FROM ZnodePimAttributeValue PPCP 
		 INNER JOIN ZnodePimAttributeXML ZPAX ON (ZPAX.PimAttributeId = PPCP.PimAttributeId)
		 INNER JOIN ZnodePimAttribute ZPA ON ZPA.PimAttributeId = PPCP.PimAttributeId
		 where not exists(select * from #AttributeValueLocale AVL where PPCP.PimProductId = AVL.PimProductId and ZPA.AttributeCode = AVL.AttributeCode and ZPAX.LocaleId = AVL.LocaleId )
		 and exists(select * from ZnodePimProductAttributeMedia b where PPCP.PimAttributeValueId = b.PimAttributeValueId )
		 and exists(select * from #ZnodePublishCategoryProduct PPCP1 where PPCP.PimProductId = PPCP1.PimProductId )
		 and not exists(select * from ZnodePimConfigureProductAttribute UOP where ZPAX.PimAttributeId = UOP.PimAttributeId and PPCP.PimProductId = UOP.PimProductId )

		 -------------configurable attribute 		 
		INSERT INTO #AttributeValueLocale ( PimProductId, AttributeCode, AttributeValue, AttributeEntity, LocaleId )
		SELECT DISTINCT  UOP.PimProductId,c.AttributeCode,'' AttributeValue ,--'<Attributes><AttributeEntity>'+
		c.AttributeXML+'<AttributeValues></AttributeValues>'+'<SelectValues>'+
					   STUFF((
							SELECT DISTINCT '  '+REPLACE(AA.DefaultValueXML,'</SelectValuesEntity>','<VariantDisplayOrder>'+CAST(ISNULL(ZPA.DisplayOrder,0) AS VARCHAR(200))+'</VariantDisplayOrder>
							<VariantSKU>'+ISNULL(ZPAVL_SKU.AttributeValue,'')+'</VariantSKU>
							<VariantImagePath>'+ISNULL((SELECT ''+ZM.Path FOR XML Path ('')),'')+'</VariantImagePath></SelectValuesEntity>')   
						 FROM ZnodePimAttributeDefaultXML AA 
						 --INNER JOIN #PimDefaultValueLocale GH ON (GH.PimAttributeDefaultXMLId = AA.PimAttributeDefaultXMLId)
						 INNER JOIN ZnodePimProductAttributeDefaultValue ZPADV ON ( ZPADV.PimAttributeDefaultValueId = AA.PimAttributeDefaultValueId )
						 INNER JOIN ZnodePimAttributeValue ZPAV1 ON (ZPAV1.PimAttributeValueId= ZPADV.PimAttributeValueId )
						 -- check/join for active variants 
						 INNER JOIN ZnodePimAttributeValue ZPAV ON (ZPAV.PimProductId =ZPAV1.PimProductId)
						 INNER JOIN ZnodePimAttributeValueLocale ZPAVL ON (ZPAV.PimAttributevalueid = ZPAVL.PimAttributeValueId AND ZPAVL.AttributeValue = 'True')
						 INNER JOIN ZnodePimProductTypeAssociation YUP ON (YUP.PimProductId = ZPAV1.PimProductId)
						 -- SKU
						 INNER JOIN ZnodePimAttributeValue ZPAV_SKU ON(YUP.PimProductId = ZPAV_SKU.PimProductId)
						 INNER JOIN ZnodePimAttributeValueLocale ZPAVL_SKU ON (ZPAVL_SKU.PimAttributeValueId = ZPAV_SKU.PimAttributeValueId)
						 LEFT  JOIN ZnodePimAttributeValue ZPAV12 ON (ZPAV12.PimProductId= YUP.PimProductId  AND ZPAV12.PimAttributeId = @PimMediaAttributeId ) 
						 LEFT JOIN ZnodePimProductAttributeMedia ZPAVM ON (ZPAVM.PimAttributeValueId= ZPAV12.PimAttributeValueId ) 
						 LEFT JOIN ZnodeMedia ZM ON (ZM.MediaId = ZPAVM.MediaId)
						 LEFT JOIN ZnodePimAttribute ZPA ON (ZPA.PimattributeId = ZPAV1.PimAttributeId)
						 WHERE (YUP.PimParentProductId  = UOP.PimProductId AND ZPAV1.pimAttributeId = UOP.PimAttributeId )
						 -- Active Variants
						 AND ZPAV.PimAttributeId = (SELECT TOP 1 PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'IsActive')
						 -- VariantSKU
						 AND ZPAV_SKU.PimAttributeId = (SELECT PimAttributeId FROM ZnodePimAttribute WHERE AttributeCode = 'SKU')
		FOR XML PATH, TYPE).value(N'.[1]', N'Nvarchar(max)'), 1, 1, '')+'</SelectValues> ' AttributeValue , --</AttributeEntity></Attributes>' 
		c.LocaleId
		FROM ZnodePimConfigureProductAttribute UOP 
		INNER JOIN ZnodePimAttributeXML c   ON (c.PimAttributeId = UOP.PimAttributeId )
		WHERE  exists(select * from #ZnodePublishCategoryProduct PPCP1 where UOP.PimProductId = PPCP1.PimProductId )
		-------------configurable attribute 
			  
		 CREATE INDEX IDX_#AttributeValueLocale ON #AttributeValueLocale(PimProductId,AttributeCode,LocaleId)
		 	
		delete ZPPAX from ZnodePublishProductAttributeXML ZPPAX
		where exists (select * from #AttributeValueLocale AVL where ZPPAX.PimProductId = AVL.PimProductId and AVL.LocaleId = ZPPAX.LocaleId )
		and not exists(select * from #AttributeValueLocale AVL where ZPPAX.PimProductId = AVL.PimProductId and AVL.LocaleId = ZPPAX.LocaleId AND ZPPAX.AttributeCode = AVL.AttributeCode )

		DECLARE @MaxCount INT, @MinRow INT, @MaxRow INT, @Rows numeric(10,2);
		SELECT @MaxCount = COUNT(*) FROM #AttributeValueLocale;

		SELECT @Rows = 200000
        
		SELECT @MaxCount = CEILING(@MaxCount / @Rows);

		IF OBJECT_ID('tempdb..#Temp_ImportLoop') IS NOT NULL
            DROP TABLE #Temp_ImportLoop;
        
		---- To get the min and max rows for import in loop
		;WITH cte AS 
		(
			SELECT RowId = 1, 
				   MinRow = 1, 
                   MaxRow = cast(@Rows as int)
            UNION ALL
            SELECT RowId + 1, 
                   MinRow + cast(@Rows as int), 
                   MaxRow + cast(@Rows as int)
            FROM cte
            WHERE RowId + 1 <= @MaxCount
		)
        SELECT RowId, MinRow, MaxRow
        INTO #Temp_ImportLoop
        FROM cte
		option (maxrecursion 0);

		DECLARE cur_BulkData CURSOR LOCAL FAST_FORWARD
        FOR SELECT MinRow, MaxRow FROM #Temp_ImportLoop
		WHERE EXISTS(SELECT * FROM #AttributeValueLocale);

        OPEN cur_BulkData;
        FETCH NEXT FROM cur_BulkData INTO  @MinRow, @MaxRow;

        WHILE @@FETCH_STATUS = 0
        BEGIN
		      update ZnodePublishProductAttributeXML set IsUpdateLocaleWise = 0 where isnull(IsUpdateLocaleWise,0) = 1
			  ----Update Product Attribute XML
			 UPDATE ZPPAX SET ZPPAX.Attributes = CAST(REPLACE(REPLACE(REPLACE(AVL.AttributeEntity,'&','&amp;'),'&amp;amp;','&amp;'),'&amp;amp;amp;','&amp;') as XML), ZPPAX.ModifiedBy = @UserId, ZPPAX.ModifiedDate = @GetDate 
			        , ZPPAX.IsUpdateLocaleWise = 0
			 FROM ZnodePublishProductAttributeXML ZPPAX 
			 INNER JOIN #AttributeValueLocale AVL ON ZPPAX.PimProductId = AVL.PimProductId and AVL.LocaleId = ZPPAX.LocaleId AND ZPPAX.AttributeCode = AVL.AttributeCode 
			 where  AVL.Id BETWEEN @MinRow AND @MaxRow and AVL.AttributeEntity is not null
		 
			 ----Insert Product Attribute XML
			 INSERT INTO ZnodePublishProductAttributeXML(PimProductId,LocaleId,AttributeCode,Attributes,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			 SELECT AVL.PimProductId, AVL.LocaleId, AVL.AttributeCode, cast(replace(replace(replace(AVL.AttributeEntity,'&','&amp;'),'&amp;amp;','&amp;'),'&amp;amp;amp;','&amp;') as XML), @UserId CreatedBy, @GetDate CreatedDate, @UserId ModifiedBy, @GetDate ModifiedDate
			 FROM #AttributeValueLocale AVL
			 WHERE NOT EXISTS(SELECT * FROM ZnodePublishProductAttributeXML ZPPAX WHERE AVL.PimProductId = ZPPAX.PimProductId AND  AVL.LocaleId = ZPPAX.LocaleId AND AVL.AttributeCode = ZPPAX.AttributeCode )
			 and  AVL.Id BETWEEN @MinRow AND @MaxRow and AVL.AttributeEntity is not null
			 GROUP BY AVL.PimProductId, AVL.AttributeEntity, AVL.LocaleId, AVL.AttributeCode

			 FETCH NEXT FROM cur_BulkData INTO  @MinRow, @MaxRow;
        END;
		CLOSE cur_BulkData;
		DEALLOCATE cur_BulkData;
		
		delete ZPPAX
		from ZnodePublishProductAttributeXML ZPPAX
		where LocaleId <> @DefaultLocaleId
		and exists( select * from ZnodePublishProductAttributeXML ZPPAX1 where ZPPAX.AttributeCode = ZPPAX1.AttributeCode and ZPPAX.PimProductId = ZPPAX1.PimProductId )
		and not exists(select * from #ProductLocaleWise AVL where AVL.PimProductId = ZPPAX.PimProductId AND  AVL.LocaleId = ZPPAX.LocaleId )
		
		
		delete  ZPPAX
		from ZnodePublishProductAttributeXML ZPPAX
		WHERE not exists(select * from #ProductLocaleWise ZLW where ZPPAX.PimProductId = ZLW.PimProductId 
			                and ZPPAX.LocaleId = ZLW.LocaleId )

		select PimProductId,Attributes,AttributeCode
		into #ZnodePublishProductAttributeXML
		from ZnodePublishProductAttributeXML 
		where LocaleId = @DefaultLocaleId

		insert into ZnodePublishProductAttributeXML (PimProductId,Attributes,LocaleId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,AttributeCode)
		select PimProductId,Attributes,b.id,@UserId,@GetDate,@UserId,@GetDate,AttributeCode
		from #ZnodePublishProductAttributeXML a
		cross apply @LocaleId b 
		where not exists(select * from ZnodePublishProductAttributeXML c where a.PimProductId = c.PimProductId and b.Id = c.LocaleId AND a.AttributeCode = c.AttributeCode )
		and b.Id <> @DefaultLocaleId
 END TRY 
 BEGIN CATCH 
  SELECT ERROR_MESSAGE()
 END CATCH 
END
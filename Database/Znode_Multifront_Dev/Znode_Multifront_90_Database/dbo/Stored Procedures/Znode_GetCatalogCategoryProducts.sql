CREATE PROCEDURE [dbo].[Znode_GetCatalogCategoryProducts]
( 
  @WhereClause      XML,
  @Rows             INT           = 100,
  @PageNo           INT           = 1,
  @Order_BY         VARCHAR(1000) = 'DisplayOrder asc',
  @RowsCount        INT OUT,
  @LocaleId         INT           = 1,
  @PimCategoryId    INT           = 0,
  @PimCatalogId     INT           = 0,
  @IsAssociated     BIT           = 0,
  @AttributeCode   VARCHAR(max) = '',
  @PimCategoryHierarchyId INT =0,
  @PortalId INT=0
  )
AS
   
/*
	   Summary:  Get product List  Catalog / category / respective product list   		   
	   Unit Testing   
	   begin tran
	   declare @p7 int = 0  
	   EXEC Znode_GetCatalogCategoryProducts_r @WhereClause=N'',@Rows=10,@PageNo=1,@Order_By=N'',
	   @RowsCount=@p7 output,@PimCategoryId=11,@PimCatalogId = 1 ,@LocaleId=1 ,@PimCategoryHierarchyId  =2
	   rollback tran
	  
    */

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
			 SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
             DECLARE @DefaultAttributeFamily INT= dbo.Fn_GetDefaultPimProductFamilyId(), @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId(), @OrderId INT= 0;
             DECLARE @SQL VARCHAR(MAX), 
					 @PimProductId TransferId,--VARCHAR(MAX)= '', 
					 @PimAttributeId VARCHAR(MAX),
					 @OutPimProductIds VARCHAR(max);
             DECLARE @TransferPimProductId TransferId 

			 DECLARE @tbl_ProductPricingSku TABLE (sku nvarchar(200),RetailPrice numeric(28,6),SalesPrice numeric(28,6),TierPrice numeric(28,6),
			 TierQuantity numeric(28,6),CurrencyCode varchar(200),CurrencySuffix varchar(2000),CultureCode varchar(2000), ExternalId NVARCHAR(2000)
			 ,Custom1 NVARCHAR(MAX), Custom2 NVARCHAR(MAX), Custom3 NVARCHAR(MAX))				

			 CREATE TABLE #TBL_PimMediaAttributeId  (PimAttributeId INT ,AttributeCode VARCHAR(600))
			 INSERT INTO #TBL_PimMediaAttributeId (PimAttributeId,AttributeCode)
			 SELECT PimAttributeId,AttributeCode FROM Dbo.Fn_GetProductMediaAttributeId ()
			  
		
      --       DECLARE @TBL_AttributeDefaultValue TABLE
      --       (
				  --PimAttributeId            INT,
				  --AttributeDefaultValueCode VARCHAR(100),
				  --IsEditable                BIT,
				  --AttributeDefaultValue     NVARCHAR(MAX),
				  --DisplayOrder INT 
      --       );
             create TABLE #TBL_AttributeDetails 
             (
				  PimProductId   INT,
				  AttributeValue NVARCHAR(MAX),
				  AttributeCode  VARCHAR(600),
				  PimAttributeId INT
				  
             );
             DECLARE @FamilyDetails TABLE
             (
				  PimProductId         INT,
				  PimAttributeFamilyId INT,
				  FamilyName           NVARCHAR(3000)
             );
             DECLARE @TBL_AttributeValue TABLE
             (
				  PimCategoryAttributeValueId INT,
				  PimCategoryId               INT,
				  CategoryValue               NVARCHAR(MAX),
				  AttributeCode               VARCHAR(300),
				  PimAttributeId              INT
             );
             IF @Order_By = ''
                 BEGIN
                     SET @Order_By = 'DisplayOrder asc'
                 END;
         			 
             IF @PimCatalogId = 0
                 BEGIN
					INSERT INTO @TransferPimProductId 
                    SELECT PimProductId 
                    FROM ZnodePimCategoryProduct AS ZCP
                    WHERE ZCP.PimCategoryId = @PimCategoryId
					AND PimProductId IS NOT NULL 
                                                   
                 END;
             ELSE
                 BEGIN
				 
                     IF @IsAssociated = 0 
                     BEGIN
						INSERT INTO @TransferPimProductId 
						SELECT DISTINCT PimProductId 
						FROM ZnodePimCategoryProduct AS ZCP
						inner join ZnodePimCategoryHierarchy ZPCH ON ZCP.PimCategoryId = ZPCH.PimCategoryId
						WHERE ZPCH.PimCatalogId = @PimCatalogId
						AND ZPCH.PimCategoryHierarchyId = @PimCategoryHierarchyId
						AND ZCP.PimProductId IS NOT NULL                                     
                     END;
                     ELSE
                         BEGIN
                             IF @IsAssociated = 1 
                             BEGIN
									INSERT INTO @TransferPimProductId 
									SELECT PimProductId
									FROM ZnodePimProduct ZPP
									where not exists(select * from  ZnodePimCategoryProduct AS ZCP
										inner join ZnodePimCategoryHierarchy ZPCH ON ZCP.PimCategoryId = ZPCH.PimCategoryId
										WHERE ZPCH.PimCatalogId = @PimCatalogId
										AND ZPCH.PimCategoryHierarchyId = @PimCategoryHierarchyId
										AND PimProductId IS NOT NULL   and ZPP.PimProductId = ZCP.PimProductId)                                         
									
									SET @IsAssociated = 0;
                             END;
                             ELSE
                             BEGIN
								INSERT INTO @TransferPimProductId 
								SELECT DISTINCT PimProductId 
								FROM ZnodePimCategoryProduct AS ZCP
								inner join ZnodePimCategoryHierarchy ZPCH ON ZCP.PimCategoryId = ZPCH.PimCategoryId
								WHERE ZPCH.PimCatalogId = @PimCatalogId
								AND ZPCH.PimCategoryHierarchyId = @PimCategoryHierarchyId 
								AND PimProductId IS NOT NULL		                         
                             END;
                         END;
                 END;
				
				 IF NOT EXISTS (SELECT TOP 1 1 FROM @TransferPimProductId)
				 BEGIN 
                  INSERT INTO @TransferPimProductId
				  SELECT '0'

				 END 
				 
				 
             DECLARE @ProductIdTable TABLE
             ([PimProductId] INT,
              [CountId]      INT,
              PimCategoryId  INT,
              RowId          INT IDENTITY(1,1)
             );
            DECLARE  @ProductListIdRTR TransferId
	 DECLARE @TAb Transferid 
	 DECLARE @tBL_mainList TABLE (Id INT,RowId INT)
	 --	IF @PimProductId <> ''  OR   @IsCallForAttribute=1
		--BEGIN 
	 SET @IsAssociated = CASE WHEN @IsAssociated = 0 THEN 1  
					 WHEN @IsAssociated = 1 THEN 0 END 
		--END 

	 INSERT INTO @ProductListIdRTR
	 EXEC Znode_GetProductList  @IsAssociated,@TransferPimProductId
	 
	

	 IF CAST(@WhereClause AS NVARCHAR(max))<> N''
	 BEGIN 
	 
	  SET @SQL = 'SELECT distinct PimProductId FROM ##Temp_PimProductId'+CAST(@@SPID AS VARCHAR(500))

	 EXEC Znode_GetFilterPimProductId @WhereClause,@ProductListIdRTR,@localeId
 
      INSERT INTO @TAB 
	  EXEC (@SQL)
	-- SELECT * FROM @TAB
	 END 
	
	
	 IF EXISTS (SELECT Top 1 1 FROM @TAb ) OR CAST(@WhereClause AS NVARCHAR(max)) <> N''
	 BEGIN 

		 SET @AttributeCode = REPLACE(dbo.FN_TRIM(REPLACE(REPLACE(@order_by,' DESC',''),' ASC','')),'DisplayOrder','ProductName')
		 --SET @order_by = REPLACE(@order_by,'DisplayOrder','ProductName')
		 INSERT INTO @TBL_MainList(id,RowId)
		 EXEC Znode_GetOrderByPagingProduct @order_by,@rows,@PageNo, @TAb ,@AttributeCode,@localeId,
		 @PimCategoryHierarchyId=@PimCategoryHierarchyId ,@PortalId=@PortalId
	 
		 END 
	 ELSE 
	 BEGIN
	     	 
	 SET @AttributeCode = REPLACE(dbo.FN_TRIM(REPLACE(REPLACE(@order_by,' DESC',''),' ASC','')),'DisplayOrder','ProductName')
	 --SET @order_by = REPLACE(@order_by,'DisplayOrder','ProductName')
	 INSERT INTO @TBL_MainList(id,RowId)
	 EXEC Znode_GetOrderByPagingProduct @order_by,@rows,@PageNo, @ProductListIdRTR ,@AttributeCode,@localeId,
	 @PimCategoryHierarchyId=@PimCategoryHierarchyId ,@PortalId=@PortalId 
	 
	 END 

	
	
			 INSERT INTO @ProductIdTable
             (PimProductId) 
			 SELECT id 
			 FROM @TBL_MainList 
            
			if @PimCategoryHierarchyId = 0
			begin
			 UPDATE @ProductIdTable
               SET
                   PimCategoryId = @PimCategoryId;
			end
			else 
			begin
				UPDATE @ProductIdTable
               SET
                   PimCategoryId = ZPCP.PimCategoryId
				from ZnodePimCategoryHierarchy ZPCH 
				inner join ZnodePimCategoryProduct ZPCP ON ZPCH.PimCategoryId = ZPCP.PimCategoryId
				inner join @ProductIdTable PT ON ZPCP.PimProductId = PT.PimProductId
				where ZPCH.PimCategoryHierarchyId = @PimCategoryHierarchyId
			end
             --SET @PimProductId = SUBSTRING(
             --                             (
             --                                 SELECT ','+CAST(PimProductId AS VARCHAR(100))
             --                                 FROM @ProductIdTable
             --                                 FOR XML PATH('')
             --                             ), 2, 4000);

			 INSERT INTO @PimProductId  ( Id )
			 SELECT PimProductId FROM @ProductIdTable

             SET @PimAttributeId = SUBSTRING((SELECT ','+CAST(PimAttributeId AS VARCHAR(50)) FROM [dbo].[Fn_GetGridPimAttributes]() FOR XML PATH('')), 2, 4000);
             
			 DECLARE @PimAttributeIds TransferId  
			 INSERT INTO @PimAttributeIds
			 SELECT PimAttributeId  
			 FROM [dbo].[Fn_GetProductGridAttributes]()
					  

			 --INSERT INTO @TBL_AttributeDefaultValue (PimAttributeId,AttributeDefaultValueCode,IsEditable,AttributeDefaultValue,DisplayOrder)   
			 --EXEC Znode_GetAttributeDefaultValueLocale @PimAttributeId,@LocaleId;

			 INSERT INTO #TBL_AttributeDetails (PimProductId,AttributeValue,AttributeCode,PimAttributeId)
             EXEC Znode_GetProductsAttributeValue @PimProductId,@PimAttributeId,@localeId;
	
             SET @PimAttributeId = [dbo].[Fn_GetCategoryNameAttributeId]();
			 
             INSERT INTO @TBL_AttributeValue (PimCategoryAttributeValueId,PimCategoryId,CategoryValue,AttributeCode,PimAttributeId)
             EXEC [dbo].[Znode_GetCategoryAttributeValue] @PimCategoryId,@PimAttributeId,@LocaleId;
         

		 
				SELECT TBAI.PimProductId , FNMA.PimAttributeId ,ISNULL(CASE WHEN ZMC.CDNURL = '' THEN NULL ELSE ZMC.CDNURL END,ZMC.URL)+ZMSM.ThumbnailFolderName+'/'+ zm.PATH as AttributeValue
				INTO #ProductMedia
				FROM ZnodeMedia AS ZM
				INNER JOIN ZnodeMediaConfiguration ZMC  ON (ZM.MediaConfigurationId = ZMC.MediaConfigurationId)
				INNER JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)
				INNER JOIN #TBL_AttributeDetails AS TBAI ON (TBAI.AttributeValue  = CAST(ZM.MediaId AS VARCHAR(50)) )
				INNER JOIN  #TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = TBAI.PimATtributeId)
				
				SELECT PimProductId, PimAttributeId,
					STUFF((SELECT ','+AttributeValue FROM #ProductMedia PM1 
					WHERE PM1.PimProductId = PM.PimProductId AND PM.PimAttributeId = PM1.PimAttributeId FOR XML PATH('')),1,1,'') AS AttributeValue 
				into #Cte_ProductMedia
				FROM #ProductMedia PM
			  
		      UPDATE TBAV SET AttributeValue = CTPM.AttributeVALue
			  FROM #TBL_AttributeDetails TBAV 
			  INNER JOIN #Cte_ProductMedia CTPM ON CTPM.PimProductId = TBAV.PimProductId  AND CTPM.PimAttributeId = TBAV.PimAttributeId 
			  AND CTPM.PimAttributeId = TBAV.PimAttributeId;
			    
             INSERT INTO @FamilyDetails (PimAttributeFamilyId,PimProductId)
             EXEC [dbo].[Znode_GetPimProductAttributeFamilyId] @PimProductId,1;

             UPDATE a
             SET
                   FamilyName = b.AttributeFamilyName
             FROM @FamilyDetails a
             INNER JOIN ZnodePimFamilyLocale b ON(a.PimAttributeFamilyId = b.PimAttributeFamilyId
                                                  AND LocaleId = @LocaleId);
             UPDATE a
             SET
                   FamilyName = b.AttributeFamilyName
             FROM @FamilyDetails a
             INNER JOIN ZnodePimFamilyLocale b ON(a.PimAttributeFamilyId = b.PimAttributeFamilyId
                                                  AND LocaleId = @DefaultLocaleId)
             WHERE a.FamilyName IS NULL
                   OR a.FamilyName = '';

			 SELECT zpp.PimProductid AS ProductId,zpp.PimProductId,@PimCatalogId AS PimCatalogId,zpp.PimCategoryId,[ProductName],
			 ProductType,ISNULL(zf.FamilyName, '') AS AttributeFamily,[SKU],[Price],[Quantity],
                    CASE
                        WHEN Piv.[IsActive] IS NULL
                        THEN CAST(0 AS BIT)
                        ELSE CAST(Piv.[IsActive] AS BIT)
                    END AS [IsActive],
                    piv.[ProductImage] ImagePath,
                    [Assortment],
                    TBAV.CategoryValue AS [CategoryName],
                    @LocaleId AS LocaleId,
                    ZPCP.[DisplayOrder],
                    zpp.RowId,
					ZCC.PimCategoryHierarchyId
			 INTO #temp_ProductDetails 
             FROM @ProductIdTable AS zpp
			 INNER JOIN @TBL_MainList TMM ON (TMM.Id = zpp.PimProductId)
                  LEFT JOIN @FamilyDetails AS zf ON(zf.PimProductId = zpp.PimProductId)
                  INNER JOIN
             (
                 SELECT PimProductId,
                        AttributeValue,
                        AttributeCode
                 FROM #TBL_AttributeDetails
             ) TB PIVOT(MAX(AttributeValue) FOR AttributeCode IN([ProductName],
                                                                 [SKU],
                                                                 [Price],
                                                                 [Quantity],
                                                                 [IsActive],
                                                                 [ProductType],
                                                                 [ProductImage],
                                                                 [Assortment],
                                                                 [DisplayOrder])) AS Piv ON(Piv.PimProductId = zpp.PimProductid)
                  LEFT JOIN @TBL_AttributeValue AS TBAV ON(TBAV.PimCategoryId = ZPP.PimCategoryId)
                  LEFT JOIN ZnodePimCategoryProduct AS ZPCP ON(ZPCP.PimProductId = Zpp.PimProductId AND ZPCP.PimCategoryId = Zpp.PimCategoryId)
                  LEFT JOIN ZnodePimCategoryHierarchy AS ZCC ON(ZCC.PimCategoryHierarchyId = @PimCategoryHierarchyId AND ZCC.PimCatalogId = @PimCatalogId)
                  
                  
            ORDER BY zpp.RowId

			DECLARE @SKUS VARCHAR(max) 
			,@userId INT = 0,@Date DATETIME  = dbo.FN_getDate() 

			SELECT @SKUS = COALESCE(@SKUS+',' ,'') + SKU
			FROM #temp_ProductDetails
			 				
			INSERT INTO @tbl_ProductPricingSku		
			EXEC Znode_GetPublishProductPricingBySku 	@SKU=@SKUS, @PortalId= @PortalId,@Userid= @userid ,@currentUtcDate=	@Date
			
			SELECT DISTINCT ProductId, PimProductId	,PimCatalogId,	isnull(PimCategoryId,0) as PimCategoryId,	ProductName	,ProductType,	
			AttributeFamily,	a.SKU	,dbo.Fn_GetPortalCurrencySymbol(@portalId)+CAST(Dbo.Fn_GetDefaultPriceRoundOff(RetailPrice) AS NVARCHAR(max)) Price,	Quantity,	
			IsActive,	ImagePath,	Assortment,	CategoryName,	LocaleId,	DisplayOrder	,RowId,	PimCategoryHierarchyId	
			FROM #temp_ProductDetails a 
			LEFT JOIN @tbl_ProductPricingSku b ON (dbo.FN_TRIM(b.SKU) = a.SKU )
			ORDER BY RowId
					  
     IF EXISTS (SELECT Top 1 1 FROM @TAb )
	 BEGIN 

		  SELECT @RowsCount = (SELECT COUNT(1) FROM @TAb) 
	 END 
	 ELSE 
	 BEGIN
	 		  SELECT @RowsCount =(SELECT COUNT(1) FROM @ProductListIdRTR)   
	 END 
	

         END TRY
         BEGIN CATCH
		    SELECT ERROR_message()
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), 
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCatalogCategoryProducts @WhereClause = '''+ISNULL(CAST(@WhereClause AS VARCHAR(MAX)),'''''')+''',@Rows='+ISNULL(CAST(@Rows AS
			VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')+',
			@LocaleId = '+ISNULL(CAST(@LocaleId AS VARCHAR(50)),'''')+',@PimCategoryId='+ISNULL(CAST(@PimCategoryId AS VARCHAR(50)),'''')+',@PimCatalogId='+ISNULL(CAST(@PimCatalogId AS VARCHAR(50)),'''')+',@IsAssociated='+ISNULL(CAST(@IsAssociated AS VARCHAR(50)),'''')+',
			@AttributeCode='''+ISNULL(CAST(@AttributeCode AS VARCHAR(50)),'''''')+''',@PimCategoryHierarchyId='+ISNULL(CAST(@PimCategoryHierarchyId AS VARCHAR(10)),'''');
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCatalogCategoryProducts',
				@ErrorInProcedure = 'Znode_GetCatalogCategoryProducts',
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
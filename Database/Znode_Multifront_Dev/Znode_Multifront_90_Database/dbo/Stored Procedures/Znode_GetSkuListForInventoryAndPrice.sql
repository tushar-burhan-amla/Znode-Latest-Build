CREATE  PROCEDURE [dbo].[Znode_GetSkuListForInventoryAndPrice](
       @WhereClause VARCHAR(MAX) ,
       @Rows        INT           = 100 ,
       @PageNo      INT           = 1 ,
       @Order_BY    VARCHAR(1000) = '' ,
       @RowsCount   INT = 0  OUT ,
       @LocaleId    INT           = 1 ,
       @PriceListId INT           = 0)
AS 
   /* 
    Summary : this procedure is used to Get the inventory list by sku 
    Unit Testing 
     EXEC Znode_GetSkuListForInventoryAndPrice  '',@Order_BY = '',@RowsCount= 1 ,@Rows = 100,@PageNo= 1,@PriceListId = 26,@LocaleId =1 
     SELECT * FROM ZnodePublishProduct WHERE PimProductid  = 4
   */
     BEGIN
         BEGIN TRY
		 DECLARE @SQLqry NVARCHAR(MAX)=''
		     DECLARE @PimProductIds TransferId, --NVARCHAR(max)= '', 
					@OutPimProductIds NVARCHAR(max)= '',
					@PimAttributeId NVARCHAR(max)=''
			 DECLARE @pimSkuAttributeId VARCHAR(50) = [dbo].[Fn_GetProductSKUAttributeId] ()
			 DECLARE @PimProductNameAttributeId VARCHAR(50) = [dbo].[Fn_GetProductNameAttributeId]()
			 DECLARE @PimProductTypeAttributeId VARCHAR(50) = [dbo].[Fn_GetProductTypeAttributeId]()
			 DECLARE @PimIsDownlodableAttributeId VARCHAR(50) = [dbo].[Fn_GetIsDownloadableAttributeId]()
			 DECLARE @PimProductImageAttributeId VARCHAR(50) = [dbo].[Fn_GetProductImageAttributeId]()

			 DECLARE @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId()
			 DECLARE @TransferPimProductId TransferId 
			 DECLARE @IsProductNotIn BIT = 1 
			 DECLARE @IMamgePAth NVARCHAR(max) = [dbo].[Fn_GetServerThumbnailMediaPath]()

			 DECLARE @ProductIdTable TABLE
             (
				PimProductId INT,
				CountId      INT,
				RowId        INT IDENTITY(1,1)
             );

			 --DECLARE @TBL_AttributeDetails AS TABLE
    --         (
				--PimProductId   INT,
				--AttributeValue NVARCHAR(MAX),
				--AttributeCode  VARCHAR(600),
				--PimAttributeId INT
    --         );
			 CREATE TABLE #TBL_AttributeDetails
			 (
				PimProductId   INT,
				AttributeValue NVARCHAR(MAX),
				AttributeCode  VARCHAR(600),
				PimAttributeId INT
             );
             IF @PriceListId > 0
             BEGIN
				INSERT INTO @TransferPimProductId 
				SELECT Distinct PimProductId 
				FROM  [dbo].[View_PimProductAttributeValueLocale] VIMP
				INNER JOIN ZnodePrice  ZP ON (Zp.SKU = VIMP.AttributeValue )
				WHERE VIMP.PimAttributeid = @pimSkuAttributeId
				AND VIMP.LocaleId = @LocaleId
				AND ZP.PriceListId = @PriceListId
					
               SET @IsProductNotIn = 1 --CASE WHEN @PimProductIds IS NULL OR @PimProductIds = '' then 1 else 0 end 

			   IF NOT EXISTS (SELECT TOP 1 1 FROM @TransferPimProductId )
			   BEGIN 
					INSERT INTO @TransferPimProductId
					SELECT '-1'
			   END 
			 --  set @PimProductIds = CASE WHEN @PimProductIds IS NULL OR @PimProductIds = '' then  else @PimProductIds end 
		     END;
			-- SELECT * FROM ZnodePrice WHERE PriceListId = 21
			DECLARE @AttributeCode NVARCHAR(max)= '',@SQL NVARCHAR(max)=''
 DECLARE  @ProductListIdRTR TransferId
	 DECLARE @TAb Transferid 
	 DECLARE @tBL_mainList TABLE (Id INT,RowId INT)
	 --	IF @PimProductId <> ''  OR   @IsCallForAttribute=1
		--BEGIN 
	 --SET @IsProductNotIn = CASE WHEN @IsProductNotIn = 0 THEN 1  
		--			 WHEN @IsProductNotIn = 1 THEN 0 END 
		--END 
	 INSERT INTO @ProductListIdRTR
	 EXEC Znode_GetProductList  0,@TransferPimProductId
	 
	 IF CAST(@WhereClause AS NVARCHAR(max))<> N''
	 BEGIN 
	 
	  SET @SQL = 'SELECT DISTINCT PimProductId FROM ##Temp_PimProductId'+CAST(@@SPID AS VARCHAR(500))

	  EXEC Znode_GetFilterPimProductId @WhereClause,@ProductListIdRTR,@localeId
	  
      INSERT INTO @TAB 
	  EXEC (@SQL)
	 
	 END 

	 IF EXISTS (SELECT Top 1 1 FROM @TAb ) OR CAST(@WhereClause AS NVARCHAR(max)) <> N''
	 BEGIN 
	 
	 SET @AttributeCode = REPLACE(dbo.FN_TRIM(REPLACE(REPLACE(@order_by,' DESC',''),' ASC','')),'DisplayOrder','ProductName')
	 SET @order_by = REPLACE(@order_by,'DisplayOrder','ProductName')
	 INSERT INTO @TBL_MainList(id,RowId)
	 EXEC Znode_GetOrderByPagingProduct @order_by,@rows,@PageNo, @TAb ,@AttributeCode,@localeId
	 
	 END 
	 ELSE 
	 BEGIN
	  
	 SET @AttributeCode = REPLACE(dbo.FN_TRIM(REPLACE(REPLACE(@order_by,' DESC',''),' ASC','')),'DisplayOrder','ProductName')
	 SET @order_by = REPLACE(@order_by,'DisplayOrder','ProductName')
	 INSERT INTO @TBL_MainList(id,RowId)
	 EXEC Znode_GetOrderByPagingProduct @order_by,@rows,@PageNo, @ProductListIdRTR ,@AttributeCode,@localeId 
	 END 
	 --SELECT * 
		--	 FROM @TBL_MainList
		

			 INSERT INTO @ProductIdTable (PimProductId)              
			 SELECT Id 
			 FROM @TBL_MainList SP 
				  	           
             --SET @PimProductIds = SUBSTRING((SELECT ','+CAST(PimProductId AS VARCHAR(100)) FROM @ProductIdTable FOR XML PATH('')), 2, 4000);
			 INSERT INTO @PimProductIds ( Id )
			 SELECT PimProductId FROM @ProductIdTable

             SET @PimAttributeId = @pimSkuAttributeId  + ',' + @PimProductNameAttributeId + ',' +@PimProductTypeAttributeId + ',' + @PimIsDownlodableAttributeId + ',' + @PimProductImageAttributeId ;
			
			  DECLARE @FamilyDetails TABLE
             (
				PimProductId         INT,
				PimAttributeFamilyId INT,
				FamilyName           NVARCHAR(3000)
             );	


			 INSERT INTO @FamilyDetails ( PimAttributeFamilyId, PimProductId )
             EXEC [dbo].[Znode_GetPimProductAttributeFamilyId] @PimProductIds, 1;

             UPDATE a
             SET  FamilyName = b.AttributeFamilyName
             FROM @FamilyDetails a
             INNER JOIN ZnodePimFamilyLocale b ON(a.PimAttributeFamilyId = b.PimAttributeFamilyId AND LocaleId = @LocaleId);

             UPDATE a
             SET FamilyName = b.AttributeFamilyName
             FROM @FamilyDetails a 
			 INNER JOIN ZnodePimFamilyLocale b ON(a.PimAttributeFamilyId = b.PimAttributeFamilyId AND LocaleId = @DefaultLocaleId)
             WHERE a.FamilyName IS NULL OR a.FamilyName = '';

			 
             --INSERT INTO @TBL_AttributeDetails ( PimProductId, AttributeValue, AttributeCode, PimAttributeId )
             INSERT INTO #TBL_AttributeDetails( PimProductId, AttributeValue, AttributeCode, PimAttributeId )
			 EXEC Znode_GetProductsAttributeValue @PimProductIds, @PimAttributeId, @localeId;
			 
			-- INSERT INTO @TBL_AttributeDetails ( PimProductId, AttributeValue, AttributeCode )
			INSERT INTO #TBL_AttributeDetails( PimProductId, AttributeValue, AttributeCode )
			 SELECT PimProductId,FamilyName ,'AttributeFamily' AttributeCode 
			 FROM @FamilyDetails 
		    
			IF @Order_BY=''
			BEGIN
			SET @Order_BY='PimProductId ASC'
			END

			SET @SQLqry =';With Cte_pimProductDetails1 AS
			(
			  SELECT PimProductId,AttributeValue,AttributeCode FROM #TBL_AttributeDetails
			)
			SELECT PimProductId,ProductName,SKU,Convert ( BIT, CASE when ISNULL(IsDownloadable,0)= ''true'' then 1 else 0 END )IsDownloadable,ZM.[Path] ProductImage, AttributeFamily,[ProductType]
			FROM Cte_pimProductDetails1 CTEPD
			PIVOT
			(
				Max(AttributeValue) FOR AttributeCode IN ([ProductName],[SKU],[IsDownloadable],[ProductImage],[AttributeFamily],[ProductType])
			) PIV
			LEFT JOIN ZnodeMedia ZM ON (ZM.MediaId = Piv.[ProductImage]) ORDER BY '+@Order_BY+''
			
			EXEC (@SQLqry)
			
			--;With Cte_pimProductDetails AS
			--(
			--  SELECT PimProductId,AttributeValue,AttributeCode FROM #TBL_AttributeDetails
			--)
			--SELECT PimProductId,ProductName,SKU,Convert ( BIT, CASE when ISNULL(IsDownloadable,0)= 'true' then 1 else 0 END )IsDownloadable,@IMamgePAth+ZM.[Path] ProductImage, AttributeFamily,[ProductType]
			--FROM Cte_pimProductDetails CTEPD
			--PIVOT
			--(
			--	Max(AttributeValue) FOR AttributeCode IN ([ProductName],[SKU],[IsDownloadable],[ProductImage],[AttributeFamily],[ProductType])
			--) PIV
			--LEFT JOIN ZnodeMedia ZM ON (ZM.MediaId = Piv.[ProductImage])
			--order by ProductName desc
			  
			
				  IF EXISTS (SELECT Top 1 1 FROM @TAb )
	 BEGIN 

		  SELECT @RowsCount= (SELECT COUNT(1) FROM @TAb)   
	 END 
	 ELSE 
	 BEGIN
	 		  SELECT  @RowsCount= (SELECT COUNT(1) FROM @ProductListIdRTR) 
	 END 
	 
	 
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), 
			 @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetSkuListForInventoryAndPrice @WhereClause = '''+ISNULL(@WhereClause,'''''')+''',@Rows='+ISNULL(CAST(@Rows AS
			VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')+',
			@LocaleId = '+ISNULL(CAST(@LocaleId AS VARCHAR(50)),'''')+',@PriceListId='+ISNULL(CAST(@PriceListId AS VARCHAR(50)),'''');
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetSkuListForInventoryAndPrice',
				@ErrorInProcedure = 'Znode_GetSkuListForInventoryAndPrice',
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
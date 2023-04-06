CREATE PROCEDURE [dbo].[Znode_GetSkuPricebyCatalog_Stock]
(   @WhereClause		NVARCHAR(max),
    @Rows				INT            = 100,
    @PageNo				INT            = 1,
    @Order_BY			VARCHAR(1000)  = '',
    @RowsCount			INT  out,
	@LocaleId			INT			   = 0,
	@Sku                VARCHAR(MAX),
	@PortalId		    INT = 0,
	@currentUtcDate     VARCHAR(200) = '',
	@PublishProductId   TransferId READONLY,
	@IsInStock			varchar(5) 
		)		
AS 
/*
    Summary: This procedure is used to find the PriceList by catalog 
	Unit Testing: 
	
    @IsInStock --- for 1 - In stock data , for 0 - out off stock data , for -1 - all data

	 EXEC Znode_GetSkuPricebyCatalog @WhereClause='' ,@Order_BY='RetailPrice',@RowsCount= 0,@Sku = 'str324,pe3251,pe0978,kw001,lm001,li001,sf001,og002', @LocaleId = 1,@PortalId=1,@currentUtcDate='2017'
*/

     BEGIN
         BEGIN TRY
		 
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @TBL_PricebyCatalog TABLE (SKU NVARCHAR(4000),RetailPrice numeric(28,6),RowId INT,CountNo INT,callforpricing int)
			 DECLARE @DefaultLocaleId INT = dbo.FN_GetDefaultLocaleId()

			DECLARE  @tbl_PricingSku TABLE (sku nvarchar(200),RetailPrice numeric(28,6),SalesPrice numeric(28,6),TierPrice numeric(28,6),
						TierQuantity numeric(28,6),CurrencyCode varchar(200),CurrencySuffix varchar(2000), CultureCode varchar(100), ExternalId NVARCHAR(2000), Custom1 NVARCHAR(MAX),
			  Custom2 NVARCHAR(MAX), Custom3 NVARCHAR(MAX))				
			
						INSERT INTO @tbl_PricingSku (sku,RetailPrice ,SalesPrice,TierPrice,TierQuantity,CurrencyCode,CurrencySuffix, CultureCode, ExternalId ,Custom1,Custom2,Custom3 )	
						EXEC Znode_GetPublishProductPricingBySku  @Sku ,@portaliD  ,@currentUtcDate,2,@PublishProductId
		
					  SELECT DISTINCT sku,RetailPrice
					  INTO #tbl_PricingSku
					  FROM   @tbl_PricingSku 
					  UNION  ALL 
					  SELECT item sku,NULL RetailPrice
					  FROM dbo.split(@Sku,',') SP 
					  WHERE NOT EXISTS (SELECT TOP 1 1  FROM @tbl_PricingSku TBSP WHERE TBSP.sku = Sp.Item) 
					  AND @Sku <> ''
					  UNION ALL 
					  SELECT SKU , NULL RetailPrice
					  FROM ZnodePublishProductDetail  a 
					  INNER JOIN @PublishProductId b ON (b.Id = a.PublishProductId) 
					  WHERE LocaleId = @DefaultLocaleId
					  AND NOT EXISTS (SELECT TOP 1 1  FROM @tbl_PricingSku TBSP WHERE TBSP.sku = a.SKU) 
					  AND @Sku = ''


					

	 SET @SQL = 
			       '
			DECLARE @tbl_ProductIDSKU Table(productid int,sku nvarchar(200))
					Insert into @tbl_ProductIDSKU (productid,sku)
					Select vw.PimProductId,vw.AttributeValue from View_PimProductAttributeValue vw where vw.AttributeCode = ''SKU'' and vw.AttributeValue in (select sku from #tbl_PricingSku);
					
			DECLARE @tbl_ProductIdCallForPricing Table(productid int,callforpricing nvarchar(200))
					Insert into @tbl_ProductIdCallForPricing (productid,callforpricing)
					select DISTINCT vw.PimProductId,vw.AttributeValue from View_PimProductAttributeValue vw where vw.AttributeCode = ''CallForPricing'' and vw.PimProductId in (Select PimProductid from View_PimProductAttributeValue where AttributeCode = ''SKU'' and AttributeValue in (select sku from #tbl_PricingSku))

            DECLARE @tbl_SKUCallForPricing Table(sku nvarchar(200),callforpricing int)
					Insert into @tbl_SKUCallForPricing (sku,callforpricing)
					select _ps.sku,Case when _pcp.callforpricing = ''true'' then 1 else 0 end callforpricing from @tbl_ProductIdCallForPricing _pcp join @tbl_ProductIDSKU _ps on _pcp.productid=_ps.productid where _ps.sku in (select sku from #tbl_PricingSku)

				
				   ;WITH CTE_GetFilteredList AS
					(
						SELECT DISTINCT A.sku,A.RetailPrice,B.callforpricing as CallForPricing, '+dbo.Fn_GetPagingRowId(@Order_BY,'A.SKU DESC ')+',Count(*)Over() CountNo
						FROM #tbl_PricingSku A 
						LEFT join @tbl_SKUCallForPricing B on A.sku = b.sku 
						WHERE 1=1
						'+dbo.Fn_GetFilterWhereClause(@WhereClause)+'			
					)
				
	
					SELECT SKU,RetailPrice,CallForPricing,RowId,CountNo
					FROM CTE_GetFilteredList
				   '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)+
					CASE WHEN @Order_BY = '' THEN '' ELSE ' ORDER BY '+ @Order_BY END

            PRINT @SQL
			INSERT INTO @TBL_PricebyCatalog(SKU,RetailPrice,CallForPricing,RowId,CountNo)
			EXEC sys.sp_sqlexec @SQL

			DECLARE @TBL_PricebyCatalogFinalResult TABLE ( SKU NVARCHAR(4000), RetailPrice numeric(28,6), Callforpricing int, RowId INT,CountNo INT )

			IF ( @IsInStock = '-1' )  
			BEGIN
				INSERT INTO @TBL_PricebyCatalogFinalResult ( SKU,RetailPrice,CallForPricing, RowId ,CountNo )
				SELECT SKU,RetailPrice,CallForPricing, RowId ,CountNo
				FROM @TBL_PricebyCatalog
			END
			ELSE IF ( @IsInStock = '1' )
			BEGIN	
				  INSERT INTO @TBL_PricebyCatalogFinalResult ( SKU,RetailPrice,CallForPricing, RowId ,CountNo )			
				  SELECT distinct TPC.SKU, TPC.RetailPrice, TPC.CallForPricing, TPC.RowId ,TPC.CountNo
				  FROM [dbo].[ZnodePimProductAttributeDefaultValue] oosdv
				  INNER JOIN ZnodePimAttributeValue oos ON oosdv.PimAttributeValueId = oos.PimAttributeValueId
				  INNER JOIN ZnodePimAttributeDefaultValue oosv ON oosdv.PimAttributeDefaultValueId = oosv.PimAttributeDefaultValueId
				  INNER JOIN ZnodePimAttributeValue sku ON oos.PimProductId = sku.PimProductId
				  INNER JOIN ZnodePimAttributeValueLocale skul ON sku.PimAttributeValueId = skul.PimAttributeValueId
				  INNER JOIN @TBL_PricebyCatalog TPC ON SKUL.AttributeValue = TPC.SKU
				  INNER JOIN ZnodePimAttribute Attr ON oos.PimAttributeId = Attr.PimAttributeId
				  WHERE Attr.AttributeCode = 'OutOfStockOptions' 
				  AND  EXISTS ( SELECT * FROM [dbo].[ZnodeInventory] I WHERE I.SKU = TPC.SKU AND 1 = (case when oosv.AttributeDefaultValueCode = 'DisablePurchasing' and I.Quantity < 1 then 0 else 1 end) )
			END
			ELSE IF ( @IsInStock = '0' )
			BEGIN
			      INSERT INTO @TBL_PricebyCatalogFinalResult ( SKU,RetailPrice,CallForPricing, RowId, CountNo )
				  SELECT distinct TPC.SKU, TPC.RetailPrice, TPC.CallForPricing, TPC.RowId ,TPC.CountNo
				  FROM [dbo].[ZnodePimProductAttributeDefaultValue] oosdv
				  INNER JOIN ZnodePimAttributeValue oos ON oosdv.PimAttributeValueId = oos.PimAttributeValueId
				  INNER JOIN ZnodePimAttributeDefaultValue oosv ON oosdv.PimAttributeDefaultValueId = oosv.PimAttributeDefaultValueId
				  INNER JOIN ZnodePimAttributeValue sku ON oos.PimProductId = sku.PimProductId
				  INNER JOIN ZnodePimAttributeValueLocale skul ON sku.PimAttributeValueId = skul.PimAttributeValueId
				  INNER JOIN @TBL_PricebyCatalog TPC ON SKUL.AttributeValue = TPC.SKU
				  INNER JOIN ZnodePimAttribute Attr ON oos.PimAttributeId = Attr.PimAttributeId
				  WHERE Attr.AttributeCode = 'OutOfStockOptions' 
				  AND  EXISTS ( SELECT TOP 1 1  FROM [dbo].[ZnodeInventory] I WHERE I.SKU = TPC.SKU  AND  oosv.AttributeDefaultValueCode = 'DisablePurchasing' GROUP BY I.SKU HAVING SUM(I.Quantity ) < 1  )
			END

			SET @RowsCount =ISNULL((SELECT TOP 1 CountNo FROM @TBL_PricebyCatalogFinalResult ),0)

			SELECT SKU,RetailPrice,CallForPricing FROM @TBL_PricebyCatalogFinalResult

		END TRY
		BEGIN CATCH
		DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetSkuPricebyCatalog @WhereClause = '
			 +CAST(@WhereClause AS VARCHAR(MAX))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))
			 +',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(20))
			 +',@PortalId= '+cast(@PortalId as varchar(200))+',@currentUtcDate= '
			 +@currentUtcDate+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status,ERROR_MESSAGE();                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetSkuPricebyCatalog',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;

		END CATCH

	END
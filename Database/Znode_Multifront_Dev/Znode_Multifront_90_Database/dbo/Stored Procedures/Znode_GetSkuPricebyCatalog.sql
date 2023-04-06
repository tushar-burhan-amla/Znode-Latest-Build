CREATE PROCEDURE [dbo].[Znode_GetSkuPricebyCatalog]
(   @WhereClause		NVARCHAR(max),
    @Rows				INT            = 100,
    @PageNo				INT            = 1,
    @Order_BY			VARCHAR(1000)  = '',
    @RowsCount			INT  out,
	@LocaleId			INT			   = 0,
	@Sku                VARCHAR(MAX),
	@PortalId		    INT = 0,
	@currentUtcDate     VARCHAR(200) = '',
	@PublishProductId   ProductForSortPrice READONLY,
	@IsInStock			varchar(5) ,
	@IsSorting			Bit = 1 
)		
AS 
/*
    Summary: This procedure is used to find the PriceList by catalog 
	Unit Testing: 
	
    @IsInStock --- for 1 - In stock data , for 0 - out off stock data , for -1 - all data

	declare @p7 int
	set @p7=NULL
	declare @p12 dbo.ProductForSortPrice
	insert into @p12 values(947,N'SimpleProduct',N'DisablePurchasing',0)
	insert into @p12 values(948,N'SimpleProduct',N'DisablePurchasing',0)
	insert into @p12 values(949,N'SimpleProduct',N'DisablePurchasing',0)
	insert into @p12 values(950,N'SimpleProduct',N'DisablePurchasing',0)
	insert into @p12 values(951,N'SimpleProduct',N'DisablePurchasing',0)
	insert into @p12 values(953,N'SimpleProduct',N'DisablePurchasing',0)
	insert into @p12 values(957,N'SimpleProduct',N'DisablePurchasing',0)
	insert into @p12 values(1002,N'SimpleProduct',N'DisablePurchasing',0)
	insert into @p12 values(1003,N'SimpleProduct',N'DisablePurchasing',0)
	insert into @p12 values(1004,N'SimpleProduct',N'DisablePurchasing',0)
	insert into @p12 values(1005,N'SimpleProduct',N'DisablePurchasing',0)
	insert into @p12 values(1026,N'BundleProduct',N'DisablePurchasing',0)
	insert into @p12 values(1030,N'BundleProduct',N'DisablePurchasing',1)
	insert into @p12 values(1039,N'GroupedProduct',N'DontTrackInventory',0)
	insert into @p12 values(1013,N'BundleProduct',N'AllowBackOrdering',0)
	insert into @p12 values(1031,N'SimpleProduct',N'DisablePurchasing',0)
	insert into @p12 values(1032,N'SimpleProduct',N'DisablePurchasing',0)
	insert into @p12 values(1042,N'SimpleProduct',N'DisablePurchasing',0)

	exec sp_executesql N'Znode_GetSkuPricebyCatalog  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT
	,@LocaleId,@Sku,@PortalId,@currentUtcDate,@PublishProductId,@IsInStock',N'@WhereClause nvarchar(4000),
	@Rows int,@PageNo int,@Order_By nvarchar(15),@RowCount int output,@LocaleId int,
	@Sku nvarchar(4000),@PortalId int,@currentUtcDate datetime,@PublishProductId [dbo].[ProductForSortPrice] 
	READONLY,@IsInStock nvarchar(2)',@WhereClause=N'',@Rows=16,@PageNo=1,@Order_By=N'retailprice asc',@RowCount=@p7 output,
	@LocaleId=1,@Sku=N'',@PortalId=1,@currentUtcDate='2017-11-30 00:00:00',@PublishProductId=@p12,@IsInStock=N'1'
	select @p7

	GO

	declare @p7 int
	set @p7=NULL
	declare @p12 dbo.TransferId
	insert into @p12 values(947)
	insert into @p12 values(948)
	insert into @p12 values(949)
	insert into @p12 values(950)
	insert into @p12 values(951)
	insert into @p12 values(953)
	insert into @p12 values(957)
	insert into @p12 values(1002)
	insert into @p12 values(1003)
	insert into @p12 values(1004)
	insert into @p12 values(1005)
	insert into @p12 values(1026)
	insert into @p12 values(1030)
	insert into @p12 values(1039)
	insert into @p12 values(1013)
	insert into @p12 values(1031)
	insert into @p12 values(1032)
	insert into @p12 values(1042)

	exec sp_executesql N'Znode_GetSkuPricebyCatalog  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT
	,@LocaleId,@Sku,@PortalId,@currentUtcDate,@PublishProductId,@IsInStock',N'@WhereClause nvarchar(4000),
	@Rows int,@PageNo int,@Order_By nvarchar(15),@RowCount int output,@LocaleId int,
	@Sku nvarchar(4000),@PortalId int,@currentUtcDate datetime,@PublishProductId [dbo].[TransferId] 
	READONLY,@IsInStock nvarchar(2)',@WhereClause=N'',@Rows=16,@PageNo=1,@Order_By=N'retailprice asc',@RowCount=@p7 output,
	@LocaleId=1,@Sku=N'',@PortalId=1,@currentUtcDate='2017-11-30 00:00:00',@PublishProductId=@p12,@IsInStock=N'1'
	select @p7

*/


     BEGIN
     BEGIN TRY
	 SET NOCOUNT ON;
			 DECLARE @ProductIdForPricing   TransferId 
             DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @TBL_PricebyCatalog TABLE (SKU NVARCHAR(4000),RetailPrice numeric(28,6),RowId INT,CountNo INT,ProductType nvarchar(200),OutOfStockOptions nvarchar(200),SalesPrice numeric(28,6))
			 CREATE TABLE #TBL_PricebyCatalogforAssociateProduct  (PimProductId int ,AssociatedProductId int,ParentSKU NVARCHAR(300),
			 ChildSKU NVARCHAR(300),RetailPrice  numeric(28,6),AssociatedProductDisplayOrder int , TypeOfProduct nvarchar(100),SalesPrice  numeric(28,6))
			 DECLARE @DefaultLocaleId INT = dbo.FN_GetDefaultLocaleId()
			 
			 DECLARE @tbl_PricingSku TABLE (sku nvarchar(200),RetailPrice numeric(28,6),SalesPrice numeric(28,6),TierPrice numeric(28,6),
						TierQuantity numeric(28,6),CurrencyCode varchar(200),CurrencySuffix varchar(2000),CultureCode VARCHAR(100), ExternalId NVARCHAR(2000),Custom1 NVARCHAR(MAX),Custom2 NVARCHAR(MAX),
			  Custom3 NVARCHAR(MAX))
			  
			 CREATE TABLE #tbl_PricingSkuOfAssociatedProduct (sku nvarchar(200),RetailPrice numeric(28,6),SalesPrice numeric(28,6),TierPrice numeric(28,6),
						TierQuantity numeric(28,6),CurrencyCode varchar(200),CurrencySuffix varchar(2000),CultureCode VARCHAR(100), ExternalId NVARCHAR(2000),Custom1 NVARCHAR(MAX), Custom2 NVARCHAR(MAX),
			  Custom3 NVARCHAR(MAX))				
			
			Select [Id],[ProductType],[OutOfStockOptions],--[CallForPricing] ,
			 Convert(varchar(300),'') SKU into #PublishProductId from @PublishProductId
			
			UPDATE PDI SET PDI.SKU = ZPPD.SKU 
						from #PublishProductId PDI inner join
						ZnodePublishProductEntity ZPPD On PDI.ID = ZPPD.ZnodeProductId
		
					--Read price for all products
					--Start
					INSERT INTO @ProductIdForPricing SELECT id FROM @PublishProductId
	 
					INSERT INTO @tbl_PricingSku (sku,RetailPrice ,SalesPrice,TierPrice,TierQuantity,CurrencyCode,CurrencySuffix,CultureCode, ExternalId ,Custom1,Custom2,Custom3)	
					EXEC Znode_GetPublishProductPricingBySku   @SKU=@Sku ,@PortalId=@portaliD  ,@currentUtcDate=@currentUtcDate,@UserId=2,@PublishProductId=@ProductIdForPricing
					--End
					
					--Read Associate Products price only
					--Start

					INSERT INTO #TBL_PricebyCatalogforAssociateProduct (PimProductId ,AssociatedProductId, ParentSKU,ChildSKU,AssociatedProductDisplayOrder,TypeOfProduct ) 
					SELECT PDI.ID  PublishedId,c.ZnodeProductId PublishProductId, PDI.SKU, c.sku ,a.AssociatedProductDisplayOrder DisplayOrder ,'ConfigurableProduct'
					from #PublishProductId PDI 
					INNER JOIN ZnodePublishConfigurableProductEntity a with(nolock) ON (a.ZnodeProductId = PDI.Id)
					INNER JOIN ZnodePublishProductEntity c  with(nolock) ON (c.ZnodeProductId = a.AssociatedZnodeProductId)
					WHERE PDI.ProductType = 'ConfigurableProduct'
					
					INSERT INTO #TBL_PricebyCatalogforAssociateProduct (PimProductId ,AssociatedProductId, ParentSKU,ChildSKU,AssociatedProductDisplayOrder,TypeOfProduct ) 
					SELECT PDI.ID  PublishedId,c.ZnodeProductId PublishProductId, PDI.SKU, c.sku ,a.AssociatedProductDisplayOrder DisplayOrder ,'GroupedProduct'
					from #PublishProductId PDI 
					INNER JOIN ZnodePublishGroupProductEntity a  with(nolock) ON (a.ZnodeProductId = PDI.Id)
					INNER JOIN ZnodePublishProductEntity c  with(nolock) ON (c.ZnodeProductId = a.AssociatedZnodeProductId)
					WHERE PDI.ProductType = 'GroupedProduct'

				--	SELECT ZPP.PublishProductId PublishedId,ZPP1.PublishProductId, PDI.SKU,ChildZPPD.SKU,ZPXML.DisplayOrder ,'ConfigurableProduct'
				--	from #PublishProductId PDI 
				--	INNER JOIN ZnodePublishProduct ZPP ON PDI.id = ZPP.PublishProductId
				----	INNER JOIN ZnodePublishAssociatedProduct ZPXML ON (ZPP.PimProductId = ZPXML.ParentPimProductId) 	
				--	INNER JOIN ZnodePublishProduct ZPP1 ON ZPXML.PimProductId = ZPP1.PimProductId				
				--	Left Outer JOIN ZnodePublishProductEntity ChildZPPD On ZPP1.PublishProductId = ChildZPPD.ZnodeProductId
				--	WHERE PDI.ProductType = 'ConfigurableProduct' AND ZPXML.IsConfigurable = 1

				--	INSERT INTO #TBL_PricebyCatalogforAssociateProduct (PimProductId ,AssociatedProductId, ParentSKU,ChildSKU, TypeOfProduct ) 
				--	SELECT ZPP.PublishProductId,ZPP1.PublishProductId, PDI.SKU,ChildZPPD.SKU,'GroupedProduct'
				--	FROM #PublishProductId PDI 
				--	INNER JOIN ZnodePublishProduct ZPP ON PDI.id = ZPP.PublishProductId
				--	INNER JOIN ZnodePublishAssociatedProduct ZPXML ON (ZPP.PimProductId = ZPXML.ParentPimProductId) 	
				--	INNER JOIN ZnodePublishProduct ZPP1 ON ZPXML.PimProductId = ZPP1.PimProductId	
				--	Left Outer JOIN ZnodePublishProductEntity ChildZPPD On ZPP1.PublishProductId = ChildZPPD.ZnodeProductId
				--	WHERE PDI.ProductType = 'GroupedProduct' AND ZPXML.IsGroup = 1

				

					DELETE FROM @ProductIdForPricing 
					INSERT INTO @ProductIdForPricing 
					SELECT Distinct AssociatedProductId 
					FROM #TBL_PricebyCatalogforAssociateProduct 
					where AssociatedProductId is not null 

					INSERT INTO #tbl_PricingSkuOfAssociatedProduct (sku,RetailPrice ,SalesPrice,TierPrice,TierQuantity,CurrencyCode,CurrencySuffix,CultureCode, ExternalId,Custom1,Custom2,Custom3 )	
					EXEC Znode_GetPublishProductPricingBySku   @SKU=@Sku ,@PortalId=@portaliD  ,@currentUtcDate=@currentUtcDate,@UserId=2,@PublishProductId=@ProductIdForPricing
								

					update PLC SET PLC.RetailPrice = PLCA.RetailPrice , PLC.SalesPrice = PLCA.SalesPrice 
					FROM #TBL_PricebyCatalogforAssociateProduct PLC 
					INNER JOIN #tbl_PricingSkuOfAssociatedProduct PLCA on PLC.ChildSKU = PLCA.sku
 
		
					SELECT DISTINCT sku,RetailPrice,SalesPrice  INTO #tbl_PricingSku FROM   @tbl_PricingSku 
					UNION  ALL 
					SELECT item sku,NULL RetailPrice  ,NULL SalesPrice FROM dbo.split(@Sku,',') SP  
					WHERE NOT EXISTS (SELECT TOP 1 1  FROM @tbl_PricingSku TBSP WHERE TBSP.sku = Sp.Item) AND @Sku <> ''
					UNION ALL 
					SELECT a.SKU , NULL RetailPrice , NULL SalesPrice  FROM ZnodePublishProductEntity  a  INNER JOIN @PublishProductId b ON (b.Id = a.ZnodeProductId) 
					WHERE LocaleId = @DefaultLocaleId AND NOT EXISTS (SELECT TOP 1 1  FROM @tbl_PricingSku TBSP WHERE TBSP.sku = a.SKU) 
					AND @Sku = ''
							
					Update PBC SET PBC.RetailPrice = 
						(Select min(Isnull(RetailPrice, SalesPrice)) from #TBL_PricebyCatalogforAssociateProduct PCBA  
						where PCBA.ParentSKU =PBC.SKU and PCBA.ParentSKU is not null)
					FROM #tbl_PricingSku  PBC  where 
					EXISTS (Select TOP 1 1  from #TBL_PricebyCatalogforAssociateProduct PCBA  where PCBA.ParentSKU =PBC.SKU and PCBA.TypeOfProduct = 'GroupedProduct')
			      
				 	Update PBC SET PBC.RetailPrice = 
						(Select TOP 1 Isnull(RetailPrice ,SalesPrice) from #TBL_PricebyCatalogforAssociateProduct PCBA  where PCBA.ParentSKU =PBC.SKU
						 and PCBA.ParentSKU is not null and PCBA.ChildSKU is not null
					Order by AssociatedProductDisplayOrder)
					from #tbl_PricingSku  PBC  where 
					Exists (Select TOP 1 1  from #TBL_PricebyCatalogforAssociateProduct PCBA  where PCBA.ParentSKU =PBC.SKU and PCBA.TypeOfProduct = 'ConfigurableProduct')
					and PBC.RetailPrice IS null 
			

 			    If @IsSorting = 1 
				BEGIN
					SET @Order_BY = Replace (@Order_BY,'RetailPrice', 'Case when SalesPrice is not null then SalesPrice else RetailPrice end ')
					
					SET @SQL = 
					';WITH CTE_GetFilteredList AS
					(
						SELECT DISTINCT A.sku,A.RetailPrice,SalesPrice , '+dbo.Fn_GetPagingRowId(@Order_BY,'A.SKU DESC ')+',Count(*)Over() CountNo
						FROM #tbl_PricingSku A 
						WHERE 1=1
						'+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
					)
					SELECT TOP '+Cast(@Rows AS VARCHAR(10))+' SKU,RetailPrice,SalesPrice,RowId,CountNo
					FROM CTE_GetFilteredList
					'+CASE WHEN @Order_BY = '' THEN '' ELSE ' ORDER BY '+ @Order_BY END
					--dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)+
					
				END
				Else 
				BEGIN
				
					SET @SQL = 
					';WITH CTE_GetFilteredList AS
					(
						SELECT DISTINCT A.sku,A.RetailPrice,A.SalesPrice ,'+dbo.Fn_GetPagingRowId(@Order_BY,'A.SKU DESC ')+',Count(*)Over() CountNo
						FROM #tbl_PricingSku A 
						WHERE 1=1
						'+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
					)
					SELECT TOP '+Cast(@Rows AS VARCHAR(10))+' SKU,RetailPrice,SalesPrice , RowId,CountNo
					FROM CTE_GetFilteredList'
					--'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
					--CASE WHEN @Order_BY = '' THEN '' ELSE ' ORDER BY '+ @Order_BY END
				END
		   

			INSERT INTO @TBL_PricebyCatalog(SKU,RetailPrice,SalesPrice,RowId,CountNo)
			EXEC sys.sp_sqlexec @SQL
        	
			DECLARE @TBL_PricebyCatalogFinalResult TABLE ( SKU NVARCHAR(4000), RetailPrice numeric(28,6),SalesPrice numeric(28,6) , RowId INT,CountNo INT, PublishProductId int  )
	
			IF ( @IsInStock = '-1' )  
			BEGIN 
				INSERT INTO @TBL_PricebyCatalogFinalResult ( SKU,RetailPrice,SalesPrice, RowId ,CountNo,PublishProductId )
				SELECT PBC.SKU,RetailPrice,SalesPrice, RowId ,CountNo,PPI.ID
				FROM @TBL_PricebyCatalog  PBC LEft Outer JOIN #PublishProductId PPI On PBC.SKU = PPI.SKU 
			END
			ELSE IF ( @IsInStock = '1' )
			BEGIN	
				  INSERT INTO @TBL_PricebyCatalogFinalResult ( SKU,RetailPrice,SalesPrice, RowId ,CountNo,PublishProductId )			
					  SELECT PBC.SKU,RetailPrice,SalesPrice, RowId ,CountNo,PPI.ID
					  FROM @TBL_PricebyCatalog  PBC LEft Outer JOIN #PublishProductId PPI On PBC.SKU = PPI.SKU 
					  WHERE EXISTS ( SELECT TOP 1 1 FROM [dbo].[ZnodeInventory] I WHERE I.SKU = PPI.SKU AND 1 =
					  (case when PPI.OutOfStockOptions = 'DisablePurchasing' and I.Quantity < 1 then 0 else 1 end))
					  Union All 
					  SELECT PBC.SKU,RetailPrice,SalesPrice, RowId ,CountNo,PPI.ID
					  FROM @TBL_PricebyCatalog  PBC LEft Outer JOIN #PublishProductId PPI On PBC.SKU = PPI.SKU 
					  WHERE NOT EXISTS ( SELECT TOP 1 1 FROM [dbo].[ZnodeInventory] I WHERE I.SKU = PPI.SKU AND 1 =
					  (case when PPI.OutOfStockOptions = 'DisablePurchasing' and I.Quantity < 1 then 0 else 1 end))
			END
			ELSE IF ( @IsInStock = '0' )
			BEGIN
		
				  INSERT INTO @TBL_PricebyCatalogFinalResult ( SKU,RetailPrice,SalesPrice, RowId ,CountNo,PublishProductId )			
					  SELECT PBC.SKU,RetailPrice,SalesPrice, RowId ,CountNo,PPI.ID
					  FROM @TBL_PricebyCatalog  PBC LEft Outer JOIN #PublishProductId PPI On PBC.SKU = PPI.SKU 
					  WHERE EXISTS ( SELECT TOP 1 1 FROM [dbo].[ZnodeInventory] I WHERE I.SKU = PPI.SKU 
					  AND  PPI.OutOfStockOptions = 'DisablePurchasing' 
					  GROUP BY I.SKU HAVING SUM(I.Quantity ) < 1   )
					  Union all 
					  SELECT PBC.SKU,RetailPrice,SalesPrice,RowId ,CountNo,PPI.ID
					  FROM @TBL_PricebyCatalog  PBC LEft Outer JOIN #PublishProductId PPI On PBC.SKU = PPI.SKU 
					  WHERE NOT EXISTS ( SELECT TOP 1 1 FROM [dbo].[ZnodeInventory] I WHERE I.SKU = PPI.SKU 
					  AND  PPI.OutOfStockOptions = 'DisablePurchasing' 
					  GROUP BY I.SKU HAVING SUM(I.Quantity ) < 1   )

			END

			SET @RowsCount =ISNULL((SELECT TOP 1 CountNo FROM @TBL_PricebyCatalogFinalResult),0)

			SELECT SKU,RetailPrice,SalesPrice,PublishProductId FROM @TBL_PricebyCatalogFinalResult
			--Order by Isnull(RetailPrice,SalesPrice)
			
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
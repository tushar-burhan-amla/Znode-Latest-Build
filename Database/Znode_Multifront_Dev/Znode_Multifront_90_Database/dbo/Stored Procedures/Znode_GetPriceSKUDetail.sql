CREATE PROCEDURE [dbo].[Znode_GetPriceSKUDetail]
(   @WhereClause NVARCHAR(MAX),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(1000) = '',
    @RowsCount   INT OUT,
    @LocaleId    INT           = 1)
AS 
    /*
     Summary : This Procedure is used to get price with product name and other details
				The Result is fetched order by PimProductId in descending order 
      Unit Testing:
     EXEC Znode_GetPriceSKUDetail  '', @RowsCount = 1  
   */
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
			 
             DECLARE @PriceRoundOff VARCHAR(10)= Dbo.Fn_GetDefaultValue('PriceRoundOff');
                     
			 DECLARE @DefaultLocaleId INT= (SELECT TOP 1 FeatureValues FROM ZnodeGlobalSetting WHERE FeatureName = 'Locale');

		 	 DECLARE @TBL_CollectDataForList TABLE (PimProductId INT ,SKU  NVARCHAR(600),ActivationDate datetime,ExpirationDate datetime,SalesPrice NUMERIC(28,6),RetailPrice NUMERIC(28,6), ProductName NVARCHAR(max),PriceListId INT,PriceId INT,RowId INT,CountNo INT )	

			 SELECT zav.PimProductId ,PriceId,PriceListId ,zts.SKU,zts.ActivationDate,zts.ExpirationDate,zts.SalesPrice,zts.RetailPrice
			 into #ProductDetailsdec
			 FROM ZnodePimAttributeValue zav 
			 INNER JOIN ZnodePimAttribute za ON (za.PimAttributeId = zav.PimAttributeId )
			 INNER JOIN ZnodePimAttributeValueLocale zavl ON (zavl.PimAttributeValueId = zav.PimAttributeValueId AND zavl.LocaleId IN (@LocaleId ,@DefaultLocaleId ) )
			 INNER JOIN [dbo].[ZnodePrice] zts ON (zts.SKU = zavl.AttributeValue)
			 WHERE za.AttributeCode = 'SKU'
			 GROUP BY zav.PimProductId,PriceId,PriceListId,zts.SKU,zts.ActivationDate,zts.ExpirationDate,zts.SalesPrice,zts.RetailPrice
			 
    		 SELECT zav.PimProductId ,SKU ,ActivationDate ,ExpirationDate , SalesPrice ,RetailPrice ,ZAVL.AttributeValue ProductName, tcx.PriceListId , tcx.PriceId,ZAVL.LocaleId
			 into #ProductSkuListForBothLocale
			 FROM #ProductDetailsdec tcx 
			 INNER JOIN ZnodePimAttributeValue zav ON (zav.PimProductId = tcx.PimProductId )
			 INNER JOIN ZnodePimAttribute ZA ON (ZA.PimAttributeId = ZAV.PimAttributeId)
			 INNER JOIN ZnodePimAttributeValueLocale ZAVL ON (ZAVL.PimAttributeValueId = ZAV.PimAttributeValueId AND ZAVL.LocaleId IN (@LocaleId ,@DefaultLocaleId ))
			 WHERE Za.AttributeCode = 'ProductName'
			
			SELECT PimProductId ,SKU ,ActivationDate ,ExpirationDate , SalesPrice ,RetailPrice , ProductName,PriceListId,PriceId,LocaleId  
			into #ForLocaleFirst
			FROM #ProductSkuListForBothLocale WHERE LocaleId = @LocaleId 

			 SELECT PimProductId ,SKU ,ActivationDate, ExpirationDate, SalesPrice, RetailPrice, ProductName, PriceListId , PriceId
			 into #ForLocaleSecond
			 FROM #ForLocaleFirst FLF 
			 UNION ALL
			 SELECT PimProductId ,SKU ,ActivationDate, ExpirationDate, SalesPrice, RetailPrice, ProductName, PriceListId , PriceId
			 FROM #ProductSkuListForBothLocale AFH WHERE LocaleId =  @DefaultLocaleId 
			 AND NOT EXISTS (SELECT TOP 1 1 FROM #ForLocaleFirst sd WHERE AFH.PimProductId = sd.PimProductId )
			  	   	  
             SET @SQL =
			 'with GenerateRowId AS (
			 SELECT PimProductId ,SKU ,ActivationDate, ExpirationDate, SalesPrice, RetailPrice, ProductName, PriceListId , PriceId
			 ,'+dbo.Fn_GetPagingRowId(@Order_BY,'PimProductId DESC')+',Count(*)Over() CountNo
			 FROM #ForLocaleSecond
			 WHERE 1=1
			 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+' 
			 )
			
			 SELECT PimProductId ,SKU ,ActivationDate, ExpirationDate,[dbo].[Fn_GetDefaultPriceRoundOff](SalesPrice),[dbo].[Fn_GetDefaultPriceRoundOff](RetailPrice), ProductName, PriceListId , PriceId,RowId,CountNo 
			 FROM GenerateRowId
			 '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
			

			 --PRINT @SQL
			 INSERT INTO @TBL_CollectDataForList(PimProductId ,SKU ,ActivationDate, ExpirationDate, SalesPrice, RetailPrice, ProductName, PriceListId , PriceId,RowId,CountNo ) 
			 EXEC(@SQL)

			 SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_CollectDataForList),0)

			 SELECT PimProductId ,dbo.Fn_Trim(SKU)SKU ,ActivationDate, ExpirationDate, SalesPrice, RetailPrice, dbo.Fn_Trim(ProductName)ProductName, PriceListId , PriceId
			 FROM @TBL_CollectDataForList             			 			 
		 
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		    SET @Status = 0;
		    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPriceSKUDetail @WhereClause = '+cast (@WhereClause AS VARCHAR(50))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@LocaleId= '+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
            SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
            EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPriceSKUDetail',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
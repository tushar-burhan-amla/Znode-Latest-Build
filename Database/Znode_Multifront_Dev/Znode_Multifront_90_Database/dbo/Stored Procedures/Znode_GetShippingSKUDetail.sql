CREATE PROCEDURE [dbo].[Znode_GetShippingSKUDetail]
(   @WhereClause NVARCHAR(4000),
	@Rows        INT            = 100,
	@PageNo      INT            = 1,
	@Order_BY    VARCHAR(1000)  = '',
	@RowsCount   INT OUT,
	@LocaleId    INT            = 1)
AS 
/* 
     Summary : get shipping with product name and other details 
     Sequence For Delete Data   
	 Unit Testing:
     EXEC Znode_GetShippingSKUDetail  '',@Order_BY=' ShippingRuleId ASC ', @RowsCount = 1     
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @TBL_CollectDataForList TABLE (PimProductId INT ,SKU  NVARCHAR(600), ProductName NVARCHAR(max), ShippingRuleId INT ,  ShippingSKUId INT,RowId INT,CountNo INT )	
		 	  	 	  
             DECLARE @DefaultLocaleId INT= (SELECT TOP 1 FeatureValues FROM ZnodeGlobalSetting WHERE FeatureName = 'Locale');
             SET @SQL = ' 

		 
				  ;With CTE_ProductDetailsdec AS (
				   SELECT zav.PimProductId ,ShippingSKUId,ShippingRuleId ,zts.SKU
				   FROM ZnodePimAttributeValue zav 
				   INNER JOIN ZnodePimAttribute za ON (za.PimAttributeId = zav.PimAttributeId )
				   INNER JOIN ZnodePimAttributeValueLocale zavl ON (zavl.PimAttributeValueId = zav.PimAttributeValueId AND zavl.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(100))+','+CAST(@DefaultLocaleId AS VARCHAR(100))+') ) 
				   INNER JOIN [dbo].[ZnodeShippingSKU] zts ON (zts.SKU = zavl.AttributeValue)
				   WHERE za.AttributeCode = ''SKU''
				   GROUP BY zav.PimProductId,ShippingSKUId,ShippingRuleId,zts.SKU
				   )

    			   ,CTE_ProductSkuListForBothLocale AS ( 
    			   SELECT zav.PimProductId ,SKU ,ZAVL.AttributeValue ProductName, tcx.ShippingRuleId , tcx.ShippingSKUId,ZAVL.LocaleId
				   FROM CTE_ProductDetailsdec tcx 
				   INNER JOIN ZnodePimAttributeValue zav ON (zav.PimProductId = tcx.PimProductId )
				   INNER JOIN ZnodePimAttribute ZA ON (ZA.PimAttributeId = ZAV.PimAttributeId)
				   INNER JOIN ZnodePimAttributeValueLocale ZAVL ON (ZAVL.PimAttributeValueId = ZAV.PimAttributeValueId AND ZAVL.LocaleId IN ('+CAST(@LocaleId AS VARCHAR(100))+','+CAST(@DefaultLocaleId AS VARCHAR(100))+') )
				   WHERE Za.AttributeCode = ''ProductName'')

				   ,CTE_ApplyFilterHere AS 
				   (SELECT PimProductId ,SKU , ProductName, ShippingRuleId , ShippingSKUId,LocaleId
				   FROM CTE_ProductSkuListForBothLocale
				   WHERE 1=1
				   '+dbo.Fn_GetFilterWhereClause(@WhereClause)+')


				  ,CTE_ForLocaleFirst AS 
				  (SELECT PimProductId ,SKU , ProductName, ShippingRuleId , ShippingSKUId 
				  FROM CTE_ApplyFilterHere
				  WHERE LocaleId = '+CAST(@LocaleId AS VARCHAR(100))+'
				  )
		
				  , CTE_ForLocaleSecond AS 
				  (
				  SELECT PimProductId ,SKU , ProductName, ShippingRuleId , ShippingSKUId
				  FROM CTE_ForLocaleFirst a 
				  UNION ALL
				  SELECT PimProductId ,SKU , ProductName, ShippingRuleId , ShippingSKUId
				  FROM CTE_ApplyFilterHere a 
				  WHERE LocaleId =  '+CAST(@DefaultLocaleId AS VARCHAR(100))+'
				  AND NOT EXISTS (SELECT TOP 1 1 FROM CTE_ForLocaleFirst sd WHERE a.PimProductId = sd.PimProductId )  
				  )

				  , CTE_GenarateRowId AS 
				  (SELECT PimProductId ,SKU , ProductName, ShippingRuleId , ShippingSKUId,'+dbo.Fn_GetPagingRowId(@Order_BY,'PimProductId DESC')+',Count(*)Over() CountNo
				  FROM CTE_ForLocaleSecond
				  WHERE 1=1
				  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
				  )

				  SELECT PimProductId ,SKU , ProductName, ShippingRuleId , ShippingSKUId,RowId,CountNo 
				  FROM CTE_GenarateRowId
				  '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)


		  INSERT INTO @TBL_CollectDataForList 
		  EXEC(@SQL)

		  SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_CollectDataForList),0)
			
		  SELECT PimProductId ,SKU , ProductName,ShippingRuleId , ShippingSKUId
		  FROM @TBL_CollectDataForList
		
        END TRY
        BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetShippingSKUDetail @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetShippingSKUDetail',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
       END CATCH;
     END;
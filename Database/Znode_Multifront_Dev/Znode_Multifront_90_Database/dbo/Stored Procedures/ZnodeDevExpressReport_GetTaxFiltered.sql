CREATE PROCEDURE [dbo].[ZnodeDevExpressReport_GetTaxFiltered]
(   @BeginDate       DATE       ,
	@EndDate         DATE       ,
	@WhereClause NVARCHAR(max) = ''
	)
AS 
/*
     Summary:- this procedure is used to get the TAX details 
	 Unit Testing:
     EXEC ZnodeReport_GetTaxFiltered @ShipToStateCode = '' ,@BeginDate = '2015-11-15 13:03:27.700', @EndDate= '2016-11-15 13:03:27.700',@PortalId = '1'
 */
	
	 BEGIN
	 BEGIN TRY
         SET NOCOUNT ON;

		  DECLARE @SQL NVARCHAR(MAX);

			 SET @SQL = '
			  ;WITH CTE_GetTaxFiltered AS
			  (

		SELECT P.[StoreName] AS ''StoreName'', ZOOD.OrderDate, OSP.ShipToStateCode AS State, ZOOD.SubTotal AS TotalSales,
                SUM(ISNULL(ZOTOD.[SalesTax], 0) + ISNULL(ZOTOD.[VAT], 0) + ISNULL(ZOTOD.[GST], 0) + ISNULL(ZOTOD.[PST], 0) + ISNULL(ZOTOD.[HST], 0)) AS Tax,
                SPACE(10) AS Title,SPACE(30) AS Custom1,ISNULL(ZC.Symbol,dbo.Fn_GetDefaultCurrencySymbol()) Symbol
         FROM ZNodeOmsOrder AS ZOO
              INNER JOIN ZnodeOmsOrderDetails AS ZOOD ON(ZOOD.OmsOrderId = ZOO.OmsOrderId AND IsActive = 1)
              INNER JOIN ZNodePortal AS P ON P.PortalID = ZOOD.PortalId
              LEFT JOIN ZNodeOmsOrderShipment AS OSP ON(EXISTS
                                                       (
                                                           SELECT TOP 1 1
                                                           FROM ZnodeOmsOrderLineItems AS ZOOLI
                                                           WHERE ZOOLI.OmsOrderShipmentId = OSP.OmsOrderShipmentId
                                                                 AND ZOOLI.OmsOrderDetailsId = ZOOD.OmsOrderDetailsId
                                                       ))
              LEFT JOIN ZnodeOmsTaxOrderDetails AS ZOTOD ON(ZOTOD.OmsOrderDetailsId = ZOOD.OmsOrderDetailsId)
		    LEFT JOIN ZnodeCurrency ZC ON (ZC.CurrencyCode = ZOOD.CurrencyCode)
         WHERE
               CAST(ZOOD.OrderDate AS DATE) BETWEEN CASE
                                                            WHEN '''+CAST(@BeginDate AS VARCHAR(30))+''' IS NULL
                                                            THEN CAST(ZOOD.OrderDate AS DATE)
                                                            ELSE '''+CAST(@BeginDate AS VARCHAR(30))+'''
                                                        END AND CASE
                                                                    WHEN '''+CAST(@EndDate AS VARCHAR(30))+''' IS NULL
                                                                    THEN CAST(ZOOD.OrderDate AS DATE)
                                                                    ELSE '''+CAST(@EndDate AS VARCHAR(30))+'''
                                                                END																                    
		 GROUP BY P.[StoreName],ZOOD.OrderDate,OSP.ShipToStateCode,ZOOD.SubTotal,ZC.Symbol
		 HAving SUM(ISNULL(ZOTOD.[SalesTax], 0) + ISNULL(ZOTOD.[VAT], 0) + ISNULL(ZOTOD.[GST], 0) + ISNULL(ZOTOD.[PST], 0) + ISNULL(ZOTOD.[HST], 0)) > 0 
		 )

		   , CTE_GetTaxDetails AS
			  ( SELECT StoreName,OrderDate,State,TotalSales,Tax,Title,Custom1,Symbol
			  FROM CTE_GetTaxFiltered
			  WHERE 1= 1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
			  )
			  SELECT StoreName,OrderDate,State,TotalSales,Tax,Title,Custom1,Symbol
			  FROM CTE_GetTaxDetails
		 '
				PRINT @SQL
				EXEC(@SQL)

		 END TRY
		 BEGIN CATCH
		 DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetTaxFiltered @BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_GetTaxFiltered',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		 END CATCH
     END;
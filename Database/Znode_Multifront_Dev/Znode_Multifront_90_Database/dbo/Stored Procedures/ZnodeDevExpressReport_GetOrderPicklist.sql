
CREATE PROCEDURE [dbo].[ZnodeDevExpressReport_GetOrderPicklist](
      --@PortalId  VARCHAR(MAX)  = '' ,
      --@Name      NVARCHAR(200) = '' ,
      @BeginDate DATE        ,
      @EndDate   DATE        ,
	  @WhereClause NVARCHAR(max) = '')
AS 
/*
     Summary :- This Procedure is used to find the Order having Submitted Status 
     Unit Testing 
     EXEC ZnodeDevExpressReport_GetOrderPicklist @BeginDate = '2016-12-21 09:53:53.043' ,@EndDate= '2017-11-21 09:53:53.043'
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON; 
			 -- Select Order PickList report data

			 DECLARE @SQL NVARCHAR(MAX)
			
			 SET @SQL ='
			
			 ;WITH CTE_GetPickList AS
			 (
             SELECT O.[OmsOrderId] ,ZO.OrderNumber , O.[BillingFirstName] , O.[BillingLastName] , O.[OrderDate] , P.[StoreName] AS ''StoreName'' , ISNULL(ZC.Symbol , [dbo].[Fn_GetDefaultCurrencySymbol]()) AS Symbol
             FROM ZnodeOmsOrderDetails AS O INNER JOIN ZNodePortal AS P ON P.PortalID = O.PortalId
             LEFT JOIN ZnodeCurrency AS ZC ON ( ZC.CurrencyCode = O.CurrencyCode )
			 INNER JOIN ZnodeOMSOrderState ZOOS ON O.OmsOrderStateId = ZOOS.OmsOrderStateId AND   ZOOS.OrderStateName = ''Submitted''
	         Inner join ZnodeOMSOrder Zo ON O.OMSOrderId = Zo.OMSOrderId  
             WHERE                   
             CAST(O.[OrderDate] AS DATE) BETWEEN CASE
                                                    WHEN '''+CAST(@BeginDate AS VARCHAR(30))+''' IS NULL
                                                    THEN CAST(O.[OrderDate] AS DATE)
                                                    ELSE '''+CAST(@BeginDate AS VARCHAR(30))+'''
                                                 END AND CASE
                                                    WHEN '''+CAST(@EndDate AS VARCHAR(30))+''' IS NULL
                                                    THEN CAST(O.[OrderDate] AS DATE)
                                                    ELSE '''+CAST(@EndDate AS VARCHAR(30))+'''
                                                 END 
												 )
			  ,CTE_GetFilteredPickList AS(
			SELECT OmsOrderId,OrderNumber,BillingFirstName,BillingLastName,OrderDate,StoreName,Symbol
			FROM CTE_GetPickList
			WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+')
			SELECT OmsOrderId,OrderNumber,BillingFirstName,BillingLastName,OrderDate,StoreName,Symbol
			FROM CTE_GetFilteredPickList
			'
			PRINT @SQL
		    EXEC(@SQL)


         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetOrderPicklist @BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'ZnodeReport_GetOrderPicklist',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
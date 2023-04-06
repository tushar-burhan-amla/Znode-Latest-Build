CREATE PROCEDURE [dbo].[ZnodeReport_GetOrderPicklist](  
      @PortalId  VARCHAR(MAX)  = '' ,  
      @Name      NVARCHAR(200) = '' ,  
      @BeginDate DATE          = NULL ,  
      @EndDate   DATE          = NULL)  
AS   
/*  
     Summary :- This Procedure is used to find the Order having Submitted Status   
     Unit Testing   
     EXEC ZnodeReport_GetOrderPicklist @Name = 'Harry Potter123' , @BeginDate = '2016-12-21 09:53:53.043' ,@EndDate= '2017-11-21 09:53:53.043'  
*/  
     BEGIN  
         BEGIN TRY  
             SET NOCOUNT ON;   
  
             -- Select Order PickList report data  
             SELECT O.[OmsOrderId] ,ZO.OrderNumber , O.[BillingFirstName] , O.[BillingLastName] , O.[OrderDate] , P.[StoreName] AS 'StoreName' , ISNULL(ZC.Symbol , [dbo].[Fn_GetDefaultCurrencySymbol]()) AS Symbol  
             FROM ZnodeOmsOrderDetails AS O INNER JOIN ZNodePortal AS P ON P.PortalID = O.PortalId  
                                            --LEFT JOIN ZnodeCurrency AS ZC ON ( ZC.CurrencyCode = O.CurrencyCode )  
											 LEFT JOIN ZnodeCulture AS ZC ON ( ZC.CultureCode = O.CurrencyCode )  
            INNER JOIN ZnodeOMSOrderState ZOOS ON O.OmsOrderStateId = ZOOS.OmsOrderStateId AND   ZOOS.OrderStateName = 'Submitted'  
            Inner join ZnodeOMSOrder Zo ON O.OMSOrderId = Zo.OMSOrderId    
             WHERE   
                   ( ( EXISTS ( SELECT TOP 1 1  
                                FROM dbo.split ( @PortalId , ','  
                                               ) AS SP  
                                WHERE CAST(P.PortalId AS VARCHAR(100)) = SP.Item  
                                      OR  
                                      @PortalId = ''  
                              ) ) )  
                   AND  
                   ( ISNULL(O.[BillingFirstName] , '')+' '+ISNULL(O.[BillingLastName] , '') LIKE '%'+@Name+'%'  
                     OR  
                     @Name = '' )  
                   AND  
                   ( CAST(O.[OrderDate] AS DATE) BETWEEN CASE  
                                                             WHEN @BeginDate IS NULL  
                                                             THEN CAST(O.[OrderDate] AS DATE)  
                                                             ELSE @BeginDate  
                                                         END AND CASE  
                                                                     WHEN @EndDate IS NULL  
                                                                     THEN CAST(O.[OrderDate] AS DATE)  
                                                                     ELSE @EndDate  
                                                                 END );  
         END TRY  
         BEGIN CATCH  
             DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
    @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetOrderPicklist @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Name='+@Name+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'ZnodeReport_GetOrderPicklist',  
    @ErrorInProcedure = @Error_procedure,  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;  
         END CATCH;  
     END;
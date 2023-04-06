CREATE PROCEDURE [dbo].[ZnodeReport_GetPopularProductFiltered](  
      @BeginDate   DATE          = NULL ,  
      @EndDate     DATE          = NULL ,  
      @PortalId    NVARCHAR(MAX) = '' ,  
      @SKU         NVARCHAR(MAX) = '' ,  
      @ProductName NVARCHAR(MAX) = '')  
AS   
/*  
     Sumarry : - This Procedure is used to find the Popular products filtered   
     Unit Testing   
     EXEC ZnodeReport_GetPopularProductFiltered   
 */  
     BEGIN  
         BEGIN TRY  
             SET NOCOUNT ON;  
            
     WITH Cte_ReportData  
                  AS   
       (SELECT ZP.StoreName AS 'StoreName' , ZOOLI.SKU , ZOOLI.ProductName , SUM(ZOOLI.Quantity) AS 'Quantity' , ZOOLI.Price AS 'Price' ,  
       SUM(ZOOLI.Quantity) * ZOOLI.Price AS TotalAmount , ISNULL(ZC.Symbol , dbo.Fn_GetDefaultCurrencySymbol()) AS Symbol  
                      FROM ZnodeOmsOrder AS ZOO INNER JOIN ZnodeOmsOrderDetails AS ZOOD ON ( ZOOD.OmsOrderId = ZOO.OmsOrderId AND ZOOD.IsActive =1 )  
                                                INNER JOIN ZnodeOmsOrderLineItems AS ZOOLI ON Zood.OmsOrderDetailsId = ZOOLI.OmsOrderDetailsId  
                                                INNER JOIN ZnodePortal AS ZP ON Zp.PortalID = ZOOD.PortalId  
                                                --LEFT JOIN ZnodeCurrency AS ZC ON ( ZC.CurrencyCode = ZOOD.CurrencyCode )  
												LEFT JOIN ZnodeCulture AS ZC ON ( ZC.CultureCode = ZOOD.CurrencyCode )  
                      WHERE ( CAST(ZOOD.OrderDate AS DATE) BETWEEN CASE  
                                                                       WHEN @BeginDate IS NULL  
                                                                       THEN CAST(ZOOD.OrderDate AS DATE)  
                                                                       ELSE @BeginDate  
                                                                   END AND CASE  
                                                                               WHEN @EndDate IS NULL  
                                                                               THEN CAST(ZOOD.OrderDate AS DATE)  
                                                                               ELSE @EndDate  
                                                                           END )  
                            AND  
                            ( EXISTS ( SELECT TOP 1 1  
                                       FROM dbo.split ( @PortalId , ','  
                                                      ) AS SP  
                                       WHERE CAST(Zp.PortalID AS VARCHAR(100)) = SP.Item  
                                             OR  
                                             @PortalId = ''  
                                     ) )  
                            AND  
                            ( ZOOLI.SKU LIKE '%'+@SKU+'%'  
                              OR  
                              @SKU = '' )  
                            AND  
                            ( ZOOLI.ProductName LIKE '%'+@ProductName+'%'  
                              OR  
                              @ProductName = '' )  
                      GROUP BY ZP.StoreName , ZOOLI.SKU , ZOOLI.ProductName , ZOOLI.Price , ZC.Symbol  
                      HAVING SUM(ZOOLI.Quantity) > 0)  
  
                  SELECT StoreName , SKU , ProductName , dbo.Fn_GetDefaultInventoryRoundOff ( Quantity ) AS Quantity , DBO.Fn_GetDefaultPriceRoundOff ( Price ) AS Price ,   
      DBO.Fn_GetDefaultPriceRoundOff ( TotalAmount ) AS TotalAmount , Symbol FROM Cte_ReportData ORDER BY Cast( Quantity as Numeric) DESC;  
         END TRY  
  
         BEGIN CATCH  
             DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
    @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetPopularProductFiltered @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@SKU='+@SKU+',@ProductName='+@ProductName+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'ZnodeReport_GetPopularProductFiltered',  
    @ErrorInProcedure = @Error_procedure,  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;  
         END CATCH;  
     END;
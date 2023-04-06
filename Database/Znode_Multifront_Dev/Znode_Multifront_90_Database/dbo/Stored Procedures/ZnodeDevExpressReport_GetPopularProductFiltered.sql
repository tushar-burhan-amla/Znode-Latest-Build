
  
CREATE PROCEDURE [dbo].[ZnodeDevExpressReport_GetPopularProductFiltered](  
      @BeginDate   DATETIME          ,  
      @EndDate     DATETIME          ,  
      @StoreName   NVARCHAR(max) = '',  
   @ShowAllProducts BIT = 1,    
   @TopProducts INT = 10   
   )  
AS   
/*  
     Sumarry : - This Procedure is used to find the Popular products filtered   
     Unit Testing   
     EXEC ZnodeDevExpressReport_GetPopularProductFiltered @BeginDate = '2018/01/01',@EndDate= '2018/12/12'     
 */  
     BEGIN  
         BEGIN TRY  
             SET NOCOUNT ON;  
          DECLARE @SQL NVARCHAR(MAX)  
    DECLARE @PageNo int = 1;   
    DECLARE @TBL_PortalId TABLE (PortalId INT );  
    DECLARE @TBL_ReportPopularProduct TABLE (PortalId INT,StoreName NVARCHAR(MAX),SKU NVARCHAR(MAX),ProductName  NVARCHAR(MAX),QuantityOrdered NUMERIC(28,6)  
    ,UnitOfMeasurement nvarchar(20)  
    )--,  TotalAmtSold NUMERIC(28,6))  
  
    IF OBJECT_ID('TEMPDB..#PopularProduct') IS NOT NULL  
       DROP TABLE #PopularProduct    
  
   INSERT INTO @TBL_PortalId  
   SELECT PortalId   
   FROM ZnodePortal ZP   
   INNER JOIN dbo.split(@StoreName,'|') SP ON (SP.Item = ZP.StoreName)  
  
   SELECT distinct SKU,ProductName,UOM 
     INTO #PopularProduct  
     FROM  
     (  
     SELECT  c.pimproductId,PA.attributecode,e.AttributeValue 
     FROM znodePimProduct c   
      INNER JOIN ZnodePimAttributeValue d on (c.PimProductid = d.PimProductid)  
      INNER JOIN ZnodePimAttributeValueLocale e on (d.PimAttributeValueId = e.PimAttributeValueId)  
      INNER JOIN ZnodePimAttribute PA ON (PA.PimAttributeId = d.PimAttributeId)  
     WHERE  AttributeCode IN ('SKU','ProductName')  
     UNION ALL  
  
     SELECT a.PimProductId, c.AttributeCode,ZPADV.AttributeDefaultValueCode
     FROM ZnodePimProductAttributeDefaultValue ZPPADV  
     INNER JOIN ZnodePimAttributeDefaultValue ZPADV ON (ZPPADV.PimAttributeDefaultValueId = ZPADV.PimAttributeDefaultValueId)  
     INNER JOIN ZnodePimAttributeValue a ON (a.PimAttributeValueId = ZPPADV.PimAttributeValueId )  
     INNER JOIN ZnodePimAttribute c ON ( c.PimAttributeId=a.PimAttributeId  )  
     WHERE AttributeCode IN ('UOM')  
     ) piv PIVOT(MAX(AttributeValue) FOR AttributeCode in ( SKU,ProductName,UOM))AS PVT  
  
  
   INSERT INTO @TBL_ReportPopularProduct  
       SELECT  ZOOD.PortalId,ZP.StoreName AS 'StoreName' , ZOOLI.SKU , ZOOLI.ProductName , SUM(ZOOLI.Quantity) AS 'QuantityOrdered'  
    ,PP.UOM UnitOfMeasurement  
                      FROM ZnodeOmsOrder AS ZOO INNER JOIN ZnodeOmsOrderDetails AS ZOOD ON ( ZOOD.OmsOrderId = ZOO.OmsOrderId )  
                                                INNER JOIN ZnodeOmsOrderLineItems AS ZOOLI ON (Zood.OmsOrderDetailsId = ZOOLI.OmsOrderDetailsId)  
            INNER JOIN #PopularProduct PP ON (PP.SKU = ZOOLI.Sku)  
                                                INNER JOIN ZnodePortal AS ZP ON (Zp.PortalID = ZOOD.PortalId)  
            INNER JOIN ZnodeOmsOrderState ZOOS ON (ZOOD.OmsOrderStateId = ZOOS.OmsOrderStateId)  
                                        
                      WHERE  CAST(ZOOD.OrderDate AS DATETIME) BETWEEN @BeginDate AND @EndDate  
       AND (EXISTS (SELECT TOP 1 1 FROM @TBL_PortalId rt WHERE rt.PortalId = ZOOD.PortalId)  
          OR NOT EXISTS (SELECT TOP 1 1 FROM @TBL_PortalId ))    
       AND ZOOS.OrderStateName NOT IN ('CANCELED','RETURNED','REJECTED','FAILED')  
       AND ZOOLI.OrderLineItemStateId NOT IN (SELECT OmsOrderStateId FROM ZnodeOmsOrderState WHERE OrderStateName  IN ('CANCELED','RETURNED','REJECTED','FAILED'))                       
       AND ParentOmsOrderLineItemsId IS NOT NULL  
                      GROUP BY ZP.StoreName , ZOOLI.SKU , ZOOLI.ProductName , ZOOD.PortalId ,PP.UOM  
                      HAVING SUM(ZOOLI.Quantity) > 0   
        
    SET @TopProducts = CASE WHEN @ShowAllProducts = 1 THEN (SELECT COUNT(1) FROM @TBL_ReportPopularProduct ) ELSE @TopProducts END   
  
  
     Select TOP (@TopProducts)  SKU , ProductName ,StoreName ,QuantityOrdered,UnitOfMeasurement     
     from @TBL_ReportPopularProduct    
     ORder by StoreName ASC, CAST (QuantityOrdered AS Numeric) DESC, ProductName ASC  
  
      
        
        IF OBJECT_ID('TEMPDB..#PopularProduct') IS NOT NULL  
       DROP TABLE #PopularProduct   
  
         END TRY  
  
         BEGIN CATCH  
             DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
    @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetPopularProductFiltered @BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'ZnodeReport_GetPopularProductFiltered',  
    @ErrorInProcedure = @Error_procedure,  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;  
         END CATCH;  
     END;
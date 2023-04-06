

CREATE PROCEDURE [dbo].[ZnodeDevExpressReport_GetAbandonedCart]  
(
 @BeginDate    DATETIME        ,  
 @EndDate      DATETIME       ,  
 @StoreName   NVARCHAR(max) = '',  
 @ShowOnlyRegisteredUsers BIT = 1   
 
 )  
AS   
/*  
     Summary :- This Procedure is used to get frequently Appear users    
     Unit Testing   
     EXEC ZnodeDevExpressReport_GetAbandonedCart @BeginDate = '2019-02-20 17:34:03.953',@EndDate= '2019-02-20 17:34:03.953'  
	a.ModifiedDate,a.CreatedDate,
*/  
     BEGIN  
         BEGIN TRY  
         DECLARE @SQL NVARCHAR(MAX)  
         DECLARE @GetDate DATETIME = dbo.Fn_GetDate()

		IF OBJECT_ID('TEMPDB..#TBL_ReportOrderDetails') IS NOT NULL
		DROP TABLE #TBL_ReportOrderDetails

		IF OBJECT_ID('TEMPDB..#TBL_PortalId') IS NOT NULL
		DROP TABLE #TBL_PortalId

		IF OBJECT_ID('TEMPDB..#TBL_ProductPricing') IS NOT NULL
		DROP TABLE #TBL_ProductPricing


   CREATE TABLE #TBL_ReportOrderDetails  (OmsSavedCartId INT ,Quantity NUMERIC(28,6) 
   , CartCreatedAt datetime,CartUpdatedAt datetime,CustomerName VARCHAR(300),StoreName nvarchar(max),Email  VARCHAR(50),
   CustomerType  VARCHAR(50), SKU nvarchar(4000),PortalId INT
   );  
  
    Create TABLE  #TBL_PortalId  (PortalId INT );  
   INSERT INTO #TBL_PortalId  
   SELECT PortalId   
   FROM ZnodePortal ZP   
   INNER JOIN dbo.split(@StoreName,'|') SP ON (SP.Item = ZP.StoreName)  
  
   INSERT INTO #TBL_ReportOrderDetails  
   select DISTINCT a.OmsSavedCartId,sum(Quantity) as Quantity,MAX(a.CreatedDate) AS CartCreatedAt,  
   MAX(a.ModifiedDate) as CartUpdatedAt, CASE WHEN REPLACE(ISNULL(FirstName,'')+' '+ISNULL(LastName,''),' ','') = ''   
   THEN f.UserName ELSE ISNULL(FirstName,'')+' '+ISNULL(LastName,'') END  CustomerName , p.StoreName ,d.Email AS Email
   ,CASE WHEN  d.AspNetUserId IS NULL THEN 'Guest User' ELSE 'Registered User' END  CustomerType, b.SKU, p.PortalId
   from ZnodeOmsSavedCart a  
   INNER JOIN ZnodeOmsSavedCartLineItem b on a.OmsSavedCartId = b.OmsSavedCartId  
   INNER JOIN ZnodeOmsCookieMapping c on a.OmsCookieMappingId = c.OmsCookieMappingId  
   INNER JOIN ZnodePortal p on c.PortalId = p.PortalId  
   LEFT JOIN ZnodeUser d on c.UserId = d.UserId  
   LEFT JOIN AspNetUsers e on d.AspNetUserId = e.Id  
   LEFT JOIN AspNetZnodeUser f on f.AspNetZnodeUserId = e.UserName  
   WHERE 
    (EXISTS (SELECT TOP 1 1 FROM #TBL_PortalId rt WHERE rt.PortalId = p.PortalId)
				      OR NOT EXISTS (SELECT TOP 1 1 FROM #TBL_PortalId )) 
	AND ((@ShowOnlyRegisteredUsers = 1 and d.AspNetUserId  IS NOT NULL) or (@ShowOnlyRegisteredUsers <> 1 ))
	AND CAST(b.ModifiedDate AS DATETIME) BETWEEN @BeginDate AND @EndDate 
   group by a.OmsSavedCartId,f.UserName, p.StoreName,FirstName, LastName  ,d.Email,d.AspNetUserId,b.SKU,p.PortalId
       
	   

    select DISTINCT a.OmsSavedCartId,sum(Quantity) as Quantity,MAX(b.CreatedDate) AS CartCreatedAt,  
   MAX(b.ModifiedDate) as CartUpdatedAt, CASE WHEN REPLACE(ISNULL(FirstName,'')+' '+ISNULL(LastName,''),' ','') = ''   
   THEN f.UserName ELSE ISNULL(FirstName,'')+' '+ISNULL(LastName,'') END  CustomerName , p.StoreName ,d.Email AS Email
   ,CASE WHEN  d.AspNetUserId IS NULL THEN 'Guest User' ELSE 'Registered User' END  CustomerType, b.SKU, p.PortalId
   INTO #tempabandonedcart 
   from ZnodeOmsSavedCart a  
   INNER JOIN ZnodeOmsSavedCartLineItem b on a.OmsSavedCartId = b.OmsSavedCartId  
   INNER JOIN ZnodeOmsCookieMapping c on a.OmsCookieMappingId = c.OmsCookieMappingId  
   INNER JOIN ZnodePortal p on c.PortalId = p.PortalId  
   LEFT JOIN ZnodeUser d on c.UserId = d.UserId  
   LEFT JOIN AspNetUsers e on d.AspNetUserId = e.Id  
   LEFT JOIN AspNetZnodeUser f on f.AspNetZnodeUserId = e.UserName  
   WHERE 
    (EXISTS (SELECT TOP 1 1 FROM #TBL_PortalId rt WHERE rt.PortalId = p.PortalId)
				      OR NOT EXISTS (SELECT TOP 1 1 FROM #TBL_PortalId )) 
	AND ((@ShowOnlyRegisteredUsers = 1 and d.AspNetUserId  IS NOT NULL) or (@ShowOnlyRegisteredUsers <> 1 ))
	AND CAST(b.ModifiedDate AS DATETIME) BETWEEN @BeginDate AND @EndDate 
	AND b.OrderLineItemRelationshipTypeId NOT IN (SELECT OrderLineItemRelationshipTypeId FROM ZnodeOmsOrderLineItemRelationshipType
    WHERE Name = 'AddOns')
   group by a.OmsSavedCartId,f.UserName, p.StoreName,FirstName, LastName  ,d.Email,d.AspNetUserId,b.SKU,p.PortalId

   
	DECLARE @skuuu SelectColumnList, @PortalId TransferId
  
    INSERT INTO @skuuu
	SELECT SKU FROM #TBL_ReportOrderDetails

	INSERT INTO @PortalId
	SELECT DISTINCT PortalId FROM #TBL_ReportOrderDetails
	
	CREATE TABLE #TBL_ProductPricing  (SKU VARCHAR(1000),RetailPrice VARCHAR(1000) );
	
	 

	INSERT INTO #TBL_ProductPricing (SKU,RetailPrice)
	EXEC Znode_GetAbandonedProductPricingBySku  @skuuu, @PortalId, @GetDate
	 

   SELECT  DISTINCT A.OmsSavedCartId,MAx(A.CartCreatedAt) CartCreatedAt,MAX(A.CartUpdatedAt) CartUpdatedAt,A.CustomerName,A.StoreName,A.Email ,
   A.CustomerType , SUM(B.RetailPrice * A.Quantity) AS SubTotal 
   INTO #tempcarttt
   FROM #TBL_ReportOrderDetails  A
   INNER JOIN #TBL_ProductPricing B ON (A.SKU = B.SKU) -- addon
   GROUP BY A.OmsSavedCartId,A.CustomerName,A.StoreName,A.Email ,
   A.CustomerType
   ORDER BY A.StoreName DESC,CartUpdatedAt DESC,CustomerName DESC  
       
	  select c.OmsSavedCartId,a.CartCreatedAt, a.CartUpdatedAt,a.CustomerName,a.StoreName,a.Email
	  ,a.CustomerType,a.SubTotal, SUM(c.Quantity) Quantity ,COUNT(*) Products
	  from  #tempcarttt a
	  inner join #tempabandonedcart C on (a.OmsSavedCartId = c.OmsSavedCartId)
	  GROUP BY C.OmsSavedCartId ,a.CartCreatedAt, a.CartUpdatedAt,a.CustomerName,a.StoreName,a.Email
	  ,a.CustomerType,a.SubTotal

			IF OBJECT_ID('TEMPDB..#TBL_ReportOrderDetails') IS NOT NULL
			DROP TABLE #TBL_ReportOrderDetails

			IF OBJECT_ID('TEMPDB..#TBL_PortalId') IS NOT NULL
			DROP TABLE #TBL_PortalId

			IF OBJECT_ID('TEMPDB..#TBL_ProductPricing') IS NOT NULL
			DROP TABLE #TBL_ProductPricing
	                
         END TRY  
         BEGIN CATCH  
		
             DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
    @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeDevExpressReport_GetAbandonedCart @BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'ZnodeDevExpressReport_GetAbandonedCart',  
    @ErrorInProcedure = @Error_procedure,  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;  
         END CATCH;  
     END;

	
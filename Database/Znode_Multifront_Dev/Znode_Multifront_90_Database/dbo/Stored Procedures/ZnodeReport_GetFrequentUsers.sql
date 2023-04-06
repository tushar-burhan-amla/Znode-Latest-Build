CREATE PROCEDURE [dbo].[ZnodeReport_GetFrequentUsers]  
(   @BeginDate   DATE         = NULL,  
 @EndDate     DATE         = NULL,  
 @PortalId    VARCHAR(MAX) = '',  
 @UserId      VARCHAR(MAX) = '',  
 @FirstName   VARCHAR(MAX) = '',  
 @LastName    VARCHAR(MAX) = '')  
AS   
/*  
     Summary :- This Procedure is used to get frequently Appear users    
     Unit Testing   
     EXEC ZnodeReport_GetFrequentUsers   
*/  
     BEGIN  
         BEGIN TRY  
          
             DECLARE @TBL_ReportOrderUser TABLE (OmsOrderId INT, UserId     INT, PortalId   INT);  
     DECLARE @TBL_ReportOrderDetails TABLE (UserId INT, BillingFirstName nvarchar(200),BillingLastName nvarchar (200),  
                                             StoreName nvarchar(max), OrderCount NVarchar(100), Total NVarchar(100),Symbol NVARCHAR(100) );  
             INSERT INTO @TBL_ReportOrderUser (OmsOrderId, UserId,PortalId)  
                      
    SELECT MAX(ZOOD.OmsOrderId) OmsOrderId,  
                           UserId,  
                           PortalId  
                    FROM ZnodeOmsOrderDetails ZOOD  
                    WHERE((EXISTS  
                          (  
                              SELECT TOP 1 1  
                              FROM dbo.split(@PortalId, ',') SP  
                              WHERE CAST(ZOOD.PortalId AS VARCHAR(100)) = SP.Item  
                                    OR @PortalId = ''  
                          )))  
                    GROUP BY UserId,  
                             PortalId;  
  
  
             WITH Cte_ReportUserDetails  
                  AS (SELECT ZOOD.UserId,  
                             ZOOD.BillingFirstName,  
                             ZOOD.BillingLastName,  
                            
                             ZOOD.PortalId,  
                             ZP.StoreName  
                      FROM @TBL_ReportOrderUser O  
                           INNER JOIN ZnodeOmsOrderDetails ZOOD ON(ZOOD.OmsOrderId = O.OmsOrderId)  
                           INNER JOIN ZnodePortal ZP ON(Zp.PortalId = ZOOD.PortalId)  
                      WHERE((EXISTS  
                            (  
                                SELECT TOP 1 1  
                                FROM dbo.split(@PortalId, ',') SP  
                                WHERE CAST(ZOOD.PortalId AS VARCHAR(100)) = SP.Item  
                                      OR @PortalId = ''  
                            ))))  
  
                  Insert into @TBL_ReportOrderDetails  (UserID , BillingFirstName ,BillingLastName , StoreName , OrderCount , Total,Symbol  )  
                  SELECT CTRO.UserId,  
                         CTRO.BillingFirstName,  
                         CTRO.BillingLastName,  
                         StoreName,  
                         COUNT(ZOOD.OmsOrderId) AS 'OrderCount',  
                         Dbo.Fn_GetDefaultPriceRoundOff(SUM(ZOOD.Total))AS 'Total',   
     ISNULL(ZC.Symbol,dbo.Fn_GetDefaultCurrencySymbol()) Symbol  
                  FROM Cte_ReportUserDetails CTRO  
                       INNER JOIN ZnodeOmsOrderDetails ZOOD ON(CTRO.UserId = ZOOD.UserId)  
                       --LEFT JOIN ZnodeCurrency ZC ON (ZC.CurrencyCode = ZOOD.CurrencyCode)  
					   LEFT JOIN ZnodeCulture ZC ON (ZC.CultureCode = ZOOD.CurrencyCode)  
                  WHERE(CAST(ZOOD.OrderDate AS DATE) BETWEEN CASE  
                                                                 WHEN @BeginDate IS NULL  
                                                                 THEN CAST(ZOOD.OrderDate AS DATE)  
                                                                 ELSE @BeginDate  
                                                             END AND   
             CASE  
                                                                         WHEN @EndDate IS NULL  
                                                                         THEN CAST(ZOOD.OrderDate AS DATE)  
                                                                         ELSE @EndDate  
                                                             END)  
                       AND ((EXISTS  
                            (  
                                SELECT TOP 1 1  
                                FROM dbo.split(@PortalId, ',') SP  
                                WHERE CAST(CTRO.PortalId AS VARCHAR(100)) = SP.Item  
                                      OR @PortalId = ''  
                            )))  
                       AND (CTRO.BillingFirstName LIKE '%'+@FirstName+'%'  
                            OR @FirstName = '')  
                       AND (CTRO.BillingLastName LIKE '%'+@LastName+'%'  
                            OR @LastName = '')  
  
                       AND ((EXISTS  
                            (  
                                SELECT TOP 1 1  
                                FROM dbo.split(@UserId, ',') SP  
                                WHERE CAST(CTRO.UserId AS VARCHAR(100)) = ltrim(rtrim(SP.Item))  
                                      OR @UserId = ''  
                            )))  
                  GROUP BY CTRO.UserId,  
                           CTRO.BillingFirstName,  
                           CTRO.BillingLastName,  
                           StoreName,ZC.Symbol  
       
       Select * from @TBL_ReportOrderDetails ORder by CAST (Total AS Numeric) DESC;  
                   
         END TRY  
         BEGIN CATCH  
             DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
    @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetFrequentUsers @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@UserId='+@UserId+',@FirstName='+@FirstName+',@LastName='+@LastName+',@S
tatus='+CAST(@Status AS VARCHAR(10));  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'ZnodeReport_GetFrequentUsers',  
    @ErrorInProcedure = @Error_procedure,  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;  
         END CATCH;  
     END;
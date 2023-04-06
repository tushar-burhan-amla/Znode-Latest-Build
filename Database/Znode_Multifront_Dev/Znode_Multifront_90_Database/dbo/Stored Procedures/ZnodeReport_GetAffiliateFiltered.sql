CREATE PROCEDURE [dbo].[ZnodeReport_GetAffiliateFiltered]  
(   @BeginDate    DATE          = NULL,  
    @EndDate      DATE          = NULL,  
    @PortalId     VARCHAR(MAX)  = '',  
    @CustomerName NVARCHAR(400) = '')  
AS   
/*  
     Summary:- this procedure is used to get affiliated filtered ( ReferralCommission )  
     Unit Testing  
     EXEC ZnodeReport_GetAffiliateFiltered @CustomerName = ''   
 */  
     BEGIN  
  BEGIN TRY  
         SET NOCOUNT ON;   
    
         SELECT(zu.FirstName+' '+zu.LastName) AS 'Name',  
                
               zp.StoreName AS 'StoreName',  
               COUNT(zoods.UserId) AS 'NumberOfOrders',  
               [dbo].[Fn_GetDefaultPriceRoundOff](SUM(zoods.Total)) AS 'OrderValue',  
               zrct.[Name] AS 'CommissionType',  
               [dbo].[Fn_GetDefaultPriceRoundOff](zorc.OrderCommission) AS 'Commission',  
               [dbo].[Fn_GetDefaultPriceRoundOff]  
         (CASE  
              WHEN zrct.ReferralCommissionTypeID = 1  
              THEN --SUM(O.Total) * RC.ReferralCommission / 100  
          (SUM(ISNULL(zoods.SubTotal, 0)) - (SUM(ISNULL(zoods.DiscountAmount, 0)) + SUM(ISNULL(zgch.TransactionAmount, 0))) * ISNULL(zorc.OrderCommission, 0)) / 100  
              WHEN zrct.ReferralCommissionTypeID = 2  
              THEN ISNULL(zorc.OrderCommission, 0)  
          END  
         ) AS 'CommissionOwned',  
               zrct.ReferralCommissionTypeID AS ReferralCommissionTypeID  
      ,ISNULL(ZC.Symbol,[dbo].[Fn_GetDefaultCurrencySymbol]())Symbol  
         FROM ZnodeUser AS zu  
              INNER JOIN ZnodeOmsOrderDetails AS zood ON zu.UserId = zood.ReferralUserId  
              INNER JOIN ZnodeOmsReferralCommission AS zorc ON zorc.UserId = zu.userId  
              INNER JOIN ZnodeReferralCommissionType AS zrct ON zu.ReferralCommissionTypeId = zrct.ReferralCommissionTypeId  
              INNER JOIN ZnodeOmsOrderDetails AS zoods ON zood.OmsOrderDetailsId = zoods.OmsOrderDetailsId  
              INNER JOIN ZnodePortal AS zp ON zood.Portalid = zp.Portalid  
              LEFT OUTER JOIN ZnodeUserAddress AS zud ON zud.UserId = zu.UserId  
              LEFT JOIN ZnodeAddress AS za ON zud.AddressId = za.AddressId  
                                               AND za.IsDefaultBilling = 1  
              LEFT OUTER JOIN ZnodeGiftCardHistory AS zgch ON zoods.OmsOrderDetailsId = zgch.OmsOrderDetailsId  
     --LEFT JOIN ZnodeCurrency ZC ON (ZC.CurrencyCode =  ZOOD.CurrencyCode)  
	 LEFT JOIN ZnodeCulture ZC ON (ZC.CultureCode =  ZOOD.CurrencyCode)  
         WHERE zu.ReferralStatus = 'A'  
               AND zoods.OmsOrderDetailsId = zorc.OmsOrderDetailsId  
               AND (ISNULL(zu.FirstName, '')+' '+ISNULL(zu.LastName, '') LIKE '%'+@CustomerName+'%'  
                    OR @CustomerName = '')  
               AND (CAST(ZOOD.OrderDate AS DATE) BETWEEN CASE  
                                                             WHEN @BeginDate IS NULL  
                                                             THEN CAST(ZOOD.OrderDate AS DATE)  
                                                             ELSE @BeginDate  
                                                         END AND CASE  
                                                                     WHEN @EndDate IS NULL  
                                                                     THEN CAST(ZOOD.OrderDate AS DATE)  
                                                                     ELSE @EndDate  
                                                                 END)  
               AND ((EXISTS  
                    (  
                        SELECT TOP 1 1  
                        FROM dbo.split(@PortalId, ',') AS SP  
                        WHERE CAST(zp.PortalId AS VARCHAR(100)) = SP.Item  
                              OR @PortalId = ''  
                    )))  
         GROUP BY zp.StoreName,  
                  zu.FirstName,  
                  zu.LastName,  
                  zrct.[Name],  
                  zorc.OrderCommission,  
                  zrct.ReferralCommissionTypeId,ZC.Symbol;  
  END TRY  
  BEGIN CATCH  
  DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),  
    @ErrorCall NVARCHAR(MAX)= 'EXEC ZnodeReport_GetAffiliateFiltered @PortalId = '+@PortalId+',@BeginDate='+CAST(@BeginDate AS VARCHAR(200))+',@EndDate='+CAST(@EndDate AS VARCHAR(200))+',@CustomerName='+@CustomerName+',@Status='+CAST(@Status AS VARCHAR(10
));  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
      
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'ZnodeReport_GetAffiliateFiltered',  
    @ErrorInProcedure = @Error_procedure,  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;  
  END CATCH  
     END;
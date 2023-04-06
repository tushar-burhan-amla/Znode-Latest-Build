CREATE PROCEDURE [dbo].[Znode_GetStoresWithCurrency]  
(  
 @UserId int =0  
)  
AS  
/*  
EXEC Znode_GetStoresWithCurrency @UserId = 7  
  
*/  
BEGIN   
 SELECT DISTINCT ZP.PortalId,ZP.StoreName, ZP.StoreName + ' ('+ZCL.Symbol+')' AS StoreNameWithCurrencySymbol   
   
  FROM    
  ZnodePortal ZP   
  INNER JOIN ZnodePortalunit ZPU on   (ZPU.PortalId =ZP.PortalId)  
  INNER JOIN ZnodeCulture ZCL on   (ZCL.CultureId =ZPU.CultureId)  
  INNER JOIN ZnodeCurrency ZC on   (ZCL.CurrencyId =ZC.CurrencyId)  
  WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeUserPortal ZUP WHERE (ZUP.PortalId=ZP.PortalId OR ZUP.PortalId IS NULL)   AND ZUP.UserId=@UserId )  
  
END
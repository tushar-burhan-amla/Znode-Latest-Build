
CREATE FUNCTION [dbo].[Fn_GetPortalCurrencySymbol]  
( 
@PortalId INTEGER  
)  
RETURNS NVARCHAR(100)  
AS  
     BEGIN  
         -- Declare the return variable here  
         DECLARE @Symbol NVARCHAR(100);  
         SET @Symbol =  
         (  
             SELECT TOP 1 ZCC.Symbol  
             FROM ZnodePortalUnit ZPU  
			 LEFT JOIN ZnodeCulture ZCC ON (ZCC.CurrencyId = ZPU.CurrencyId)  
             WHERE ZPU.PortalId = @PortalId 
         );  
         RETURN @Symbol;  
     END;
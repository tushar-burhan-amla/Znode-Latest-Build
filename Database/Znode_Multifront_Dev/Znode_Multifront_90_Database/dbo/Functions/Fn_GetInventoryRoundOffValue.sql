-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================


CREATE FUNCTION [dbo].[Fn_GetInventoryRoundOffValue]()
RETURNS INT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @InventoryRoundOffValue INT;
         SELECT @InventoryRoundOffValue= FeatureValues FROM ZnodeGlobalSetting WHERE FeatureName = 'InventoryRoundOff';
	    RETURN @InventoryRoundOffValue;
     END;
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================


CREATE FUNCTION [dbo].[Fn_GetDefaultLocaleId]()
RETURNS INT
AS
     BEGIN
         -- Declare the return variable here
         DECLARE @DefaultLocaleId INT;
         
		 SET @DefaultLocaleId = (SELECT TOP 1 FeatureValues FROM ZnodeGlobalSetting  ZGS WHERE ZGS.FeatureName = 'Locale' )

                   
         RETURN @DefaultLocaleId;
     END;
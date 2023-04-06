-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
-- SELECT [dbo].[FN_GetPimAttributeDefaultValues] ('199,200')

CREATE FUNCTION [dbo].[FN_GetPimAttributeDefaultValues]
(
@PimAttributeDefaultValueId Varchar(1000) 
,@LocaleId INT = 0 
)
RETURNS VARCHAr(4000)
AS
BEGIN
	-- Declare the return variable here
	-- SELECT * FROM ZnodePimAttributeDefaultValue 
	-- SELECT * FROM ZNodePimAttributeDefaultValueLocale
	IF @LocaleId = 0 
	BEGIN
	SET  @LocaleId = (SELECT FeatureValues FROM ZnodeGlobalSetting WHERE FeatureName = 'Locale' )
	END 

     DECLARE @V_PimAttributeDefaultValueId VARCHAr(4000)
	
	
	SET  @V_PimAttributeDefaultValueId = SUBSTRING ((SELECT ','+AttributeDefaultValue  FROM ZNodePimAttributeDefaultValueLocale q WHERE  EXISTS (SELECT TOP 1 1 FROM dbo.Split(@PimAttributeDefaultValueId,',') a WHERE q.PimAttributeDefaultValueId = a.item) AND LocaleId=@LocaleId FOR XML PATH ('') ) ,2,4000)     
	
	RETURN @V_PimAttributeDefaultValueId

END
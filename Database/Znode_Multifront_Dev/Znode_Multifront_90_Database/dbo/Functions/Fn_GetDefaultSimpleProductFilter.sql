-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================


CREATE FUNCTION [dbo].[Fn_GetDefaultSimpleProductFilter]
(
                @LocaleId INT 
			   
)
RETURNS NVARCHAR(MAX)
AS

	-- Summary :- This function isused to find the default value filtre 
	-- Unit Testing 
	-- EXEC [dbo].[Fn_GetDefaultFilter]

     BEGIN
         -- Declare the return variable here
         DECLARE @DefaultValue NVARCHAR(MAX)= '';
		 DECLARE @DefaultLocaleId INT = Dbo.Fn_GetDefaultValue ('Locale')
		      
		 ;With Cte_DefaultProductType AS 
		  (
		  SELECT ZPA.PimAttributeId,   ZPA.AttributeCode+'='''+ZPADVL.AttributeDefaultValue+'' AttributeValue ,ZPADVL.LocaleId
			 FROM ZnodePImAttributeDefaultValueLocale ZPADVL 
			 INNER JOIN ZnodePimAttributeDefaultvalue ZPADV ON (ZPADV.PimAttributeDefaultvalueId = ZPADVL.PimAttributeDefaultvalueId) 
			 INNER JOIN ZnodePimAttribute ZPA ON (ZPADV.PimAttributeId = ZPA.PimAttributeId)
			 WHERE ZPA.AttributeCode = 'ProductType'
			 AND AttributeDefaultValueCode = 'SimpleProduct'
			 AND LocaleId IN (@DefaultLocaleId,@LocaleId)
		  )
		  , Cte_GetAttributeValue AS
		  (
		    SELECT PimAttributeId,AttributeValue
			FROM Cte_DefaultProductType CTDPT 
			WHERE CTDPT.LocaleId = @LocaleId
		  )
		  ,Cte_AttributeIds AS 
		  (
		     
			 SELECT * 
			 FROM Cte_GetAttributeValue
			 UNION ALL 
			 SELECT PimAttributeId,AttributeValue
			 FROM Cte_DefaultProductType CTDPT 
			 WHERE CTDPT.LocaleId = @DefaultLocaleId
		     AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_GetAttributeValue CTGAV WHERE CTGAV.PimAttributeId = CTDPT.PimAttributeId )
		  )

		 SELECT TOP 1 @DefaultValue = AttributeValue FROM Cte_AttributeIds


                   
         RETURN @DefaultValue;
     END;
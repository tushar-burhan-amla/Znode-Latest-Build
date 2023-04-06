
CREATE PROCEDURE [dbo].[Znode_GetDefaultSimpleProductFilter]
( @LocaleId INT )

AS
/*
     Summary :- This function is used to find the default value filter 
     Unit Testing 
	 begin tran
     EXEC Znode_GetDefaultSimpleProductFilter 1
	 rollback tran
*/
     BEGIN
	   SET NOCOUNT ON;
	     BEGIN TRY
         -- Declare the return variable here
         DECLARE @DefaultValue NVARCHAR(MAX)= '';
         DECLARE @DefaultLocaleId INT= Dbo.Fn_GetDefaultValue('Locale');

         WITH Cte_DefaultProductType
              AS (SELECT ZPA.PimAttributeId,ZPADV.AttributeDefaultValueCode AttributeValue,ZPADVL.LocaleId
                  FROM ZnodePImAttributeDefaultValueLocale ZPADVL
                  INNER JOIN ZnodePimAttributeDefaultvalue ZPADV ON(ZPADV.PimAttributeDefaultvalueId = ZPADVL.PimAttributeDefaultvalueId)
                  INNER JOIN ZnodePimAttribute ZPA ON(ZPADV.PimAttributeId = ZPA.PimAttributeId)
                  WHERE ZPA.AttributeCode = 'ProductType'
                        AND AttributeDefaultValueCode = 'SimpleProduct'
                        AND LocaleId IN(@DefaultLocaleId, @LocaleId)),

              Cte_GetAttributeValue
              AS (SELECT PimAttributeId,AttributeValue                      
                  FROM Cte_DefaultProductType CTDPT
                  WHERE CTDPT.LocaleId = @LocaleId),

              Cte_AttributeIds
              AS (
              SELECT *
              FROM Cte_GetAttributeValue
              UNION ALL
              SELECT PimAttributeId, AttributeValue                   
              FROM Cte_DefaultProductType CTDPT
              WHERE CTDPT.LocaleId = @DefaultLocaleId
                    AND NOT EXISTS
              (
                  SELECT TOP 1 1
                  FROM Cte_GetAttributeValue CTGAV
                  WHERE CTGAV.PimAttributeId = CTDPT.PimAttributeId
              ))
              SELECT  TOP 1 AttributeValue
              FROM Cte_AttributeIds;
		END TRY
		BEGIN CATCH
			  DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetDefaultSimpleProductFilter @LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetDefaultSimpleProductFilter',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		END CATCH
     END;
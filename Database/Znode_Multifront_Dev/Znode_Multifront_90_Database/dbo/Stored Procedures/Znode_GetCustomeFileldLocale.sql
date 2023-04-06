CREATE PROCEDURE [dbo].[Znode_GetCustomeFileldLocale]
( @PimProductId VARCHAR(MAX),
  @LocaleId     INT)
AS 

/*
     Summary:- This Procedure is used to find the customized fields of products 
     Unit TEsting 
     EXEC Znode_GetCustomeFileldLocale 87,1
*/	 
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();
             DECLARE @TBL_PimProductIds TABLE(PimProductId INT);
             INSERT INTO @TBL_PimProductIds(PimProductId)
                    SELECT item
                    FROM dbo.split(@PimProductId, ',');

			 -- this flag is used to get personalies attribute
             WITH Cte_CustomField
                  AS (SELECT ZPCF.PimProductId,ZPCF.CustomCode,ZPCFL.CustomKey,CustomKeyValue,ZPCFL.LocaleId   
                      FROM ZnodePimCustomField AS ZPCF
                      INNER JOIN ZnodePimCustomFieldLocale AS ZPCFL ON(ZPCFL.PimCustomFieldId = ZPCF.PimCustomFieldId
                                                                    AND ZPCFL.LocaleId IN(@DefaultLocaleId, @LocaleId))  
                      -- this check only those product present in PimCustomField table
					  WHERE EXISTS
                      (
                          SELECT TOP 1 1
                          FROM @TBL_PimProductIds AS TBP
                          WHERE ZPCF.PimProductId = TBP.PimProductId
                      ) 
             ),
                  Cte_CustomFieldFirstLocale
                  AS (SELECT PimProductId,CustomCode,CustomKey,CustomKeyValue
                      FROM Cte_CustomField
                      WHERE localeId = @LocaleId),

                  Cte_CustomFieldSecondLocale
                  AS (
                  SELECT PimProductId,CustomCode,CustomKey,CustomKeyValue
                  FROM Cte_CustomFieldFirstLocale
                  UNION ALL
                  SELECT PimProductId,CustomCode,CustomKey,CustomKeyValue
                  FROM Cte_CustomField CTC
                  WHERE NOT EXISTS
                  (
                      SELECT TOP 1 1
                      FROM Cte_CustomFieldFirstLocale CTFL
                      WHERE CTFL.PimProductId = CTC.PimProductId
                            AND CTFL.CustomCode = CTC.CustomCode
                  )
                        AND LocaleId = @DefaultLocaleId)

                  SELECT PimProductId,CustomCode,CustomKey,CustomKeyValue
                  FROM Cte_CustomFieldSecondLocale;
         END TRY
         BEGIN CATCH
		  DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCustomeFileldLocale @PimProductId = '+@PimProductId+',@LocaleId='+CAST(@LocaleId AS VARCHAR(10))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		    
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCustomeFileldLocale',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
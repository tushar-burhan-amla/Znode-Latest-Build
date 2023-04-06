CREATE PROCEDURE [dbo].[Znode_GetCategoryAttributeValueId]
( @PimCategoryId  TRANSFERID READONLY  ,
  @PimAttributeId VARCHAR(MAX),
  @LocaleId       INT)
AS 
/*
     Summary:- This Procedure is used to get the category attribute values 
     Unit Testing 
     EXEC Znode_GetCategoryAttributeValue 1,7,1	 
  */
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();
             DECLARE @TBL_AttributeCode TABLE
             (PimAttributeId INT,
              AttributeCode  VARCHAR(300)
             );
             INSERT INTO @TBL_AttributeCode(PimAttributeId,AttributeCode)
                    SELECT PimAttributeId, AttributeCode         
                    FROM ZnodePimAttribute ZPA
                    WHERE EXISTS
                    (
                        SELECT TOP 1 1
                        FROM dbo.split(@PimAttributeId, ',') SP
                        WHERE Sp.Item = ZPA.PimAttributeId
                    );
             DECLARE @TBL_AttributeValue TABLE
             (PimCategoryAttributeValueId INT,
              PimCategoryId               INT,
              CategoryValue               NVARCHAR(MAX),
              AttributeCode               VARCHAR(300),
              PimAttributeId              INT
             );
             WITH Cte_AttributeValueLocale
                  AS (SELECT ZPCAV.PimCategoryAttributeValueId,PimCategoryId,ZPCAVL.CategoryValue,AttributeCode,ZPA.PimAttributeId,ZPCAVL.LocaleId
                      FROM ZnodePimCategoryAttributeValue ZPCAV
                      INNER JOIN ZnodePimCategoryAttributeValueLocale ZPCAVL ON(ZPCAVL.PimCategoryAttributeValueId = ZPCAV.PimCategoryAttributeValueId)
                      INNER JOIN ZnodePimAttribute ZPA ON(ZPA.PimAttributeId = ZPCAV.PimAttributeId)
                      WHERE EXISTS
                      (
                          SELECT TOP 1 1
                          FROM @PimCategoryId SP
                          WHERE ZPCAV.PimCategoryId = SP.ID 
                      )
                           AND EXISTS
                      (
                          SELECT TOP 1 1
                          FROM @TBL_AttributeCode TBAC
                          WHERE ZPA.AttributeCode = TBAC.AttributeCode
                      )
                           AND ZPCAVL.LocaleId IN(@DefaultLocaleId, @LocaleId)),

                  Cte_AttributeValueFirstLocale
                  AS (SELECT PimCategoryAttributeValueId,PimCategoryId,CategoryValue,AttributeCode,PimAttributeId
                      FROM Cte_AttributeValueLocale CTAVL
                      WHERE LocaleId = @LocaleId),

                  Cte_AttributeValueSecondLocale
                  AS (
                  SELECT PimCategoryAttributeValueId,PimCategoryId,CategoryValue,AttributeCode,PimAttributeId
                  FROM Cte_AttributeValueFirstLocale
                  UNION ALL
                  SELECT PimCategoryAttributeValueId,PimCategoryId,CategoryValue,AttributeCode,PimAttributeId
                  FROM Cte_AttributeValueLocale CTAVL
                  WHERE LocaleId = @DefaultLocaleId
                        AND NOT EXISTS
                  (
                      SELECT TOP 1 1
                      FROM Cte_AttributeValueFirstLocale CTAVFL
                      WHERE CTAVFL.PimCategoryId = CTAVL.PimCategoryId
                            AND CTAVFL.AttributeCode = CTAVL.AttributeCode
                  )),

                  Cte_AddAttributeDefaultValue
                  AS (SELECT PimCategoryAttributeValueId,PimCategoryId,CategoryValue,AttributeCode,PimAttributeId
                      FROM Cte_AttributeValueSecondLocale CTAVS)

                  INSERT INTO @TBL_AttributeValue(PimCategoryAttributeValueId,PimCategoryId,CategoryValue,AttributeCode,PimAttributeId)
                         SELECT PimCategoryAttributeValueId,PimCategoryId,CategoryValue,AttributeCode,PimAttributeId
                         FROM Cte_AddAttributeDefaultValue;

              SELECT *  FROM @TBL_AttributeValue;
            
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCategoryAttributeValue @PimCategoryId = ,@PimAttributeId='+@PimAttributeId+',@LocaleId='+CAST(@LocaleId AS VARCHAR(10))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		 
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCategoryAttributeValue',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
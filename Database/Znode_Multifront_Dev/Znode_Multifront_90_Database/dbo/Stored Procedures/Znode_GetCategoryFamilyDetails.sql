CREATE PROCEDURE [dbo].[Znode_GetCategoryFamilyDetails]
( @PimCategoryId VARCHAR(2000) = '',
  @LocaleId      INT           = 1)
AS 
  /*
     Summary :- This Procedure is used to find the family of category 
     Unit Testing 
     EXEC Znode_GetCategoryFamilyDetails 1,1
	*/
	 BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @PimCategoryDefaultId INT= [dbo].[Fn_GetCategoryDefaultFamilyId](), @DefaultLocaleId INT= [dbo].[Fn_GetDefaultLocaleId]();
             DECLARE @TBL_PimCategoryId TABLE(PimCategoryId INT);

             INSERT INTO @TBL_PimCategoryId(PimCategoryId)
                    SELECT Item
                    FROM dbo.split(@PimCategoryId, ',');

             WITH Cte_PimFamilyBothLocale
                  AS (SELECT ZPAF.PimAttributeFamilyId,ZPFL.AttributeFamilyName,ZPFL.LocaleId,ZPCAV.PimCategoryId
                      FROM ZnodePimAttributeFamily ZPAF
                      LEFT JOIN ZnodePimFamilyLocale ZPFL ON(ZPFL.PimAttributeFamilyId = ZPAF.PimAttributeFamilyId)
                      INNER JOIN ZnodePimCategory ZPCAV ON(ZPCAV.PimAttributeFamilyId = ZPAF.PimAttributeFamilyId)
                      WHERE ZPFL.LocaleId IN(@DefaultLocaleId, @LocaleId)
                           AND CASE
                                   WHEN ZPCAV.PimAttributeFamilyId IS NULL
                                   THEN @PimCategoryDefaultId
                                   ELSE ZPCAV.PimAttributeFamilyId
                               END = ZPAF.PimAttributeFamilyId
                           AND EXISTS
                      (
                          SELECT TOP 1 1
                          FROM @TBL_PimCategoryId TBPC
                          WHERE TBPC.PimCategoryId = ZPCAV.PimCategoryId
                      )),

                  Cte_PimFamilyFirstLocale
                  AS (SELECT PimAttributeFamilyId,AttributeFamilyName,PimCategoryId
                      FROM Cte_PimFamilyBothLocale CTPFBL
                      WHERE LocaleId = @LocaleId),

                  Cte_PimFamilyDefaultLocale
                  AS (SELECT PimAttributeFamilyId,AttributeFamilyName,PimCategoryId
                      FROM Cte_PimFamilyFirstLocale CTPFFL
                      UNION ALL
                      SELECT PimAttributeFamilyId,AttributeFamilyName,PimCategoryId
                      FROM Cte_PimFamilyBothLocale CTPBL
                      WHERE LocaleId = @DefaultLocaleId
                        AND NOT EXISTS
                  (
                      SELECT TOP 1 1
                      FROM Cte_PimFamilyFirstLocale CTPFFL
                      WHERE CTPFFL.PimCategoryId = CTPBL.PimCategoryId
                            AND CTPFFL.PimAttributeFamilyId = CTPBL.PimAttributeFamilyId
                  ))
                  SELECT PimAttributeFamilyId,AttributeFamilyName,PimCategoryId
                  FROM Cte_PimFamilyDefaultLocale
                  GROUP BY PimAttributeFamilyId,AttributeFamilyName,PimCategoryId;
         END TRY
         BEGIN CATCH
            DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCategoryFamilyDetails @PimCategoryId = '+@PimCategoryId+',@LocaleId='+CAST(@LocaleId AS VARCHAR(10))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCategoryFamilyDetails',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
CREATE PROCEDURE [dbo].[Znode_GetFamilyValueLocale]
( @PimAttributeFamilyId VARCHAR(MAX) = '',
  @LocaleId             INT)
AS 
/*
     Summary :- This Procedure is used to get the family locale wise 
     Unit Testing
     EXEC Znode_GetFamilyValueLocale @LocaleId= 1

 */ 
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DefaultLocaleId INT= Dbo.Fn_GetDefaultLocaleId();
             WITH Cte_AttributeFamilyLocale
                  AS (SELECT ZPAF.PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,IsCategory,ZPFL.AttributeFamilyName,ZPFL.LocaleId
                      FROM ZnodePimAttributeFamily ZPAF
                           INNER JOIN ZnodePimFamilyLocale ZPFL ON(ZPFL.PimAttributeFamilyId = ZPAF.PimAttributeFamilyId)
                      WHERE LocaleId IN(@LocaleId, @DefaultLocaleId)
                           AND EXISTS
                      (
                          SELECT TOP 1 1
                          FROM dbo.split(@PimAttributeFamilyId, ',') SP
                          WHERE sp.Item = CAST(ZPAF.PimAttributeFamilyId AS VARCHAR(50))
                                OR @PimAttributeFamilyId = ''
                      )),

                  Cte_AttributeFirstLocale
                  AS (SELECT PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,IsCategory,AttributeFamilyName
                      FROM Cte_AttributeFamilyLocale
                      WHERE LocaleId = @LocaleId),

                  Cte_AttributeBothLocale
                  AS (
                  SELECT PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,IsCategory,AttributeFamilyName
                  FROM Cte_AttributeFirstLocale
                  UNION ALL
                  SELECT PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,IsCategory,AttributeFamilyName
                  FROM Cte_AttributeFamilyLocale CTAFL
                  WHERE LocaleId = @DefaultLocaleId
                        AND NOT EXISTS
                  (
                      SELECT TOP 1 1
                      FROM Cte_AttributeFirstLocale CTAFL
                      WHERE CTAFL.PimAttributeFamilyId = CTAFL.PimAttributeFamilyId
                  ))
                  SELECT PimAttributeFamilyId,FamilyCode,IsSystemDefined,IsDefaultFamily,IsCategory,AttributeFamilyName
                  FROM Cte_AttributeBothLocale;
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetFamilyValueLocale @PimAttributeFamilyId = '+@PimAttributeFamilyId+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetFamilyValueLocale',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
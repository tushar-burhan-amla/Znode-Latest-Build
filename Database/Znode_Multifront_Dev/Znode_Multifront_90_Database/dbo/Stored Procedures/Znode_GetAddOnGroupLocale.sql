CREATE PROCEDURE [dbo].[Znode_GetAddOnGroupLocale]
( @PimAddOnGroupId VARCHAR(MAX) = '',
  @LocaleId        INT          = 0
)
AS
/*
     Summary:- This Procedure is used to get the addon group locale wise on the basis of PimAddonGroupId 
     Unit Testing 
	 begin tran
     EXEC Znode_GetAddOnGroupLocale '',1
	 rollback tran
*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();

			 -- If required filter on PimAddOnGroupId in future then we can use commented code

            /* DECLARE @TBL_PimAddOnGroupId TABLE(PimAddOnGroupId INT);
             INSERT INTO @TBL_PimAddOnGroupId(PimAddOnGroupId)
                    SELECT PimAddOnGroupId FROM ZnodePimAddonGroup ZPAG
                    WHERE EXISTS
                    (
                        SELECT Item
                        FROM dbo.split(@PimAddOnGroupId, ',') SP
                        WHERE SP.Item = ZPAG.PimAddonGroupId
                              OR @PimAddOnGroupId = ''
                    ); */
             WITH Cte_AddonsWithBothLocale
                  AS (SELECT ZPAG.PimAddonGroupId,ZPAG.DisplayType,ZPAGL.AddonGroupName,ZPAGL.LocaleId
                      FROM dbo.ZnodePimAddonGroup AS ZPAG
                      INNER JOIN dbo.ZnodePimAddonGroupLocale AS ZPAGL ON(ZPAG.PimAddonGroupId = ZPAGL.PimAddonGroupId)
                      WHERE ZPAGL.LocaleId IN(@DefaultLocaleId, @LocaleId)),

                  Cte_AddOnsWithFirstLocale
                  AS (SELECT PimAddonGroupId,DisplayType,AddonGroupName
                      FROM Cte_AddonsWithBothLocale
                      WHERE LocaleId = @LocaleId),

                  AddonsList
                  AS (
                  SELECT PimAddonGroupId,DisplayType,AddonGroupName
                  FROM Cte_AddOnsWithFirstLocale
                  UNION ALL
                  SELECT PimAddonGroupId,DisplayType,AddonGroupName
                  FROM Cte_AddonsWithBothLocale CTAWBL
                  WHERE LocaleId = @DefaultLocaleId
                  AND NOT EXISTS
                  (
                      SELECT *
                      FROM Cte_AddOnsWithFirstLocale CTAFL
                      WHERE CTAFL.PimAddonGroupId = CTAWBL.PimAddonGroupId
                  ))
                  SELECT PimAddonGroupId,DisplayType,AddonGroupName FROM AddonsList;
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetAddOnGroupLocale @PimAddOnGroupId = '+@PimAddOnGroupId+',@LocaleId='+CAST(@LocaleId AS VARCHAR(10))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		    
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetAddOnGroupLocale',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
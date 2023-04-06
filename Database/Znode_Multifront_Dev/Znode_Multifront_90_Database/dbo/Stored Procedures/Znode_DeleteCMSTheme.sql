CREATE PROCEDURE [dbo].[Znode_DeleteCMSTheme]
( @CMSThemeId VARCHAR(2000),
  @Status     BIT OUT)
AS 
  /* 
     Summary: Remove CMS theme with details
			  Before delete check is not associated with portal 
			  output dataset contain the status if passed @CMSThemeIds all ids are deleted then this will true other wise false 
			  Delete table sequence 
			  1.[ZnodeCMSThemeCSS]
			  2.[ZnodeCMSTheme]
     Unit Testing 
	 begin tran  
     EXEC [Znode_DeleteCMSTheme] 1
	 rollback tran
   
*/
     BEGIN
         BEGIN TRAN DeleteCMSTheme;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @TBL_ThemeId TABLE(CMSThemeId INT); -- table holds the cmsthemeid 
             INSERT INTO @TBL_ThemeId
                    SELECT item
                    FROM dbo.Split(@CMSThemeId, ',');
             DECLARE @TBL_DeleteThemeId TABLE(CMSThemeId INT);
             INSERT INTO @TBL_DeleteThemeId
                    SELECT ZCT.CMSThemeId
                    FROM [dbo].ZnodeCMSTheme AS ZCT
                         INNER JOIN @TBL_ThemeId AS TI ON(ZCT.CMSThemeId = TI.CMSThemeId)
                    WHERE NOT EXISTS
                    (   -- check not associated with portal
                        SELECT TOP 1 1
                        FROM ZnodeCMSPortalTheme AS zx
                        WHERE zx.CMSThemeId = zct.CMSThemeId
                    ) AND
					NOT EXISTS
                    (   
                        SELECT TOP 1 1
                        FROM ZnodeCMSTheme AS zx
                        WHERE zx.ParentThemeId = zct.CMSThemeId
                    );
					  
             DELETE FROM ZnodeCMSThemeCSS
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteThemeId AS DTI
                 WHERE DTI.CMSThemeId = ZnodeCMSThemeCSS.CMSThemeId
             );
             DELETE FROM ZnodeCMSTheme
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteThemeId AS DTI
                 WHERE DTI.CMSThemeId = ZnodeCMSTheme.CMSThemeId
             );
             IF
             (
                 SELECT COUNT(1)
                 FROM @TBL_DeleteThemeId
             ) =
             (   -- if equal then dataset status return true other wise false 
                 SELECT COUNT(1)
                 FROM @TBL_ThemeId
             ) 
                 BEGIN
                     SELECT 1 AS ID,
                            CAST(1 AS BIT) AS Status;
                     SET @Status = 1;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID,
                            CAST(0 AS BIT) AS Status;
                     SET @Status = 0;
                 END;
             COMMIT TRAN DeleteCMSTheme;
         END TRY
         BEGIN CATCH
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteCMSTheme @CMSThemeId = '+@CMSThemeId+',@Status='+CAST(@Status AS VARCHAR(50));
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
             SET @Status = 0;
             ROLLBACK TRAN DeleteCMSTheme;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteCMSTheme',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;
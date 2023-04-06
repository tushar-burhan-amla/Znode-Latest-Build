CREATE PROCEDURE [dbo].[_Znode_DeleteContentPageGroups](@CMSContentPageGroupId VARCHAR(2000),
                                                       @Status                BIT OUT)
AS
    -------------------------------------------------------------------------------
    -- Summary : Remove Content Page Group details with their referance data 
    -- Here complete delete the Content Pages Group and their references without any check  
    -- If passed @CMSContentPageGroupId are matched with deleted count then data set return true other wise false 
    -- dbo.Split function use to make comma separeted data in table rows 
    -- 1 ZnodeCMSContentPagesProfile
    -- 2 ZnodeCMSContentPagesLocale
    -- 3 ZnodeCMSContentPageGroupMapping
    -- 4 ZnodeCMSContentPages
    -- 5 ZnodeCMSContentPageGroupLocale
    -- 6 ZnodeCMSContentPageGroup
    -- Unit Testing 
    -- DEclare @Status bit 
    -- EXEC Znode_DeleteContentPageGroups  '3' ,@Status =@Status OUT 
    -- SELECT @Status 
    -------------------------------------------------------------------------------
     BEGIN
         BEGIN TRAN DeleteContentPageGroups;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @TBL_DeleteGroupId TABLE(CMSContentPageGroupId INT); -- table used to hold the CMSContentPageGroupId 
             INSERT INTO @TBL_DeleteGroupId
                    SELECT CMSContentPageGroupId
                    FROM dbo.Fn_GetRecurciveContentPageGroup(@CMSContentPageGroupId);
             --SELECT CMSContentPageGroupId -- this  will check the valied  CMSContentPageGroupId 
             --FROM [dbo].[ZnodeCMSContentPageGroup] AS a INNER JOIN dbo.Split ( @CMSContentPageGroupId , ',' -- dbo.Split function use to make comma separeted data in table rows 
             --                                                                ) AS b ON ( a.CMSContentPageGroupId = b.Item );
             DECLARE @TBL_DeleteContentPage TABLE(CMSContentPagesId INT);
             INSERT INTO @TBL_DeleteContentPage
                    SELECT DISTINCT
                           CMSContentPagesId
                    FROM ZnodeCMSContentPageGroupMapping
                         INNER JOIN @TBL_DeleteGroupId AS s ON(s.CMSContentPageGroupId = ZnodeCMSContentPageGroupMapping.CMSContentPageGroupId);
             DELETE FROM ZnodeCMSContentPagesProfile
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteContentPage AS DCP
                 WHERE DCP.CMSContentPagesId = ZnodeCMSContentPagesProfile.CMSContentPagesId
             );
             DELETE FROM ZnodeCMSContentPagesLocale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteContentPage AS DCP
                 WHERE DCP.CMSContentPagesId = ZnodeCMSContentPagesLocale.CMSContentPagesId
             );
             DELETE FROM ZnodeCMSContentPageGroupMapping
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteGroupId AS s
                 WHERE s.CMSContentPageGroupId = ZnodeCMSContentPageGroupMapping.CMSContentPageGroupId
             );
             DELETE FROM ZnodeCMSContentPages
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteContentPage AS DCP
                 WHERE DCP.CMSContentPagesId = ZnodeCMSContentPages.CMSContentPagesId
             );
             DELETE FROM ZnodeCMSContentPageGroupLocale
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteGroupId AS s
                 WHERE s.CMSContentPageGroupId = ZnodeCMSContentPageGroupLocale.CMSContentPageGroupId
             );
             DELETE FROM ZnodeCMSContentPageGroup
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteGroupId AS s
                 WHERE s.CMSContentPageGroupId = ZnodeCMSContentPageGroup.CMSContentPageGroupId
             );
             IF
             (
                 SELECT COUNT(1)
                 FROM @TBL_DeleteGroupId
             ) =
             (
                 SELECT COUNT(1)
                 FROM dbo.Split(@CMSContentPageGroupId, ',')
             ) -- if both count are same then data set status return true other wise false 
                 BEGIN
                     SELECT 1 AS ID,
                            CAST(1 AS BIT) AS [Status];
                     SET @Status = 1;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID,
                            CAST(0 AS BIT) AS [Status];
                     SET @Status = 0;
                 END;
             COMMIT TRAN DeleteContentPageGroups;
         END TRY
         BEGIN CATCH
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteContentPageGroups @CMSContentPageId = '+@CMSContentPageGroupId+',@Status='+CAST(@Status AS VARCHAR(50));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS [Status];
             ROLLBACK TRAN DeleteContentPageGroups;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteContentPageGroups',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;
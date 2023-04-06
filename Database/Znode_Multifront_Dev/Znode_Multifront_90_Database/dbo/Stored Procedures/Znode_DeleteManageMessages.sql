CREATE PROCEDURE [dbo].[Znode_DeleteManageMessages]
( @CMSPortalMessageId VARCHAR(2000),
  @Status             BIT OUT)
AS
   /* 
     Summary : Remove messages of the portal with their reference data 
			   Here complete delete the messages of portal and their references without any check  
			   If passed @CMSPortalMessageId are matched with deleted count then data set return true other wise false 
			   dbo.Split function use to make comma seperated data in table rows 
			   1 ZnodeCMSPortalMessage
			   2 ZnodeCMSMessage
			   3 ZnodeCMSMessageKey
     Unit Testing 
	 begin tran
     Declare @Status bit 
     EXEC [Znode_DeleteManageMessages] 1795 ,@Status =@Status OUT 
	 rollback tran
     */
	 BEGIN
         BEGIN TRAN DeleteManageMessages;
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @TBL_MessageIds TABLE
             (PortalId        INT,
              CmsMessageKeyId INT
             );
             INSERT INTO @TBL_MessageIds
                    SELECT ZCPM.PortalId,ZCPM.CmsMessageKeyId                         
                    FROM ZnodeCMSPortalMessage AS ZCPM
					     -- dbo.Split function use to make comma separeted data in table rows
                         INNER JOIN dbo.Split(@CMSPortalMessageId, ','  
                         ) AS SP ON(ZCPM.CMSPortalMessageId = SP.Item);
             DECLARE @TBL_DeletedMessagKey TABLE(CMSMessageKeyId INT);
             DECLARE @TBL_DeleteCMSMessageId TABLE
             (CMSPortalMessageId INT,
              CMSMessageId       INT
             );

             INSERT INTO @TBL_DeleteCMSMessageId
                    SELECT ZCPM.CMSPortalMessageId,CMSMessageId                          
                    FROM [dbo].ZnodeCMSPortalMessage AS ZCPM
                         INNER JOIN @TBL_MessageIds AS TBM ON(
						 (isnull(ZCPM.PortalId,-1) = isnull(TBM.portalid,-1))
                         AND ZCPM.CMSMessageKeyId = TBM.CmsMessageKeyId);
             DELETE FROM ZnodeCMSPortalMessage
			 -- catch the deleted values MessageKey
             OUTPUT DELETED.CMSMessageKeyId
                    INTO @TBL_DeletedMessagKey 
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteCMSMessageId AS TBDCM
                 WHERE TBDCM.CMSPortalMessageId = ZnodeCMSPortalMessage.CMSPortalMessageId
             );

			 -- Fetch MessageKeyDetails for publishing in mongo
			 DECLARE @TBL_DeletedMessage TABLE(CMSMessageKeyId INT, MessageKey Nvarchar(100))
			 INSERT INTO @TBL_DeletedMessage (CMSMessageKeyId,MessageKey)
			 SELECT a.CMSMessageKeyId,a.MessageKey  
			 FROM ZnodeCMSMessageKey a
			 INNER JOIN @TBL_DeletedMessagKey b on (a.CMSMessageKeyId = b.CMSMessageKeyId)

             DELETE FROM ZnodeCMSMessage
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeleteCMSMessageId AS TBDCM
                 WHERE TBDCM.CMSMessageId = ZnodeCMSMessage.CMSMessageId
             )
                   AND NOT EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodeCMSPortalMessage AS TBDCM
                 WHERE TBDCM.CMSMessageId = ZnodeCMSMessage.CMSMessageId
             );
             DELETE FROM ZnodeCMSPortalMessageKeyTag
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedMessagKey AS TBDMK
                 WHERE TBDMK.CMSMessageKeyId = ZnodeCMSPortalMessageKeyTag.CMSMessageKeyId
             )
                   AND NOT EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodeCMSPortalMessage AS TBDMK
                 WHERE TBDMK.CMSMessageKeyId = ZnodeCMSPortalMessageKeyTag.CMSMessageKeyId
             );
             DELETE FROM ZnodeCMSMessageKey
             WHERE EXISTS
             (
                 SELECT TOP 1 1
                 FROM @TBL_DeletedMessagKey AS TBDMK
               WHERE TBDMK.CMSMessageKeyId = ZnodeCMSMessageKey.CMSMessageKeyId
             )
                   AND NOT EXISTS
             (
                 SELECT TOP 1 1
                 FROM ZnodeCMSPortalMessage AS TBDMK
                 WHERE TBDMK.CMSMessageKeyId = ZnodeCMSMessageKey.CMSMessageKeyId
             );

             IF(SELECT COUNT(1) FROM @TBL_DeleteCMSMessageId) = ( SELECT COUNT(1) FROM @TBL_DeletedMessagKey) -- if count are equal then dataset status return true other wise false 
                 BEGIN
                     SELECT a.CMSMessageKeyId AS ID, b.MessageKey AS MessageDetails ,
                            CAST(1 AS BIT) AS Status
					 FROM @TBL_DeletedMessagKey a
					 INNER JOIN @TBL_DeletedMessage b on (a.CMSMessageKeyId = b.CMSMessageKeyId)
					
					 DELETE FROM ZnodePublishGlobalMessageEntity WHERE MessageKey in 	
					 (SELECT b.MessageKey FROM @TBL_DeletedMessagKey a
					 INNER JOIN @TBL_DeletedMessage b on (a.CMSMessageKeyId = b.CMSMessageKeyId)
					 where a.CMSMessageKeyId = 0)


					 DELETE FROM ZnodePublishGlobalMessageEntity WHERE MessageKey in 	
					 (SELECT b.MessageKey FROM @TBL_DeletedMessagKey a
					 INNER JOIN @TBL_DeletedMessage b on (a.CMSMessageKeyId = b.CMSMessageKeyId)
					 where a.CMSMessageKeyId = 0)

                     SET @Status = 1;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID,
                            CAST(0 AS BIT) AS Status;
                     SET @Status = 0;
                 END;
             COMMIT TRAN DeleteManageMessages;
         END TRY
         BEGIN CATCH
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteManageMessages @CMSContentPageId = '+@CMSPortalMessageId+',@Status='+CAST(@Status AS VARCHAR(50));
             SET @Status = 0;
             SELECT 0 AS ID,
                    CAST(0 AS BIT) AS Status;
             ROLLBACK TRAN DeleteManageMessages;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_DeleteManageMessages',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;
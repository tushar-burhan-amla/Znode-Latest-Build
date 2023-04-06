

CREATE PROCEDURE [dbo].[Znode_GetManageMessageForEdit]  
( @PortalId        INT = 0,  
  @CMSMessageKeyId INT,  
  @LocaleId        INT,  
  @CMSMessageId    INT = 0)  
AS  
/*  
 Summary: Get Managed Message Details filtered by CMSMessageId  
 Unit Testing:  
   EXEC Znode_GetManageMessageForEdit @PortalId  = 4,@CMSMessageKeyId = 12,@LocaleId = 1, @CMSMessageId=14  
  
*/  
     BEGIN  
         BEGIN TRY  
             SET NOCOUNT ON;  
             DECLARE @TBL_CMSMessage TABLE  
             (CMSMessageId    INT,  
              [Message]       NVARCHAR(MAX),  
              MessageKey      NVARCHAR(100),  
              StoreName       NVARCHAR(MAX),  
              LocaleId        INT,  
              PortalId        INT,  
              CMSMessageKeyId INT,  
              TagXML          NVARCHAR(MAX)  
             );  
             INSERT INTO @TBL_CMSMessage  
                    SELECT ZCM.CMSMessageId,ZCM.[Message],ZCMK.MessageKey AS Location,ZP.StoreName,ZCM.LocaleId,ZP.PortalId,ZCPM.CMSMessageKeyId,CONVERT( NVARCHAR(MAX), ZCPMKT.TagXML) AS TagXML  
                    FROM [dbo].[ZnodeCMSMessage] AS ZCM  
                         INNER JOIN [dbo].[ZnodeCMSPortalMessage] AS ZCPM ON(ZCM.CMSMessageId = ZCPM.CMSMessageId)  
                         LEFT JOIN [dbo].[ZnodeCMSMessageKey] AS ZCMK ON(ZCMK.CMSMessageKeyId = ZCPM.CMSMessageKeyId)  
                         LEFT JOIN [dbo].ZnodePortal AS ZP ON(ZP.PortalId = ZCPM.PortalId)  
                         LEFT JOIN [dbo].ZnodeCMSPortalMessageKeyTag AS ZCPMKT ON(ISNULL(ZCPMKT.PortalId,-1) = ISNULL(ZCPM.PortalId,-1)  
                                                                               AND ZCPMKT.CMSMessageKeyId = ZCMK.CMSMessageKeyId)  
                    WHERE((ZP.PortalId = @PortalId) OR (@PortalId = 0 and ZCPM.PortalId is Null))  
                    AND (ZCM.CMSMessageId = @CMSMessageId OR @CMSMessageId = 0)  
                    AND ZCMK.CMSMessageKeyId = @CMSMessageKeyId  
                    AND ZCM.LocaleId = @LocaleId;  
             SELECT CMSMessageKeyId,MessageKey,CMSMessageId,[Message],LocaleId,ZCM.PortalId,ZCM.StoreName,TagXML MessageTag  
             FROM @TBL_CMSMessage AS ZCM;  
              
         END TRY  
         BEGIN CATCH  
               DECLARE @Status BIT ;  
       SET @Status = 0;  
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetManageMessageForEdit @PortalId = '+CAST(@PortalId AS VARCHAR(50
))+',@CMSMessageKeyId='+CAST(@CMSMessageKeyId AS VARCHAR(50))+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@CMSMessageId='+CAST(@CMSMessageId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));  
                    
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                      
     
             EXEC Znode_InsertProcedureErrorLog  
    @ProcedureName = 'Znode_GetManageMessageForEdit',  
    @ErrorInProcedure = @Error_procedure,  
    @ErrorMessage = @ErrorMessage,  
    @ErrorLine = @ErrorLine,  
    @ErrorCall = @ErrorCall;      
         END CATCH;  
     END;
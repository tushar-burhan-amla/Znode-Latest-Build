
CREATE PROCEDURE [dbo].[Znode_DeleteMessageKey](
       @PortalIds       VARCHAR(1000) ,
       @CMSAreaIds      VARCHAR(1000) ,
       @CMSMessageKeyId INT ,
       @CMSMessageIds   VARCHAR(1000) ,
       @Status          BIT           = 1 OUT)
AS 
  /*
    Summary:  Delete the message key and message also remove the mapping of message key with area and portal 
    Unit Testing   
    Begin 
    	Begin Transaction 
    		Exec Znode_DeleteMessageKey    @PortalIds = '70,71',  @CMSAreaIds =37 ,@CMSMessageKeyId = 19,@CMSMessageIds= '', @Status = 0   
     SELECT * FROM ZnodeCMSPortalMessage 
     SELECT * FROM ZnodeCMSAreaMessageKey
    	Rollback Transaction 
    ENd  
  */
     BEGIN
         BEGIN TRAN B;
         BEGIN TRY
             SET NOCOUNT ON; 
             ---- Declare the table to store the comma separeted data into record format
             DECLARE @TBL_Portal TABLE (
                                       ID       INT ,
                                       PortalId INT
                                       );
             DECLARE @TBL_Area TABLE (
                                     ID        INT ,
                                     CMSAreaId INT
                                     );
             DECLARE @TBL_MessageId TABLE (
                                          ID           INT ,
                                          CMSMessageId INT
                                          ); 
			 
             ---- Declare this table to find the actual deleted message keyids -----
             DECLARE @TBL_DeletedMessageId TABLE (
                                                 id              INT IDENTITY(1 , 1) ,
                                                 CMSMessageKeyId INT
                                                 );
             INSERT INTO @TBL_Portal
                    SELECT ID , ITEm
                    FROM dbo.split ( @PortalIds , ','
                                   ); --- store the comma separeted portaid into variable table 

             INSERT INTO @TBL_Area
                    SELECT ID , ITEm
                    FROM dbo.split ( @CMSAreaIds , ','
                                   ); --- store the comma separeted AreaId into variable table 

             INSERT INTO @TBL_MessageId
                    SELECT ID , ITEm
                    FROM dbo.split ( @CMSMessageIds , ','
                                   ); --- store the comma separeted MessageId into variable table 
             ------ deleted the record portalid present in variable table and messagekeyid equal to the message key id passed in parameter

             DELETE FROM ZnodeCMSPortalMessage
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_Portal AS tp
                            WHERE tp.PortalId = ZnodeCMSPortalMessage.PortalId
                          )
                   AND
                   EXISTS ( SELECT TOP 1 1
                            FROM @TBL_MessageId AS tm
                            WHERE tm.CMSMessageId = ZnodeCMSPortalMessage.CMSMessageId
                          )
                   AND
                   ZnodeCMSPortalMessage.CMSMessageKeyId = @CMSMessageKeyId;
				
             ------ deleted the record CMSAreaId present in variable table and messagekeyid equal to the message key id passed in parameter

             DELETE FROM ZnodeCMSAreaMessageKey
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_Area AS ta
                            WHERE ta.CMSAreaId = ZnodeCMSAreaMessageKey.CMSAreaId
                          )
                   AND
                   ZnodeCMSAreaMessageKey.CMSMessageKeyId = @CMSMessageKeyId;
				
             ------ deleted the record of messgekeyid 

             DELETE FROM ZnodeCMSAreaMessageKey
             OUTPUT DELETED.CMSMessageKeyId
                    INTO @TBL_DeletedMessageId(CMSMessageKeyId)
             WHERE CMSMessageKeyId = @CMSMessageKeyId
                   AND
                   NOT EXISTS ( SELECT TOP 1 1
                                FROM ZnodeCMSPortalMessage AS zcpm
                                WHERE zcpm.CMSMessageKeyId = @CMSMessageKeyId
                              )
                   AND
                   NOT EXISTS ( SELECT TOP 1 1
                                FROM ZnodeCMSAreaMessageKey AS zcam
                                WHERE zcam.CMSMessageKeyId = @CMSMessageKeyId
                              );
											   	
             ----- delete the message of message key id 

             DELETE FROM ZnodeCMSMessage
             WHERE EXISTS ( SELECT TOP 1 1
                            FROM @TBL_MessageId AS tm
                            WHERE tm.CMSMessageId = ZnodeCMSMessage.CMSMessageId
                          )
                   AND
                   NOT EXISTS ( SELECT TOP 1 1
                                FROM ZnodeCMSPortalMessage AS zcpm
                                WHERE zcpm.CMSMessageId = ZnodeCMSMessage.CMSMessageId
                              );
             SET @Status = 1;
             IF @CMSMessageKeyId = ( SELECT CMSMessageKeyId
                                     FROM @TBL_DeletedMessageId
                                   )
                 BEGIN
                     SELECT 1 AS ID , CAST(1 AS BIT) AS Status;
                 END;
             ELSE
                 BEGIN
                     SELECT 0 AS ID , CAST(0 AS BIT) AS Status;
                 END;
             COMMIT TRAN B;
         END TRY
         BEGIN CATCH
           
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_DeleteMessageKey @PortalIds = '+@PortalIds+',@CMSAreaIds='+@CMSAreaIds+',@CMSMessageKeyId='+CAST(@Status AS VARCHAR(50))+',@CMSMessageIds='+@CMSMessageIds+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN B;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_DeleteMessageKey',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
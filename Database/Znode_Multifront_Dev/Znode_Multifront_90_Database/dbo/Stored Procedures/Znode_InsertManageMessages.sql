    
CREATE PROCEDURE [dbo].[Znode_InsertManageMessages]    
(    
 @PortalIds varchar(2000),    
 @MessageKey nvarchar(100),     
 @MessageTag nvarchar(2000)= NULL,    
 @Description nvarchar(max)=null,    
 @LocaleId int,    
 @UserId int,     
 @CMSMessageId int,     
 @CMSMessageKeyId int= 0,     
 @Status bit= 0 OUT,     
 @IsDebug bit= 0)    
AS     
 /*    
 Summary:  This procedure use for insert manage message and update on the basis of message key                      
 Unit Testing       
    SELECT * FROM ZnodePortal    
  SELECT * FROM ZnodeCMSMessageKey  WHERE messageKey = 'test111'    
  SELECT * FROM [ZnodeCMSMessage] WHERE message = 'fjkxdbjlf'    
  SELECT * FROM ZnodeCMSPortalMessage WHERE PortalId = 28    
  EXEC Znode_InsertManageMessages '29','test111','fjkxdbjlf1',1,2,0,0,0     
    
 */    
BEGIN    
 BEGIN TRAN A;    
 BEGIN TRY    
  SET NOCOUNT ON;    
  DECLARE @MessageKeyDetail int;    
     DECLARE @GetDate DATETIME = dbo.Fn_GetDate();    
  DECLARE @CMSMessageId_New int= 0;    
  DECLARE @PortalId TABLE    
  (     
  ID int, PortalId int    
  );    
  DECLARE @AreaId TABLE    
  (     
  ID int, AreaId int    
  );    
  INSERT INTO @PortalId    
      SELECT ID, ITEM    
      FROM dbo.split( @PortalIds, ',' ) AS a;     
    
  -- Insert Data into Temp table     
    
  DECLARE @InsertDetails TABLE    
  (     
  PortalId int, CMSMessageKeyId int, CMSMessageId int, MessageKey varchar(1000), Messagedescription nvarchar(max), LocaleId int    
  );    
  DECLARE @CurrentPortalAreaIds TABLE    
  (     
  PortalId int, CMSMessageKeyId int, CMSMessageId int    
  );    
  INSERT INTO @InsertDetails    
      SELECT Case when a.PortalId = 0 then null else a.PortalId end, @CMSMessageKeyId, @CMSMessageId, @MessageKey, @Description, @LocaleId    
      FROM @PortalId AS a;    
    
    
  IF EXISTS    
  (    
   SELECT TOP 1 1  FROM ZnodeCMSPortalMessage AS ZCPM  INNER JOIN    
     ZnodeCMSMessageKey AS ZCMK  ON(ZCPM.CMSMessageKeyId = ZCMK.CMSMessageKeyId)    
   WHERE EXISTS  (  SELECT TOP 1 1  FROM @InsertDetails AS TBID  WHERE ZCMK.MessageKey = @MessageKey AND  (TBID.PortalId is null or  ZCPm.PortalId = TBID.PortalId)  )    
  ) AND   @CMSMessageKeyId = 0    
  BEGIN    
   RAISERROR(15600, -1, -1, '');    
  END;     
         
  -- First check locale wise messsage are for how many portal or area    
  INSERT INTO @CurrentPortalAreaIds  SELECT PortalId, a.CMSMessageId, a.CMSMessageKeyId  FROM ZnodeCMSPortalMessage AS a    
     INNER JOIN  ZnodeCMSMessage AS b  ON(a.CMSMessageId = b.CMSMessageId)    
      WHERE a.CMSMessageKeyId = @CMSMessageKeyId AND   a.CMSMessageId = @CMSMessageId;    
  IF EXISTS    
  (    
   SELECT TOP 1 1    
   FROM ZnodeCMSPortalMessage AS a    
     INNER JOIN    
     [ZnodeCMSMessageKey] AS b    
     ON( a.CMSMessageKeyId = b.CMSMessageKeyId AND     
      b.MessageKey = @MessageKey    
       )    
     INNER JOIN    
     ZnodeCMSMessage AS c    
     ON( c.CMSMessageId = a.CMSMessageId AND     
      c.LocaleId = @LocaleId AND     
      C.Message = @Description    
       )    
   WHERE EXISTS    
   (    
    SELECT TOP 1 1    
    FROM @InsertDetails AS vc    
    WHERE vc.PortalId = a.PortalId    
   )    
  )    
  BEGIN    
   SET @MessageKeyDetail =    
   (    
    SELECT TOP 1 CMSMessageKeyId    
    FROM [dbo].[ZnodeCMSMessageKey] AS zcmk    
    WHERE zcmk.MessageKey = @MessageKey    
   );    
    
   --  RAISERROR (15600,-1,-1, ' Is Already Exists ');     
    
  END;    
  IF NOT EXISTS    
  (    
   SELECT TOP 1 1    
   FROM ZnodeCMSMessageKey    
   WHERE MessageKey = @MessageKey    
  )    
  BEGIN    
   INSERT INTO [ZnodeCMSMessageKey]( MessageKey, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )    
   VALUES( @MessageKey, @UserId, @GetDate, @UserId, @GetDate );    
    
   SET @MessageKeyDetail = SCOPE_IDENTITY();    
  END;    
  ELSE    
  BEGIN    
   SET @MessageKeyDetail =    
   (    
    SELECT CMSMessageKeyId    
    FROM [ZnodeCMSMessageKey]    
    WHERE MessageKey = @MessageKey    
   );    
   UPDATE [dbo].[ZnodeCMSMessageKey]    
     SET ModifiedBy = @UserId, ModifiedDate = @GetDate    
   WHERE MessageKey = @MessageKey;    
    
  END;    
  IF NOT EXISTS    
  (    
   SELECT TOP 1 1    
   FROM [ZnodeCMSMessage] AS zcm    
   INNER JOIN ZnodeCMSPortalMessage ZCPM ON (ZCPM.CMSMessageId = ZCM.CMSMessageId)    
   WHERE Zcm.CMSMessageId = @CMSMessageId   
   AND ((@PortalIds = '0' and ZCPM.PortalId is null ) OR (@PortalIds <>'0' and ZCPM.PortalId =  @PortalIds ))  
   --AND (@PortalIds = '0' or ZCM.[Message] = @Description)   
   AND   zcm.LocaleId = @LocaleId    
  )    
  BEGIN    
   DELETE FROM ZnodeCMSPortalMessage WHERE PortalId = @PortalIds AND CMSMessageKeyId = @CMSMessageKeyId    
      AND  CMSMessageId IN ( SELECT CMSMessageId FROM [ZnodeCMSMessage] ZCM WHERE  CMSMessageId = @CMSMessageId AND LocaleId = @LocaleId)    
    
   INSERT INTO [dbo].[ZnodeCMSMessage]( LocaleId, [Message], CreatedBy, CreatedDate, ModifiedBy, ModifiedDate )    
   VALUES( @LocaleId, @Description, @UserId, @GetDate, @UserId, @GetDate );    
   SET @CMSMessageId_New = SCOPE_IDENTITY();    
    
  END;    
  ELSE    
  BEGIN    
   UPDATE [dbo].[ZnodeCMSMessage]    
     SET [Message] = @Description, ModifiedBy = @UserId, ModifiedDate = @GetDate    
   WHERE CMSMessageId = @CMSMessageId;    
   SET @CMSMessageId_New = @CMSMessageId;    
  END;    
  
  IF(@PortalIds = '0')  
  BEGIN  
    
  IF NOT EXISTS    
  (   
 SELECT TOP 1 1    
 FROM [ZnodeCMSPortalMessage] AS ZCPM   
 WHERE ZCPM.PortalId IS NULL AND   
 ZCPM.CMSMessageKeyId = CAST(@MessageKeyDetail AS int) AND     
    ZCPM.CMSMessageId = @CMSMessageId_NEW   
  )  
  BEGIN  
 INSERT INTO ZnodeCMSPortalMessage(PortalId, CMSMessageKeyId, CMSMessageId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) VALUES(NULL, CAST(@MessageKeyDetail AS int), @CMSMessageId_New, @UserId, @GetDate, @UserId, @GetDate );    
  END  
  ELSE  
  BEGIN  
 UPDATE ZnodeCMSPortalMessage  SET    
 ModifiedBy = @UserId, ModifiedDate = @GetDate  
 WHERE PortalId IS NULL AND   
 CMSMessageKeyId = CAST(@MessageKeyDetail AS int) AND     
    CMSMessageId = @CMSMessageId_NEW   
  END  
  
  IF NOT EXISTS    
  (   
 SELECT TOP 1 1    
 FROM [ZnodeCMSPortalMessageKeyTag] AS ZCPMT   
 WHERE ZCPMT.PortalId IS NULL AND   
 ZCPMT.CMSMessageKeyId = CAST(@MessageKeyDetail AS int)      
  )  
  BEGIN  
 INSERT INTO [ZnodeCMSPortalMessageKeyTag](PortalId, CMSMessageKeyId, TagXML, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) VALUES(NULL, CAST(@MessageKeyDetail AS int), @MessageTag, @UserId, @GetDate, @UserId, @GetDate );    
  END  
  ELSE  
  BEGIN  
 UPDATE [ZnodeCMSPortalMessageKeyTag]  SET    
 TagXML = @MessageTag, ModifiedBy = @UserId, ModifiedDate = @GetDate  
 WHERE PortalId IS NULL AND   
 CMSMessageKeyId = CAST(@MessageKeyDetail AS int)      
  END  
  
  END  
  ELSE  
  BEGIN  
  MERGE INTO ZnodeCMSPortalMessage TARGET    
  USING @InsertDetails SOURCE    
  ON TARGET.PortalId = SOURCE.PortalId AND     
     TARGET.CMSMessageKeyId = CAST(@MessageKeyDetail AS int) AND     
     TARGET.CMSMessageId = @CMSMessageId_NEW    
      
  WHEN MATCHED    
     THEN UPDATE SET TARGET.CMSMessageId = @CMSMessageId_New, TARGET.ModifiedBy = @UserId, TARGET.ModifiedDate = @GetDate    
  WHEN NOT MATCHED    
     THEN INSERT(PortalId, CMSMessageKeyId, CMSMessageId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) VALUES(SOURCE.PortalId, CAST(@MessageKeyDetail AS int), @CMSMessageId_New, @UserId, @GetDate, @UserId, @GetDate );    
      
  MERGE INTO ZnodeCMSPortalMessageKeyTag TARGET    
  USING @InsertDetails SOURCE    
  ON TARGET.PortalId = SOURCE.PortalId AND     
     TARGET.CMSMessageKeyId = CAST(@MessageKeyDetail AS int)    
  WHEN MATCHED    
     THEN UPDATE SET TagXML = @MessageTag, ModifiedBy = @userId, ModifiedDate = @GetDate    
  WHEN NOT MATCHED    
     THEN INSERT(PortalId, CMSMessageKeyId, TagXML, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate) VALUES(SOURCE.PortalId, CAST(@MessageKeyDetail AS int), @MessageTag, @UserId, @GetDate, @UserId, @GetDate );    
    
  DELETE FROM [ZnodeCMSMessage]    
  WHERE NOT EXISTS    
  (    
   SELECT TOP 1 1    
   FROM ZnodeCMSPortalMessage AS ss    
   WHERE ss.CMSMessageId = [ZnodeCMSMessage].CMSMessageId    
  );    
    
  DELETE FROM ZnodeCMSMessageKey    
  WHERE NOT EXISTS    
  (    
   SELECT TOP 1 1    
   FROM ZnodeCMSPortalMessage AS ss    
   WHERE ss.CMSMessageKeyId = ZnodeCMSMessageKey.CMSMessageKeyId    
  ) AND     
     NOT EXISTS    
  (    
   SELECT TOP 1 1    
   FROM dbo.ZnodeCMSPortalMessageKeyTag AS ss    
   WHERE ss.CMSMessageKeyId = ZnodeCMSMessageKey.CMSMessageKeyId    
  );    
  
  END  
  
  SELECT @MessageKeyDetail AS ID, 'Successful' AS [MessageDetails], CAST(1 AS bit) AS [Status];    
  -- output paramater     
  SET @Status = 1;    
  COMMIT TRAN A;    
 END TRY    
 BEGIN CATCH    
        
       SET @Status = 0;    
       DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),     
    @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertManageMessages @PortalIds = '+@PortalIds+',@MessageKey='+@MessageKey+',@MessageTag='+@MessageTag+',@Description='+@Description+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@CMSMessageId='+CAST(@CMSMessageId AS VARCHAR(50))+',@CMSMessageKeyId='+CAST(@CMSMessageKeyId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));    
                      
             --SELECT 0 AS ID,CAST(0 AS BIT) AS Status;     
    SELECT @CMSMessageId AS ID, ISNULL(CAST(@MessageKeyDetail AS varchar(2000)), '')+'  Is Already Exists' AS [MessageDetails], CAST(0 AS bit) AS [Status];    
                         
       ROLLBACK TRAN A;    
             EXEC Znode_InsertProcedureErrorLog  @ProcedureName = 'Znode_InsertManageMessages',  @ErrorInProcedure = @Error_procedure,  @ErrorMessage = @ErrorMessage,  @ErrorLine = @ErrorLine,  @ErrorCall = @ErrorCall;        
 END CATCH;    
END;
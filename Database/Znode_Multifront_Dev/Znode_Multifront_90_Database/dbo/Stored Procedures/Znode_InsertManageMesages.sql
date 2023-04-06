-- SELECT * FROM ZnodePortal 
-- SELECT * FROM [ZnodeCMSMessage]
-- SELECT *  FROM [ZnodeCMSMessageKey]
-- SELECT * FROM [ZnodeCMSAreaMessageKey]
-- SELECT * FROM ZnodeCMSPortalMessage
-- EXEC Znode_InsertManageMesages '2,3,12','1','test111','test212',1,2,239,212,0 
CREATE Procedure [dbo].[Znode_InsertManageMesages]
(
	@PortalIds Varchar(1000),
	@AreaIds   VARCHAr(1000),
	@MessageKey  NVARCHAR(1000),
	@Description  NVARCHAR(MAX),
	@LocaleId     INT ,
	@User         INT , 
	@CMSMessageId INT ,
	@CMSMessageKeyId INT = 0 , 
	@Status       BIT = 0 OUT
)
AS 
 BEGIN 
  BEGIN TRAN A 
	  BEGIN TRY 
	   SET NOCOUNT ON 

	   DECLARE @MessageKeyDetail VARCHAR(3000) =''
	   DECLARE @CMSMessageId_New  INT = 0
	


	   DECLARE @PortalId TABLE (ID INT , PortalId INT)
	   DECLARE @AreaId   TABLE (ID INT , AreaId INT)
	   


	   INSERT INTO @PortalId 
	   SELECT ID ,ITEM FROM dbo.split(@PortalIds,',') a 
	   -- Insert Data into Temp table 
	   INSERT INTO @AreaId 
	   SELECT ID ,ITEM FROM dbo.split(@AreaIds,',') a 


	 
	     
       IF NOT EXISTS (SELECT TOP 1  1 FROM [ZnodeCMSMessage]  zcm WHERE Zcm.CMSMessageId = @CMSMessageId )
	   BEGIN -- Check already exists 

		   INSERT INTO  [dbo].[ZnodeCMSMessage] (LocaleId
											,[Message]
											,CreatedBy
											,CreatedDate
											,ModifiedBy
											,ModifiedDate)
		   Values ( @LocaleId , @Description,@User,GETUTCDATE(),@User,GETUTCDATE())
           

		   SET @CMSMessageId_New = SCOPE_IDENTITY()


	   END 
	   ELSE IF NOT EXISTS (SELECT TOP 1  1 FROM [ZnodeCMSMessage]  zcm WHERE Zcm.CMSMessageId = @CMSMessageId AND zcm.LocaleId = @LocaleId AND zcm.[Message]   = @Description ) 
	   BEGIN
	      
	       INSERT INTO  [dbo].[ZnodeCMSMessage] (LocaleId
											,[Message]
											,CreatedBy
											,CreatedDate
											,ModifiedBy
											,ModifiedDate)
		   Values ( @LocaleId , @Description,@User,GETUTCDATE(),@User,GETUTCDATE())
		   SET @CMSMessageId_New = SCOPE_IDENTITY()
		END 
		ELSE 
		BEGIN
			Update  [dbo].[ZnodeCMSMessage]
			SET 
			LocaleId    = @LocaleId
			,[Message]   = @Description
			,ModifiedBy  = @User
			,ModifiedDate = GETUTCDATE()
			WHERE  CMSMessageId = @CMSMessageId

			SET @CMSMessageId_New = @CMSMessageId

	   END 

	   PRINT CAST( @CMSMessageId AS VARCHAR(1111) )

	   IF EXISTS (SELECT TOP 1 1 FROM [dbo].[ZnodeCMSMessageKey] zcmk WHERE zcmk.MessageKey =  @MessageKey AND @CMSMessageId = 0 )
	   BEGIN
	   
	   SET @MessageKeyDetail =  (SELECT TOP 1 MessageKey FROM [dbo].[ZnodeCMSMessageKey] zcmk WHERE zcmk.MessageKey =  @MessageKey)

	   RAISERROR (15600,-1,-1, ' Is Already Exists '); 
	  
	   END 
	   ELSE IF NOT EXISTS (SELECT TOP 1  1 FROM [ZnodeCMSMessage]  zcm WHERE Zcm.CMSMessageId = @CMSMessageId AND zcm.LocaleId = @LocaleId AND zcm.[Message]   = @Description ) 
	   BEGIN  
	   print '11'
	   --DELETE FROM [ZnodeCMSAreaMessageKey] WHERE CMSMessageKeyId = @CMSMessageKeyId 
	   DELETE FROM ZnodeCMSPortalMessage WHERE CMSMessageKeyId = @CMSMessageKeyId  AND EXISTS (SELECT TOP 1 1 FROM @PortalId a WHERE a.PortalId = ZnodeCMSPortalMessage.PortalId )
																				AND EXISTS (SELECT TOP 1 1 FROM @AreaId a WHERE a.AreaId = ZnodeCMSPortalMessage.CMSAreaId )
																				
	   
	   END 
       ELSE    	  
	   BEGIN -- Delete the existing data for  CMSMessageKey 
	   
	  
	   DELETE FROM ZnodeCMSPortalMessage WHERE CMSMessageKeyId = @CMSMessageKeyId 
	   AND EXISTS (SELECT TOP 1 1 FROM @PortalId a WHERE a.PortalId = ZnodeCMSPortalMessage.PortalId )
																				AND EXISTS (SELECT TOP 1 1 FROM @AreaId a WHERE a.AreaId = ZnodeCMSPortalMessage.CMSAreaId )
	   
	    
	   END 

	   -- 
	   IF @CMSMessageKeyId = 0 
	   BEGIN 
	   INSERT INTO [ZnodeCMSMessageKey] (MessageKey,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
	   
	   VALUES (@MessageKey,@User,GETUTCDATE(),@User,GETUTCDATE())
	   
	   SET @MessageKeyDetail = SCOPE_IDENTITY()
	   END 
	   ELSE 
	   BEGIN 
	   
	   UPDATE [ZnodeCMSMessageKey]
	   SET MessageKey = @MessageKey
	   ,CreatedBy= @User
	   ,CreatedDate = GETUTCDATE()
	   ,ModifiedBy = @User
	   ,ModifiedDate = GETUTCDATE()
	   WHERE CMSMessageKeyId = @CMSMessageKeyId 
	   SET @MessageKeyDetail = @CMSMessageKeyId
	   END 
	   


	   PRINT '11223'
	   
	    --- insert into mapping table 
	   -- INSERT INTO [ZnodeCMSAreaMessageKey](CMSAreaId
		  --                               ,CMSMessageKeyId
				--						,CreatedBy
				--						,CreatedDate
				--						,ModifiedBy
				--						,ModifiedDate)
	   
	   --SELECT AreaId,CAST (@MessageKeyDetail AS INT ),@User,GETUTCDATE(),@User,GETUTCDATE()
	   --FROM @AreaId ai

	   INSERT INTO ZnodeCMSPortalMessage (PortalId
											,CMSAreaId
											,CMSMessageKeyId
											,CMSMessageId
											,CreatedBy
											,CreatedDate
											,ModifiedBy
											,ModifiedDate)

       SELECT a.PortalId ,ai.AreaId,CAST(@MessageKeyDetail AS INT ),@CMSMessageId_New,@User,GETUTCDATE(),@User,GETUTCDATE()
	   FROM @PortalId a 
	   CROSS JOIN  @AreaId ai

	   DELETE FROM 	[ZnodeCMSMessage] WHERE NOT EXISTS (SELECT TOP 1 1 FROM ZnodeCMSPortalMessage ss WHERE ss.CMSMessageId = [ZnodeCMSMessage].CMSMessageId )
	  

	   SELECT @CMSMessageId_New ID ,' Successful ' [MessageDetails] ,CAST(1 AS BIT )[Status]
	   -- output paramater 
	   SET @Status = 1


  COMMIT TRAN A 
	  END TRY 
	  BEGIN CATCH 
	  SELECT ERROR_MESSAGE ()
	   SET @Status = 0
	   SELECT @CMSMessageId ID ,@MessageKeyDetail+'  Is Already Exists'[MessageDetails], CAST(0 AS BIT )[Status]
  ROLLBACK TRAN A    
	  END CATCH 
 END
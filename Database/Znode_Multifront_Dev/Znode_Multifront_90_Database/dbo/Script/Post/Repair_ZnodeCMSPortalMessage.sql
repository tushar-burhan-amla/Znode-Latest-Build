

DECLARE @_MinJID int,@_MaxJID int
IF OBJECT_ID('tempdb..#Temptable') IS NOT NULL
	DROP TABLE #Temptable 
CREATE  TABLE #Temptable (RowId int identity, CMSMessageKeyId INT,CMSMessageId int)
	INSERT INTO #Temptable
	SELECT  CMSMessageKeyId,CMSMessageId
	FROM ZnodeCMSPortalMessage ZCPM
	GROUP BY CMSMessageKeyId,CMSMessageId
	HAVING COUNT(CMSMessageKeyId) - (select count(*) from ZnodeCMSMessage ZCM where ZCM.CMSMessageId=ZCPM.CMSMessageId)> 0

	SELECT @_MinJID = MIN(RowId),@_MaxJID = MAX(RowId)  FROM #Temptable

WHILE @_MinJID <= @_MaxJID
   BEGIN
  
			DECLARE @CMSMessageId int = (select CMSMessageId from #Temptable where RowId = @_MinJID)
			DECLARE @PortalCount int = (select count(*) from ZnodeCMSPortalMessage where CMSMessageKeyId = (select CMSMessageId from #Temptable where RowId = @_MinJID))
			DECLARE @MessageKeyCount int = (select Count(*) from ZnodeCMSMessage where CMSMessageId =@CMSMessageId)
			DECLARE @InsertingRowCount int = @PortalCount - @MessageKeyCount
			DECLARE @index int = 1

			WHILE @index <= @InsertingRowCount
			BEGIN
				DECLARE @firstCMSPortalMessageId int=0,@NewlyCMSMessageId int,@RemainingCMSPortalMessageId INT
				INSERT INTO ZnodeCMSMessage (LocaleId,Message,IsPublished,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PublishStateId)
				select LocaleId,Message,IsPublished,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,PublishStateId FROM ZnodeCMSMessage where CMSMessageId = @CMSMessageId
				SET @NewlyCMSMessageId =SCOPE_IDENTITY()
				SELECT TOP 1 @firstCMSPortalMessageId = CMSPortalMessageId FROM ZnodeCMSPortalMessage WHERE CMSMessageId = @CMSMessageId
				SET  @RemainingCMSPortalMessageId = (SELECT TOP 1 CMSPortalMessageId FROM ZnodeCMSPortalMessage WHERE CMSMessageId = @CMSMessageId and CMSMessageId not in (@firstCMSPortalMessageId))
				UPDATE ZnodeCMSPortalMessage set CMSMessageId =@NewlyCMSMessageId  WHERE CMSPortalMessageId = @RemainingCMSPortalMessageId
				SET @index = @index + 1

			end
			
		SET @_MinJID = @_MinJID + 1;
	END

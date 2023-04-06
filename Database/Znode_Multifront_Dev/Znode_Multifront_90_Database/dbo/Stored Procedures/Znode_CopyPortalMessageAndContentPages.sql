
	 CREATE PROCEDURE [dbo].[Znode_CopyPortalMessageAndContentPages]
(	@CopyPortalId    INT,
    @PortalId        INT,
    @UserId          INT,
    @Status          INT = 1 OUT)
AS 
    
	/*
     Summary :- This procedure is used to copy existing portal messages and content pages to the another portal 
     Fn_GetDefaultValue use to find the defualt value of locale 
     copy only default locale data and active records 
     Affected tables "ZnodeCMSContentPages" copy the record of one portal to another portal 
     "ZnodeCMSContentPagesLocale" copy the record of one portal to another portal 
     "ZnodeCMSPortalMessage" copy the record of one portal to another portal 
     Unit Testing
	 begin tran
     EXEC Znode_CopyPortalMessageAndContentPages @CopyPortalId = 1 ,@PortalId = 1 ,@UserId = 2 ,@Status =0 
     rollback tran  
   */
    
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @GetDate DATETIME = dbo.Fn_GetDate();
             DECLARE @TBL_ContentPageDetail TABLE
             (
			    CMSContentPagesId    INT,
                OldCMSContentPagesId INT,
				PageName NVARCHAR(200)
                
             );
             DECLARE @TBL_CMSSEODetailId TABLE
             (
			    CMSSEODetailId    INT,
                OldCMSSEODetailId INT 
                 
             );
			 -- copy the  messages for portal
			 Declare @messageid table (id int, message varchar(max) null,messagekeyid int null);

			 Insert into ZnodeCMSMessage (LocaleId,	Message,	IsPublished,	CreatedBy,	CreatedDate,	ModifiedBy,	ModifiedDate,	PublishStateId )
			 output inserted.CMSMessageId, inserted.Message into @messageid (id,message)
				select LocaleId,	Message,	IsPublished,@UserId,@GetDate,@UserId,@GetDate,	PublishStateId
					from ZnodeCMSMessage ZCM inner join ZnodeCMSPortalMessage AS ZCPM ON(ZCM.CMSMessageId = ZCPM.CMSMessageId)
					WHERE ZCPM.PortalId = @CopyPortalId;  

			Update m
			set messagekeyid = ZCPM.CMSMessageKeyId
			 FROM ZnodeCMSPortalMessage AS ZCPM
             INNER JOIN ZnodeCMSMessage AS ZCM ON(ZCM.CMSMessageId = ZCPM.CMSMessageId)
			 inner join @messageid m on m.message = ZCM.message
			WHERE ZCPM.PortalId = @CopyPortalId; 

             INSERT INTO ZnodeCMSPortalMessage (PortalId, CMSMessageKeyId,CMSMessageId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
             SELECT DISTINCT @PortalId,ZCPM.CMSMessageKeyId,m.id,@UserId,@GetDate,@UserId,@GetDate
             FROM ZnodeCMSPortalMessage AS ZCPM
             INNER JOIN ZnodeCMSMessage AS ZCM ON(ZCM.CMSMessageId = ZCPM.CMSMessageId)
			 inner join @messageid m on ZCPM.CMSMessageKeyId =m.messagekeyid
             WHERE ZCPM.PortalId = @CopyPortalId;  
            
			 INSERT INTO ZnodeCMSPortalMessageKeyTag (PortalId,CMSMessageKeyId,TagXML,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
             SELECT @PortalId,CMSMessageKeyId,TagXML,@UserId,@GetDate,@UserId,@GetDate
             FROM ZnodeCMSPortalMessageKeyTag ZCPMT
             WHERE PortalId = @CopyPortalId;
			 -- this cte use to collect the required data to merge with ZnodeCMSContentPages 
             WITH Cte_CMSContentPages
             AS 
             (SELECT DISTINCT ZCCP.CMSContentPagesId,@PortalId AS PortalId,CMSTemplateId,PageName,ActivationDate,ExpirationDate,IsActive,@UserId AS CreatedBy,@GetDate AS CreatedDate,@UserId AS ModifiedBy,@GetDate AS ModifiedDate 
	         FROM ZnodeCMSContentPages AS ZCCP
             WHERE IsActive = 1 AND PortalId = @CopyPortalId)
			 -- merge use for catching the output 
             MERGE INTO ZnodeCMSContentPages TARGET
             USING Cte_CMSContentPages SOURCE 
             ON 1 = 0
                      WHEN NOT MATCHED
                      THEN INSERT(PortalId,CMSTemplateId,PageName,ActivationDate,ExpirationDate,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
					  VALUES (SOURCE.PortalId,SOURCE.CMSTemplateId,SOURCE.PageName,SOURCE.ActivationDate,SOURCE.ExpirationDate,SOURCE.IsActive,SOURCE.CreatedBy,SOURCE.CreatedDate,SOURCE.ModifiedBy,SOURCE.ModifiedDate)
                      OUTPUT Inserted.CMSContentPagesId,
                      SOURCE.CMSContentPagesId,Inserted.PageName 
                      INTO @TBL_ContentPageDetail;

             -- here collect the inserted data for further use 

             INSERT INTO ZnodeCMSContentPageGroupMapping (CMSContentPageGroupId,CMSContentPagesId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
             SELECT CMSContentPageGroupId,TBCPD.CMSContentPagesId,@UserId,@GetDate,@userId,@GetDate
			 FROM ZnodeCMSContentPageGroupMapping ZCCPGM
             INNER JOIN @TBL_ContentPageDetail TBCPD ON(TBCPD.OldCMSContentPagesId = ZCCPGM.CMSContentPagesId);

             INSERT INTO ZnodeCMSContentPagesLocale(CMSContentPagesId,LocaleId,PageTitle,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
             SELECT TBCPD.CMSContentPagesId,LocaleId,PageTitle,@UserId,@GetDate,@userId,@GetDate -- insert default locale records 
             FROM ZnodeCMSContentPagesLocale AS ZCCPL
             INNER JOIN @TBL_ContentPageDetail AS TBCPD ON(TBCPD.OldCMSContentPagesId = ZCCPL.CMSContentPagesId);

             INSERT INTO ZnodeCMSContentPagesProfile (ProfileId,CMSContentPagesId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
             SELECT ProfileId,TBCPD.CMSContentPagesId,@UserId,@GetDate,@userId,@GetDate
             FROM ZnodeCMSContentPagesProfile ZCPP
             INNER JOIN @TBL_ContentPageDetail TBCPD ON(TBCPD.OldCMSContentPagesId = ZCPP.CMSContentPagesId)
             WHERE EXISTS  ( SELECT TOP 1 1 FROM ZnodePortalProfile ZPP WHERE ZPP.PortalId = @CopyPortalId AND isnull(ZPP.ProfileId,0) = isnull(ZCPP.ProfileId,0) )
             -- this cte use to collect the required data to merge with ZnodeCMSContentPages 
			 ;WITH Cte_CMSSEODetails
             AS  
             (
			 SELECT ZCSD.CMSSEODetailId,ZCSD.CMSSEOTypeId,TBCPD.CMSContentPagesId,IsRedirect,MetaInformation,@PortalId PortalId,SEOUrl,@UserId CREATEDBy,@GetDate CREATEDDATE,@userId MODIFIEDBY,@GetDate MODIFIEDDATE
			 ,ZCSD.IsPublish,ZCSD.SEOCode
             FROM ZnodeCMSSEODetail ZCSD
             INNER JOIN @TBL_ContentPageDetail TBCPD ON(TBCPD.PageName = ZCSD.SEOCode)
             INNER JOIN ZnodeCMSSEOType ZCST ON(ZCST.CMSSEOTypeId = ZCSD.CMSSEOTypeId AND ZCST.Name = 'Content Page')
			 WHERE ZCSD.PortalId = @CopyPortalId)
			 -- merge use for catching the output
             MERGE INTO ZnodeCMSSEODetail TARGET
             USING Cte_CMSSEODetails SOURCE 
             ON 1 = 0
             WHEN NOT MATCHED
             THEN INSERT(CMSSEOTypeId,SEOId,IsRedirect,MetaInformation,PortalId,SEOUrl,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsPublish,SEOCode)
			 VALUES (SOURCE.CMSSEOTypeId,NULL,SOURCE.IsRedirect,SOURCE.MetaInformation,SOURCE.PortalId,SOURCE.SEOUrl,SOURCE.CreatedBy,SOURCE.CreatedDate,SOURCE.ModifiedBy,SOURCE.ModifiedDate,SOURCE.IsPublish,SOURCE.SEOCode)
             OUTPUT Inserted.CMSSEODetailId,
             SOURCE.CMSSEODetailId 
             INTO @TBL_CMSSEODetailId;

             INSERT INTO ZnodeCMSSEODetailLocale (CMSSEODetailId,LocaleId,SEOTitle,SEODescription,SEOKeywords,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate, CanonicalURL, RobotTag)
             SELECT TBCPD.CMSSEODetailId,ZCSDl.LocaleId,ZCSDl.SEOTitle,ZCSDl.SEODescription,ZCSDl.SEOKeywords,@UserId as CreatedBy,@GetDate as CreatedDate,@userId as ModifiedBy,@GetDate as ModifiedDate, ZCSDl.CanonicalURL, ZCSDl.RobotTag
             FROM ZnodeCMSSEODetailLocale ZCSDl
             INNER JOIN @TBL_CMSSEODetailId TBCPD ON(TBCPD.OldCMSSEODetailId = ZCSDl.CMSSEODetailId);
                          
			 INSERT INTO ZnodePortalCountry (PortalId,CountryCode,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate) 
			 SELECT @PortalId,CountryCode,IsDefault,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate
			 FROM ZnodePortalCountry ZPC
			 WHERE PortalId = @CopyPortalId
			 AND IsDefault = 1 
			 AND NOT EXISTS (SELECT TOP 1 1 FROM ZnodePortalCountry ZPPP WHERE ZPPP.PortalId = @PortalId AND ZPPP.CountryCode = CountryCode );

			 EXEC Znode_CopyPortalEmailTemplate @PortalId,@CopyPortalId,@userId
			 SET @Status = 1;
			 SELECT @CopyPortalId AS ID,CAST(1 AS BIT) AS [Status];
         END TRY
         BEGIN CATCH
             DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= ' EXEC Znode_CopyPortalMessageAndContentPages @CopyPortalId = '+CAST(@CopyPortalId AS VARCHAR(100))+' ,@PortalId='+CAST(@PortalId AS VARCHAR(100))+' ,@UserId= '+CAST(@UserId AS VARCHAR(100))+',@Status='+CAST(@Status AS VARCHAR(50));
             SELECT @PortalId AS ID,
                    CAST(0 AS BIT) AS [Status];
             SET @Status = 0;
             EXEC Znode_InsertProcedureErrorLog
                  @ProcedureName = 'Znode_CopyPortalMessageAndContentPages',
                  @ErrorInProcedure = @Error_procedure,
                  @ErrorMessage = @ErrorMessage,
                  @ErrorLine = @ErrorLine,
                  @ErrorCall = @ErrorCall;
         END CATCH;
     END;
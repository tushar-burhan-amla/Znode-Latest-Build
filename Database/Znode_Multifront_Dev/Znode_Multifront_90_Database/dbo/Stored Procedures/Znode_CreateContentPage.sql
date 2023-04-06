



CREATE  procedure [dbo].[Znode_CreateContentPage]
(
 @ContentPageXML   XML
,@Status       Bit OUT 
,@UserId int
)
AS 



--<ContentPageModel>
--  <CMSContentPagesId>0</CMSContentPagesId>
--  <PortalId>2</PortalId>
--  <CMSContentPageGroupId>2</CMSContentPageGroupId>
--  <ProfileIds>5,4,1</ProfileIds>
--  <CMSTemplateId>1</CMSTemplateId>
--  <LocaleId>1</LocaleId>
--  <CMSContentPagesLocaleId>0</CMSContentPagesLocaleId>
--  <PageTitle>dsfdsfsd</PageTitle>
--  <PageName>fsdfsdfd</PageName>
--  <SEOTitle>fdsfds</SEOTitle>
--  <SEODescription>fdsfdsf</SEODescription>
--  <SEOKeywords>fdsfsd</SEOKeywords>
--  <SEOUrl>dsfdsf</SEOUrl>
--  <MetaInformation>dsfsdfsd</MetaInformation>
--  <IsRedirect>false</IsRedirect>
--  <IsConfigurable>false</IsConfigurable>
--  <ActivationDate>2016-08-04</ActivationDate>
--  <ExpirationDate>2016-08-12</ExpirationDate>
--</ContentPageModel>



BEGIN 
BEGIN TRAN A 
BEGIN TRY 


			 DECLARE @InsertContentPage  TABLE 
				(
					CMSContentPagesId		int ,
					PortalId				varchar(300),
					CMSContentPageGroupId  varchar(300),
					ProfileIds             varchar(300),
					CMSTemplateId			varchar(300),
					LocaleId		varchar(300),
					CMSContentPagesLocaleId		varchar(300),
					PageTitle				varchar(300),
					PageName		varchar(300),
					SEOTitle				varchar(100),
					SEODescription				nvarchar(max),
					SEOKeywords				nvarchar(max),
					SEOUrl				nvarchar(max),
					MetaInformation				nvarchar(max),
					IsRedirect	VARCHAR(200),
					IsConfigurable	VARCHAR(600),
					ActivationDate	varchar(300) NULL,
					ExpirationDate	varchar(300) NULL 
				)

				INSERT INTO @InsertContentPage
		SELECT  
				Tbl.Col.value('CMSContentPagesId[1]', 'VARCHAR(300)'),  
				Tbl.Col.value('PortalId[1]', 'VARCHAR(300)'),  
				Tbl.Col.value('CMSContentPageGroupId[1]', 'VARCHAR(300)'),
				Tbl.Col.value('ProfileIds[1]', 'VARCHAR(300)'),
				Tbl.Col.value('CMSTemplateId[1]', 'VARCHAR(300)'),
				Tbl.Col.value('LocaleId[1]', 'VARCHAR(300)'),
				Tbl.Col.value('CMSContentPagesLocaleId[1]', 'VARCHAR(300)'),
				Tbl.Col.value('PageTitle[1]', 'VARCHAR(300)'),
				Tbl.Col.value('PageName[1]', 'VARCHAR(200)'), 
				Tbl.Col.value('SEOTitle[1]', 'VARCHAR(300)'),
				Tbl.Col.value('SEODescription[1]', 'VARCHAR(300)'),
				Tbl.Col.value('SEOKeywords[1]', 'VARCHAR(300)'),
				Tbl.Col.value('SEOUrl[1]', 'VARCHAR(300)'),
				Tbl.Col.value('MetaInformation[1]', 'VARCHAR(300)'),
				Tbl.Col.value('IsRedirect[1]', 'VARCHAR(600)'), 
				Tbl.Col.value('IsConfigurable[1]', 'VARCHAR(300)'),
				Tbl.Col.value('ActivationDate[1]', 'VARCHAR(300)'),
				Tbl.Col.value('ExpirationDate[1]', 'VARCHAR(300)')
		 FROM   @ContentPageXML.nodes('/ContentPageModel') Tbl(Col)
		

			--insert into @InsertContentPage (CMSContentPagesId,PortalId ,ProfileIds ,CMSTemplateId,LocaleId,CMSContentPagesLocaleId 
			--								,PageTitle,PageName,IsRedirect,IsConfigurable ,	ActivationDate,ExpirationDate)  
			--Select CMSContentPagesId,PortalId ,ProfileIds ,CMSTemplateId,LocaleId,CMSContentPagesLocaleId ,PageTitle,PageName
			--						,IsRedirect,IsConfigurable,ActivationDate,ExpirationDate from @InsertContentPageForValidation

			insert into ZnodeCMSContentPages (PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate,ExpirationDate
												,IsActive,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			select PortalId,CMSTemplateId,PageTitle,PageName,ActivationDate,ExpirationDate
							,IsConfigurable,@UserId, GETUTCDATE(),@UserId,GETUTCDATE() 
			from @InsertContentPage

			insert into ZnodeCMSSEODetail(CMSSEOTypeId,SEOId,SEOTitle,SEODescription,SEOKeywords,SEOUrl,IsRedirect
											,MetaInformation,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			Select b.CMSSEOTypeId,1,SEOTitle,SEODescription,SEOKeywords,SEOUrl
					,IsRedirect,MetaInformation,@UserId, GETUTCDATE(),@UserId,GETUTCDATE() 
			from @InsertContentPage a 
			INNER JOIN ZnodeCMSSEOType b ON (name = 'Content Page' )

			DECLARE @Profiledata TABLE (ProfileId INT )
			SELECT item 
			FROM dbo.Split((SElect top 1 ProfileIds from @InsertContentPage ),',') 

			insert into ZnodeCMSContentPagesProfile(CMSContentPagesId,ProfileId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			select q.CMSContentPagesId ,s.ProfileId
			,@UserId, GETUTCDATE(),@UserId,GETUTCDATE()
			 from @InsertContentPage a 
			 LEFT JOIN  ZnodeCMSContentPages  q ON (a.PageName = q.PageName)
			 CROSS APPLY @Profiledata s

			insert into ZnodeCMSContentPageGroupMapping(CMSContentPageGroupId,CMSContentPagesId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate)
			select CMSContentPageGroupId, b.CMSContentPagesId,@UserId, GETUTCDATE(),@UserId,GETUTCDATE() 
			
			from @InsertContentPage a 
			INNER JOIN ZnodeCMSContentPages b ON (a.pageName = b.PageName)

			--select ZCP.CMSContentPagesId,ZCP.PortalId,ZCP.CMSTemplateId,ZCP.PageTitle,ZCP.PageName,ZCP.ActivationDate,ZCP.ExpirationDate,ZSD.SEOTitle,ZSD.SEODescription,ZSD.SEOKeywords,ZSD.SEOUrl,ZSD.IsRedirect,ZSD.MetaInformation
			--from ZnodeCMSContentPages ZCP inner join ZnodeCMSSEODetail ZSD on ZCP.CMSContentPagesId = ZSD.SEOId
			SET @Status = 1 
	COMMIT TRAN A 

END TRY 
BEGIN CATCH 
	--SELECT 0 ID , cast(0 As Bit ) Status  
	SET @Status = 0
	SELECT ERROR_LINE(),ERROR_MESSAGE(),ERROR_PROCEDURE()
	ROLLBACK TRAN A 

END CATCH 
END


--select * from ZnodeCMSContentPages ZCP inner join  @InsertContentPage IP on ZCP.PageName = IP.PageName
  
CREATE PROCEDURE [dbo].[Znode_InsertUpdateContentPage]
(   @ContentPageXML XML,
	@UserId         INT,
    @Status         BIT OUT)
AS 
   /* 
    Summary : To Insert content page details with their referance table 
    If CMSContentPagesId =0 then insert else update 
    Unit Testing 
    <ContentPageModel>
      <CMSContentPagesId>0</CMSContentPagesId>
      <PortalId>2</PortalId>
      <CMSContentPageGroupId>2</CMSContentPageGroupId>
      <ProfileIds>5,4,1</ProfileIds>
      <CMSTemplateId>1</CMSTemplateId>
      <LocaleId>1</LocaleId>
      <CMSContentPagesLocaleId>0</CMSContentPagesLocaleId>
      <PageTitle>dsfdsfsd</PageTitle>
      <PageName>fsdfsdfd</PageName>
      <SEOTitle>fdsfds</SEOTitle>
      <SEODescription>fdsfdsf</SEODescription>
      <SEOKeywords>fdsfsd</SEOKeywords>
      <SEOUrl>dsfdsf</SEOUrl>
      <MetaInformation>dsfsdfsd</MetaInformation>
      <IsRedirect>false</IsRedirect>
      <IsConfigurable>false</IsConfigurable>
      <ActivationDate>2016-08-04</ActivationDate>
      <ExpirationDate>2016-08-12</ExpirationDate>
    </ContentPageModel>
  
	*/
     BEGIN
         BEGIN TRAN A;
         BEGIN TRY
             DECLARE @Profiledata TABLE(ProfileId INT);
			 DECLARE @GetDate DATETIME = dbo.Fn_GetDate(),@PublishStateId INT = dbo.Fn_GetPublishStateIdForDraftState() ;
             DECLARE @DefaultlocaleId INT= dbo.Fn_GetDefaultLocaleId();
             DECLARE @NewCMSContentPagesId INT;
             DECLARE @CmsSeoDetailId INT;
             DECLARE @InsertContentPage TABLE
             (CMSContentPagesId       INT,
              PortalId                INT,
              CMSContentPageGroupId   INT,
              ProfileIds              VARCHAR(MAX),
              CMSTemplateId           INT,
              LocaleId                INT,
              CMSContentPagesLocaleId INT,
              PageTitle               NVARCHAR(200),
              PageName                NVARCHAR(200),
              SEOTitle                NVARCHAR(MAX),
              SEODescription          NVARCHAR(MAX),
              SEOKeywords             NVARCHAR(MAX),
              SEOUrl                  NVARCHAR(MAX),
              MetaInformation         NVARCHAR(MAX),
              IsRedirect              BIT,
              IsActive                BIT,
              ActivationDate          Datetime NULL ,
              ExpirationDate          Datetime NULL,
			  IsPublished			  BIT,
			  CanonicalURL            VARCHAR(200),
			  RobotTag                VARCHAR(50)
             );
             INSERT INTO @InsertContentPage(CMSContentPagesId, PortalId, CMSContentPageGroupId, ProfileIds, CMSTemplateId ,LocaleId ,CMSContentPagesLocaleId ,PageTitle ,PageName ,SEOTitle ,
											SEODescription ,SEOKeywords ,SEOUrl ,MetaInformation ,IsRedirect ,IsActive ,ActivationDate ,ExpirationDate ,IsPublished ,CanonicalURL ,RobotTag)
                    SELECT Tbl.Col.value('CMSContentPagesId[1]', 'INT'),
                           Tbl.Col.value('PortalId[1]', ' INT '),
                           Tbl.Col.value('CMSContentPageGroupId[1]', 'INT'),
                           Tbl.Col.value('ProfileIds[1]', 'VARCHAR(max)'),
                           Tbl.Col.value('CMSTemplateId[1]', 'INT'),
                           Tbl.Col.value('LocaleId[1]', 'INT'),
                           Tbl.Col.value('CMSContentPagesLocaleId[1]', 'INT '),
                           Tbl.Col.value('PageTitle[1]', 'NVARCHAR(200)'),
                           Tbl.Col.value('PageName[1]', 'NVARCHAR(300)'),
                           Tbl.Col.value('SEOTitle[1]', 'NVARCHAR(max)'),
                           Tbl.Col.value('SEODescription[1]', 'NVARCHAR(MAX)'),
                           Tbl.Col.value('SEOKeywords[1]', 'NVARCHAR(MAX)'),
                           Tbl.Col.value('SEOUrl[1]', 'VARCHAR(300)'),
                           Tbl.Col.value('MetaInformation[1]', 'NVARCHAR(MAX)'),
                           Tbl.Col.value('IsRedirect[1]', 'BIT'),
                           Tbl.Col.value('IsActive[1]', 'BIT'),
					  nullif(Tbl.Col.value('ActivationDate[1]', 'Datetime'), '1900-01-01 00:00:00.000'),
					  nullif(Tbl.Col.value('ExpirationDate[1]', 'Datetime'), '1900-01-01 00:00:00.000'),
					       Tbl.Col.value('IsPublished[1]', 'BIT'),
						   Tbl.Col.value('CanonicalURL[1]', 'VARCHAR(200)'),
						   Tbl.Col.value('RobotTag[1]', 'VARCHAR(50)')
                    FROM @ContentPageXML.nodes('/ContentPageModel') AS Tbl(Col);
             SET @NewCMSContentPagesId = 0;
             UPDATE @InsertContentPage
               SET
                   ActivationDate = NULL
             WHERE ISNULL(RTRIM(LTRIM(ActivationDate)), '') = '';
             UPDATE @InsertContentPage
               SET
                   ExpirationDate = NULL
             WHERE ISNULL(LTRIM(RTRIM(ExpirationDate)), '') = '';


             IF EXISTS
             (
                 SELECT TOP 1 1
                 FROM @InsertContentPage
                 WHERE CMSContentPagesId = 0
             )
                 BEGIN
				 
                     INSERT INTO ZnodeCMSContentPages
                     (PortalId,
                      CMSTemplateId,
                      PageName,
                      ActivationDate,
                      ExpirationDate,
                      IsActive,
                      CreatedBy,
                      CreatedDate,
                      ModifiedBy,
                      ModifiedDate,PublishStateId 
                     )
                            SELECT PortalId,
                                   CMSTemplateId,
                                   PageName,
                                   ActivationDate,
                                   ExpirationDate,
                                   IsActive,
                                   @UserId,
                                   @GetDate,
                                   @UserId,
                                   @GetDate,@PublishStateId
                            FROM @InsertContentPage;
                     SET @NewCMSContentPagesId = @@Identity;
                     INSERT INTO ZnodeCMSContentPagesLocale
                     (CMSContentPagesId,
                      LocaleId,
                      PageTitle,
                      CreatedBy,
                      CreatedDate,
                      ModifiedBy,
                      ModifiedDate
                     )
                            SELECT @NewCMSContentPagesId,
                                   LocaleId,
                                   PageTitle,
                                   @UserId,
                                   @GetDate,
                                   @UserId,
                                   @GetDate
                            FROM @InsertContentPage;


				--IF EXISTS (SELECT TOP 1 1 FROM @InsertContentPage WHERE --(SEOTitle IS NOT NULL OR SEOTitle <> '') AND 
				--(SEOUrl IS NOT NULL OR SEOUrl <> '' ) )
				--	BEGIN 
					
                     INSERT INTO ZNODECMSSEODETAIL
                     (CMSSEOTYPEID,
                      SEOID,
                      ISREDIRECT,
                      MetaInformation,
                      PortalId,
                      SEOUrl,
                      CreatedBy,
                      CreatedDate,
                      ModifiedBy,
                      ModifiedDate,
					  IsPublish,
					  SEOCode,PublishStateId
                     )
                            SELECT b.CMSSEOTypeId,
                                   NULL,
                                   IsRedirect,
                                   MetaInformation,
                                   PortalId,
                                   SEOUrl,
                                   @UserId,
                                   @GetDate,
                                   @UserId,
                                   @GetDate,
								   a.IsPublished,
								   PageName, 
								   @PublishStateId
								   --CASE WHEN SEOUrl = '' OR SEOurl IS NULL THEN '' ELSE @PublishStateId END PublishStateId
                            FROM @InsertContentPage AS a
                                 INNER JOIN ZnodeCMSSEOType AS b ON(name = 'Content Page');
                     SET @CmsSeoDetailId = SCOPE_IDENTITY();
                     INSERT INTO ZnodeCMSSEODetailLocale
                     (CMSSEODetailId,
                      LocaleId,
                      SEOTitle,
                      SEODescription,
                      SEOKeywords,
                      CreatedBy,
                      CreatedDate,
                      ModifiedBy,
                      ModifiedDate,
					  CanonicalURL,
					  RobotTag
                     )
                            SELECT @CmsSeoDetailId,
                                   @DefaultlocaleId,
                                   SEOTitle,
                                   SEODescription,
                                   SEOKeywords,
                                   @UserId,
                                   @GetDate,
                                   @UserId,
                                   @GetDate,
								   CanonicalURL,
								   RobotTag
                            FROM @InsertContentPage AS a
                                 INNER JOIN ZnodeCMSSEOType AS b ON(name = 'Content Page')
					--END
					
                     INSERT INTO @Profiledata
                            SELECT item
                            FROM dbo.Split
                            (
                            (
                                SELECT TOP 1 ProfileIds
                                FROM @InsertContentPage
                            ), ','
                            );
                     INSERT INTO ZnodeCMSContentPagesProfile
                     (CMSContentPagesId,
                      ProfileId,
                      CreatedBy,
                      CreatedDate,
                      ModifiedBy,
                      ModifiedDate
                     )
                            SELECT @NewCMSContentPagesId,
                                   s.ProfileId,
                                   @UserId,
                                   @GetDate,
                                   @UserId,
                                   @GetDate
                            FROM @InsertContentPage AS a
                                 CROSS APPLY @Profiledata AS s;
								 
                     INSERT INTO ZnodeCMSContentPageGroupMapping
                     (CMSContentPageGroupId,
                      CMSContentPagesId,
                      CreatedBy,
                      CreatedDate,
                      ModifiedBy,
                      ModifiedDate
                     )
                            SELECT CMSContentPageGroupId,
                                   @NewCMSContentPagesId,
                                   @UserId,
                                   @GetDate,
                                   @UserId,
                                   @GetDate
                            FROM @InsertContentPage AS a;

                     SELECT ZCP.CMSContentPagesId,
                            ZCP.PortalId,
                            ZCP.CMSTemplateId,
                            ZCCPL.PageTitle,
                            ZCP.PageName,
                            ZCP.ActivationDate,
                            ZCP.ExpirationDate,
                            ZSDl.SEOTitle,
                            ZSDl.SEODescription,
                            ZSDl.SEOKeywords,
                            ZSD.SEOUrl,
                            ZSD.IsRedirect,
                            ZSD.MetaInformation,
                            ICP.ProfileIds,
							ZSDL.CanonicalURL, 
							ZSDL.RobotTag
                     FROM ZnodeCMSContentPages AS ZCP
                          LEFT JOIN ZnodeCMSSEODetail AS ZSD ON ZCP.PageName = ISNULL(ZSD.SEOCode,'')
                          LEFT JOIN ZnodeCMSSEODetailLocale AS ZSDl ON(ZSDL.CMSSEODetailId = ZSD.CMSSEODetailId
                                                                        AND ZSDl.LocaleId = @DefaultlocaleId)
                          LEFT JOIN ZnodeCMSSEOType AS ZCST ON ZCST.CMSSEOTypeId = ZSD.CMSSEOTypeId
                                                                AND ZCST.name = 'Content Page'
                          INNER JOIN @InsertContentPage AS ICP ON ZCP.PageName = ICP.PageName
                          INNER JOIN ZnodeCMSContentPagesLocale AS ZCCPL ON ZCCPL.CMSContentPagesId = ZCP.CMSContentPagesId
                                                                            AND ZCCPL.LocaleId = @DefaultlocaleId
                     WHERE ZCP.CMSContentPagesId = @NewCMSContentPagesId;
			

                 END;
             ELSE
                 BEGIN
                     UPDATE ZCCP
                       SET
                           ZCCP.PortalId = ICP.PortalId,
                           ZCCP.CMSTemplateId = ICP.CMSTemplateId,
                           ZCCP.PageName = ICP.PageName,
                           ZCCP.ActivationDate = ICP.ActivationDate,
                           ZCCP.ExpirationDate = ICP.ExpirationDate,
                           ZCCP.IsActive = ICP.IsActive,
                           ZCCP.ModifiedBy = @UserId,
                           ZCCP.ModifiedDate = @GetDate,
						   ZCCP.PublishStateId = @PublishStateId
                     FROM ZnodeCMSContentPages ZCCP
                          INNER JOIN @InsertContentPage ICP ON ZCCP.CMSContentPagesId = ICP.CMSContentPagesId;
                     
					 UPDATE ZCCPL
                     SET ZCCPL.PageTitle = ICP.PageTitle
                     FROM ZnodeCMSContentPagesLocale ZCCPL
                     INNER JOIN @InsertContentPage ICP ON ZCCPL.CMSContentPagesId = ICP.CMSContentPagesId
                                                               AND ZCCPL.LocaleId = ICP.LocaleId;
                     INSERT INTO ZnodeCMSContentPagesLocale
                     (CMSContentPagesId,
                      LocaleId,
                      PageTitle,
                      CreatedBy,
                      CreatedDate,
                      ModifiedBy,
                      ModifiedDate
                     )
                            SELECT CMSContentPagesId,
                                   LocaleId,
                                   PageTitle,
                                   @UserId,
                                   @GetDate,
                                   @UserId,
                                   @GetDate
                            FROM @InsertContentPage TBICP
                            WHERE NOT EXISTS
                            (
                                SELECT TOP 1 1
                                FROM ZnodeCMSContentPagesLocale ZCCPL
                                WHERE ZCCPL.CMSContentPagesId = TBICP.CMSContentPagesId
                                      AND ZCCPL.LocaleId = TBICP.LocaleId
                            );


                     UPDATE ZCSD
                       SET
                           ZCSD.IsRedirect = ICP.IsRedirect,
                           ZCSD.MetaInformation = ICP.MetaInformation,
                           ZCSD.SEOUrl = ICP.SEOUrl,
                           ZCSD.ModifiedBy = @UserId,
                           ZCSD.ModifiedDate = @GetDate,
						   ZCSD.IsPublish = ICP.IsPublished ,--case when ICP.SEOUrl IS NULL OR ICP.SEOUrl = ''  then 0 else 1 end,
						   ZCSD.SEOCode=ICP.PageName,
						   ZCSD.PublishStateId = @PublishStateId 
                     FROM ZnodeCMSSEODetail ZCSD
                          INNER JOIN ZnodeCMSContentPages ZCCP ON ISNULL(ZCSD.SEOCode,'') = ZCCP.PageName
                          INNER JOIN ZnodeCMSSEOType ZCST ON ZCSD.CMSSEOTypeId = ZCST.CMSSEOTypeId
                                                             AND ZCST.Name = 'Content Page'
                          INNER JOIN @InsertContentPage ICP ON ZCCP.CMSContentPagesId = ICP.CMSContentPagesId;

						     

                   
				     UPDATE ZCSDL
                       SET
                           ZCSDl.SEOTitle = ICP.SEOTitle,
                           ZCSDl.SEODescription = ICP.SEODescription,
                           ZCSDl.SEOKeywords = ICP.SEOKeywords,
                           ZCSDl.ModifiedBy = @UserId,
                           ZCSDl.ModifiedDate = @GetDate,
						   ZCSDl.CanonicalURL = ICP.CanonicalURL,
						   ZCSDl.RobotTag = ICP.RobotTag
                     FROM ZnodeCMSSEODetailLocale ZCSDL
                          INNER JOIN ZnodeCMSSEODetail ZCSD ON(ZCSD.CMSSEODetailId = ZCSDL.CMSSEODetailId)
                          INNER JOIN ZnodeCMSContentPages ZCCP ON ISNULL(ZCSD.SEOCode,'') = ZCCP.PageName
                          INNER JOIN ZnodeCMSSEOType ZCST ON ZCSD.CMSSEOTypeId = ZCST.CMSSEOTypeId
                                                             AND ZCST.Name = 'Content Page'
                          INNER JOIN @InsertContentPage ICP ON ZCCP.CMSContentPagesId = ICP.CMSContentPagesId
                                                               AND ZCSDL.LocaleId = ICP.LocaleId
                                                               AND ZCSDL.LocaleId = ICP.LocaleId;

					--IF EXISTS (SELECT TOP 1 1 FROM @InsertContentPage WHERE --(SEOTitle IS NOT NULL OR SEOTitle <> '') AND 
					--(SEOUrl IS NOT NULL OR SEOUrl <> '' ) )
					--BEGIN 

                      INSERT INTO ZnodeCMSSEODetail (CMSSEOTypeId,SEOId,IsRedirect,MetaInformation,PortalId,SEOUrl,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,IsPublish,SEOCode,PublishStateId)					   
					   SELECT  CMSSEOTypeId,NULL,IsRedirect,MetaInformation   ,PortalId ,SEOUrl ,@UserId, @GetDate, @UserId, @GetDate,
					   IsPublished, 
					   PageName ,  
					   @PublishStateId 
             			FROM  @InsertContentPage  TBL
						LEFT JOIN ZnodeCMSSEOType ZCST   ON (ZCST.Name = 'Content Page')
						 WHERE NOT EXISTS
                            (
                                SELECT TOP 1 1
                                FROM ZnodeCMSSEODetail ZCSODL
                                WHERE ZCSODL.SEOCode = TBL.PageName
                                     
                            );
					    
					 INSERT INTO ZnodeCMSSEODetailLocale
                     (CMSSEODetailId,
                      LocaleId,
                      SEOTitle,
                      SEODescription,
                      SEOKeywords,
                      CreatedBy,
                      CreatedDate,
                      ModifiedBy,
                      ModifiedDate,
					  CanonicalURL,
					  RobotTag
                     )
                            SELECT ZCSD.CMSSEODetailId,
                                   ICP.LocaleId,
                                   ICP.SEOTitle,
                                   ICP.SEODescription,
                                   ICP.SEOKeywords,
                                   @UserId,
                                   @GetDate,
                                   @UserId,
                                   @GetDate,
								   ICP.CanonicalURL,
								   ICP.RobotTag
                            FROM ZnodeCMSSEODetail ZCSD
                                 INNER JOIN ZnodeCMSContentPages ZCCP ON ISNULL(ZCSD.SEOCode,'') = ZCCP.PageName
                                 INNER JOIN ZnodeCMSSEOType ZCST ON ZCSD.CMSSEOTypeId = ZCST.CMSSEOTypeId
                                                                    AND ZCST.Name = 'Content Page'
                                 INNER JOIN @InsertContentPage ICP ON ZCCP.CMSContentPagesId = ICP.CMSContentPagesId
                            WHERE NOT EXISTS
                            (
                                SELECT TOP 1 1
                                FROM ZnodeCMSSEODetailLocale ZCSODL
                                WHERE ZCSODL.CMSSEODetailId = ZCSD.CMSSEODetailId
                                      AND ZCSODL.LocaleId = ICP.LocaleId
                            )
					--	END

                     INSERT INTO @Profiledata
                            SELECT item
                            FROM dbo.Split
                            (
                            (
                                SELECT TOP 1 ProfileIds
                                FROM @InsertContentPage
                            ), ','
                            );
                     DELETE FROM ZnodeCMSContentPagesProfile
                     WHERE CMSContentPagesId IN
                     (
                         SELECT CMSContentPagesId
                         FROM @InsertContentPage
                     );
                     INSERT INTO ZnodeCMSContentPagesProfile
                     (CMSContentPagesId,
                      ProfileId,
                      CreatedBy,
                      CreatedDate,
                      ModifiedBy,
                      ModifiedDate
                     )
                            SELECT q.CMSContentPagesId,
                                   s.ProfileId,
                                @UserId,
                                   @GetDate,
                                   @UserId,
                                   @GetDate
                            FROM @InsertContentPage AS a
                                 LEFT JOIN ZnodeCMSContentPages AS q ON(a.CMSContentPagesId = q.CMSContentPagesId)
                                 CROSS APPLY @Profiledata AS s;
                    
					 DELETE FROM ZnodeCMSContentPageGroupMapping
                     WHERE CMSContentPagesId IN
                     (
                         SELECT CMSContentPagesId
                         FROM @InsertContentPage    
                     );
                     
					 
					  
				     INSERT INTO ZnodeCMSContentPageGroupMapping
                     (CMSContentPageGroupId,
                      CMSContentPagesId,
                      CreatedBy,
                      CreatedDate,
                      ModifiedBy,
                      ModifiedDate
                     )
                            SELECT CMSContentPageGroupId,
                                   b.CMSContentPagesId,
                                   @UserId,
                                   @GetDate,
                                   @UserId,
                                   @GetDate
                            FROM @InsertContentPage AS a
                                 INNER JOIN ZnodeCMSContentPages AS b ON(a.CMSContentPagesId = b.CMSContentPagesId AND a.pageName = b.PageName)
						    WHERE  NOT EXISTS
                            (
                                SELECT TOP 1 1
                                FROM ZnodeCMSContentPageGroupMapping ZCCPL
                                WHERE ZCCPL.CMSContentPagesId = b.CMSContentPagesId
                                      AND ZCCPL.CMSContentPageGroupId = a.CMSContentPageGroupId
                            );
						  	
                     SELECT ZCP.CMSContentPagesId,
                            ZCP.PortalId,
                            ZCP.CMSTemplateId,
                            ZCPL.PageTitle,
                            ZCP.PageName,
                            ZCP.ActivationDate,
                            ZCP.ExpirationDate,
                            ZSDl.SEOTitle,
                            ZSDl.SEODescription,
                            ZSDl.SEOKeywords,
                            ZSD.SEOUrl,
                            ZSD.IsRedirect,
                            ZSD.MetaInformation,
                            ICP.ProfileIds,
							ZSDL.CanonicalURL,
							ZSDL.RobotTag
                     FROM ZnodeCMSContentPages AS ZCP
                          LEFT JOIN ZnodeCMSSEODetail AS ZSD ON ZCP.PageName = ISNULL(ZSD.SEOCode,'')
                          LEFT JOIN ZnodeCMSSEODetailLocale AS ZSDL ON(ZSD.CMSSEODetailId = ZSDL.CMSSEODetailId
                                                                        AND ZSDL.LocaleId = @DefaultlocaleId)
                          LEFT JOIN ZnodeCMSSEOType AS ZCST ON ZCST.CMSSEOTypeId = ZSD.CMSSEOTypeId
                                                                AND ZCST.name = 'Content Page'
                          INNER JOIN @InsertContentPage AS ICP ON ZCP.CMSContentPagesId = ICP.CMSContentPagesId
                          INNER JOIN ZnodeCMSContentPagesLocale AS ZCPL ON ZCP.CMSContentPagesId = ZCPL.CMSContentPagesId;
                 END;
             SET @Status = 1;
             COMMIT TRAN A;
         END TRY
         BEGIN CATCH
		  SELECT ERROR_MESSAGE() 
             
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
			 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_InsertUpdateContentPage @ContentPageXML = '+CAST(@ContentPageXML AS VARCHAR(max))+',@UserId = '+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN A;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_InsertUpdateContentPage',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
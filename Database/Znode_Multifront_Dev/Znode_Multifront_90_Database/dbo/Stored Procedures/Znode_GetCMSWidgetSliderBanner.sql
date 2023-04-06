

CREATE   PROCEDURE [dbo].[Znode_GetCMSWidgetSliderBanner]
( @PortalId INT = 0 ,
  @UserId   INT = 0
 ,@CMSSliderId INT = 0 
 ,@LocaleId INT = 0
 ,@CMSContentPagesId INT = 0
 )
AS 
   /* 
    Summary : Get all ZnodeCMSWidgetSliderBanner associated to PortalId and to all Contentpages
    		  associated to portal ID(using PortalMapping,ContentPageMapping)
    		  Get ZnodeCMSSliderBanner Associated to ZnodeCMSWidgetSliderBanner
    		  Data should Repeat According to LocaleId
     Unit Testing 
     EXEC [Znode_GetCMSWidgetSliderBanner_bak]  @PortalId = 1,@CMSContentPagesId =88
	
	*/
     BEGIN
         BEGIN TRAN;
         BEGIN TRY 
             
             DECLARE @TBL_Locale TABLE
             (RowId      INT IDENTITY(1, 1),
              LocaleId   INT,
              LocaleCode NVARCHAR(600),
              IsDefault  BIT
             );

             INSERT INTO @TBL_Locale(LocaleId,LocaleCode,IsDefault)
                    SELECT LocaleId,Name,IsDefault
                    FROM ZnodeLocale
                    WHERE IsActive = 1 
					AND ( LocaleId = @LocaleId  OR ISNULL(@LocaleId,0) = 0 ) ;

             DECLARE @V_LocaleId INT, @V_LocaleDefaultId INT=
             (
                 SELECT FeatureValues
                 FROM ZnodeGlobalSetting
                 WHERE FeatureName = 'Locale'
             );

             DECLARE @V_LocaleCode NVARCHAR(600);
             DECLARE @v_Count INT= 1;
             DECLARE @v_Count_forProduct INT= 1;
             DECLARE @Xmlreturn TABLE
             (CMSWidgetSliderBannerId INT,
              ReturnXML               NVARCHAR(MAX)
             );
             DECLARE @XmlFullreturn TABLE(ReturnXML XML);

             --CMSContentPage associated with portal 
			 DECLARE @TBL_CMSContentPagesPortalWise TABLE(CMSContentPagesId INT);
	

             INSERT INTO @TBL_CMSContentPagesPortalWise(CMSContentPagesId)
                    SELECT CMSContentPagesId
                    FROM ZnodeCMSContentPages
                    WHERE PortalId = @PortalId
                          AND IsActive = 1
						 AND (CMSContentPagesId = @CMSContentPagesId OR @CMSContentPagesId  = 0)

						

             WHILE @v_Count <= ISNULL(
                                     (
                                         SELECT MAX(RowId)
                                         FROM @TBL_Locale
                                     ), 0)
                 BEGIN
                     SET @V_LocaleId =
                     (
                         SELECT LocaleId
                         FROM @TBL_Locale
                         WHERE RowID = @v_Count
                     );
                     SET @V_LocaleCode =
                     (
                         SELECT LocaleCode
                         FROM @TBL_Locale
                         WHERE RowID = @v_Count
                     );

                     DECLARE @Tlb_ZnodeCMSWidgetSliderBanner TABLE
                     (CMSWidgetSliderBannerId INT,
                      CMSMappingId            INT,
                      PortalId                INT,
                      Type                    NVARCHAR(100) NULL,
                      Navigation              NVARCHAR(100) NULL,
                      AutoPlay                BIT,
                      AutoplayTimeOut         INT,
                      AutoplayHoverPause      BIT,
                      TransactionStyle        NVARCHAR(100) NULL,
                      WidgetsKey              NVARCHAR(256),
                      TypeOFMapping           NVARCHAR(100),
                      CMSSliderId             INT
                     );

                     DECLARE @TBL_ZnodeCMSSliderDetail TABLE
                     (CMSSliderId        INT,
                      CMSSliderBannerId  INT,
                      MediaPath          VARCHAR(300),
                      Title              NVARCHAR(1000),
                      ButtonLabelName    NVARCHAR(1200),
                      ButtonLink         NVARCHAR(600),
                      TextAlignment      NVARCHAR(200),
                      BannerSequence     INT,
                      ActivationDate     DATETIME,
                      ExpirationDate     DATETIME,
                      ImageAlternateText NVARCHAR(1000),
                      DEscription        NVARCHAR(MAX)
                     );

                     DECLARE @TBL_ZnodeCMSSliderDetail_Locale TABLE
                     (CMSSliderId        INT,
                      CMSSliderBannerId  INT,
                      MediaPath          VARCHAR(300),
                      Title              NVARCHAR(1000),
                      ButtonLabelName    NVARCHAR(1200),
                      ButtonLink         NVARCHAR(600),
                      TextAlignment      NVARCHAR(200),
                      BannerSequence     INT,
                      ActivationDate     DATETIME,
                      ExpirationDate     DATETIME,
                      ImageAlternateText NVARCHAR(1000),
                      DEscription        NVARCHAR(MAX),
                      LocaleId           INT
                     );

                    
                     INSERT INTO @Tlb_ZnodeCMSWidgetSliderBanner(CMSWidgetSliderBannerId,CMSMappingId,PortalId,Type,Navigation,AutoPlay,AutoplayTimeOut,AutoplayHoverPause,TransactionStyle,WidgetsKey,TypeOFMapping,CMSSliderId)
                            SELECT ACWSB.CMSWidgetSliderBannerId,ACWSB.CMSMappingId,CASE @CMSContentPagesId WHEN 0 THEN ACWSB.CMSMappingId
													ELSE @PortalId END
							AS PortalId,ACWSB.Type,ACWSB.Navigation,ACWSB.AutoPlay,ACWSB.AutoplayTimeOut
							,ACWSB.AutoplayHoverPause,ACWSB.TransactionStyle,ACWSB.WidgetsKey,ACWSB.TypeOfMapping
							OFMapping,ACWSB.CMSSliderId
                            FROM ZnodeCMSWidgetSliderBanner AS ACWSB
                            WHERE(  (TypeOfMapping = 'PortalMapping'
                                  AND CMSMappingId = @PortalId OR @PortalId = 0 )
                                  OR (TypeOfMapping = 'ContentPageMapping'
                                     AND CMSMappingId IN
                                    (
                                        SELECT CMSContentPagesId
                                        FROM @TBL_CMSContentPagesPortalWise
                                    ) ))
							   AND (ACWSB.CMSSliderId = @CMSSliderId OR @CMSSliderId = 0   )
							   AND (CMSMappingId = @CMSContentPagesId OR @CMSContentPagesId  = 0);

                     INSERT INTO @TBL_ZnodeCMSSliderDetail_Locale
                            SELECT ZCSB.CMSSliderId,ZCSB.CMSSliderBannerId,ZM.Path,ZCSBL.Title,ZCSBL.ButtonLabelName,ZCSBL.ButtonLink,ZCSB.TextAlignment,ZCSB.BannerSequence,ZCSB.ActivationDate,ZCSB.ExpirationDate,ZCSBL.ImageAlternateText,ZCSBL.DEscription,ISNULL(ZCSBL.LocaleId, @V_LocaleDefaultId) AS LocaleId
                            FROM ZnodeCMSSliderBanner AS ZCSB
                                 LEFT JOIN ZnodeCMSSliderBannerLocale AS ZCSBL ON(ZCSB.CMSSliderBannerId = ZCSBL.CMSSliderBannerId
                                                                                  AND ZCSBL.LocaleId IN(@V_LocaleDefaultId, @V_LocaleId))
                                 LEFT OUTER JOIN ZnodeMEdia  AS ZM ON ZCSBL.MediaId = ZM.MediaId
								 
                            WHERE EXISTS
                            (
                                SELECT TOP 1 1
                                FROM @Tlb_ZnodeCMSWidgetSliderBanner AS ACWSB
                                WHERE ACWSB.CMSSliderId = ZCSB.CMSSliderId
                            )
							--AND (ZCSB.CMSSliderId = @CMSSliderId OR @CMSSliderId = 0   )
							;

				

                     INSERT INTO @TBL_ZnodeCMSSliderDetail
                            SELECT CMSSliderId,CMSSliderBannerId,MediaPath,Title,ButtonLabelName,ButtonLink,TextAlignment,BannerSequence,ActivationDate,ExpirationDate,ImageAlternateText,DEscription
                            FROM @TBL_ZnodeCMSSliderDetail_Locale
                            WHERE LocaleId = @V_LocaleId;


                     INSERT INTO @TBL_ZnodeCMSSliderDetail
                            SELECT CMSSliderId,CMSSliderBannerId,MediaPath,Title,ButtonLabelName,ButtonLink,TextAlignment,BannerSequence,ActivationDate,ExpirationDate,ImageAlternateText,DEscription
                            FROM @TBL_ZnodeCMSSliderDetail_Locale AS TZCSDL
                            WHERE LocaleId = @V_LocaleDefaultId
              AND NOT EXISTS
                            (
                                SELECT TOP 1 1
                                FROM @TBL_ZnodeCMSSliderDetail AS TZCSD
                                WHERE TZCSD.CMSSliderBannerId = TZCSDL.CMSSliderBannerId
                                      AND TZCSD.CMSSliderId = TZCSDL.CMSSliderId
                            );		 


                     INSERT INTO @Xmlreturn
                            SELECT CMSWidgetSliderBannerId,
                            (
                                SELECT
                                (
                                    SELECT CMSSliderId AS SliderId,CMSSliderBannerId AS SliderBannerId,MediaPath,Title,ButtonLabelName,ButtonLink,TextAlignment,BannerSequence,ActivationDate,ExpirationDate,ImageAlternateText,Description
                                    FROM @TBL_ZnodeCMSSliderDetail AS wd
                                    WHERE wd.CMSSliderId = TZCWSB.CMSSliderId 
                                    FOR XML PATH('SliderBannerEntity'), TYPE
                                )
                                FROM @Tlb_ZnodeCMSWidgetSliderBanner AS a
                                WHERE a.CMSWidgetSliderBannerId = TZCWSB.CMSWidgetSliderBannerId
                                FOR XML PATH(''), ROOT('SliderBanners')
                            ) AS XMLGEM

                            FROM @Tlb_ZnodeCMSWidgetSliderBanner AS TZCWSB;

                     INSERT INTO @XmlFullreturn
                            SELECT DISTINCT
                                   '<WidgetSliderBannerEntity>'+
                            (
                                SELECT DISTINCT
                                       CMSWidgetSliderBannerId AS WidgetSliderBannerId,CMSMappingId AS MappingId,PortalId,@V_LocaleId AS LocaleId,Type,Navigation,AutoPlay,AutoplayTimeOut,AutoplayHoverPause,TransactionStyle,WidgetsKey,TypeOFMapping,CMSSliderId AS SliderId,TransactionStyle
                                FROM @Tlb_ZnodeCMSWidgetSliderBanner AS pr
                                WHERE pr.CMSWidgetSliderBannerId = p.CMSWidgetSliderBannerId
                                FOR XML PATH('')
                            )+
                            (
                                SELECT ReturnXML
                                FROM @Xmlreturn AS q
                                WHERE q.CMSWidgetSliderBannerId = p.CMSWidgetSliderBannerId
                            )+'</WidgetSliderBannerEntity>'
                            FROM @Tlb_ZnodeCMSWidgetSliderBanner AS p;

                     DELETE FROM @Xmlreturn;
                     DELETE FROM @TBL_ZnodeCMSSliderDetail_Locale;
                     DELETE FROM @TBL_ZnodeCMSSliderDetail;
                     DELETE FROM @Tlb_ZnodeCMSWidgetSliderBanner;
                     SET @v_Count = @v_Count + 1;
                 END;
             SELECT *
             FROM @XmlFullreturn;
             COMMIT TRAN;
         END TRY
         BEGIN CATCH
              DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCMSWidgetSliderBanner @PortalId = '+CAST(@PortalId AS VARCHAR(50))+',@UserId='+CAST(@UserId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		     ROLLBACK TRAN;
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCMSWidgetSliderBanner',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
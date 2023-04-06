
CREATE PROCEDURE [dbo].[Znode_GetSeoDetails]
(   @SeoCode SelectColumnList Readonly,
    --@SeoCode    NVARCHAR(MAX) = '',
	@SeoType  NVARCHAR(200),
	@LocaleId INT,
	@PortalId INT=0)
AS 
   /*
     Summary:- This Procedure is used to get the Seo Detials on the bassis of seo type  
     Unit Testing 
     EXEC Znode_GetSeoDetails 1,'Category'
	 Znode_GetSeoDetails  @SeoType='Category',@LocaleId=1,@PortalId=1
   */
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();
             DECLARE @TBL_SeoId TABLE(SeoCode NVARCHAR(MAX));

             INSERT INTO @TBL_SeoId(SeoCode)
			 SELECT StringColumn FROM @SeoCode
			 --SELECT item FROM dbo.Split(@SeoCode, ',') SP;

            ; WITH Cte_SeoDetailsIds
              AS (SELECT ZCSD.CMSSEODetailId, SEOTitle,SEOKeywords,SEOURL,ZCSD.ModifiedDate,ZCSDL.SEODescription,MetaInformation,IsRedirect,
              CMSSEODetailLocaleId,LocaleId, ZCSD.IsPublish,ZCSD.SEOCode, ZCSDL.CanonicalURL, ZCSDL.RobotTag 
			  FROM ZnodeCMSSEODetail AS ZCSD 
              INNER JOIN ZnodeCMSSEOType AS ZCST ON(ZCST.CMSSEOTypeId = ZCSD.CMSSEOTypeId  AND ZCST.Name = @SeoType)                                                   
              INNER JOIN ZnodeCMSSEODetailLocale AS ZCSDL ON(ZCSDL.CMSSEODetailId = ZCSD.CMSSEODetailId)
              WHERE LocaleId IN(@LocaleId, @DefaultLocaleId) AND  (ZCSD.PortalId= @PortalId OR @PortalId = 0 )
              AND( EXISTS (SELECT TOP 1 1 FROM @TBL_SeoId TBSD WHERE TBSD.SeoCode = ZCSD.SeoCode) OR
			  EXISTS ( SELECT TOP 1 1 FROM @SeoCode SC WHERE SC.StringColumn = '' ))  )

            ,Cte_SeoDetailsFirstLocale
             AS (SELECT CMSSEODetailId,SEOTitle,SEOKeywords,SEOURL,ModifiedDate,SEODescription,MetaInformation,IsRedirect,CMSSEODetailLocaleId
             ,IsPublish,SEOCode, CanonicalURL, RobotTag 
			 FROM Cte_SeoDetailsIds WHERE LocaleId = @LocaleId)

            ,Cte_SeoDetailDefaultLocale
				AS (
				SELECT CMSSEODetailId,SEOTitle,SEOKeywords,SEOURL,ModifiedDate,SEODescription,MetaInformation,IsRedirect,CMSSEODetailLocaleId
				,IsPublish,SEOCode,CanonicalURL, RobotTag  FROM Cte_SeoDetailsFirstLocale
				UNION ALL
				SELECT CMSSEODetailId,SEOTitle,SEOKeywords,SEOURL,ModifiedDate,SEODescription,MetaInformation,IsRedirect,CMSSEODetailLocaleId
				,IsPublish,SEOCode,CanonicalURL, RobotTag  FROM Cte_SeoDetailsIds CTSD
				WHERE LocaleId = @DefaultLocaleId
				AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_SeoDetailsFirstLocale CTSDL WHERE CTSDL.CMSSEODetailId = CTSD.CMSSEODetailId))

        SELECT CMSSEODetailId,SEOTitle,SEOKeywords,SEOURL,ModifiedDate,SEODescription,MetaInformation,IsRedirect,CMSSEODetailLocaleId
        ,Case When Isnull(IsPublish ,0 ) = 0 then 'Draft' ELSE 'Published' END   PublishStatus,SEOCode,CanonicalURL, RobotTag 
		 FROM Cte_SeoDetailDefaultLocale;
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		    SET @Status = 0;
		    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetSeoDetails @SeoType='+@SeoType+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
            SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
            EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetSeoDetails',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
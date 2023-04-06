CREATE PROCEDURE [dbo].[Znode_GetContentFeedList]
( @PortalId NVARCHAR(MAX) = NULL,
  @LocaleId INT)
AS
/*
	Summary : This procedure is used to get effective keyword feeding of content list
	Unit Testing:
	EXEC Znode_GetContentFeedList 4,1
	
*/
     BEGIN
         SET NOCOUNT ON;
		 BEGIN TRY
         DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();
         DECLARE @TBL_DomainName TABLE
         (PortalId   INT,
          DomainName NVARCHAR(300),
          RowId      INT
         );

         DECLARE @TBL_PortalIds TABLE(PortalId INT);
         DECLARE @TBL_SEODetails TABLE
         (loc                   NVARCHAR(MAX),
          lastmod               DATETIME,
          [g:condition]         VARCHAR(100),
          [description]         NVARCHAR(MAX),
          [g:id]                INT,
          link                  VARCHAR(100),
          [g:identifier_exists] VARCHAR(200),
          DomainName            NVARCHAR(300),
          PortalId              INT,
		  SEOCode               NVARCHAR(4000),
		  CanonicalURL          NVARCHAR(200),
		  RobotTag              NVARCHAR(50)
         );

		 if object_id('tempdb..#CMSSEOData') is not null
			drop table #CMSSEOData

		 CREATE TABLE #CMSSEOData 
		 (loc varchar(max),lastmod DateTime,DomainName varchar(200),[image] varchar(1000),Name varchar(500), CMSContentPagesId Int,PortalId Int, SEOCode NVARCHAR(4000)
		 , CanonicalURL NVARCHAR(200), RobotTag NVARCHAR(50))

         INSERT INTO @TBL_PortalIds
                SELECT Zp.PortalId
                FROM Znodeportal AS ZP
                INNER JOIN ZnodeCMSContentPages AS ZPC ON(ZPC.PortalId = Zp.PortalId)
                WHERE EXISTS
                (
                    SELECT TOP 1 1
                    FROM DBO.Split(@PortalID, ',') AS Sp
                    WHERE(CAST(sp.Item AS INT) = ZP.PortalId
                          OR @PortalID = '0' )
                )
                GROUP BY Zp.PortalId;

				
         INSERT INTO @TBL_DomainName
                SELECT TOP 1 ZD.PortalId,ZD.DomainName,                     
                       ROW_NUMBER() OVER(PArtition BY ZD.DomainName,
                                                  ZD.PortalId ORDER BY ZD.DomainName,
                                                  ZD.PortalId) RowId
                FROM ZnodeDomain ZD
                WHERE EXISTS
                (
                    SELECT TOP 1 1
                    FROM @TBL_PortalIds TBP
                    WHERE TBP.PortalId = ZD.PortalId
                )
			AND IsActive =1 
			AND applicationType = 'Webstore'
			AND ZD.IsDefault = 1


			if not exists(select top 1 1 from @TBL_DomainName) 
			INSERT INTO @TBL_DomainName
                SELECT TOP 1 ZD.PortalId,ZD.DomainName,                     
                       ROW_NUMBER() OVER(PArtition BY ZD.DomainName,
                                                  ZD.PortalId ORDER BY ZD.DomainName,
                                                  ZD.PortalId) RowId
                FROM ZnodeDomain ZD
                WHERE EXISTS
                (
                    SELECT TOP 1 1
                    FROM @TBL_PortalIds TBP
                    WHERE TBP.PortalId = ZD.PortalId
                )
			AND IsActive =1 
			AND applicationType = 'Webstore'
			AND ZD.IsDefault = 0

         ;WITH Cte_SeoDetailsWithLocale
              AS (SELECT DISTINCT ZCSD.CMSSEODetailId,  ZCSD.SEOURL AS loc,ZCSD.ModifiedDate AS lastmod,'new' AS [g:condition],ZCSDL.SEODescription AS [description],ZPCC.CMSContentPagesId AS [g:id],'' AS link,'false' AS [g:identifier_exists],TBDN.DomainName,ZPCC.PortalId,ISNULL(ZCSDL.LocaleId, @DefaultLocaleId) AS LocaleId,ZCSD.SEOCode
			      , ZCSDL.CanonicalURL, ZCSDL.RobotTag
                 FROM ZnodeCMSContentPages AS ZPCC 
                 LEFT JOIN ZnodeCMSSEODetail AS ZCSD ON(ZPCC.PageName = ZCSD.SEOCode AND EXISTS ( SELECT TOP 1 1 FROM ZnodeCMSSEOType AS ZCST WHERE ZCST.CMSSEOTypeId = ZCSD.CMSSEOTypeId
                                                   AND ZCST.Name = 'Content Page') AND ZCSD.PortalId = ZPCC.PortalId )
                 LEFT JOIN ZnodeCMSSEODetailLocale AS ZCSDL ON(ZCSDL.CMSSEODetailId = ZCSD.CMSSEODetailId
                                                            AND LocaleId IN(@LocaleId, @DefaultLocaleId))
                 LEFT JOIN @TBL_DomainName AS TBDN ON(TBDN.RowId = 1 AND TBDN.PortalId = ZPCC.PortalId )
                  WHERE  ZPCC.IsActive = 1 AND exists (select top 1 1 from ZnodePublishState ZPS where ZPS.StateName in ( 'Publish','Preview') and ZPCC.PublishStateId= ZPS.PublishStateId)
				  AND EXISTS
                  (
                      SELECT TOP 1 1
                      FROM @TBL_PortalIds TBP
                      WHERE ZPCC.PortalId = TBP.PortalId
                  )
				  AND EXISTS (SELECT TOP 1 1 FROM   @TBL_PortalIds TBPL
                      WHERE ZCSD.PortalId = TBPL.PortalId  
					  )
				  )
				   
				  --select  * from Cte_SeoDetailsWithLocale

             ,Cte_SeoDetailsWithFirstLocale
              AS (SELECT *
                  FROM Cte_SeoDetailsWithLocale
                  WHERE LocaleId = @LocaleId),

              Cte_SeoDetailsWithDefaultLocale
              AS (
              SELECT *
              FROM Cte_SeoDetailsWithFirstLocale
              UNION ALL
              SELECT *
              FROM Cte_SeoDetailsWithLocale AS CTSDWL
              WHERE LocaleId = @DefaultLocaleId
                    AND NOT EXISTS
              (
                  SELECT TOP 1 1
                  FROM Cte_SeoDetailsWithFirstLocale AS CTSDWDL
                  WHERE CTSDWDL.CMSSEODetailId = CTSDWL.CMSSEODetailId
              ))

              INSERT INTO @TBL_SEODetails
                     SELECT loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName,PortalId,SEOCode,CanonicalURL,RobotTag
                     FROM Cte_SeoDetailsWithDefaultLocale;

         WITH Cte_ContentPages
              AS (SELECT ZCCP.CMSContentPagesId,ZCCPl.PageTitle,ZCCPL.LocaleId,ZCCP.PageName
                  FROM ZnodeCMSContentPages ZCCP
                  INNER JOIN ZnodeCMSContentPagesLocale ZCCPL ON(ZCCPL.CMSContentPagesId = ZCCP.CMSContentPagesId)
                  WHERE ZCCP.IsActive = 1 AND exists (select top 1 1 from ZnodePublishState ZPS where ZPS.PublishStateCode in ( 'Publish','Preview') and ZCCP.PublishStateId= ZPS.PublishStateId)				  
				  AND EXISTS
                  (
                      SELECT TOP 1 1
                      FROM @TBL_SEODetails TBSD
                      WHERE TBSD.[g:id] = ZCCP.CMSContentPagesId
                  )
                        AND LocaleID IN(@LocaleId, @DefaultLocaleId)),

              Cte_ContentPageFirstLocale
              AS (SELECT *
                  FROM Cte_ContentPages CTCP
                  WHERE LocaleId = @LocaleId),

              Cte_ContentPageSecondLocale
              AS (
              SELECT *
              FROM Cte_ContentPageFirstLocale CTPFL
              UNION ALL
              SELECT *
              FROM Cte_ContentPages CTCP
              WHERE LocaleId = @LocaleId
                    AND NOT EXISTS
              (
                  SELECT TOP 1 1
                  FROM Cte_ContentPageFirstLocale CTCPFL
                  WHERE CTCPFL.CMSContentPagesId = CTCP.CMSContentPagesId
              ))
			  INSERT INTO #CMSSEOData(loc,lastmod,DomainName,[image], Name, CMSContentPagesId,PortalId,SEOCode,CanonicalURL,RobotTag)
              SELECT DISTINCT loc,lastmod,DomainName,'' AS [image],PageName Name,[g:id] CMSContentPagesId,PortalId,SEOCode,CanonicalURL,RobotTag
              FROM @TBL_SEODetails TBSD
              LEFT JOIN Cte_ContentPageSecondLocale CTCPSL ON(TBSD.[g:id] = CTCPSL.CMSContentPagesId);

			  ---- CMS SEO Blog News Details
			  INSERT INTO #CMSSEOData(loc,lastmod,DomainName,[image], Name, CMSContentPagesId,PortalId,SEOCode,CanonicalURL,RobotTag)
			  EXEC [Znode_GetBlogFeedList] @PortalId, @LocaleId

			  SELECT DISTINCT loc, REPLACE(CONVERT(VARCHAR(30),lastmod,102),'.','-') AS lastmod,DomainName,[image], Name, CMSContentPagesId,PortalId,CanonicalURL,RobotTag FROM #CMSSEOData

		END TRY
		BEGIN CATCH
		  DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetContentFeedList @PortalId = '+@PortalId+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetContentFeedList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		END CATCH
     END;

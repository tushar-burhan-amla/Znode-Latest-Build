   
CREATE PROCEDURE [dbo].[Znode_GetBlogFeedList]
( 
	@PortalId NVARCHAR(MAX) = NULL,
	@LocaleId INT
)
AS
/*
	Summary : This procedure is used to get effective keyword feeding of content list
	Unit Testing:
	EXEC [Znode_GetBlogFeedList] 2,1
	R
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

         INSERT INTO @TBL_PortalIds
                SELECT Zp.PortalId
                FROM Znodeportal AS ZP
                INNER JOIN ZnodeBlogNews AS ZBN ON(ZBN.PortalId = Zp.PortalId)
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


		 ---------- CMS Content Page Details
         ;WITH Cte_SeoDetailsWithLocale AS 
		 (
				SELECT DISTINCT ZCSD.CMSSEODetailId,ZCSD.SEOURL AS loc,ZCSD.ModifiedDate AS lastmod,'new' AS [g:condition],ZCSDL.SEODescription AS [description],ZBN.BlogNewsId AS [g:id],'' AS link,'false' AS [g:identifier_exists],TBDN.DomainName,ZBN.PortalId,ISNULL(ZCSDL.LocaleId, @DefaultLocaleId) AS LocaleId, ZCSD.SEOCode
				,ZCSDL.CanonicalURL, ZCSDL.RobotTag
                FROM ZnodeBlogNews AS ZBN 
                LEFT JOIN ZnodeCMSSEODetail AS ZCSD ON(ZBN.BlogNewsCode = ZCSD.SEOCode AND EXISTS ( SELECT TOP 1 1 FROM ZnodeCMSSEOType AS ZCST WHERE ZCST.CMSSEOTypeId = ZCSD.CMSSEOTypeId
                                                AND ZCST.Name = 'BlogNews'))
                LEFT JOIN ZnodeCMSSEODetailLocale AS ZCSDL ON(ZCSDL.CMSSEODetailId = ZCSD.CMSSEODetailId
                                                        AND LocaleId IN(@LocaleId, @DefaultLocaleId))
                LEFT JOIN @TBL_DomainName AS TBDN ON(TBDN.RowId = 1 AND TBDN.PortalId = ZBN.PortalId )
                WHERE EXISTS
                (
                    SELECT TOP 1 1
                    FROM @TBL_PortalIds TBP
                    WHERE ZBN.PortalId = TBP.PortalId
                )
				AND ZBN.IsBlogNewsActive =1 
			),
			Cte_SeoDetailsWithFirstLocale AS 
			(
				SELECT * FROM Cte_SeoDetailsWithLocale WHERE LocaleId = @LocaleId
			),
			Cte_SeoDetailsWithDefaultLocale AS 
			(
				  SELECT * FROM Cte_SeoDetailsWithFirstLocale
				  UNION ALL
				  SELECT * FROM Cte_SeoDetailsWithLocale AS CTSDWL
				  WHERE LocaleId = @DefaultLocaleId
				  AND NOT EXISTS
				  (
					  SELECT TOP 1 1
					  FROM Cte_SeoDetailsWithFirstLocale AS CTSDWDL
					  WHERE CTSDWDL.CMSSEODetailId = CTSDWL.CMSSEODetailId
				  )
			)
			INSERT INTO @TBL_SEODetails
            SELECT loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName,PortalId, SEOCode,CanonicalURL,RobotTag
            FROM Cte_SeoDetailsWithDefaultLocale

         ;WITH Cte_BlogNews AS 
		 (
			SELECT ZBN.BlogNewsId,ZBNL.BlogNewsTitle,ZBNL.LocaleId,ZBNL.BlogNewsTitle Name
            FROM ZnodeBlogNews ZBN
            INNER JOIN ZnodeBlogNewsLocale ZBNL ON(ZBN.BlogNewsId = ZBNL.BlogNewsId)
            WHERE EXISTS
            (
                SELECT TOP 1 1
                FROM @TBL_SEODetails TBSD
                WHERE TBSD.[g:id] = ZBN.BlogNewsId
            )
            AND LocaleID IN(@LocaleId, @DefaultLocaleId)
		),
		Cte_BlogNewsFirstLocale AS 
		(
			SELECT *
            FROM Cte_BlogNews CTCP
            WHERE LocaleId = @LocaleId
		),
		Cte_BlogNewsSecondLocale AS 
		(
              SELECT *
              FROM Cte_BlogNewsFirstLocale CTPFL
              UNION ALL
              SELECT *
              FROM Cte_BlogNews CTCP
              WHERE LocaleId = @LocaleId
                    AND NOT EXISTS
              (
                  SELECT TOP 1 1
                  FROM Cte_BlogNewsFirstLocale CTCPFL
                  WHERE CTCPFL.BlogNewsId = CTCP.BlogNewsId
              )
		)
        SELECT DISTINCT loc,lastmod,DomainName,'' AS [image], Name,[g:id] BlogNewsId,PortalId,SEOCode, CanonicalURL, RobotTag
        FROM @TBL_SEODetails TBSD
        LEFT JOIN Cte_BlogNewsSecondLocale CTCPSL ON(TBSD.[g:id] = CTCPSL.BlogNewsId);

		END TRY
		BEGIN CATCH
		  DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetBlogFeedList @PortalId = '+@PortalId+',@LocaleId='+CAST(@LocaleId AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetBlogFeedList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		END CATCH

     END;
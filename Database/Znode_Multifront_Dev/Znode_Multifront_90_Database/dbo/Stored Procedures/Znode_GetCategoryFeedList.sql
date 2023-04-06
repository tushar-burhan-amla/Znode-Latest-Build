CREATE PROCEDURE [dbo].[Znode_GetCategoryFeedList]
( @PortalId         NVARCHAR(MAX) = NULL,
  @LocaleId         INT,
  --will be used for CategoryId
  @CommaSeparatedId NVARCHAR(MAX) = NULL 
)
AS
/*
 Summary:This procedure is used to get effective keyword feeding of category list
 Unit Testing:
 EXEC Znode_GetCategoryFeedList 1 

*/

	BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultValue('Locale');

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
		CanonicalURL        VARCHAR(200), 
		RobotTag            VARCHAR(50)
		);

		INSERT INTO @TBL_PortalIds(PortalId)
		SELECT Zp.PortalId
		FROM Znodeportal AS ZP
		INNER JOIN ZnodePortalCatalog AS ZPC ON(ZPC.PortalId = Zp.PortalId)
		INNER JOIN ZnodePublishCatalog AS ZPPC ON(ZPPC.PimCatalogId = ZPC.PublishCatalogId)
		INNER JOIN ZnodePublishCategoryEntity AS ZPP ON(ZPP.ZnodeCatalogId = ZPPC.PimCatalogId)
		WHERE EXISTS
			(
			SELECT TOP 1 1
			FROM DBO.Split(@PortalID, ',') AS Sp
			WHERE(CAST(sp.Item AS INT) = ZP.PortalId
			OR @PortalID = '0' )
			)
		GROUP BY Zp.PortalId;
	
	
		INSERT INTO @TBL_DomainName(PortalId,DomainName,RowId)
		SELECT TOP 1 ZD.PortalId,ZD.DomainName,    
		ROW_NUMBER() OVER(Partition BY ZD.DomainName,ZD.PortalId ORDER BY ZD.DomainName,ZD.PortalId) RowId                               
		FROM ZnodeDomain ZD
		WHERE EXISTS
			(
			SELECT TOP 1 1
			FROM @TBL_PortalIds TBP
			WHERE TBP.PortalId = ZD.PortalId
			)
		AND ApplicationType = 'Webstore'
		AND IsActive =1 
		AND IsDefault = 1;

		If Not Exists (Select TOP 1 1 from @TBL_DomainName)
        Begin
            INSERT INTO @TBL_DomainName(PortalId,DomainName,RowId)
            SELECT TOP 1 ZD.PortalId,ZD.DomainName,    
            ROW_NUMBER() OVER(Partition BY ZD.DomainName,ZD.PortalId ORDER BY ZD.DomainName,ZD.PortalId) RowId                               
            FROM ZnodeDomain ZD
            WHERE EXISTS
                (
                SELECT TOP 1 1
                FROM @TBL_PortalIds TBP
                WHERE TBP.PortalId = ZD.PortalId
                )
            AND ApplicationType = 'Webstore'
            AND IsActive =1 
            AND IsDefault = 0;
        End

		;WITH Cte_SeoDetailsWithLocale
		AS (
			SELECT DISTINCT ZCSD.CMSSEODetailId,ZCSD.SEOURL AS loc,ZCSD.ModifiedDate AS lastmod,'new' AS [g:condition],
			ZCSDL.SEODescription AS [description],ZPCC.ZnodeCategoryId AS [g:id],'' AS link,'false' AS [g:identifier_exists],TBDN.DomainName,ZPC.PortalId,ISNULL(ZCSDL.LocaleId, @DefaultLocaleId) AS LocaleId ,ZCSD.SEOCode, ZCSDL.CanonicalURL, ZCSDL.RobotTag
			FROM ZnodePublishCategoryEntity AS ZPCC 
			LEFT JOIN ZnodePublishCatalog AS ZPPC ON(ZPPC.PimCatalogId = ZPCC.ZnodeCatalogId)
			LEFT JOIN ZnodePortalCatalog AS ZPC ON(ZPC.PublishCatalogId = ZPPC.PimCatalogId)
			LEFT JOIN @TBL_DomainName AS TBDN ON(TBDN.RowId = 1 AND TBDN.PortalId = ZPC.PortalId)
			LEFT JOIN ZnodeCMSSEODetail AS ZCSD ON(ZPCC.CategoryCode = ZCSD.SEOCode AND ZPC.PortalId = ZCSD.PortalId
			AND EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEOType AS ZCST
			WHERE ZCST.CMSSEOTypeId = ZCSD.CMSSEOTypeId  AND ZCST.Name = 'Category')) 
			LEFT  JOIN ZnodeCMSSEODetailLocale AS ZCSDL ON(ZCSDL.CMSSEODetailId = ZCSD.CMSSEODetailId
			AND ZCSDL.LocaleId IN(@LocaleId, @DefaultLocaleId))
			WHERE EXISTS
					(
					SELECT TOP 1 1
					FROM @TBL_PortalIds TBP
					WHERE ZPC.PortalId = TBP.PortalId
					)
			AND EXISTS (SELECT TOP 1 1 FROM  dbo.split(@CommaSeparatedId,',' ) SP WHERE SP.Item = ZPCC.CategoryCode)
		),
		
		Cte_SeoDetailsWithFirstLocale
		AS (SELECT CMSSEODetailId,loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName
		,PortalId,LocaleId,SEOCode, CanonicalURL, RobotTag
		FROM Cte_SeoDetailsWithLocale
		WHERE LocaleId = @LocaleId),

		Cte_SeoDetailsWithDefaultLocale
		AS (SELECT CMSSEODetailId,loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName
		,PortalId,LocaleId,SEOCode, CanonicalURL, RobotTag
		FROM Cte_SeoDetailsWithFirstLocale
		UNION ALL
		SELECT CMSSEODetailId,loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName
		,PortalId,LocaleId,SEOCode, CanonicalURL, RobotTag
		FROM Cte_SeoDetailsWithLocale AS CTSDWL
		WHERE LocaleId = @DefaultLocaleId
		AND NOT EXISTS
		(
		SELECT TOP 1 1
		FROM Cte_SeoDetailsWithFirstLocale AS CTSDWDL
		WHERE CTSDWDL.CMSSEODetailId = CTSDWL.CMSSEODetailId
		))

		INSERT INTO @TBL_SEODetails (loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName
		,PortalId,SEOCode, CanonicalURL, RobotTag)
		SELECT loc,lastmod,[g:condition],[description],[g:id],link,[g:identifier_exists],DomainName,PortalId,SEOCode, CanonicalURL, RobotTag
		FROM Cte_SeoDetailsWithDefaultLocale;
		
		SELECT DISTINCT loc,lastmod,DomainName,[g:id] AS id,PortalId,b.Name AS Name,SEOCode, CanonicalURL, RobotTag
		FROM @TBL_SEODetails a
		left JOIN ZnodePublishCategoryEntity b ON(b.ZnodeCategoryId = a.[g:id] AND b.LOcaleId = @LocaleId)
		WHERE DomainName IS NOT NULL
		
		END TRY
		BEGIN CATCH
		DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCategoryFeedList @PortalId = '+@PortalId+',@LocaleId='+CAST(@LocaleId AS VARCHAR(10))+',@CommaSeparatedId='+@CommaSeparatedId+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		   
		EXEC Znode_InsertProcedureErrorLog
		@ProcedureName = 'Znode_GetCategoryFeedList',
		@ErrorInProcedure = @Error_procedure,
		@ErrorMessage = @ErrorMessage,
		@ErrorLine = @ErrorLine,
		@ErrorCall = @ErrorCall;
		END CATCH;
     END;
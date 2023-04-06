CREATE PROCEDURE [dbo].[Znode_GetBlogNewsList] 
(   @WhereClause NVarchar(Max) = '',
	@Rows        INT           = 100,
	@PageNo      INT           = 1,
	@Order_BY VARCHAR(1000)    = '',
	@RowsCount   INT OUT,
	@LocaleId    INT           = 0
)
AS 
/*
   Summary:- This proceudre is used to get the blog commets details 
    SELECT * FROM ZnodeCMSSeoType
	Title(BlogNewsTitle)  
 Type (BlogNewsType)
 View Comment (Show total count of comments against that blog/news)
 Start Date (ActivationDate)
 End date(ExpirationDate)
 Created On (CreatedDate)
 Store Name (StoreName)
 SEO Title (SEOTitle)
 SEO Description (SEODescription)
 SEO Keywords (SEOKeywords)
 SEO Friendly URL(SEOUrl)
 Is Active (IsBlogNewsActive)
 Is Allow Guest Comment (IsAllowGuestComment)
   
   EXEC Znode_GetBlogNewsList '' ,100,1,'',0,1
     
*/
BEGIN 
BEGIN TRY 
SET NOCOUNT ON 
 
 DECLARE @DefaultlocaleId INT = dbo.fn_GetDefaultLocaleId()
 DECLARE @TBL_GetBlogComments TABLE (BlogNewsId INT  ,BlogNewsTitle NVARCHAR(1200),BlogNewsType VARCHAR(300),CountComments VARCHAR(2000) 
    ,ActivationDate DATETIME ,ExpirationDate DATETIME , CreatedDate DATETIME
	,StoreName NVARCHAR(max) ,IsAllowGuestComment BIT,IsBlogNewsActive BIT
	--,SEOTitle NVARCHAR(max) ,SEODescription NVARCHAr(max), SEOKeywords NVARCHAR(max),SEOUrl NVARCHAR(max) 
	, RowID INT , CountNo INT ,PublishStatus varchar(50), PublishStateId int )
DECLARE @SQL NVARCHAR(max) = '' 

SET @SQL = '
 ;With Cte_GetBlogComments AS 
 (
   SELECT ZBN.BlogNewsId,BlogNewsTitle ,BlogNewsType ,(SELECT COUNT(1) FROM ZnodeBlogNewsComment ZBC WHERE ZBC.BlogNewsId = ZBN.BlogNewsId)  CountComments
    ,ActivationDate,ExpirationDate ,ZBN.CreatedDate 
	,ZP.StoreName ,ZBCL.localeId ,'+CAST(@DefaultlocaleId AS VARCHAR(50))+' SeoLocaleId ,IsAllowGuestComment,IsBlogNewsActive
	, ZPS.DisplayName as PublishStatus, ZBN.PublishStateId
	--,SEOTitle,SEODescription, SEOKeywords,SEOUrl
   FROM  ZnodeBlogNews ZBN
   LEFT JOIN ZnodeBlogNewsLocale ZBCL ON (ZBCL.BlogNewsId = ZBN.BlogNewsId)
   INNER JOIN ZnodePublishState ZPS ON isnull(ZBN.PublishStateId,1) = ZPS.PublishStateId
   LEFT JOIN ZnodePortal ZP ON (Zp.PortalId = ZBN.PortalId )
   WHERE ZBCL.localeId IN ('+CAST(@DefaultlocaleId AS VARCHAR(50))+','+CAST(@LocaleId AS VARCHAR(50))+' )
 )
 ,Cte_BlogNewForLocale AS 
 (
   SELECT BlogNewsId ,BlogNewsTitle,BlogNewsType ,CountComments
    ,ActivationDate,ExpirationDate , [CreatedDate]
	,StoreName ,localeId , SeoLocaleId ,IsAllowGuestComment,IsBlogNewsActive,PublishStatus, PublishStateId
	--,SEOTitle,SEODescription, SEOKeywords,SEOUrl
   FROM Cte_GetBlogComments 
   WHERE localeId = '+CAST(@LocaleId AS VARCHAR(50))+'
   AND (SeoLocaleId IS NULL OR SeoLocaleId = '+CAST(@LocaleId AS VARCHAR(50))+')
   '+[dbo].[Fn_GetFilterWhereClause](@whereClause)+'
 )
 ,Cte_DefaultLocaleData AS 
 (
   SELECT BlogNewsId ,BlogNewsTitle,BlogNewsType ,CountComments
    ,ActivationDate,ExpirationDate , [CreatedDate]
	,StoreName ,localeId , SeoLocaleId ,IsAllowGuestComment,IsBlogNewsActive, PublishStatus, PublishStateId
	--,SEOTitle,SEODescription, SEOKeywords,SEOUrl
   FROM Cte_BlogNewForLocale
   UNION ALL 
   SELECT BlogNewsId ,BlogNewsTitle,BlogNewsType ,CountComments
    ,ActivationDate,ExpirationDate , [CreatedDate]
	,StoreName ,localeId , SeoLocaleId ,IsAllowGuestComment,IsBlogNewsActive, PublishStatus, PublishStateId
	--,SEOTitle,SEODescription, SEOKeywords,SEOUrl
  FROM Cte_GetBlogComments CTED 
  WHERE NOT EXISTS (SELECT TOP 1 1 FROM Cte_BlogNewForLocale CteBN WHERE CteBN.BlogNewsId= CTED.BlogNewsId )
  AND localeId = '+CAST(@DefaultlocaleId AS VARCHAR(50))+'
  AND (SeoLocaleId IS NULL OR SeoLocaleId = '+CAST(@DefaultlocaleId AS VARCHAR(50))+')
   '+[dbo].[Fn_GetFilterWhereClause](@whereClause)+'
 )
 ,Cte_filterData AS 
 (
 SELECT BlogNewsId ,BlogNewsTitle,BlogNewsType ,CountComments
    ,ActivationDate,ExpirationDate , [CreatedDate]
	,StoreName ,localeId , SeoLocaleId ,IsAllowGuestComment,IsBlogNewsActive, PublishStatus, PublishStateId
	--,SEOTitle,SEODescription, SEOKeywords,SEOUrl
	, '+dbo.Fn_GetPagingRowId(@Order_BY,'BlogNewsId DESC')+',Count(*)Over() CountNo
 FROM Cte_DefaultLocaleData 
 ) 
  SELECT BlogNewsId ,BlogNewsTitle,BlogNewsType ,''View Comments - ''+ CAST(CountComments AS VARCHAR(500)) CountComments
    ,ActivationDate,ExpirationDate , [CreatedDate]
	,StoreName ,IsAllowGuestComment,IsBlogNewsActive, PublishStatus, PublishStateId
	--,SEOTitle,SEODescription, SEOKeywords,SEOUrl
	,RowId,CountNo
  FROM Cte_filterData
  '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows) 
  PRINT @SQL 

 INSERT INTO @TBL_GetBlogComments (BlogNewsId ,BlogNewsTitle,BlogNewsType ,CountComments
    ,ActivationDate,ExpirationDate , [CreatedDate]
	,StoreName  ,IsAllowGuestComment,IsBlogNewsActive, PublishStatus, PublishStateId
--	,SEOTitle,SEODescription, SEOKeywords,SEOUrl
	,RowId,CountNo)
 EXEC (@SQL)

 SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_GetBlogComments),0)

 SELECT BlogNewsId ,BlogNewsTitle,BlogNewsType ,CountComments
    ,ActivationDate,ExpirationDate , CreatedDate
	,StoreName  ,IsAllowGuestComment,IsBlogNewsActive, PublishStatus, PublishStateId
	--,SEOTitle,SEODescription, SEOKeywords,SEOUrl
 FROM @TBL_GetBlogComments
 END TRY 
 BEGIN CATCH 
  SELECT ERROR_MESSAGE ()
 END CATCH 
 END
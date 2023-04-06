CREATE  PROCEDURE Znode_GetBlogCommments 
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
   
   EXEC Znode_GetBlogCommments '' ,100,1,'',0,1
     
*/
BEGIN 
BEGIN TRY 
SET NOCOUNT ON 
 
 DECLARE @DefaultlocaleId INT = dbo.fn_GetDefaultLocaleId()
 DECLARE @TBL_GetBlogComments TABLE (BlogNewsId INT  ,BlogNewsTitle NVARCHAR(1200),BlogNewsType VARCHAR(300),CountComments INT 
    ,ActivationDate DATETIME ,ExpirationDate DATETIME , [Created] DATETIME-- why this column required
	,StoreName NVARCHAR(max) ,IsAllowGuestComment BIT,IsBlogNewsActive BIT
	,SEOTitle NVARCHAR(max) ,SEODescription NVARCHAr(max), SEOKeywords NVARCHAR(max),SEOUrl NVARCHAR(max) , RowID INT , CountNo INT )
DECLARE @SQL NVARCHAR(max) = '' 

SET @SQL = '
 ;With Cte_GetBlogComments AS 
 (
   SELECT ZBN.BlogNewsId,BlogNewsTitle ,BlogNewsType ,(SELECT COUNT(1) FROM ZnodeBlogComment ZBC WHERE ZBC.BlogNewsId = ZBN.BlogNewsId)  CountComments
    ,ActivationDate,ExpirationDate ,ZBN.CreatedDate AS [Created]-- why this column required
	,ZP.StoreName ,ZBCL.localeId ,ZSDL.localeId  SeoLocaleId ,IsAllowGuestComment,IsBlogNewsActive
	,SEOTitle,SEODescription, SEOKeywords,SEOUrl
   FROM  ZnodeBlogNews  ZBN
   LEFT JOIN ZnodeBlogNewsLocale ZBCL ON (ZBCL.BlogNewsId = ZBN.BlogNewsId)
   LEFT JOIN ZnodePortal ZP ON (Zp.PortalId = ZBN.PortalId )
   LEFT JOIN ZnodeCMSSeoDetail ZSD ON ( ZSD.SEOId= ZBN.BlogNewsId)
   INNER JOIN ZnodeCMSSeoType ZCST ON (ZCST.CMSSEOTypeId= ZSD.CMSSEOTypeId AND ZCST.Name = ''BlogNews'')
   LEFT JOIN ZnodeCmsSeoDetailLocale ZSDL ON (ZSDL.CMSSEODetailId = ZSD.CMSSEODetailId  AND ZSDL.localeId IN ('+CAST(@DefaultlocaleId AS VARCHAR(50))+','+CAST(@LocaleId AS VARCHAR(50))+'))
   WHERE ZBCL.localeId IN ('+CAST(@DefaultlocaleId AS VARCHAR(50))+','+CAST(@LocaleId AS VARCHAR(50))+' )
 )
 ,Cte_BlogNewForLocale AS 
 (
   SELECT BlogNewsId ,BlogNewsTitle,BlogNewsType ,CountComments
    ,ActivationDate,ExpirationDate , [Created]-- why this column required
	,StoreName ,localeId , SeoLocaleId ,IsAllowGuestComment,IsBlogNewsActive
	,SEOTitle,SEODescription, SEOKeywords,SEOUrl
   FROM Cte_GetBlogComments 
   WHERE localeId = '+CAST(@LocaleId AS VARCHAR(50))+'
   AND (SeoLocaleId IS NULL OR SeoLocaleId = '+CAST(@LocaleId AS VARCHAR(50))+')
   '+[dbo].[Fn_GetFilterWhereClause](@whereClause)+'
 )
 ,Cte_DefaultLocaleData AS 
 (
   SELECT BlogNewsId ,BlogNewsTitle,BlogNewsType ,CountComments
    ,ActivationDate,ExpirationDate , [Created]-- why this column required
	,StoreName ,localeId , SeoLocaleId ,IsAllowGuestComment,IsBlogNewsActive
	,SEOTitle,SEODescription, SEOKeywords,SEOUrl
   FROM Cte_BlogNewForLocale
   UNION ALL 
   SELECT BlogNewsId ,BlogNewsTitle,BlogNewsType ,CountComments
    ,ActivationDate,ExpirationDate , [Created]-- why this column required
	,StoreName ,localeId , SeoLocaleId ,IsAllowGuestComment,IsBlogNewsActive
	,SEOTitle,SEODescription, SEOKeywords,SEOUrl
  FROM Cte_GetBlogComments CTED 
  WHERE NOT EXISTS (SELECT TOP 1 1 FROM Cte_BlogNewForLocale CteBN WHERE CteBN.BlogNewsId= CTED.BlogNewsId )
  AND localeId = '+CAST(@DefaultlocaleId AS VARCHAR(50))+'
  AND (SeoLocaleId IS NULL OR SeoLocaleId = '+CAST(@DefaultlocaleId AS VARCHAR(50))+')
   '+[dbo].[Fn_GetFilterWhereClause](@whereClause)+'
 )
 ,Cte_filterData AS 
 (
 SELECT BlogNewsId ,BlogNewsTitle,BlogNewsType ,CountComments
    ,ActivationDate,ExpirationDate , [Created]-- why this column required
	,StoreName ,localeId , SeoLocaleId ,IsAllowGuestComment,IsBlogNewsActive
	,SEOTitle,SEODescription, SEOKeywords,SEOUrl, '+dbo.Fn_GetPagingRowId(@Order_BY,'BlogNewsId DESC')+',Count(*)Over() CountNo
 FROM Cte_DefaultLocaleData 
 ) 
  SELECT BlogNewsId ,BlogNewsTitle,BlogNewsType ,CountComments
    ,ActivationDate,ExpirationDate , [Created]-- why this column required
	,StoreName ,IsAllowGuestComment,IsBlogNewsActive
	,SEOTitle,SEODescription, SEOKeywords,SEOUrl,RowId,CountNo
  FROM Cte_filterData
  '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows) 
  PRINT @SQL 

 INSERT INTO @TBL_GetBlogComments (BlogNewsId ,BlogNewsTitle,BlogNewsType ,CountComments
    ,ActivationDate,ExpirationDate , [Created]-- why this column required
	,StoreName  ,IsAllowGuestComment,IsBlogNewsActive
	,SEOTitle,SEODescription, SEOKeywords,SEOUrl,RowId,CountNo)
 EXEC (@SQL)

 SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_GetBlogComments),0)

 SELECT BlogNewsId ,BlogNewsTitle,BlogNewsType ,CountComments
    ,ActivationDate,ExpirationDate , [Created]-- why this column required
	,StoreName  ,IsAllowGuestComment,IsBlogNewsActive
	,SEOTitle,SEODescription, SEOKeywords,SEOUrl
 FROM @TBL_GetBlogComments
 END TRY 
 BEGIN CATCH 
  SELECT ERROR_MESSAGE ()
 END CATCH 
 END
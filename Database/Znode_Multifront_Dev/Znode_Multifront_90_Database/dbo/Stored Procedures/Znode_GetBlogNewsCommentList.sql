
CREATE  PROCEDURE [dbo].[Znode_GetBlogNewsCommentList]
(   @WhereClause NVarchar(Max) = '',
	@Rows        INT           = 100,
	@PageNo      INT           = 1,
	@Order_BY VARCHAR(1000)    = '',
	@RowsCount   INT OUT,
	@LocaleId    INT           = 0
)
AS 
/*
   Summary:- This proceudre is used to get the blog comments details 
    SELECT * FROM ZnodeCMSSeoType
	Title(BlogNewsTitle)  
 Type (BlogNewsType)
 Created On (CreatedDate)
 Store Name (StoreName)
 Customer (Email)
 Comment (BlogNewsComment)
 Is Approved (IsApproved)
 EXEC Znode_GetBlogNewsCommentList @RowsCount =0 
   
 
*/
BEGIN 
BEGIN TRY 
SET NOCOUNT ON 
 
 DECLARE @DefaultlocaleId INT = dbo.fn_GetDefaultLocaleId()
 DECLARE @TBL_GetBlogNewsComments TABLE (
											BlogNewsCommentId INT  ,
											BlogNewsTitle NVARCHAR(1200),
											BlogNewsType VARCHAR(300), 
											CreatedDate DATETIME,
											StoreName NVARCHAR(max),
											IsApproved BIT,
											Customer NVARCHAR(max),
											BlogNewsComment NVARCHAR(max),
											RowID INT ,
											CountNo INT,LocaleName NVARCHAR(200)  )
DECLARE @SQL VARCHAR(max) = '' 

	SET @SQL = '
	 ;With Cte_GetBlogComments AS 
	 (
	   SELECT ZBNC.BlogNewsCommentId,ZBNCLL.BlogNewsTitle ,ZBN.BlogNewsType ,ZBNC.CreatedDate ,ZBNCL.BlogComment BlogNewsComment
		,ZP.StoreName ,ZBNCLL.localeId ,ZBNCL.localeId  SeoLocaleId ,ZBNC.IsApproved,
		CASE WHEN ZU.AspNetUserId IS NOT NULL THEN  ISNULL(ASZU.FirstName,'''')+'' ''+ISNULL(ASZU.LastName,'''') ELSE  ''Guest'' END  Customer ,ZBN.BlogNewsId,ZL.Name LocaleName  
	   FROM  ZnodeBlogNews ZBN
	   INNER  JOIN ZnodeBlogNewsLocale ZBNCLL On(ZBNCLL.BlogNewsId = ZBN.BlogNewsId)
	   INNER  JOIN ZnodeBlogNewsComment ZBNC ON (ZBNC.BlogNewsId = ZBN.BlogNewsId)
	   INNER  JOIN ZnodeBlogNewsCommentLocale ZBNCL ON (ZBNCL.BlogNewsCommentId = ZBNC.BlogNewsCommentId AND ZBNCLL.LocaleId = ZBNCL.LocaleID )
	   LEFT JOIN ZnodePortal ZP ON (ZP.PortalId = ZBN.PortalId )
	   LEFT JOIN ZnodeUser ZU ON (ZU.UserId=ZBNC.UserId)
	   LEFT JOIN AspNetUsers ASU ON (Asu.Id = ZU.AspNetUserId )
	   LEFT JOIN ZnodeUSer ASZU ON (ASZU.AspNetUserId = ASU.Id )
	   LEFT JOIN ZnodeLocale ZL ON (ZL.LocaleID = ZBNCL.localeId)
	  -- WHERE ZBNCL.localeId IN ('+CAST(@DefaultlocaleId AS VARCHAR(50))+','+CAST(@LocaleId AS VARCHAR(50))+' )
	)
	 ,Cte_BlogNewForLocale AS 
	 (
	   SELECT *--BlogNewsCommentId,BlogNewsTitle,BlogNewsType,[CreatedDate],StoreName,IsApproved,Customer,BlogNewsComment,localeId,BlogNewsId,LocaleName
	   FROM Cte_GetBlogComments
	   WHERE 1 = 1
	   --localeId = '+CAST(@LocaleId AS VARCHAR(50))+'
	   --AND (SeoLocaleId IS NULL OR SeoLocaleId = '+CAST(@LocaleId AS VARCHAR(50))+')
	   '+[dbo].[Fn_GetFilterWhereClause](@whereClause)+'
	 )
	,Cte_BlogComments AS
	(
	   SELECT ZBNC.BlogNewsCommentId,ZBNCLL.BlogNewsTitle ,ZBN.BlogNewsType ,ZBNC.CreatedDate ,ZBNCL.BlogComment BlogNewsComment
		,ZP.StoreName ,ZBNCLL.localeId ,ZBNCL.localeId  SeoLocaleId ,ZBNC.IsApproved,
		CASE WHEN ZU.AspNetUserId IS NOT NULL THEN  ISNULL(ASZU.FirstName,'''')+'' ''+ISNULL(ASZU.LastName,'''') ELSE  ''Guest'' END  Customer ,ZBN.BlogNewsId,ZL1.Name LocaleName  
	   FROM  ZnodeBlogNews ZBN
	   INNER JOIN ZnodeBlogNewsLocale ZBNCLL On(ZBNCLL.BlogNewsId = ZBN.BlogNewsId)
	   LEFT JOIN ZnodeBlogNewsComment ZBNC ON (ZBNC.BlogNewsId = ZBN.BlogNewsId)
	   LEFT JOIN ZnodeBlogNewsCommentLocale ZBNCL ON (ZBNCL.BlogNewsCommentId = ZBNC.BlogNewsCommentId )--AND ZBNCLL.LocaleId = ZBNCL.LocaleID )
	   LEFT JOIN ZnodePortal ZP ON (ZP.PortalId = ZBN.PortalId )
	   LEFT JOIN ZnodeUser ZU ON (ZU.UserId=ZBNC.UserId)
	   LEFT JOIN AspNetUsers ASU ON (Asu.Id = ZU.AspNetUserId )
	   LEFT JOIN ZnodeUSer ASZU ON (ASZU.AspNetUserId = ASU.Id )
	   LEFT JOIN ZnodeLocale ZL ON (ZL.LocaleID = ZBNCLL.localeId)
	   LEFT JOIN ZnodeLocale ZL1 ON (ZL1.LocaleID = ZBNCL.localeId)
	   INNER JOIN Cte_BlogNewForLocale BNL ON ( ZBNCLL.BlogNewsTitle = BNL.BlogNewsTitle )
	   WHERE NOT EXISTS( SELECT * FROM Cte_BlogNewForLocale A WHERE ZBNC.BlogNewsCommentId = ISNULL( A.BlogNewsCommentId, 0 ) )
	    AND ZL.IsDefault = 1 AND ZBNC.BlogNewsCommentId  IS NOT NULL 
	   UNION
	   SELECT ZBNC.BlogNewsCommentId,ZBNCLL.BlogNewsTitle ,ZBN.BlogNewsType ,ZBNC.CreatedDate ,ZBNCL.BlogComment BlogNewsComment
		,ZP.StoreName ,ZBNCLL.localeId ,ZBNCL.localeId  SeoLocaleId ,ZBNC.IsApproved,
		CASE WHEN ZU.AspNetUserId IS NOT NULL THEN  ISNULL(ASZU.FirstName,'''')+'' ''+ISNULL(ASZU.LastName,'''') ELSE  ''Guest'' END  Customer ,ZBN.BlogNewsId,ZL1.Name LocaleName
	   FROM  ZnodeBlogNews ZBN
	   INNER  JOIN ZnodeBlogNewsLocale ZBNCLL On(ZBNCLL.BlogNewsId = ZBN.BlogNewsId)
	   INNER  JOIN ZnodeBlogNewsComment ZBNC ON (ZBNC.BlogNewsId = ZBN.BlogNewsId)
	   INNER  JOIN ZnodeBlogNewsCommentLocale ZBNCL ON (ZBNCL.BlogNewsCommentId = ZBNC.BlogNewsCommentId )-- AND ZBNCLL.LocaleId = ZBNCL.LocaleID )
	   LEFT JOIN ZnodePortal ZP ON (ZP.PortalId = ZBN.PortalId )
	   LEFT JOIN ZnodeUser ZU ON (ZU.UserId=ZBNC.UserId)
	   LEFT JOIN AspNetUsers ASU ON ( Asu.Id = ZU.AspNetUserId )
	   LEFT JOIN ZnodeUSer ASZU ON ( ASZU.AspNetUserId = ASU.Id )
	   LEFT JOIN ZnodeLocale ZL ON (ZL.LocaleID = ZBNCLL.localeId)
	   LEFT JOIN ZnodeLocale ZL1 ON (ZL1.LocaleID = ZBNCL.localeId)
	   WHERE EXISTS ( SELECT * FROM ZnodeBlogNewsCommentLocale ZBNC1 WHERE ZBNC.BlogNewsCommentId = ZBNC1.BlogNewsCommentId AND ZBNCL.localeId = ZBNC1.localeId )  
	   AND NOT EXiSTS ( SELECT * FROM ZnodeBlogNewsLocale ZBNL1 WHERE ZBNCLL.BlogNewsId = ZBNL1.BlogNewsId AND ZBNCL.localeId = ZBNL1.localeId )
	   AND NOT EXISTS( SELECT * FROM Cte_BlogNewForLocale A WHERE ZBNC.BlogNewsCommentId = ISNULL( A.BlogNewsCommentId, 0 ) )
	   AND ZL.IsDefault = 1 
	)
	,Cte_BlogCommentsfilter AS
	(
		SELECT * FROM Cte_BlogComments
		WHERE 1 = 1 '+[dbo].[Fn_GetFilterWhereClause](@whereClause)+'
	)
	,Cte_BlogWiseComments AS
	(
		SELECT * FROM Cte_BlogNewForLocale
		UNION ALL
		SELECT * FROM Cte_BlogCommentsfilter
	)
	,Cte_filterData AS 
	(
	  SELECT BlogNewsCommentId,BlogNewsTitle,BlogNewsType,[CreatedDate],StoreName,IsApproved,Customer,BlogNewsComment,'+dbo.Fn_GetPagingRowId(@Order_BY,'BlogNewsId DESC')+',Count(*)Over() CountNo,LocaleName
	  FROM Cte_BlogWiseComments
	) 
	SELECT BlogNewsCommentId,BlogNewsTitle,BlogNewsType,[CreatedDate],StoreName,IsApproved,Customer,BlogNewsComment,RowId, CountNo,LocaleName
	FROM Cte_filterData
	  '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows) 

	  PRINT @SQL 
 
	 INSERT INTO @TBL_GetBlogNewsComments (BlogNewsCommentId,BlogNewsTitle,BlogNewsType,[CreatedDate],StoreName,IsApproved,Customer,BlogNewsComment,RowId,CountNo,LocaleName)
	 EXEC (@SQL)

	 SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_GetBlogNewsComments),0)

	 SELECT BlogNewsCommentId,BlogNewsTitle,BlogNewsType,CreatedDate,StoreName,IsApproved,Customer,BlogNewsComment,RowId,CountNo,LocaleName
	 FROM @TBL_GetBlogNewsComments

 END TRY 
 BEGIN CATCH 
  SELECT ERROR_MESSAGE ()
 END CATCH 
 END
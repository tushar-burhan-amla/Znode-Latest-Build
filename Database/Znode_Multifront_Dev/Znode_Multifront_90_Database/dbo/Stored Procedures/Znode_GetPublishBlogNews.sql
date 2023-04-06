CREATE  PROCEDURE [dbo].[Znode_GetPublishBlogNews]
(
 @PortalId INT = 0 
  ,@LocaleId INT = 0 
)
AS
/*
   This Procedure is used to publish the blog news against the store 
  
 EXEC Znode_GetPublishBlogNews 1 



*/
BEGIN 
BEGIN TRY 
SET NOCOUNT ON
   DECLARE @SetLocaleId INT , @DefaultLocaleId INT = dbo.Fn_GetDefaultLocaleId(), @MaxCount INT =0 , @IncrementalId INT = 1  
   DECLARE @TBL_Locale TABLE (LocaleId INT , RowId INT IDENTITY(1,1))
   DECLARE @TBL_BlogData TABLE (BlogNewsId INT,PortalId INT ,MediaId INT ,BlogNewsType NVARCHAR(max),IsBlogNewsActive BIT ,IsAllowGuestComment BIT,LocaleId INT ,BlogNewsTitle NVARCHAR(max),CMSContentPagesId INT
   ,BodyOverview NVARCHAR(max),Tags NVARCHAR(max),BlogNewsContent NVARCHAR(max),CreatedDate DATETIME,ActivationDate DATETIME ,ExpirationDate DATETIME, MediaPath varchar(max),BlogNewsCode NVARCHAR(4000) )
   INSERT INTO @TBL_Locale (LocaleId)
   SELECT LocaleId 
   FROM ZnodeLocale 
   WHERE IsActive =1 
   AND (LocaleId  = @LocaleId OR @LocaleId = 0 )

   SET @MaxCount = ISNULL((SELECT MAx(RowId) FROM @TBL_Locale),0)
   
   WHILE @IncrementalId <= @MaxCount
   BEGIN 

   SET @SetLocaleId = (SELECT Top 1 LocaleId FROM @TBL_locale WHERE RowId = @IncrementalId)

  ;With Cte_GetCmsBlogNewsData AS 
  (
    SELECT ZBN.BlogNewsId,PortalId,ZBN.MediaId,BlogNewsType,IsBlogNewsActive,IsAllowGuestComment,ZBNL.LocaleId
	,BlogNewsTitle,ZBN.CMSContentPagesId,BodyOverview,Tags,BlogNewsContent,ZBN.CreatedDate,ActivationDate,ExpirationDate,zm.Path MediaPath,ZBN.BlogNewsCode
	FROM ZnodeBlogNews ZBN 
	INNER JOIN ZnodeBlogNewsLocale ZBNL ON (ZBNL.BlogNewsId = ZBN.BlogNewsId)
	LEFT JOIN ZnodeBlogNewsContent ZBNC ON ( ZBNC.BlogNewsId = ZBN.BlogNewsId AND ZBNC.LocaleId = ZBNL.LocaleId) 
	left join znodemedia ZM on(ZM.MediaId = ZBN.MediaId)
	WHERE (ZBNL.LocaleId = @SetLocaleId OR ZBNL.LocaleId = @DefaultLocaleId)  
	AND ZBn.PortalId = @PortalId 
	AND ZBN.IsBlogNewsActive = 1 
	
  )
  , Cte_GetFirstFilterData AS
  (
    SELECT BlogNewsId,PortalId,MediaId,BlogNewsType,IsBlogNewsActive,IsAllowGuestComment,LocaleId
	,BlogNewsTitle,CMSContentPagesId,BodyOverview,Tags,BlogNewsContent,CreatedDate,ActivationDate,ExpirationDate,MediaPath,BlogNewsCode
	FROM Cte_GetCmsBlogNewsData 
	WHERE LocaleId = @SetLocaleId
  )
  , Cte_GetDefaultFilterData AS
  (
   SELECT BlogNewsId,PortalId,MediaId,BlogNewsType,IsBlogNewsActive,IsAllowGuestComment,LocaleId
   ,BlogNewsTitle,CMSContentPagesId,BodyOverview,Tags,BlogNewsContent,CreatedDate,ActivationDate,ExpirationDate,MediaPath,BlogNewsCode
   FROM  Cte_GetFirstFilterData 
   UNION ALL 
   SELECT BlogNewsId,PortalId,MediaId,BlogNewsType,IsBlogNewsActive,IsAllowGuestComment,LocaleId
   ,BlogNewsTitle,CMSContentPagesId,BodyOverview,Tags,BlogNewsContent,CreatedDate,ActivationDate,ExpirationDate,MediaPath,BlogNewsCode
   FROM Cte_GetCmsBlogNewsData CTEC 
   WHERE LocaleId = @DefaultLocaleId 
   AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_GetFirstFilterData CTEFD WHERE CTEFD.BlogNewsId = CTEC.BlogNewsId )
   )
   INSERT INTO @TBL_BlogData (BlogNewsId,PortalId,MediaId,BlogNewsType,IsBlogNewsActive,IsAllowGuestComment,LocaleId,BlogNewsTitle
   ,CMSContentPagesId,BodyOverview,Tags,BlogNewsContent,CreatedDate,ActivationDate,ExpirationDate,MediaPath,BlogNewsCode)
   SELECT BlogNewsId,PortalId,MediaId,BlogNewsType,IsBlogNewsActive,IsAllowGuestComment,@SetLocaleId,BlogNewsTitle
   ,CMSContentPagesId,BodyOverview,Tags,BlogNewsContent,CreatedDate,ActivationDate,ExpirationDate,MediaPath,BlogNewsCode
   FROM Cte_GetDefaultFilterData

  SET @IncrementalId = @IncrementalId +1 
  END 

SELECT BlogNewsId,PortalId,MediaId,BlogNewsType,IsBlogNewsActive,IsAllowGuestComment,LocaleId,BlogNewsTitle,CMSContentPagesId,BodyOverview,Tags,BlogNewsContent,CreatedDate,ActivationDate,ExpirationDate,MediaPath,BlogNewsCode
FROM @TBL_BlogData 


END TRY 
BEGIN CATCH 
SELECT ERROR_MESSAGE()
END CATCH
END
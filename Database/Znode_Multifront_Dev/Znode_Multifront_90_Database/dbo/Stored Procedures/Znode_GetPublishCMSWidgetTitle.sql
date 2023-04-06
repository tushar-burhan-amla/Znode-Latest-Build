
 
CREATE PROCEDURE [dbo].[Znode_GetPublishCMSWidgetTitle]
(
 @PortalId INT = 0 
  ,@LocaleId INT = 0 
  ,@CMSContentPagesId INT = 0
)
AS 
/*
 This Procedure is used to get the record of content pages for publish  portal 
  EXEC Znode_GetPublishCMSWidgetTitle @PortalId =1,  @CMSContentPagesId = 280
*/
BEGIN 
 BEGIN TRY 
  SET NOCOUNT ON 
   
   DECLARE @TBL_LocaleId TABLE ( RowId INT IDENTITY(1,1) PRIMARY KEY , LocaleId INT )
   DECLARE @TBL_GetDATATable TABLE (WidgetTitleConfigurationId INT ,MappingId INT ,  PortalId INT 
			, LocaleId INT , MediaPath   VARCHAR(300) , Title nvarchar (600) ,TitleCode nvarchar(600) ,TypeOFMapping NVARCHAR(100) 
			,Url nvarchar(600)  , WidgetsKey  NVARCHAR(256),IsNewTab bit, DisplayOrder INT)
   INSERT INTO @TBL_LocaleId (LocaleId )
   SELECT LocaleId 
   FROm ZnodeLocale 
   WHERE IsActive = 1
   AND (LocaleId = @LocaleId OR   ISNULL(@LocaleId,0) = 0)
    
    DECLARE @SetLocaleId INT  ,@DefaultLocaleId INT = dbo.FN_GEtDEFAULTLocaleID() 
   DECLARE @CounterId INT = 1 , @MaxRowId INT = (SELECT Max(RowId ) FROM @TBL_LocaleId)  

   WHILE @CounterId <= @MaxRowId
   BEGIN 
   SET @SetLocaleId = (SELECT TOP 1 LocaleID FROM @TBL_LocaleId WHERe RowId =@CounterId )  
  
   DECLARE @TBL_TitleWidgetsData TABLE (CMSWidgetTitleConfigurationId INT , CMSWidgetTitleConfigurationLocaleId INT )
  ;With Cte_GetContentPagesId AS 
   (
     SELECT ZCWTC.CMSWidgetTitleConfigurationId , ZCWTCL.CMSWidgetTitleConfigurationLocaleId ,LocaleId, IsNewTab
	 FROM ZnodeCMSWidgetTitleConfiguration ZCWTC 
	 INNER JOIN ZnodeCMSWidgetTitleConfigurationLocale  ZCWTCL ON (ZCWTCL.CMSWidgetTitleConfigurationId = ZCWTC.CMSWidgetTitleConfigurationId )
     WHERE ZCWTCL.LocaleId   IN (@DefaultLocaleId,@SetLocaleId) 
   ) 
   , Cte_LocaleIdFiletr AS 
   (
    SELECT CMSWidgetTitleConfigurationId,CMSWidgetTitleConfigurationLocaleId
    FROM Cte_GetContentPagesId CET1 
	WHERE LocaleId = @SetLocaleId
    )
	,Cte_CompleteRecords AS 
	(
	 SELECT CMSWidgetTitleConfigurationId,CMSWidgetTitleConfigurationLocaleId
	 FROM Cte_LocaleIdFiletr
	 UNION ALL 
	 SELECT CMSWidgetTitleConfigurationId,CMSWidgetTitleConfigurationLocaleId
	 FROM Cte_GetContentPagesId  CTE2
	 WHERE Cte2.localeId = @DefaultLocaleId
	 AND NOT EXISTS (SELECT TOP 1 1 FROM Cte_LocaleIdFiletr CTE1 WHERE Cte1.CMSWidgetTitleConfigurationId = CTE2.CMSWidgetTitleConfigurationId )
	)


	INSERT INTO @TBL_TitleWidgetsData (CMSWidgetTitleConfigurationId,CMSWidgetTitleConfigurationLocaleId)
	SELECT CMSWidgetTitleConfigurationId,CMSWidgetTitleConfigurationLocaleId
	FROM Cte_CompleteRecords 
	 
	 INSERT INTO @TBL_GetDATATable (WidgetTitleConfigurationId  ,MappingId  ,  PortalId  , LocaleId  , MediaPath   , Title  ,TitleCode  ,TypeOFMapping  ,Url   , WidgetsKey,IsNewTab ,DisplayOrder)
	 SELECT ZCWTC.CMSWidgetTitleConfigurationId WidgetTitleConfigurationId,ZCWTC.CMSMappingId MappingId,CASE @CMSContentPagesId WHEN 0 THEN ZCWTC.CMSMappingId
													ELSE @PortalId END
							AS PortalId,@SetLocaleId LocaleId,ZM.[path]MediaPath,ZCWTCL.Title,ZCWTC.TitleCode,ZCWTC.TypeOFMapping,ZCWTCl.Url,ZCWTC.WidgetsKey,IsNewTab
							,ZCWTCL.DisplayOrder
	 FROM ZnodeCMSWidgetTitleConfiguration ZCWTC 
	 INNER JOIN ZnodeCMSWidgetTitleConfigurationLocale  ZCWTCL ON (ZCWTCL.CMSWidgetTitleConfigurationId = ZCWTC.CMSWidgetTitleConfigurationId )
	 INNER JOIN @TBL_TitleWidgetsData TBLW ON (TBLW.CMSWidgetTitleConfigurationLocaleId  =  ZCWTCL.CMSWidgetTitleConfigurationLocaleId)
	 LEFT JOIN ZnodeMedia ZM ON (Zm.MediaId = ZCWTCL.MediaId)
	 --WHERE ZCWTC.TypeOFMapping = 'PortalMapping'
	 --AND ZCWTC.CMSMappingId  = @PortalId
	 WHERE
	 ((ZCWTC.TypeOfMapping = 'PortalMapping'
                                  AND ZCWTC.CMSMappingId = @PortalId OR  @PortalId = 0)
                                  OR (ZCWTC.TypeOfMapping = 'ContentPageMapping'
                                     AND ZCWTC.CMSMappingId  = @CMSContentPagesId OR @CMSContentPagesId  = 0)
                                    )
	 --AND (CMSMappingId = @CMSContentPagesId OR @CMSContentPagesId  = 0)
	 --AND (CMSMappingId = @PortalId OR @PortalId  = 0)


	 SET @CounterId = @CounterId+1 
	 DELETE FROM @TBL_TitleWidgetsData
   END 

   SELECT WidgetTitleConfigurationId  ,MappingId  ,  PortalId  , LocaleId  , MediaPath   , Title  ,TitleCode  ,TypeOFMapping  ,Url   , WidgetsKey,IsNewTab,DisplayOrder
   FROM @TBL_GetDATATable

   END TRY 
   BEGIN CATCH 
   SELECT ERROR_MESSAGE()
   END CATCH 
   
END
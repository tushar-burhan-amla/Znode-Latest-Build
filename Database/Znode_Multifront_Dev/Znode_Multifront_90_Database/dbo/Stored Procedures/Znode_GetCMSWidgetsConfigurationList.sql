
CREATE PROCEDURE [dbo].[Znode_GetCMSWidgetsConfigurationList]
( @WhereClause VARCHAR(1000),
  @Rows        INT           = 1000,
  @PageNo      INT           = 0,
  @Order_BY    VARCHAR(100)  = NULL,
  @RowsCount   INT OUT , 
  @LocaleId    INT =0 )
AS  
    /*
	
    Summary: To get list of CMS widgets configuration 
    		 Use tablevariable @CMSWidgetsConfigurationList to sort data 	                  
    		 User view : View_CMSWidgetsConfigurationList
    
    Unit Testing   
    EXEC Znode_GetCMSWidgetsConfigurationList @WhereClause='cmswidgetsid = 2 and cmsmappingid = 1 and widgetskey like '%2253%' and typeofmapping like '%portalmapping%' and localeid = 2',@RowsCount=0,@Rows = 100,@PageNo=0,@Order_BY = Null

	*/

     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
		  DECLARE @DefultLocaleId INT  = dbo.FN_getDefaultLocaleId()

             DECLARE @SQL NVARCHAR(MAX);

             SET @SQL = '
			 DECLARE @CMSWidgetsConfigurationList  TABLE 
			(
									CMSWidgetTitleConfigurationId	int
									,CMSMappingId					int
									,cmsWidgetsId					int
									,WidgetsKey						nvarchar (256)
									,TypeOFMapping					nvarchar (100)
									,Title							nvarchar (600)
									,Url							nvarchar(Max)
									,MediaId						INT 
									,Image							varchar (300)
									,LocaleId						INT
									,TitleCode						nvarchar (600)
									,CMSWidgetTitleConfigurationLocaleId	int
									,IsNewTab						bit
									,DisplayOrder                   INT
								)
				;With Cte_Getdate AS (  
				SELECT a.CMSWidgetTitleConfigurationLocaleId, b.CMSWidgetTitleConfigurationId, b.CMSMappingId, b.cmsWidgetsId, b.WidgetsKey , b.TypeOFMapping ,a.Title,a.Url,a.LocaleId,b.TitleCode,a.MediaId,c.[Path] [Image]
				--,CASE WHEN a.IsNewTab = 1 THEN ''True''  ELSE ''False'' END IsNewTab
				,a.IsNewTab,a.DisplayOrder
				FROM ZnodeCMSWidgetTitleConfigurationLocale a
				LEFT JOIN ZnodeCMSWidgetTitleConfiguration b ON (b.CMSWidgetTitleConfigurationId = a.CMSWidgetTitleConfigurationId )
				LEFT JOIN ZNodeMedia c ON (a.MediaId = c.MediaId  )
				WHERE LocaleId IN ('+CAST(@DefultLocaleId AS VARCHAR(50))+','+CAST(@LocaleId AS VARCHAR(50))+')
				)

				, Cte_dsadga AS 
				(  SELECT * 
				  FROM Cte_Getdate WER 
				  WHERE WER.LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+'
				) 
				, Cte_there AS (
				  SELECT  CMSWidgetTitleConfigurationLocaleId	,CMSWidgetTitleConfigurationId	,CMSMappingId,	cmsWidgetsId	,WidgetsKey,	TypeOFMapping,	Title	,Url,	LocaleId	,TitleCode,	MediaId,	Image,IsNewTab,DisplayOrder
				  FROM Cte_dsadga 
				  UNION ALL 
				  SELECT CMSWidgetTitleConfigurationLocaleId	,CMSWidgetTitleConfigurationId	,CMSMappingId,	cmsWidgetsId	,WidgetsKey,	TypeOFMapping,	Title	,Url,	LocaleId	,TitleCode,	MediaId,	Image,IsNewTab,DisplayOrder
				  FROM Cte_Getdate WER 
				  WHERE WER.LocaleId = '+CAST(@DefultLocaleId AS VARCHAR(50))+' 
				  AND NOT EXISTS  (SELECT TOP 1 1 FROM  Cte_dsadga GTR WHERE GTR.CMSWidgetTitleConfigurationId = WER.CMSWidgetTitleConfigurationId)
				)
				 INSERT INTO @CMSWidgetsConfigurationList 
					SELECT CMSWidgetTitleConfigurationId, CMSMappingId, cmsWidgetsId, WidgetsKey , TypeOFMapping ,Title,Url,MediaId,
					[dbo].[FN_GetMediaThumbnailMediaPath]( [Image]),LocaleId,TitleCode,CMSWidgetTitleConfigurationLocaleId,IsNewTab,DisplayOrder
					FROM Cte_there 
  
					SELECT @Count = COUNT (1) FROM @CMSWidgetsConfigurationList

					SELECT * FROM @CMSWidgetsConfigurationList WHERE '+CASE
																		   WHEN @WhereClause = ''
																		   THEN '1=1'
																		   ELSE @WhereClause
																	   END+' Order BY '+ISNULL(CASE
																								   WHEN @Order_BY = ''
																								   THEN NULL
																								   ELSE @Order_BY
																							   END, '1');
						--select @SQL
						PRINT @SQL

             EXEC SP_executesql
                  @SQL,
                  N'@Count INT OUT',
                  @Count = @RowsCount OUT;

				  

         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCMSWidgetsConfigurationList @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCMSWidgetsConfigurationList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
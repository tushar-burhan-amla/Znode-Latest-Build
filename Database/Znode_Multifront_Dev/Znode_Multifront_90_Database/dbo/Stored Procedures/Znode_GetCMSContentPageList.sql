
CREATE PROCEDURE [dbo].[Znode_GetCMSContentPageList]
(@WhereClause NVARCHAR(3000),
 @Rows        INT            = 100,
 @PageNo      INT            = 1,
 @Order_BY    VARCHAR(1000)  = 'PageName',
 @RowsCount   INT OUT,
 @LocaleId    INT            = 1)
 
AS

/*
 Summary :
 This procedure use to find the list of Content pages associated to the portal 
 SELECT * FROM Information_schema.Columns WHERE COLUMN_NAME = 'profileid'
 EXEC Znode_GetCMSContentPageList 'ccp.portalid = 4 and profileid = 1 and (CMSContentPagesId = 11 or CMSContentPagesId = 12 or CMSContentPagesId = 13)' ,@RowsCount = 0 

*/
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY
             DECLARE @SQL NVARCHAR(MAX);
             SET @WhereClause = REPLACE(@WhereClause, 'CMSContentPagesId', 'ccp.CMSContentPagesId');
			 SET @SQL = '
					DECLARE @ConatenPage TABLE (CMSContentPagesId  INT ,PortalId INT , CMSTemplateId INT ,IsActive BIT ,PageName Nvarchar(200),SEOTitle NVARCHAR(200),SEODescription NVARCHAR(max) 
											,SEOKeywords NVARCHAR(max),SEOUrl NVARCHAR(max),ContentPageHtml NVARCHAR(max),PageTemplateName NVARCHAR(200), PageTemplateFileName NVARCHAR(4000),ActivationDate DATETIME , DeactivationDate DATETIME  ,LocaleId INT, CanonicalURL VARCHAR(200), RobotTag VARCHAR(50)  )
					INSERT INTO @ConatenPage
					SELECT  ccp.CMSContentPagesId ,ccp.PortalId, ccp.CMSTemplateId,IsActive,PageName ,ZCSDL.SEOTitle,ZCSDL.SEODescription ,ZCSDL.SEOKeywords,csd.SEOUrl,ccpl.text ContentPageHtml,ct.Name PageTemplateName, ct.FileName PageTemplateFileName,ccp.ActivationDate, ccp.ExpirationDate DeactivationDate  ,ccpl.LocaleId,
					        ZCSDL.CanonicalURL, ZCSDL.RobotTag
					FROM ZnodeCMSContentPages ccp  
					INNER  JOIN ZnodeCMSContentPagesProfile cpp ON (cpp.CMSContentPagesId = ccp.CMSContentPagesId)
					INNER JOIN ZnodeCMSTextWidgetConfiguration ccpl ON (ccpl.CMSMappingId = ccp.CMSContentPagesId AND TypeOFMapping = ''ContentPageMapping'')
					INNER JOIN ZnodeCMSTemplate ct ON (ct.CMSTemplateId = ccp.CMSTemplateId)
					LEFT JOIN ZnodeCMSSEODetail csd ON (EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEOType ww WHERE ww.Name = ''Content Page'' AND ccp.PageName = csd.SEOCode  )  )
					LEFT JOIN ZnodeCMSSEODetailLocale ZCSDL ON (ZCSDL.CMSSEODetailId = csd.CMSSEODetailId AND ZCSDL.LocaleId = '+CAST(@LocaleId AS VARCHAR(100))+') 
					WHERE 1=1 '+CASE
									WHEN @WhereClause = ''
									THEN ''
									ELSE ' AND '+@WhereClause
								END+' ORDER BY '+CASE
													 WHEN @Order_BY = ''
													 THEN ' 1 '
													 ELSE ' '+@Order_BY
												 END+' SELECT @Count = COUNT (1) FROM @ConatenPage   SELECT * FROM @ConatenPage '; 

								 EXEC SP_executesql
									  @SQL,
									  N'@Count INT OUT ',
									  @Count = @RowsCount OUT;
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCMSContentPageList @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCMSContentPageList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END
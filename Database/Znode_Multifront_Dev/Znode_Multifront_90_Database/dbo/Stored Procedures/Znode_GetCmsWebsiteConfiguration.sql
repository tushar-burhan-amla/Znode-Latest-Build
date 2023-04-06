CREATE PROCEDURE [dbo].[Znode_GetCmsWebsiteConfiguration]
( @WhereClause VARCHAR(1000),
  @Rows        INT           = 100,
  @PageNo      INT           = 1,
  @Order_BY    VARCHAR(100)  = '',
  @RowsCount   INT OUT,
  @LocaleId    INT           = 1)
AS
  /*  
    Summary : this procedure is used to Get the Website CMS Theme Details in Descending Order
    Unit Testing 	
     EXEC Znode_GetCmsWebsiteConfiguration  '',@Order_BY = '' ,@RowsCount= 1 
   
	*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
			 DECLARE @CmsThemeDetail TABLE ( CMSPortalThemeId INT ,PortalId INT ,StoreName NVARCHAR(max),CMSThemeId INT , ThemeName varchar(200),RowId INT,CountNo INT  )
             DECLARE @DefaultLocaleId VARCHAR(100)= (SELECT TOP 1 FeatureValues FROM ZnodeGlobalSetting WHERE FeatureName = 'Locale')

             SET @SQL = '
				
				;With CmsThemeDetail AS 
				(	
				SELECT CMSPortalThemeId,ZCPT.PortalId ,ZP.StoreName,ZCPT.CMSThemeId , ZCT.Name ThemeName
				FROM ZnodeCMSPortalTheme ZCPT
				INNER JOIN ZnodeCMSTheme ZCT ON (ZCT.CMSThemeId = ZCPT.CMSThemeId)
				INNER JOIN ZnodePortal Zp ON (ZP.PortalId = ZCPT.PortalId) 
				) 
				, FilterAboveDataHere AS
				(
				 SELECT *,'+dbo.Fn_GetPagingRowId(@Order_BY,'CMSPortalThemeId DESC')+',Count(*)Over() CountNo
				 FROM  CmsThemeDetail 
				 WHERE 1=1
			     '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
     			)

				SELECT CMSPortalThemeId,PortalId,StoreName,CMSThemeId,ThemeName,RowId,CountNo
				FROM FilterAboveDataHere
				'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
								
				INSERT INTO @CmsThemeDetail 
				EXEC(@SQL)

				SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @CmsThemeDetail),0)
				
				SELECT CMSPortalThemeId,PortalId,StoreName,CMSThemeId,ThemeName
				FROM @CmsThemeDetail  
				                        
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCmsWebsiteConfiguration @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCmsWebsiteConfiguration',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
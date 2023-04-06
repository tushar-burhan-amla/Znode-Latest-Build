CREATE PROCEDURE [dbo].[Znode_GetPortalCMSTheme]
( @WhereClause NVarchar(Max)  = '',
  @Rows        INT            = 100,
  @PageNo      INT            = 1,
  @Order_BY VARCHAR(1000)	  = '',
  @RowsCount   INT OUT,
  @LocaleId    INT            = 0)
AS

/*
 Summary : This Procedure is used to get  Portaldetails and CMSTHEME details		  
 Unit Testing
 EXEC Znode_GetPortalCMSTheme  @RowsCount = 0
 
*/
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY

		 --SELECT ZP.PortalId, ZP.StoreName, ZP.CompanyName ,ZCTC.CSSName,ZT.Name as ThemeName, ZPBC.CatalogName 
		 --from ZnodePortal ZP
		 --inner join ZnodeCMSPortalTheme ZCPT on (ZP.PortalId = ZCPT.PortalId)
		 --inner join ZnodeCMSThemeCSS ZCTC on (ZCPT.CMSThemeId = ZCTC.CMSThemeId)
		 --inner join ZnodeCMSTheme ZT on (ZCTC.CMSThemeId = ZT.CMSThemeId)
		 --inner join ZnodePortalCatalog ZPC on (ZP.PortalId = ZPC.PortalId)
		 --inner join ZnodePublishCatalog ZPBC on (ZPBC.PublishCatalogId = ZPC.PublishCatalogId)
		 
		  DECLARE @SQL NVARCHAR(MAX);
		  DECLARE @TBL_CmsThemeDetails TABLE (PortalId INT,CompanyName NVARCHAR(MAX),StoreName NVARCHAR(MAX),ThemeName VARCHAR(200),CSSName NVARCHAR(2000),CatalogName NVARCHAR(MAX),RowId INT, CountNo INT)

			SET @SQL = '

			;WITH CTE_GetCmsTheme AS
			(
			SELECT  ZP.PortalId, ZP.CompanyName ,ZP.StoreName, ZCT.Name as ThemeName, ZCTC.CSSName, ZPUC.CatalogName
			FROM ZnodeCMSPortalTheme AS ZCPT 
			INNER JOIN ZnodeCMSThemeCSS AS ZCTC ON ZCPT.CMSThemeCSSId = ZCTC.CMSThemeCSSId 
			INNER JOIN ZnodeCMSTheme AS ZCT ON ZCPT.CMSThemeId = ZCT.CMSThemeId AND ZCTC.CMSThemeId = ZCT.CMSThemeId 
			INNER JOIN ZnodePortal AS ZP ON ZCPT.PortalId = ZP.PortalId 
			INNER JOIN ZnodePortalCatalog ZPC ON ZP.PortalId = ZPC.PortalId 
			INNER JOIN ZnodePublishCatalog ZPUC ON ZPC.PublishCatalogId = ZPUC.PublishCatalogId )

			,CTE_GetCmsThemeList AS
			(
			SELECT  PortalId, CompanyName ,  StoreName,  ThemeName, CSSName, CatalogName, '+dbo.Fn_GetPagingRowId(@Order_BY,'PortalId DESC')+',Count(*)Over() CountNo 
			FROM CTE_GetCmsTheme
			WHERE 1=1 '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'	
			)

			SELECT PortalId, CompanyName ,  StoreName,  ThemeName, CSSName, CatalogName,RowId,CountNo
			FROM CTE_GetCmsThemeList
			'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
			
			INSERT INTO @TBL_CmsThemeDetails
			EXEC(@SQL)
			
			SET @RowsCount =ISNULL((SELECT TOP 1 CountNo FROM @TBL_CmsThemeDetails ),0)
			
			SELECT PortalId, CompanyName ,  StoreName,  ThemeName, CSSName, CatalogName
			FROM @TBL_CmsThemeDetails

		 END TRY
		 BEGIN CATCH
		   DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPortalCMSTheme @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPortalCMSTheme',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		 END CATCH

    END
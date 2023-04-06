
CREATE PROCEDURE [dbo].[Znode_GetStoreList]
(
	@WhereClause NVARCHAR(MAX),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
	@UserId		 INT=0,
    @RowsCount   INT OUT

)
AS 
/*
	 Summary :- This Procedure is used to get the publish status of the Portal 
	 Unit Testig
	 begin tran 
	 EXEC  Znode_GetStoreList '',100,'','',0

	 EXEC  Znode_GetStoreList '',100,1,null,2,0
	 rollback tran
	 select * from ZnodeDomain
*/
   BEGIN 
		BEGIN TRY 
			SET NOCOUNT ON 

			 DECLARE @SQL  NVARCHAR(MAX) 
			 DECLARE @TBL_PortalId TABLE (PortalId INT, PublishPortalLogId INT,StoreName NVARCHAR(MAX),CompanyName NVARCHAR(MAX),LogoPath NVARCHAR(MAX),UseSSL BIT,AdminEmail 
			 NVARCHAR(MAX),SalesEmail NVARCHAR(MAX),CustomerServiceEmail NVARCHAR(MAX),SalesPhoneNumber NVARCHAR(MAX),CustomerServicePhoneNumber NVARCHAR(MAX),ImageNotAvailablePath NVARCHAR(MAX),ShowSwatchInCategory BIT,
			 ShowAlternateImageInCategory BIT,ExternalID VARCHAR(50),MobileLogoPath NVARCHAR(MAX),DefaultOrderStateID INT,DefaultReviewStatus NVARCHAR(2),SplashCategoryID INT,SplashImageFile NVARCHAR(MAX),
			 MobileTheme NVARCHAR(MAX),CopyContentBasedOnPortalId INT,CreatedBy INT,CreatedDate DATETIME,ModifiedBy INT,ModifiedDate DATETIME,InStockMsg NVARCHAR(max),OutOfStockMsg NVARCHAR(MAX),BackOrderMsg NVARCHAR(MAX)
			 ,PublishStatus VARCHAR(300),ThemeName VARCHAR(200),CSSName NVARCHAr(2000),CatalogName NVARCHAR(max),DomainUrl NVARCHAR(200),OrderStatus NVARCHAR(200),LocaleId INT,PublishCatalogId INT,StoreCode NVARCHAR(200),RowId INT ,CountId INT)
	

			 SET @SQL = '
			 ;With Cte_MaxPublish AS 
			 (
			 SELECT MAX(PublishPortalLogId) PublishPortalLogId,PortalId
			 FROM ZnodePublishPortalLog ZPCL  
			 GROUP BY PortalId
			 )
			 ,Cte_PortalLog AS (
			 SELECT ZPC.StoreName ,ZPC.CompanyName ,ZPC.LogoPath ,ZPC.UseSSL ,ZPC.AdminEmail ,ZPC.SalesEmail ,ZPC.CustomerServiceEmail ,ZPC.SalesPhoneNumber ,ZPC.CustomerServicePhoneNumber ,
			 ZPC.ImageNotAvailablePath ,ZPC.ShowSwatchInCategory ,ZPC.ShowAlternateImageInCategory ,ZPC.ExternalID ,ZPC.MobileLogoPath ,ZPC.DefaultOrderStateID ,ZPC.DefaultReviewStatus ,
			 ZPC.SplashCategoryID ,ZPC.SplashImageFile ,ZPC.MobileTheme ,ZPC.CopyContentBasedOnPortalId ,
			 ZPC.CreatedBy ,ZPC.CreatedDate ,ZPC.ModifiedBy ,ZPC.ModifiedDate ,ZPC.InStockMsg ,ZPC.OutOfStockMsg ,ZPC.BackOrderMsg , PublishPortalLogId ,DT.DisplayName    PublishStatus ,ZPC.PortalId
			 , ZCT.Name as ThemeName, ZCTC.CSSName, ZPUC.CatalogName,
			 CASE WHEN ZD.DomainName IS NULL  THEN ''#'' ELSE ZD.DomainName END DomainUrl,ZOOS.Description OrderStatus,
			 ZPL.LocaleId, ZPPC.PublishCatalogId,ZPC.StoreCode
			FROM ZnodePortal ZPC 
			INNER JOIN ZnodeCMSPortalTheme AS ZCPT ON (ZCPT.PortalId = ZPC.PortalId )
			INNER JOIN ZnodeCMSThemeCSS AS ZCTC ON ZCPT.CMSThemeCSSId = ZCTC.CMSThemeCSSId 
			INNER JOIN ZnodeCMSTheme AS ZCT ON ZCPT.CMSThemeId = ZCT.CMSThemeId AND ZCTC.CMSThemeId = ZCT.CMSThemeId 
			LEFT JOIN ZnodePortalCatalog ZPCI ON ZPCI.PortalId = ZPC.PortalId 
			LEFT JOIN ZnodePimCatalog ZPUC ON ZPCI.PublishCatalogId = ZPUC.PimCatalogId 
			LEFT JOIN ZnodeOmsOrderState ZOOS ON ZPC.DefaultOrderStateID = ZOOS.OmsOrderStateId
			LEFT JOIN ZnodePortalCatalog ZPPC ON ZPPC.PortalId = ZPC.PortalId
			LEFT JOIN ZnodePortalLocale ZPL ON (ZPL.LocaleId = ( select TOP 1 LocaleId from ZnodePortalLocale WHERE ZPC.PortalId = ZPL.PortalId AND ZPL.IsDefault = 1)) 
			LEFT JOIN ZnodePublishPortalLog ZPCL  ON ( EXISTS (SELECT TOP 1 1 FROM Cte_MaxPublish CTE WHERE CTE.PortalId = ZPC.PortalId AND CTE.PublishPortalLogId =  ZPCL.PublishPortalLogId)  )
			LEFT JOIN ZnodePublishState DT ON (DT.PublishStateId = ZPCL.PublishStateId)
			LEFT JOIN ZnodeDomain ZD ON (ZD.DomainId = (SELECT TOP 1 DomainId FROM ZnodeDomain ZDR WHERE ZDR.PortalId = ZPC.PortalId AND ZDR.IsActive = 1 AND ZDR.ApplicationType = ''WebStore''))
			WHERE EXISTS (SELECT TOP 1 1 FROM ZnodeUserPortal ZUP WHERE (ZUP.PortalId=ZPC.PortalId OR ZUP.PortalId IS NULL)   AND ZUP.UserId='+CAST(@UserId AS VARCHAR (200)) +')
			 )
			 ,Cte_PublishStatus 
			 AS (
			 SELECT PortalId,PublishPortalLogId, StoreName,CompanyName,LogoPath,UseSSL,AdminEmail,SalesEmail,CustomerServiceEmail,SalesPhoneNumber,CustomerServicePhoneNumber,ImageNotAvailablePath,
			 ShowSwatchInCategory,ShowAlternateImageInCategory,ExternalID,MobileLogoPath,DefaultOrderStateID,DefaultReviewStatus,SplashCategoryID,SplashImageFile,MobileTheme,CopyContentBasedOnPortalId,
			 CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,InStockMsg,OutOfStockMsg,BackOrderMsg, PublishStatus,ThemeName,CSSName,CatalogName,DomainUrl,OrderStatus,LocaleId,PublishCatalogId,StoreCode,
			 '+[dbo].[Fn_GetPagingRowId](@Order_BY,'PortalId ,PublishPortalLogId DESC')+' , Count(*)Over() CountId FROM Cte_PortalLog
			 WHERE 1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+' )
	 
			 SELECT PortalId,PublishPortalLogId,StoreName,CompanyName,LogoPath,UseSSL,AdminEmail,SalesEmail,CustomerServiceEmail,SalesPhoneNumber,CustomerServicePhoneNumber,ImageNotAvailablePath,
			 ShowSwatchInCategory,ShowAlternateImageInCategory,ExternalID,MobileLogoPath,DefaultOrderStateID,DefaultReviewStatus,SplashCategoryID,SplashImageFile,MobileTheme,CopyContentBasedOnPortalId,
			 CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,InStockMsg,OutOfStockMsg,BackOrderMsg,PublishStatus,ThemeName,CSSName,CatalogName,DomainUrl,OrderStatus,LocaleId,PublishCatalogId,StoreCode,RowId,CountId 
			 FROM Cte_PublishStatus 
			 '+[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)
	

	         PRINT @SQL
			 INSERT INTO @TBL_PortalId 
			
			 EXEC (@SQL)

			 SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM @TBL_PortalId),0)

		
			 SELECT  PortalId,PublishPortalLogId,StoreName,CompanyName,LogoPath,UseSSL,AdminEmail,SalesEmail,CustomerServiceEmail,SalesPhoneNumber,CustomerServicePhoneNumber,ImageNotAvailablePath,
			 ShowSwatchInCategory,ShowAlternateImageInCategory,ExternalID,MobileLogoPath,DefaultOrderStateID,DefaultReviewStatus,SplashCategoryID,SplashImageFile,MobileTheme,CopyContentBasedOnPortalId,
			 CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,InStockMsg,OutOfStockMsg,BackOrderMsg,PublishStatus,ThemeName,CSSName,CatalogName,DomainUrl,OrderStatus,LocaleId,PublishCatalogId,StoreCode
			 FROM @TBL_PortalId
	 
				

		 END TRY 
		 BEGIN CATCH 
		 SELECT ERROR_MESSAGE()
			DECLARE @Status BIT ;
			SET @Status = 0;
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetStoreList @WhereClause = '''+ISNULL(@WhereClause,'''''')+''',@Rows='+ISNULL(CAST(@Rows AS
			VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')
             
			SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		 
			EXEC Znode_InsertProcedureErrorLog
					@ProcedureName = 'Znode_GetStoreList',
					@ErrorInProcedure = 'Znode_GetStoreList',
					@ErrorMessage = @ErrorMessage,
					@ErrorLine = @ErrorLine,
					@ErrorCall = @ErrorCall;
		 END CATCH 
   END
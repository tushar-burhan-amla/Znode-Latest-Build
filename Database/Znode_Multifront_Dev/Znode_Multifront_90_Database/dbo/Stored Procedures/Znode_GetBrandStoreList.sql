CREATE PROCEDURE [dbo].[Znode_GetBrandStoreList]
(
	@WhereClause NVARCHAR(MAX),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
    @RowsCount   INT OUT
)
AS 
/*
	 Summary :- This Procedure is used to get the publish status of the Portal 
	 Unit Testig 
	 EXEC  Znode_GetStoreList '',100,1,'',0
	 select * from ZnodeDomainR
*/
   BEGIN 
		BEGIN TRY 
			SET NOCOUNT ON 

			 DECLARE @SQL  NVARCHAR(MAX) 
			 DECLARE @TBL_PortalId TABLE (PortalId INT, StoreName NVARCHAR(MAX),CompanyName NVARCHAR(MAX),LogoPath NVARCHAR(MAX),UseSSL BIT,AdminEmail 
			 NVARCHAR(MAX),SalesEmail NVARCHAR(MAX),CustomerServiceEmail NVARCHAR(MAX),SalesPhoneNumber NVARCHAR(MAX),CustomerServicePhoneNumber NVARCHAR(MAX),ImageNotAvailablePath NVARCHAR(MAX),ShowSwatchInCategory BIT,
			 ShowAlternateImageInCategory BIT,ExternalID VARCHAR(50),MobileLogoPath NVARCHAR(MAX),DefaultOrderStateID INT,DefaultReviewStatus NVARCHAR(2),SplashCategoryID INT,SplashImageFile NVARCHAR(MAX),
			 MobileTheme NVARCHAR(MAX),CopyContentBasedOnPortalId INT,CreatedBy INT,CreatedDate DATETIME,ModifiedBy INT,ModifiedDate DATETIME,InStockMsg NVARCHAR(max),OutOfStockMsg NVARCHAR(MAX),BackOrderMsg NVARCHAR(MAX),
			 --PublishStatus VARCHAR(300),ThemeName VARCHAR(200),CSSName NVARCHAr(2000),CatalogName NVARCHAR(max),DomainUrl NVARCHAR(200),OrderStatus NVARCHAR(200),LocaleId INT,PublishCatalogId INT,
			 BrandId int, BrandCode NVARCHAR(600), BrandName NVARCHAR(600), LocaleId int, IsAssociatedStore bit ,RowId INT ,CountId INT)
	 
			 SET @SQL = '
			 ;With Cte_PortalLog AS 
			 (
				 SELECT ZPC.PortalId, ZPC.StoreName ,ZPC.CompanyName ,ZPC.LogoPath ,ZPC.UseSSL ,ZPC.AdminEmail ,ZPC.SalesEmail ,ZPC.CustomerServiceEmail ,ZPC.SalesPhoneNumber ,ZPC.CustomerServicePhoneNumber ,
				 ZPC.ImageNotAvailablePath ,ZPC.ShowSwatchInCategory ,ZPC.ShowAlternateImageInCategory ,ZPC.ExternalID ,ZPC.MobileLogoPath ,ZPC.DefaultOrderStateID ,ZPC.DefaultReviewStatus ,
				 ZPC.SplashCategoryID ,ZPC.SplashImageFile ,ZPC.MobileTheme ,ZPC.CopyContentBasedOnPortalId ,
				 ZPC.CreatedBy ,ZPC.CreatedDate ,ZPC.ModifiedBy ,ZPC.ModifiedDate ,ZPC.InStockMsg ,ZPC.OutOfStockMsg ,ZPC.BackOrderMsg , ZBD.BrandId, ZBD.BrandCode, ZBDL.BrandName, ZBDL.LocaleId,
				 case when ZBP.PortalId is null then 0 else 1 end IsAssociatedStore , ZBD.IsActive, ZBD.DisplayOrder
				FROM ZnodePortal ZPC 
				CROSS APPLY ZnodeBrandDetails AS ZBD 
				INNER JOIN ZnodeBrandDetailLocale AS ZBDL ON ( ZBD.BrandId = ZBDL.BrandId )
				LEFT JOIN ZnodeBrandPortal AS ZBP ON (ZBP.PortalId = ZPC.PortalId and ZBP.BrandId = ZBD.BrandId)
				--LEFT JOIN ZnodeBrandPortal AS ZBP ON (ZBP.PortalId = ZPC.PortalId )
				--LEFT JOIN ZnodeBrandDetails AS ZBD ON ( ZBP.BrandId = ZBD.BrandId )
				--LEFT JOIN ZnodeBrandDetailLocale AS ZBDL ON ( ZBD.BrandId = ZBDL.BrandId )
			 )
			 ,Cte_PublishStatus AS 
			 (
				 SELECT PortalId, StoreName,CompanyName,LogoPath,UseSSL,AdminEmail,SalesEmail,CustomerServiceEmail,SalesPhoneNumber,CustomerServicePhoneNumber,ImageNotAvailablePath,
				 ShowSwatchInCategory,ShowAlternateImageInCategory,ExternalID,MobileLogoPath,DefaultOrderStateID,DefaultReviewStatus,SplashCategoryID,SplashImageFile,MobileTheme,CopyContentBasedOnPortalId,
				 CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,InStockMsg,OutOfStockMsg,BackOrderMsg, BrandId, BrandCode, BrandName,LocaleId,IsAssociatedStore,IsActive, DisplayOrder,
				 '+[dbo].[Fn_GetPagingRowId](@Order_BY,'PortalId  DESC')+' , Count(*)Over() CountId FROM Cte_PortalLog
				 WHERE 1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+' 
			)	 
			 SELECT PortalId, StoreName,CompanyName,LogoPath,UseSSL,AdminEmail,SalesEmail,CustomerServiceEmail,SalesPhoneNumber,CustomerServicePhoneNumber,ImageNotAvailablePath,
			 ShowSwatchInCategory,ShowAlternateImageInCategory,ExternalID,MobileLogoPath,DefaultOrderStateID,DefaultReviewStatus,SplashCategoryID,SplashImageFile,MobileTheme,CopyContentBasedOnPortalId,
			 CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,InStockMsg,OutOfStockMsg,BackOrderMsg,BrandId, BrandCode, BrandName,LocaleId,IsAssociatedStore,RowId,CountId 
			 FROM Cte_PublishStatus 
			 '+[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)
	
	         PRINT @SQL
			 INSERT INTO @TBL_PortalId 
			 EXEC (@SQL)

			 SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM @TBL_PortalId),0)

		 
			 SELECT  PortalId,StoreName,CompanyName,LogoPath,UseSSL,AdminEmail,SalesEmail,CustomerServiceEmail,SalesPhoneNumber,CustomerServicePhoneNumber,ImageNotAvailablePath,
			 ShowSwatchInCategory,ShowAlternateImageInCategory,ExternalID,MobileLogoPath,DefaultOrderStateID,DefaultReviewStatus,SplashCategoryID,SplashImageFile,MobileTheme,CopyContentBasedOnPortalId,
			 CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,InStockMsg,OutOfStockMsg,BackOrderMsg,BrandId, BrandCode, BrandName,LocaleId, IsAssociatedStore
			 FROM @TBL_PortalId
	 
		 END TRY 
		 BEGIN CATCH 
			DECLARE @Status BIT ;
			SET @Status = 0;
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetBrandStoreList @WhereClause = '+@WhereClause+',@Rows='+CAST(@Rows AS
			VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
			SELECT 0 AS ID,CAST(0 AS BIT) AS Status; 
			select ERROR_MESSAGE()

			EXEC Znode_InsertProcedureErrorLog
					@ProcedureName = 'Znode_GetBrandStoreList',
					@ErrorInProcedure = @Error_procedure,
					@ErrorMessage = @ErrorMessage,
					@ErrorLine = @ErrorLine,
					@ErrorCall = @ErrorCall;
		 END CATCH 
   END
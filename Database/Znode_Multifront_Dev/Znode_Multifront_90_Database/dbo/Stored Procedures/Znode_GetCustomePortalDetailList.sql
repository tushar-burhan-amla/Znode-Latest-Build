CREATE PROCEDURE [dbo].[Znode_GetCustomePortalDetailList]
( @WhereClause NVarchar(Max)  = '',
  @Rows        INT            = 100,
  @PageNo      INT            = 1,
  @Order_BY VARCHAR(1000)	  = '',
  @RowsCount   INT OUT
 )
AS

/*
 Summary : This Procedure is used to get CustomPortalDetailList 
 Unit Testing
 EXEC [Znode_GetCustomePortalDetailList]  @RowsCount = 0
 
*/
     BEGIN
         SET NOCOUNT ON;
         BEGIN TRY

		  DECLARE @SQL NVARCHAR(MAX);
		  DECLARE @TBL_GetCustomePortalDetailList TABLE (CustomePortalDetailsId INT,PortalId INT,CustomeData1 VARCHAR(max) 
		  ,CustomeData2 VARCHAR(max),CustomeData3 VARCHAR(max),CompanyName nvarchar(max) ,
		  StoreName nvarchar(max) ,LogoPath nvarchar(max) ,AdminEmail nvarchar(max),UseSSL bit ,SalesEmail nvarchar(max),CustomerServiceEmail nvarchar(max) ,
		  SalesPhoneNumber nvarchar(max),CustomerServicePhoneNumber nvarchar(max) ,ImageNotAvailablePath nvarchar(max) ,ShowSwatchInCategory bit ,
		  ShowAlternateImageInCategory bit ,ExternalID varchar(50) ,MobileLogoPath nvarchar(max) ,DefaultOrderStateID int,DefaultReviewStatus nvarchar(1),
		  SplashCategoryID int ,SplashImageFile nvarchar(max) ,	MobileTheme nvarchar(max) ,CopyContentBasedOnPortalId int ,InStockMsg nvarchar(max),
		  OutOfStockMsg nvarchar(max),BackOrderMsg nvarchar(max),RowId INT,CountNo INT )

					SET @SQL = '
					;WITH CTE_GetCustomePortalDetail AS
					(
					SELECT CustomePortalDetailsId,zp. PortalId,CustomeData1,CustomeData2,CustomeData3,CompanyName,StoreName,LogoPath,AdminEmail,UseSSL
					,SalesEmail,CustomerServiceEmail,SalesPhoneNumber,CustomerServicePhoneNumber,ImageNotAvailablePath,ShowSwatchInCategory,ShowAlternateImageInCategory
					,ExternalID,MobileLogoPath,DefaultOrderStateID,DefaultReviewStatus,SplashCategoryID,SplashImageFile,MobileTheme,CopyContentBasedOnPortalId
					,InStockMsg,OutOfStockMsg,BackOrderMsg	
					FROM ZnodePortal ZP LEFT JOIN ZnodeCustomPortalDetail ZCPD ON (ZCPD.PortalId= ZP.PortalId)		   
		   
					)

					,CTE_GetCustomePortalDetailList AS
					(
					SELECT  CustomePortalDetailsId,PortalId,CustomeData1,CustomeData2,CustomeData3,CompanyName,StoreName,LogoPath,AdminEmail,UseSSL
					,SalesEmail,CustomerServiceEmail,SalesPhoneNumber,CustomerServicePhoneNumber,ImageNotAvailablePath,ShowSwatchInCategory,ShowAlternateImageInCategory
					,ExternalID,MobileLogoPath,DefaultOrderStateID,DefaultReviewStatus,SplashCategoryID,SplashImageFile,MobileTheme,CopyContentBasedOnPortalId
					,InStockMsg,OutOfStockMsg,BackOrderMsg	,'+dbo.Fn_GetPagingRowId(@Order_BY,'CustomePortalDetailsId DESC,PortalId DESC')+',Count(*)Over() CountNo
					FROM CTE_GetCustomePortalDetail
					WHERE 1=1 
					'+dbo.Fn_GetFilterWhereClause(@WhereClause)+'	
		   
					)

					SELECT CustomePortalDetailsId,PortalId,CustomeData1,CustomeData2,CustomeData3,CompanyName,StoreName,LogoPath,AdminEmail,UseSSL
					,SalesEmail,CustomerServiceEmail,SalesPhoneNumber,CustomerServicePhoneNumber,ImageNotAvailablePath,ShowSwatchInCategory,ShowAlternateImageInCategory
					,ExternalID,MobileLogoPath,DefaultOrderStateID,DefaultReviewStatus,SplashCategoryID,SplashImageFile,MobileTheme,CopyContentBasedOnPortalId
					,InStockMsg,OutOfStockMsg,BackOrderMsg	,RowId,CountNo
					FROM CTE_GetCustomePortalDetailList
					'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)

					
					INSERT INTO @TBL_GetCustomePortalDetailList
					EXEC(@SQL);
					
					SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_GetCustomePortalDetailList), 0);

					SELECT CustomePortalDetailsId,PortalId,CustomeData1,CustomeData2,CustomeData3,CompanyName,CompanyName,StoreName,LogoPath,AdminEmail,UseSSL
					,SalesEmail,CustomerServiceEmail,SalesPhoneNumber,CustomerServicePhoneNumber,ImageNotAvailablePath,ShowSwatchInCategory,ShowAlternateImageInCategory
					,ExternalID,MobileLogoPath,DefaultOrderStateID,DefaultReviewStatus,SplashCategoryID,SplashImageFile,MobileTheme,CopyContentBasedOnPortalId
					,InStockMsg,OutOfStockMsg,BackOrderMsg
					FROM @TBL_GetCustomePortalDetailList

		   END TRY
		   BEGIN CATCH
		     DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCustomePortalDetailList @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCustomePortalDetailList',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;

				SELECT ERROR_MESSAGE()
		  END CATCH

   END
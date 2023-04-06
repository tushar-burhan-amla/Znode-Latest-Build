CREATE PROCEDURE [dbo].[Znode_GetCMSCustomerReviewInformation]
( @WhereClause NVARCHAR(Max),
  @Rows        INT           = 100,
  @PageNo      INT           = 1,
  @Order_BY    VARCHAR(1000)  = '',
  @RowsCount   INT OUT,
  @LocaleId    INT           = 0,
  @PortalId    INT           = 0
  )
AS
/*
 Summary : Procedure is used to Get Customer Review Information.
 Unit Testing:
 exec Znode_GetCMSCustomerReviewInformation @WhereClause='',@RowsCount=null,@Rows = 100,@PageNo=1,@Order_BY = '',@PortalId = 0,@LocaleId = 1
*/
 BEGIN
   BEGIN TRY
      SET NOCOUNT ON;
		DECLARE @SQL NVARCHAR(MAX);
		IF @LocaleId = 0
		BEGIN
		SELECT @LocaleId = dbo.Fn_GetDefaultLocaleId();
		END;
		DECLARE @TBL_CustomerReview TABLE (CMSCustomerReviewId INT ,PublishProductId INT ,UserId INT,Headline NVARCHAR(400) ,Comments NVARCHAR(MAX),UserName NVARCHAR(600),StoreName NVARCHAR(600)
		,UserLocation NVARCHAR(2000),Rating INT,[Status] NVARCHAR(20),ProductName NVARCHAR(max),CreatedDate DATETIME,ModifiedDate DATETIME,CreatedBy INT,ModifiedBy INT,SEOUrl NVARCHAR(max),RowId INT,CountNo INT)
			 
		 SET @SQL = ' 
		  ;With Cte_CustomerReview AS 
		  (
		   SELECT DISTINCT CMSCustomerReviewId,a.PublishProductId,UserId,Headline,Comments,UserName,UserLocation,Rating,Status,ZPPD.Name ProductName,ZPPD.LocaleId,ZP.StoreName
		   ,a.CreatedDate,a.ModifiedDate,a.CreatedBy,a.ModifiedBy,ZCSD.SEOUrl,ZCSD.PortalId
			FROM ZNODECMSCUSTOMERREVIEW A 
			INNER JOIN ZnodePublishProductEntity ZPPD with(nolock) ON (A.PUBLISHPRODUCTID = ZPPD.ZnodeProductId AND ZPPD.LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+')
			LEFT OUTER JOIN ZnodeCMSSEODetail ZCSD on (ZPPD.SKU = ZCSD.SEOCode AND  ( A.PortalId = ZCSD.PortalId )
			AND EXISTS (SELECT TOP 1 1 FROM ZnodeCMSSEOType ZCST WHERE  (ZCSD.CMSSEOTypeId = ZCST.CMSSEOTypeId AND ZCST.NAME = ''Product'')
			)
			 )
			INNER  JOIN ZnodePortal ZP ON (A.PortalId = ZP.PortalId)
			WHERE ZP.PortalId = '+CAST(@PortalId AS VARCHAR(50))+' OR '+CAST(@PortalId AS VARCHAR(50))+' = 0 			
		  )
		  ,Cte_CustomerInfo AS 
		  (		  
		   SELECT CMSCustomerReviewId,PublishProductId,UserId,Headline,Comments,UserName,UserLocation,Rating,Status,ProductName,StoreName
		  ,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy,SEOUrl ,'+dbo.Fn_GetPagingRowId(@Order_BY,'CMSCustomerReviewId')+',Count(*)Over() CountNo  
		   FROM Cte_CustomerReview 
		   WHERE 1=1  
		   '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
		  )
		  SELECT CMSCustomerReviewId,PublishProductId,UserId,Headline,Comments,UserName,UserLocation,Rating,Status,ProductName,StoreName
		  ,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy,SEOUrl,RowId,CountNo
		  FROM Cte_CustomerInfo 
		  '+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)                                                                                                                                                                                                                                                                        
          
		  INSERT INTO @TBL_CustomerReview (CMSCustomerReviewId,PublishProductId,UserId,Headline,Comments,UserName,UserLocation,Rating,[Status],ProductName,StoreName
					,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy,SEOUrl,RowId,CountNo)                                                                                                                                                                                                                                        
          EXEC (@SQL)

		  SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_CustomerReview ),0)

		  SELECT CMSCustomerReviewId,PublishProductId,UserId,Headline,Comments,UserName,UserLocation,Rating,[Status],ProductName
					,CreatedDate,ModifiedDate,CreatedBy,ModifiedBy,SEOUrl,StoreName
		  FROM @TBL_CustomerReview

           
         END TRY
         BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCMSCustomerReviewInformation @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@LocaleId = '+CAST(@LocaleId AS VARCHAR(50))+',@PortalId='+CAST(@PortalId AS VARCHAR(50))+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetCMSCustomerReviewInformation',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
         END CATCH;
     END;
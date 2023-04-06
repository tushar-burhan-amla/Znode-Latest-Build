CREATE PROCEDURE [dbo].[Znode_GetPublishCategoryDetail]
(   @WhereClause NVARCHAR(MAX),
	@Rows        INT           = 100,
	@PageNo      INT           = 1,
	@Order_BY    VARCHAR(100)  = '',
	@RowsCount   INT OUT)
AS 
   /* Summary :- This procedure is used to get the publish category details 
				 The result is displayed order by PublishCategoryProductId
     Unit Testing 
     EXEC Znode_GetPublishCategoryDetail '',@RowsCount=0
	*/
     BEGIN
         BEGIN TRY
             SET NOCOUNT ON;
             DECLARE @SQL NVARCHAR(MAX);
             DECLARE @TBL_PublishCategoryDetail TABLE (PublishCategoryProductId INT,PublishCategoryName NVARCHAR(4000),ProductName NVARCHAR(4000),sku NVARCHAR(2000),PublishCategoryId INT,
             PublishProductId  INT,PublishCatalogId  INT,LocaleId INT,RowId INT,CountNo INT);
     
            SET @SQL = '
			;With Cte_PublishCatalogDetail AS 
			(
			SELECT pcp.PublishCategoryProductId, pcp.PublishCatalogId,pcp.PublishProductId,pcp.PublishCategoryId,pcd.PublishCategoryName,ppd.ProductName,ppd.sku,pcd.LocaleId,ZSGPC.Boost
			FROM ZnodePublishCategoryProduct pcp
			JOIN ZnodePublishCategoryDetail pcd ON pcp.PublishCategoryId=pcd.PublishCategoryId
			JOIN ZnodePublishProductDetail ppd ON pcp.PublishProductId=ppd.PublishProductId and ppd.LocaleId=pcd.LocaleId
			LEFT JOIN ZnodeSearchGlobalProductCategoryBoost ZSGPC ON (ZSGPC.PublishCategoryId = pcd.PublishCategoryId AND ZSGPC.PublishProductId  = ppd.PublishProductId)
			)
			, Cte_PublishCategoryDetails AS
			(	
			SELECT *,'+dbo.Fn_GetPagingRowId(@Order_BY,'PublishCategoryProductId ,PublishCategoryId ')+',Count(*)Over() CountNo
			FROM Cte_PublishCatalogDetail CTPC 
			WHERE 1=1 
		    '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'
			)

			SELECT PublishCategoryProductId,PublishCategoryName ,ProductName , sku, PublishCategoryId,PublishProductId,PublishCatalogId,LocaleId,RowId,CountNo
			FROM Cte_PublishCategoryDetails 
			'+dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows)
			
			INSERT INTO @TBL_PublishCategoryDetail
			EXEC (@SQL);

			SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_PublishCategoryDetail), 0);

			SELECT PublishCategoryProductId,PublishCategoryName AS PublishCategoryName,ProductName,sku,PublishCategoryId,PublishProductId,PublishCatalogId,LocaleId
			FROM @TBL_PublishCategoryDetail;
		
        END TRY
        BEGIN CATCH
             DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishCategoryDetail @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPublishCategoryDetail',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
        END CATCH;
     END;
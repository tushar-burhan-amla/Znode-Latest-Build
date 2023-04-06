CREATE PROCEDURE [dbo].[Znode_GetPublishStatus]
(
	@WhereClause NVARCHAR(MAX),
    @Rows        INT           = 100,
    @PageNo      INT           = 1,
    @Order_BY    VARCHAR(100)  = '',
    @RowsCount   INT OUT
)
AS 
/*
	 Summary :- This Procedure is used to get the publish status of the catalog 
	 Unit Testig 
	 EXEC  Znode_GetPublishStatus_TP '',10,1,'',0
*/
   BEGIN 
		BEGIN TRY 
		SET NOCOUNT ON 

		 DECLARE @SQL  NVARCHAR(max) 
		 DECLARE @TBL_CatalogId TABLE (PublishCatalogLogId INT,PublishStatus VARCHAR(300),UserName NVARCHAR(512),PublishCategoryCount INT ,PublishProductCount INT ,CreatedDate DATETIME ,ModifiedDate DATETIME ,RowId INT ,CountId INT)
	 
		 SET @SQL = '

		  DECLARE @TBL_PublishProductId TABLE (PublishProductId int,PublishCatalogId int )
		INSERT INTO @TBL_PublishProductId
		SELECT COUNT( DISTINCT PublishProductId ),PublishCatalogId
		FROM ZnodePublishCategoryProduct a
		WHERE PublishCatalogId IN  (select PublishCatalogId from ZnodePublishCatalog b where a.PublishCatalogId = b.PublishCatalogId)
		AND a.PublishCategoryId  <> 0 and a.PublishCategoryId is not null
		GROUP BY PublishCatalogId 


		 ;With Cte_CatalogLog AS
		 (
		 SELECT PublishCatalogLogId,CASE WHEN IsCatalogPublished IS NULL THEN ''Processing'' WHEN IsCatalogPublished = 0 THEN ''Publish Failed''
         WHEN IsCatalogPublished = 1 THEN  ''Published Successfully'' END   PublishStatus ,APZU.UserName ,ISNULL(ZPCL.PublishCategoryId,0) PublishCategoryCount,
		 ISNULL(ZPCL.PublishProductId,0) PublishProductCount,ZPCL.CreatedDate,ZPCL.ModifiedDate ,PimCatalogId
	     FROM ZnodePublishCatalogLog  ZPCL 
		 LEFT JOIN ZnodeUser ZU ON (ZU.UserId = ZPCL.CreatedBy )
	     LEFT JOIN AspNetUsers APU ON (APU.Id = ZU.AspNetUserId) 
		 LEFT JOIN AspNetZnodeUser APZU ON (APZU.AspNetZnodeUserId = APU.UserName)
		 LEFT JOIN  @TBL_PublishProductId a on (zpcl.PublishCatalogId = a.PublishCatalogId)
		 WHERE ZPCL.PublishType = ''Catalog'' OR ZPCL.PublishType IS NULL  
		 )
	 
	     ,Cte_PublishStatus 
		 AS (SELECT PublishCatalogLogId, PublishStatus ,UserName , PublishCategoryCount, PublishProductCount,CreatedDate,ModifiedDate ,
		 '+[dbo].[Fn_GetPagingRowId](@Order_BY,'PublishCatalogLogId DESC')+' , Count(*)Over() CountId FROM Cte_CatalogLog
         WHERE 1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+' )
	 
		 SELECT PublishCatalogLogId,PublishStatus,UserName,PublishCategoryCount,PublishProductCount,CreatedDate,ModifiedDate,RowId,CountId 
		 FROM Cte_PublishStatus 
		 '+[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)+' '
	
		 PRINT @SQL
		 INSERT INTO @TBL_CatalogId
		 EXEC (@SQL)

		 SELECT  PublishCatalogLogId,PublishStatus,UserName,PublishCategoryCount,PublishProductCount,CreatedDate,ModifiedDate
		 FROM @TBL_CatalogId

		 SET @RowsCount = ISNULL((SELECT TOP 1 COUNTID FROM @TBL_CatalogId),0)
	 
		 END TRY 
		 BEGIN CATCH 
			DECLARE @Status BIT ;
			SET @Status = 0;
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPublishStatus @WhereClause = '+@WhereClause+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
			SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
			EXEC Znode_InsertProcedureErrorLog
					@ProcedureName = 'Znode_GetPublishStatus',
					@ErrorInProcedure = @Error_procedure,
					@ErrorMessage = @ErrorMessage,
					@ErrorLine = @ErrorLine,
					@ErrorCall = @ErrorCall;
		 END CATCH 
   END
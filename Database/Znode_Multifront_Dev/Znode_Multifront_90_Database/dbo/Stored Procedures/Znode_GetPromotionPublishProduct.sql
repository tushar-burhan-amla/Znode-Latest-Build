CREATE PROCEDURE [dbo].[Znode_GetPromotionPublishProduct]
( @WhereClause NVarchar(Max)  = '',
  @Rows        INT            = 100,
  @PageNo      INT            = 1,
  @Order_BY VARCHAR(1000)	  = '',
  @RowsCount   INT OUT,
  @LocaleId    int = 0,
  @PromotionId    int = 0
 )
AS

/*
 Summary : This Procedure is used to get PromotionPublishProduct  
 Unit Testing
 EXEC [Znode_GetPromotionPublishProduct]  @RowsCount = 0
 
*/
     BEGIN
		
         SET NOCOUNT ON;
         BEGIN TRY
		  DECLARE @SQL NVARCHAR(MAX);
		  DECLARE @TBL_GetPromotionPublishProduct   TABLE (PublishProductId INT,ProductName NVARCHAR(4000),SKU NVARCHAR(4000),CatalogName VARCHAR(max),RowId INT,CountNo INT )
		   SET @SQL = CONCAT ('
		   ;WITH CTE_PromotionPublishProduct AS
		   (
			SELECT distinct ZPCP.ZnodeProductId PublishProductId,ZPCP.Name as name,ZPCP.SKU,ZPC.CatalogName
			FROM ZnodePublishProductEntity ZPCP with(nolock)
			INNER JOIN ZnodePimCatalog ZPC ON (ZPC.PimCatalogId = ZPCP.ZnodecatalogId )
		    where ZPCP.LocaleId = ',@LocaleId,'
		   )
		   , CTE_PromotionPublishProductList AS
		   (
		   SELECT PublishProductId,name,SKU,CatalogName,',dbo.Fn_GetPagingRowId(@Order_BY,'PublishProductId DESC'),',Count(*)Over() CountNo
		   FROM CTE_PromotionPublishProduct
		   WHERE 1=1 
		   ',dbo.Fn_GetFilterWhereClause(@WhereClause),'	
		   )
		   SELECT PublishProductId,name,SKU,CatalogName,RowId,CountNo
		   FROM CTE_PromotionPublishProductList
		    ',dbo.Fn_GetPaginationWhereClause(@PageNo,@Rows))

		print @SQL;
		INSERT INTO @TBL_GetPromotionPublishProduct
		EXEC(@SQL);

		SET @RowsCount = ISNULL((SELECT TOP 1 CountNo FROM @TBL_GetPromotionPublishProduct), 0);

		SELECT PublishProductId,ProductName Name,SKU,CatalogName , @PromotionId PromotionId
		FROM @TBL_GetPromotionPublishProduct
		
		
		END TRY
		BEGIN CATCH
			DECLARE @Status BIT ;
		     SET @Status = 0;
		     DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetPromotionPublishProduct @WhereClause = '+CAST(@WhereClause AS VARCHAR(max))+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
             SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
             EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetPromotionPublishProduct',
				@ErrorInProcedure = @Error_procedure,
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
		END CATCH
	END
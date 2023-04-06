
CREATE PROCEDURE [dbo].[Znode_GetProductFeedDetailsList]
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
	 EXEC  Znode_GetProductFeedDetailsList '',100,1,'',0

*/
   BEGIN 
		BEGIN TRY 
		SET NOCOUNT ON 

		 DECLARE @SQL  NVARCHAR(max) 
		 DECLARE @TBL_ProductFeed TABLE (ProductFeedId int, ProductFeedTypeName VARCHAR(300),FileName VARCHAR(300),RowId INT, CountId INT, CreatedDate DATETIME, ModifiedDate DATETIME, StoreName NVARCHAR(100),LocaleName Varchar(100))
	 
		 SET @SQL = '
		;With Cte_ProductFeed AS (
		SELECT ZPCL.ProductFeedId, 
		ZPFT.ProductFeedTypeName ProductFeedTypeName, ZPCL.FileName FileName,
		ZPCL.CreatedDate,ZPCL.ModifiedDate,ZP.StoreName,
		ZL.Name AS LocaleName
		FROM
		ZnodeProductFeed ZPCL
		INNER JOIN ZnodeProductFeedType ZPFT ON ZPFT.ProductFeedTypeId = ZPCL.ProductFeedTypeId
		Inner join ZnodePortal ZP on ZP.PortalId = ZPCL.PortalId
		LEFT JOIN ZnodeLocale ZL on ZL.LocaleId = ZPCL.LocaleId
		)
	 
	     ,Cte_PublishFeedStatus 
		 AS (
		 SELECT ProductFeedId,ProductFeedTypeName,FileName,CreatedDate,ModifiedDate,StoreName,LocaleName,
		 '+[dbo].[Fn_GetPagingRowId](@Order_BY,'ProductFeedId DESC')+' , Count(*)Over() CountId FROM Cte_ProductFeed
         WHERE 1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+' )
	 
		 SELECT ProductFeedId,ProductFeedTypeName,FileName,RowId,CountId,CreatedDate,ModifiedDate,StoreName,LocaleName
		 FROM Cte_PublishFeedStatus 
		 '+[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)+' '
	
		 INSERT INTO @TBL_ProductFeed 
		 EXEC (@SQL)


		 SELECT  ProductFeedId,ProductFeedTypeName, FileName,CreatedDate,ModifiedDate,StoreName,LocaleName
		 FROM @TBL_ProductFeed

		 SET @RowsCount = ISNULL((SELECT TOP 1 COUNTID FROM @TBL_ProductFeed),0)
	 
		 END TRY 
		 BEGIN CATCH 
			DECLARE @Status BIT ;
			SET @Status = 0;
			DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(), @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetProductFeedDetailsList @WhereClause = '+@WhereClause+',@Rows='+CAST(@Rows AS VARCHAR(50))+',@PageNo='+CAST(@PageNo AS VARCHAR(50))+',@Order_BY='+@Order_BY+',@RowsCount='+CAST(@RowsCount AS VARCHAR(50))+',@Status='+CAST(@Status AS VARCHAR(10));
              			 
			SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
		  
			EXEC Znode_InsertProcedureErrorLog
					@ProcedureName = 'Znode_GetProductFeedDetailsList',
					@ErrorInProcedure = @Error_procedure,
					@ErrorMessage = @ErrorMessage,
					@ErrorLine = @ErrorLine,
					@ErrorCall = @ErrorCall;
		 END CATCH 
   END
CREATE PROCEDURE [dbo].[Znode_GetPublishProductDetail]
(
	   @WhereClause NVARCHAR(max) ,
       @Rows        INT           = 100 ,
       @PageNo      INT           = 1 ,
       @Order_BY    VARCHAR(100)  = '' ,
       @RowsCount   INT OUT 
)
AS 
	-- Summary :- This procedure is used to get the publish products details 
	-- Unit Testing 
	-- EXEC Znode_GetPublishProductDetail '',@RowsCount=0
BEGIN 
 BEGIN TRY 
  SET NOCOUNT ON; 
   
   DECLARE @SQL NVARCHAR(max)

   DECLARE @TBL_PublishProductDetail TABLE ( ProductName NVARCHAR(4000),PublishProductId INT,PublishCatalogId INT ,CountId INT  )

   SET @SQL = '
		;With Cte_PublishCatalogDetail AS 
		(
		SELECT zpd.ProductName,zpd.PublishProductId,Zp.PublishCatalogId,zpd.LocaleId
		FROM ZnodePublishProductDetail zpd 
		INNER JOIN ZNodePublishProduct zp ON zpd.PublishProductId=zp.PublishProductId 
		)
		, Cte_PublishProductDetails AS
		(
		
		SELECT *,RANK()OVER('+dbo.Fn_GetOrderByClause(@Order_BY,' PublishProductId ')+',PublishProductId ) RowId , COUNT(*)OVER() CountId  
		FROM Cte_PublishCatalogDetail CTPC 
		'+dbo.Fn_GetWhereClause (@whereClause , ' WHERE ')+'
		)

		SELECT ProductName,PublishProductId ,PublishCatalogId , CountId
		FROM Cte_PublishProductDetails 
		WHERE '+dbo.Fn_GetRowsForPagination(@PageNo,@Rows,' RowId ') +'
		'+dbo.Fn_GetOrderByClause(@Order_BY,' PublishProductId ')
		PRINT @sql
		INSERT INTO @TBL_PublishProductDetail
		EXEC (@SQL )

		SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM @TBL_PublishProductDetail ),0)

		SELECT ProductName,PublishProductId ,PublishCatalogId
		FROM @TBL_PublishProductDetail

	END TRY 
	BEGIN CATCH 
	SELECT ERROR_MESSAGE()
	END CATCH 
END
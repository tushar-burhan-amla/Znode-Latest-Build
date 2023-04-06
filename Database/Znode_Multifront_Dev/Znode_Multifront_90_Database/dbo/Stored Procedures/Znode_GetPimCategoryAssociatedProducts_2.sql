 --SELECT * INTO #CategoryUnAssociatedProduct FROM  View_CategoryUnAssociatedProduct WHERE 1=1 AND   SELECT  @Count=Count(1) FROM  #TEMP_Account11  SELECT * FROM #TEMP_Account11  Order BY 1 OFFSET 0 ROWS FETCH NEXT 1000 ROWS ONLY  
-- DECLARE @FFF BIGINT  EXEC [Znode_GetPimCategoryAssociatedProducts] @WhereClause='AccociatedProduct = 0' ,@RowsCount =@FFF OUT  SELECT @FFF
CREATE  Procedure [dbo].[Znode_GetPimCategoryAssociatedProducts_2]
(
 @WhereClause Varchar(1000)   
,@Rows INT = 1000   
,@PageNo INT = 0   
,@Order_BY VARCHAR(100) =  NULL
,@RowsCount int out
,@PimCategoryId INT = '' 
,@LocaleId int =1
)
AS
BEGIN  
 SET NOCOUNT ON 
 BEGIN TRY 
            
            DECLARE @V_SQL NVARCHAR(MAX)
			
			SET @PageNo = CASE WHEN @PageNo = 0 THEN @PageNo ELSE  (@PageNo-1)*@Rows END 
            SET @WhereClause = REPLACE (@WhereClause , 'PimCategoryId', 'ISNULL(PimCategoryId,0)')
			SET @WhereClause = REPLACE (@WhereClause , 'AccociatedProduct', 'Case WHEN d.PimProductId IS NULL THEN 0 ELSE 1 END ')
			
			print @PageNo
			SET @V_SQL = ' SELECT  b.PimProductId,CAST(d.PimCategoryProductId AS INT ) PimCategoryProductId,CASE WHEN d.PimCategoryId Is NULL THEN '
			+ CAST (@PimCategoryId  AS VARCHAR(800))+' ELSE d.PimCategoryId END  PimCategoryId,d.DisplayOrder,d.Status,b.
			ProductName,CASE WHEN ImagePath IS NULL OR ImagePath = '''' THEN ''/MediaFolder/no-image.png'' ELSE  
			CASE WHEN dbo.FN_GetThumbnailMediaPath(b.ImagePath,0) IS NULL THEN ''/MediaFolder/no-image.png'' ELSE 
			dbo.FN_GetThumbnailMediaPath(b.ImagePath,0) END  END
			ImagePath  INTO #CategoryUnAssociatedProduct 
				FROM  View_CategoryUnAssociatedProduct b
				LEFT JOIN ZnodePimCategoryProduct d ON (b.PimProductId = d.PimProductId AND d.PimCategoryId = '+ CAST (@PimCategoryId  AS VARCHAR(800))+' ) 
			WHERE 1=1  '+CASE WHEN  @WhereClause IS NOT NULL OR @WhereClause <> '' THEN ' AND '+@WhereClause ELSE '' END 
						+' SELECT  @Count=Count(1) FROM  #CategoryUnAssociatedProduct  SELECT * FROM #CategoryUnAssociatedProduct ' 
						+' Order BY '+CASE WHEN @Order_BY IS NULL OR @Order_BY = '' THEN '1' ELSE @Order_BY END + ' OFFSET '+CAST(@PageNo AS varchar(100))+' ROWS FETCH NEXT '+CAST(@Rows AS varchar(100))+' ROWS ONLY  '

			print @V_SQL
			EXEC SP_executesql @V_SQL,N'@Count INT OUT' ,@Count=@RowsCount out
 
 
 END TRY 

 BEGIN CATCH 
   
   SELECT  ERROR_LINE(),ERROR_MESSAGE(),ERROR_NUMBER()

 END CATCH 
END
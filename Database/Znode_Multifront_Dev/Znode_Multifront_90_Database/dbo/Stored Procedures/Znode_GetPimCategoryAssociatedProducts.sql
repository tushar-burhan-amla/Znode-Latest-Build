


 --SELECT * INTO #TEMP_Account11 FROM  View_CategoryUnAssociatedProduct WHERE 1=1 AND   SELECT  @tempo=Count(1) FROM  #TEMP_Account11  SELECT * FROM #TEMP_Account11  Order BY 1 OFFSET 0 ROWS FETCH NEXT 1000 ROWS ONLY  
-- DECLARE @FFF BIGINT  EXEC [Znode_GetPimCategoryAssociatedProducts] @WhereClause='AccociatedProduct = 0' ,@RowsCount =@FFF OUT  SELECT @FFF
CREATE Procedure [dbo].[Znode_GetPimCategoryAssociatedProducts]
(
 @WhereClause Varchar(1000)   
,@Rows INT = 1000   
,@PageNo INT = 0   
,@Order_BY VARCHAR(100) =  NULL
,@RowsCount int out
,@PimCategoryId INT = NULL 
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

			SET @V_SQL = ' SELECT d.PimCategoryProductId,CASE WHEN d.PimCategoryId Is NULL THEN '+ CAST (@PimCategoryId  AS VARCHAR(800))+' ELSE d.PimCategoryId END  PimCategoryId,d.DisplayOrder,d.Status,b.PimProductId,b.ProductName,b.ImagePath INTO #TEMP_Account11 
															FROM  View_CategoryUnAssociatedProduct b
															LEFT JOIN ZnodePimCategoryProduct d ON (b.PimProductId = d.PimProductId AND d.PimCategoryId = '+ CAST (@PimCategoryId  AS VARCHAR(800))+' ) 
WHERE 1=1  '+CASE WHEN  @WhereClause IS NOT NULL OR @WhereClause <> '' THEN ' AND '+@WhereClause ELSE '' END 
							 +' SELECT  @tempo=Count(1) FROM  #TEMP_Account11  SELECT * FROM #TEMP_Account11 ' 
								+' Order BY '+CASE WHEN @Order_BY IS NULL OR @Order_BY = '' THEN '1' ELSE @Order_BY END + ' OFFSET '+CAST(@PageNo AS varchar(100))+' ROWS FETCH NEXT '+CAST(@Rows AS varchar(100))+' ROWS ONLY  '

			print @V_SQL
			EXEC SP_executesql @V_SQL,N'@tempo INT OUT' ,@tempo=@RowsCount out
 
 
 END TRY 

 BEGIN CATCH 
   
   SELECT  ERROR_LINE(),ERROR_MESSAGE(),ERROR_NUMBER()

 END CATCH 
END 

--Create View View_CategoryUnAssociatedProduct
--AS 

--With Attribute_values AS (
--SELECT NULL PimCategoryProductId,NULL PimCategoryId,b.PimProductId,0 DisplayOrder,1 Status,b.AttributeValue , c.AttributeCode
--FROM View_ZnodePimAttributeValue b
--INNER JOIN View_PimAttributeLocale c ON (c.PimAttributeId = b.PimAttributeId)
--WHERE c.AttributeCode IN ('ProductName','ImagePath')
--)

--SELECT * 
--FROM Attribute_values a 
--PIVOT 
--(

--Max(AttributeValue) FOR AttributeCode IN (ProductName,ImagePath)
--) Piv
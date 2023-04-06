  -- SELECT * FROM View_ManageProductList
  -- exec Znode_ManageProductListByFamily @WhereClause='',@PimFamilyId = 31 ,@RowsCount=null,@Rows = 1000,@PageNo=1,@Order_BY = Null
CREATE Procedure [dbo].[Znode_ManageProductListByFamily]  
  
(  
 @WhereClause Varchar(1000) 
,@PimFamilyId   INT = NULL     
,@Rows INT = 1000     
,@PageNo INT = 0     
,@Order_BY VARCHAR(100) =  NULL  
,@RowsCount int out  
,@LocaleId int = 0   
)  
AS  
BEGIN    
 SET NOCOUNT ON   
 BEGIN TRY   
     
   DECLARE @V_SQL NVARCHAR(MAX)  

   IF @LocaleId = 0 
    BEGIN
	
	SELECT @LocaleId = a.FeatureValues  FROM ZnodeGlobalSetting a WHERE a.FeatureName = 'Locale'
	 
	END  

	SELECT DISTINCT PimProductId
    INTO #PimProductIds 
	FROM ZnodePimFamilyGroupMapper a 
	INNER JOIN ZnodePimAttributeValue b ON (a.PimAttributeId = b.PimAttributeId) 
	WHERE a.PimAttributeFamilyId = @PimFamilyId

	--SELECT * FROM #PimProductIds

   SET @PageNo = CASE WHEN @PageNo = 0 THEN @PageNo ELSE  (@PageNo-1)*@Rows END   
              
   SET @V_SQL = '  SELECT ProductId, [ProductName],  ProductType, AttributeFamily , [SKU], [Price], [Quantity], [Status],CASE WHEN ImagePath IS NULL OR ImagePath = '''' THEN ''../../../../Content/Images/no-image.png'' ELSE  CASE WHEN dbo.FN_GetThumbnailMediaPath(ImagePath) IS NULL THEN ''../../../../Content/Images/no-image.png'' ELSE dbo.FN_GetThumbnailMediaPath(ImagePath) END  END  ImagePath,[Assortment] ,LocaleId  INTO #ManageProductList FROM '+ ' View_ManageProductList a ' + 'WHERE 1=1 AND EXISTS (SELECT TOP 1 1 FROM #PimProductIds qq WHERE qq.PimProductId = a.ProductId ) AND LocaleId = '  +CasT (@LocaleId AS VARCHAR(300))
        + CASE WHEN @WhereClause = '' THEN '' ELSE ' AND '+@WhereClause END 
	    +' SELECT  @Count=Count(1) FROM  #ManageProductList  SELECT * FROM #ManageProductList '   
        +' Order BY '+ISNULL(CASE WHEN @Order_BY=''THEN NULL ELSE @Order_BY END ,'1')+ ' OFFSET '+CAST(@PageNo AS varchar(100))+' ROWS FETCH NEXT '+CAST(@Rows AS varchar(100))+' ROWS ONLY  '  
  
   print @V_SQL  
   EXEC SP_executesql @V_SQL,N'@Count INT OUT' ,@Count=@RowsCount out  
   
   DROP TABLE #PimProductIds
   
 END TRY   
  
 BEGIN CATCH   
     
   SELECT  ERROR_LINE(),ERROR_MESSAGE(),ERROR_NUMBER()  
  
 END CATCH   
  
END
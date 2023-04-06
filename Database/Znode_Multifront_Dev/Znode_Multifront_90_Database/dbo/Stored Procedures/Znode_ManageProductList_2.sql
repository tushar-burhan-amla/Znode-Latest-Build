  -- SELECT * FROM View_ManageProductList
  -- exec Znode_ManageProductList @WhereClause='',@RowsCount=null,@Rows = 100,@PageNo=0,@Order_BY = Null,@LocaleId=1
Create  Procedure [dbo].[Znode_ManageProductList_2]  
  
(  
 @WhereClause Varchar(1000)     
,@Rows INT = 1000     
,@PageNo INT = 0     
,@Order_BY VARCHAR(100) =  NULL  
,@RowsCount int out  
,@LocaleId int = 0  
 
)  


AS  
BEGIN    
SET NOCOUNT on
 BEGIN TRY   
     
   DECLARE  @V_SQL NVARCHAR(MAX),
			@V_SQLForLocale NVARCHAR(MAX),
			@DefaultLocaleId int  

   --IF @LocaleId = 0 
   BEGIN
		SELECT @DefaultLocaleId =a.FeatureValues  FROM ZnodeGlobalSetting a WHERE a.FeatureName = 'Locale'
   END  


   SET @PageNo = CASE WHEN @PageNo = 0 THEN @PageNo ELSE  (@PageNo-1)*@Rows END   
              
   --SET @V_SQL = '  SELECT ProductId, [ProductName],  ProductType, AttributeFamily , [SKU], [Price], [Quantity], [Status],CASE WHEN ImagePath IS NULL OR ImagePath = '''' THEN ''../../../../Content/Images/no-image.png'' ELSE  CASE WHEN dbo.FN_GetThumbnailMediaPath(ImagePath,0) IS NULL THEN ''../../../../Content/Images/no-image.png'' ELSE dbo.FN_GetThumbnailMediaPath(ImagePath,0) END  END  ImagePath,[Assortment] ,LocaleId  INTO #ManageProductList FROM '+ ' View_ManageProductList ' + 'WHERE 1=1  AND LocaleId = '  +CasT (@LocaleId AS VARCHAR(300))
   --     + CASE WHEN @WhereClause = '' THEN '' ELSE ' AND '+@WhereClause END 
	  --  +' SELECT  @Count=Count(1) FROM  #ManageProductList  SELECT * FROM #ManageProductList '   
   --     +' Order BY '+ISNULL(CASE WHEN @Order_BY=''THEN NULL ELSE @Order_BY END ,'1')+ ' OFFSET '+CAST(@PageNo AS varchar(100))+' ROWS FETCH NEXT '+CAST(@Rows AS varchar(100))+' ROWS ONLY  '  
  
     SET @V_SQLForLocale = ' SELECT Isnull(MPL2.ProductId , MPL1.ProductId) ProductId,
									Isnull(MPL2.ProductName , MPL1.ProductName) ProductName,
									Isnull(MPL2.ProductType , MPL1.ProductType) ProductType,
									Isnull(MPL2.AttributeFamily , MPL1.AttributeFamily) AttributeFamily,
									Isnull(MPL2.SKU , MPL1.SKU) SKU,
									Isnull(MPL2.Price , MPL1.Price) Price,
									Isnull(MPL2.Quantity , MPL1.Quantity) Quantity,
									Isnull(MPL2.Status , MPL1.Status) Status,
									Isnull(MPL2.ImagePath , MPL1.ImagePath) ImagePath,
									Isnull(MPL2.Assortment , MPL1.Assortment) Assortment,
									Isnull(MPL2.LocaleId , MPL1.LocaleId) LocaleId
								FROM #ManageProductList MPL1 Left Outer join #ManageProductList MPL2 ON MPL2.ProductId =  MPL1.ProductId '

	 SET @V_SQLForLocale = @V_SQLForLocale + ' and MPL2.LocaleId = ' + CasT (@LocaleId AS VARCHAR(300)) + ' Where MPL1.LocaleId = ' + CasT (@DefaultLocaleId AS VARCHAR(300))

     SET @V_SQL = '  SELECT ProductId, [ProductName],  ProductType, AttributeFamily , [SKU], [Price], [Quantity], [Status],CASE WHEN ImagePath IS NULL OR ImagePath = '''' THEN ''/MediaFolder/no-image.png'' ELSE  CASE WHEN dbo.FN_GetThumbnailMediaPath(ImagePath,0) IS NULL THEN ''/MediaFolder/no-image.png'' ELSE dbo.FN_GetThumbnailMediaPath(ImagePath,0) END  END  ImagePath,[Assortment] ,LocaleId  INTO #ManageProductList FROM '+ ' View_ManageProductList ' + 'WHERE 1=1  AND LocaleId in  ('  
	    + CasT (@LocaleId AS VARCHAR(300)) + ',' + CasT (@DefaultLocaleId AS VARCHAR(300)) + ')' 
        + CASE WHEN @WhereClause = '' THEN '' ELSE ' AND '+@WhereClause END 
	    +' SELECT  @Count=Count(1) FROM  #ManageProductList  '  + @V_SQLForLocale 
        +' Order BY '+ISNULL(CASE WHEN @Order_BY=''THEN NULL ELSE @Order_BY END ,'1')+ ' OFFSET '+CAST(@PageNo AS varchar(100))+' ROWS FETCH NEXT '+CAST(@Rows AS varchar(100))+' ROWS ONLY  '  
  


  ---- #ManageProductList 
   print @V_SQL  
   EXEC SP_executesql @V_SQL,N'@Count INT OUT' ,@Count=@RowsCount out  
   
   
 END TRY   
  
 BEGIN CATCH   
     
   SELECT  ERROR_LINE(),ERROR_MESSAGE(),ERROR_NUMBER()  
  
 END CATCH   
  
END
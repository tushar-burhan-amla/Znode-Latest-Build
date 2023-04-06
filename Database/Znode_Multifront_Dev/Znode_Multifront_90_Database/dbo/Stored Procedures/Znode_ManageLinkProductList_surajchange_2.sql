-- EXEC Znode_ManageLinkProductList '' , @RowsCount = 0 
Create  procedure [dbo].[Znode_ManageLinkProductList_surajchange]
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
 SET NOCOUNT ON   
 BEGIN TRY   
     
    DECLARE @DefaultLocale VARCHAR(100)  
    SET @DefaultLocale = (SELECT FeatureValues FROM ZnodeGlobalSetting WHERE FeatureName = 'Locale')


    DECLARE @V_SQL NVARCHAR(MAX)  
	,@Rows_start varchar(1000) 
	,@Rows_end   Varchar(1000)

	SET @Rows_start = CASE WHEN @Rows >= 1000000 THEN 0 ELSE (@Rows*(@PageNo-1))+1 END 

	SET @Rows_end = CASE WHEN @Rows >= 1000000 THEN @Rows ELSE @Rows*(@PageNo) END
 
     
   SET @PageNo = CASE WHEN @PageNo = 0 THEN @PageNo ELSE  (@PageNo-1)*@Rows END   
              
   SET @V_SQL = ' ;With LinkProductList AS 
					(	 SELECT * FROM  view_ManageLinkProductList WHERE 1=1 '  
        +case WHEN @WhereClause IS NOT NULL and @WhereClause <> ''    THEN  ' AND '+REPLACE(@WhereClause,'PimProductId = -1 AND','') ELSE '' END  +' AND LocaleId IN ( '+Cast(@LocaleId As VARCHAr(100))+' , '+@DefaultLocale+' ) )'
		+' , DataForLocaleFirst  AS (SELECT  * FROM LinkProductList a WHERE a.LOcaleId = '+Cast(@LocaleId As VARCHAr(100))+') '
		+' , DataForLocaleDefault AS (SELECT * FROM DataForLocaleFirst UNION ALL SELECT * FROM LinkProductList a WHERE a.LOcaleID =  '+@DefaultLocale+' AND NOT EXISTS (SELECT TOP 1 1 FROM DataForLocaleFirst b WHERE b.SKU= a.SKU)   ) '
		+'SELECT *,RANK()OVER('+' Order BY '+ISNULL(CASE WHEN @Order_BY=''THEN NULL ELSE @Order_BY END ,' SKU ')+') RowId   INTO #ManageLinkProductList FROM DataForLocaleDefault '+' Order BY '+ISNULL(CASE WHEN @Order_BY=''THEN NULL ELSE @Order_BY END ,' SKU ')+
        +' SELECT  @Count=Count(1) FROM  #ManageLinkProductList  SELECT * FROM #ManageLinkProductList  WHERE RowId BETWEEN '+@Rows_start+' AND '+@Rows_end   
         
  
   print @V_SQL  
   EXEC SP_executesql @V_SQL,N'@Count INT OUT' ,@Count=@RowsCount out  
   
   
 END TRY   
  
 BEGIN CATCH   
     
   SELECT  ERROR_LINE(),ERROR_MESSAGE(),ERROR_NUMBER()  
  
 END CATCH   
  
END   


-- 
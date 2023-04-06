-- DECLARE @D INT= 1  EXEC  [dbo].[Znode_ManageProductList_3]   @WhereClause = 'ProductType LIKE ''%Simple%'' AND ProductName LIKE ''%App%''' , @Rows = 10 , @PageNo = 14 ,@Order_BY = 'SKU DESC', @RowsCount = @D OUT SELECT @D

CREATE Procedure [dbo].[Znode_ManageProductList_3]  
(  
	 @WhereClause nVarchar(3000)     
	,@Rows INT = 100     
	,@PageNo INT = 0     
	,@Order_BY VARCHAR(1000) =  'ProductName'  
	,@RowsCount int out  
	,@LocaleId int = 1  
 )  
AS
-- This Procedure is used for get product List --- 
  
BEGIN    
SET NOCOUNT on
 BEGIN TRY  

DECLARE @DefaultAttributeFamily INT = (SELECT TOP 1  PimAttributeFamilyId FROM ZnodePimAttributeFamily WHERE IsCategory = 0 AND IsDefaultFamily = 1   )
,@LocaleIdDefault INT = (SELECT TOP 1 FeatureValues FROM ZnodeGlobalSetting WHERE FeatureName = 'Locale' ) 
,@Rows_start varchar(1000) 
,@Rows_end   Varchar(1000)
 
DECLARE @SQL NVARCHAR(max)

DECLARE @ProductIdTable  TABLE (PimProductId INT ) 

SET @Rows_start = CASE WHEN @Rows >= 1000000 THEN 0 ELSE (@Rows*(@PageNo-1))+1 END 
SET @Rows_end = CASE WHEN @Rows >= 1000000 THEN @Rows ELSE @Rows*(@PageNo) END



DECLARE @CHECKDESK VARCHAR(MAx)
	DECLARE @TableValue  Table (Id INT IDENTITY(1,1) , FilterColumn Varchar(3000),FilterClause Varchar(max))
	DECLARE @String Varchar(max),@WhereClause_inner Varchar(max)= ''
	 
 

	INSERT INTO @TableValue(FilterColumn,FilterClause)
	EXEC [dbo].[Znode_SplitWhereClause] @WhereClause,1

	INSERT INTO  @TableValue(FilterColumn,FilterClause)
	SELECT 'ORDER BY ', 'AttributeCode = '''+RTRIM(LTRIM(REPLACE(REPLACE(@Order_BY,'ASC',''),'DESC','')))+''''
	WHERE CASE WHEN @Order_BY = '' THEN 1 ELSE 0 END = 0 


    DECLARE @ValueId INT = 1 
	SET @SQL = 'DECLARE @COUNTROWS TABLE (PimProductId INT ,AttributeValue Nvarchar(max),RowId INT  ) ;With '
	WHILE @ValueId <= ISNULL((SELECT max(Id) FROM @TableValue ),1)
	BEGIN 

			SET @String = CASE WHEN @ValueId = 1 THEN ' TestCheck'+CAST(@ValueId AS VARCHAr(100)) ELSE ', TestCheck'+CAST(@ValueId AS VARCHAr(100)) END +'  AS ( SELECT a.PimProductId 
			'+CASE WHEN @ValueId = ISNULL((SELECT max(Id) FROM @TableValue ),1) THEN ' 
				, RANK()Over(Order By a.PimProductId  ) RowId,a.AttributeValue  ' ELSE '' END  +'
				FROM   View_LoadMangaeProduct a  
				WHERE '+ISNULL((SELECT FilterClause FROM @TableValue WHERE Id =@ValueId  ),'')+'
				'+CASE WHEN @ValueId = 1 THEN '' ELSE ' AND EXISTS (SELECT TOP 1 1 FROM TestCheck'+CAST(@ValueId-1 AS VARCHAr(100))+' q WHERE q.PimProductId = a.PimProductId )' END +'
				'+CASE WHEN @ValueId = 1 THEN CASE WHEN EXISTS (SELECT FilterClause FROM @TableValue WHERE Id =@ValueId  ) THEN  'AND ' ELSE '' END +' a.AttributeCode IN (''ProductName'', ''SKU'', ''Price''	, ''Quantity'', ''IsActive'',''ProductType'',''Image'',''Assortment''
				'+CASE WHEN @WhereClause_inner = '' THEN '' ELSE ','+@WhereClause_inner END +' )'
				ELSE '' END  +' GROUP BY a.PimProductId '+CASE WHEN @ValueId = ISNULL((SELECT max(Id) FROM @TableValue ),1) THEN ' 
				,a.AttributeValue  ' ELSE '' END  +' 
				) '  
	 
			SET   @SQL = @SQL +  @String
			SET @ValueId = @ValueId +1 


	END 

	SET @SQL = @SQL +' INSERT INTO @COUNTROWS SELECT PimProductId,AttributeValue,Row_NUMBER()OVER(ORDER BY '+CASE WHEN @Order_BY = '' OR @Order_BY = 'PimPRoductId' THEN 'PimProductId' ELSE 'AttributeValue ' +CASE WHEN @Order_BY LIKE '% DESC%' THEN ' DESC' ELSE ' ASC' END  END+' ) FROM  TestCheck'+CAST(@ValueId-1 AS VARCHAr(100))+' OPTION  (MAXDOP 1) SELECT @COunt = COUNT (1) FROM @COUNTROWS    SELECT PimProductId FROM @COUNTROWS ' 
			
	+' WHERE RowId Between '+ @Rows_start +' AND ' + @Rows_End





	---- Find the product ids ----- 
	


			
				
		--PRINT @SQL
		--INSERT INTO @ProductIdTable
		--EXEC SP_executesql @SQL, N'@Count INT OUT ' ,@Count=@RowsCount out

 

--IF @WhereClause <> '' 
--		Begin 
--			SET @SQL = '
--						DECLARE @TempToProduct TABLE (PimProductId INT,RowId INT)
--						INSERT INTO @TempToProduct
--						SELECT zp.PimProductId  , DENSE_RANK()OVER(ORDER BY zp.PimProductId ) ROwid
--						FROM ZnodePimProduct zp 
--						INNER JOIN dbo.ZnodePimAttributeValue zpav ON (zp.PimProductId = zpav.PimProductId) 
--						INNER JOIN dbo.ZnodePimAttribute zpa  ON (zpa.PimAttributeId = zpav.PimAttributeId )
--						INNER JOIN dbo.ZnodePimAttributeValueLocale zpavl ON (zpavl.PimAttributeValueId=zpav.PimAttributeValueId AND zpavl.LOcaleid  IN ('+CAST(@LocaleId AS VARCHAr(100))+','+CAST(@LocaleIdDefault AS VARCHAr(100))+') )
--						WHERE 1=1  '+  CASE WHEN @WhereClause = '' THEN '' ELSE ' AND ( '+@WhereClause+' )' END  + ' 					
--						GROUP BY zp.PimProductId 


--						SELECT @COUNT = COUNT(PimProductId) FROM @TempToProduct 

--						SELECT pimProductId FROM @TempToProduct WHERE RowId BETWEEN '+@Rows_start+' AND '+@Rows_end
--		End 
--		Else 
--		Begin
--			---- Find the product ids ----- 

--					SET @SQL = ' 
--							;With  TempToProduct AS (
--							SELECT zp.PimProductId  
--							FROM ZnodePimProduct zp 
--							INNER JOIN dbo.ZnodePimAttributeValue zpav ON (zp.PimProductId = zpav.PimProductId) 
--							INNER JOIN dbo.ZnodePimAttribute zpa  ON (zpa.PimAttributeId = zpav.PimAttributeId )
--							INNER JOIN dbo.ZnodePimAttributeValueLocale zpavl ON (zpavl.PimAttributeValueId=zpav.PimAttributeValueId AND zpavl.LOcaleid  IN ('+CAST(@LocaleId AS VARCHAr(100))+','+CAST(@LocaleIdDefault AS VARCHAr(100))+') )
--							WHERE 1=1  '+  CASE WHEN @WhereClause = '' THEN '' ELSE ' AND ( '+@WhereClause+' )' END  + ' Order by ' + @Order_BY + '		 
--							GROUP BY zp.PimProductId 
--							)

--							SELECT @COUNT = COUNT(PimProductId) FROM TempToProduct 

--							;With  TempToProduct2 AS (
--							SELECT zp.PimProductId  , Row_number()OVER(ORDER BY zp.PimProductId ) Rowid
--							FROM ZnodePimProduct zp 
--							INNER JOIN dbo.ZnodePimAttributeValue zpav ON (zp.PimProductId = zpav.PimProductId) 
--							INNER JOIN dbo.ZnodePimAttribute zpa  ON (zpa.PimAttributeId = zpav.PimAttributeId )
--							INNER JOIN dbo.ZnodePimAttributeValueLocale zpavl ON (zpavl.PimAttributeValueId=zpav.PimAttributeValueId AND zpavl.LOcaleid  IN ('+CAST(@LocaleId AS VARCHAr(100))+','+CAST(@LocaleIdDefault AS VARCHAr(100))+') )
--							WHERE 1=1  '+ CASE WHEN @WhereClause = '' THEN '' ELSE ' AND ( '+@WhereClause+' )' END + ' Order by ' + @Order_BY + '		
--							GROUP BY zp.PimProductId 
--							)

--							SELECT pimProductId FROM TempToProduct2 WHERE RowId BETWEEN '+@Rows_start+' AND '+@Rows_end
--		End 			

			    PRINT @SQL
				INSERT INTO @ProductIdTable
				EXEC SP_executesql @SQL,N'@Count INT OUT' ,@Count=@RowsCount out

				DECLARE @AttributeDetails_locale Table (PimProductId INT , AttributeValue Nvarchar(Max)
						,AttributeCode varchar(600),AttributeFamilyName Nvarchar(max),LocaleId INT  )
				DECLARE @AttributeDetails Table (PimProductId INT , AttributeValue Nvarchar(Max)
						,AttributeCode varchar(600),AttributeFamilyName Nvarchar(max),LocaleId INT  )

				INSERT INTO @AttributeDetails_locale
				SELECT        zpav.PimProductId,  zpavl.AttributeValue , zpa.AttributeCode,'' AttributeFamilyName ,zpaL.LocaleId
				FROM  ZnodePimAttribute zpa 
				INNER JOIN ZnodePimAttributeLocale zpal ON (zpa.PimAttributeId = zpal.PimAttributeId) 
				INNER JOIN ZnodePimAttributeValue zpav ON (zpa.PimAttributeId = zpav.PimAttributeId) 
				INNER JOIN ZnodePimAttributeValueLocale zpavl ON (zpavl.PimAttributeValueId=zpav.PimAttributeValueId AND zpal.LOcaleid = zpavl.LOcaleid )
				WHERE        zpa.AttributeCode IN ('ProductName', 'SKU', 'Price', 'Quantity', 'IsActive','ProductType','Image','Assortment')
				AND EXISTS (SELECT TOP 1  1 FROM @ProductIdTable cq WHERE cq.PimProductId = zpav.PimProductId)
				AND zpaL.LocaleId IN (@LocaleId,@LocaleIdDefault) 

				INSERT INTO  @AttributeDetails
				SELECT * 
				FROM @AttributeDetails_locale 
				WHERE LocaleId = @LocaleId 

				INSERT INTO  @AttributeDetails
				SELECT * 
				FROM @AttributeDetails_locale q
				WHERE LocaleId = @LocaleIdDefault 
				AND NOT EXISTS (SELECT TOP 1 1 FROM @AttributeDetails a WHERE a.PimProductId = q.PimProductId AND a.AttributeCode = q.AttributeCode )
				--- find the specific attributes and values ---- 

				DECLARE @FamilyDetails TABLE (PimProductId INT ,PimAttributeFamilyId  INT ,FamilyName Nvarchar(3000))

				INSERT INTO @FamilyDetails (PimProductId,PimAttributeFamilyId)
				SELECT a.PimProductId , PimAttributeFamilyId
				FROM @ProductIdTable a 
				INNEr JOIN ZnodePimAttributeValue b ON (a.PimProductId = b.PimProductId)
				WHERE b.PimAttributeFamilyId <> @DefaultAttributeFamily
				AND ISNULL(b.PimAttributeFamilyId,0) <> 0  
				GROUP BY a.PimProductId , PimAttributeFamilyId
				UNION 
				SELECT a.PimProductId , b.PimFamilyId
				FROM @ProductIdTable a 
				INNEr JOIN ZnodePimConfigureProductAttribute b ON (a.PimProductId = b.PimProductId)
				WHERE b.PimFamilyId <> @DefaultAttributeFamily
				GROUP BY a.PimProductId , PimFamilyId
				 --- find the product families  

					 UPDATE a 
					 SET FamilyName = b.AttributeFamilyName
					 FROM @FamilyDetails a 
					 INNER JOIN ZnodePimFamilyLocale b ON (a.PimAttributeFamilyId = b.PimAttributeFamilyId AND LocaleId = @LocaleId)

					  UPDATE a 
					 SET FamilyName = b.AttributeFamilyName
					 FROM @FamilyDetails a 
					 INNER JOIN ZnodePimFamilyLocale b ON (a.PimAttributeFamilyId = b.PimAttributeFamilyId AND LocaleId = @LocaleIdDefault)
					 WHERe a.FamilyName IS NULL OR a.FamilyName = ''

					  --- Update the  product families name locale wise   

  
   --     select * from @AttributeDetails
		SELECT zpp.PimProductid Productid, [ProductName],ProductType ,ISNULL(zf.FamilyName,'')  AttributeFamily , [SKU], [Price], [Quantity]
				, CASE WHEN [IsActive] IS NULL THEN CAST(0 AS BIT) ELSE   CAST([IsActive] AS BIT) END  [Status], [dbo].FN_GetMediaThumbnailMediaPath(zm.Path)  ImagePath,[Assortment]  ,Piv.LocaleId
		
		FROM ZNodePimProduct zpp 
		LEFT JOIN  @FamilyDetails zf ON (zf.PimProductId = zpp.PimProductId)
		INNER JOIN @AttributeDetails 
		PIVOT 
		(
		 Max(AttributeValue) FOR AttributeCode  IN ([ProductName],[SKU],[Price],[Quantity],[IsActive],[ProductType],[Image],[Assortment] )
		)Piv  ON (Piv.PimProductId = zpp.PimProductid) 
		LEFT JOIN ZnodeMedia zm ON (zm.MediaId = piv.[Image] )
   
	
   -- find the all locale values 
 END TRY   
  
 BEGIN CATCH   
     
   SELECT  ERROR_LINE(),ERROR_MESSAGE(),ERROR_NUMBER()  
  
 END CATCH   
  
END
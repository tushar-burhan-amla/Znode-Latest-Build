CREATE procedure [dbo].[Znode_GetBrandDetail] 
(
	   @WhereClause    NVARCHAR(Max) ,
       @Rows           INT            = 10 ,
       @PageNo         INT            = 1 ,
       @Order_BY       VARCHAR(1000)  = '' ,
       @RowsCount      INT  = 0 OUT,
       @LocaleId       INT            = 1 ,
	   @IsAssociated   BIT			= 0 

)
AS 
	-- Summary :- Find the brand list 
	
    --  Unit Testing  
    -- declare @p7 int
    --set @p7=NULL
    --declare @p8 int
    --set @p8=2
    --declare @p9 bit
    --set @p9=NULL
    --exec sp_executesql N'Znode_GetBrandDetail @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT,@LocaleId,@IsAssociated',N'@WhereClause nvarchar(19),@Rows int,@PageNo int,@Order_By nvarchar(4000),@RowCount int output,@LocaleId int output,@IsAssociated bit output',@WhereClause=N'brandname like ''s%''',@Rows=2147483647,@PageNo=1,@Order_By=N'',@RowCount=@p7 output,@LocaleId=@p8 output,@IsAssociated=@p9 output
    --select @p7, @p8, @p9

BEGIN 
	BEGIN TRY 
	  SET NOCOUNT ON ; 
   
		DECLARE @SQL NVARCHAR(MAX) , @RowsStart VARCHAR(50) , @RowsEnd VARCHAR(50);
		DECLARE @TBL_PimBrandDetail TABLE (BrandId	int,BrandCode	nvarchar(1200),MediaPath	VARCHAR(600),WebsiteLink	nvarchar(1000),DisplayOrder	int,IsActive	bit
											,CreatedBy	int,CreatedDate	datetime,ModifiedBy	int,ModifiedDate	datetime ,AttributeDefaultValueCode VARCHAR(100),BrandName NVARCHAR(Max)
											,PimAttributeDefaultValueId INT ,LocaleId INT,MediaId INT,RowId INT,CountId INT )
        SET @RowsStart = CASE
                             WHEN @Rows >= 1000000
                             THEN 0
                             ELSE ( @Rows * ( @PageNo - 1 ) ) + 1
                         END;
        SET @RowsEnd = CASE
                           WHEN @Rows >= 1000000
                           THEN @Rows
                           ELSE @Rows * ( @PageNo )
                       END;
        DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId()

		SET @WhereClause = ' WHERE '+ @WhereClause + CASE WHEN  @IsAssociated = 1 THEN CASE WHEN @WhereClause = '' THEN  ' ' ELSE ' AND ' END 
							+ ' EXISTS ( SELECT TOP 1 1 FROM ZnodePimAttributeValue ZAV INNER JOIN ZnodePimAttribute ZA ON (ZA.PimAttributeId = ZAV.PimAttributeId AND ZA.AttributeCode = ''Brand'') INNER JOIN ZnodePimAttributeValueLocale ZAVL ON (ZAV.PimAttributeValueId= ZAVL.PimAttributeValueId ) 
										WHERE (ZAV.PimAttributeDefaultValueId = CTSPADV.PimAttributeDefaultValueId  OR ZAVL.AttributeValue = CTSPADV.AttributeDefaultValueCode))'  ELSE  CASE WHEN @WhereClause = '' THEN  ' 1 = 1  ' ELSE '' END  END 
		SET @Order_BY = ' ORDER BY '+ CASE WHEN @Order_BY = '' THEN ' BrandId DESC ' ELSE @Order_BY END 

		 
   SET @SQL = '
		
		CREATE TABLE #TBL_BrandDetails  (Description NVARCHAR(max),BrandId INT, BrandCode VARCHAR(600) , DisplayOrder INT ,IsActive BIT ,WebsiteLink NVARCHAR(1000),BrandDetailLocaleId INT,SEOFriendlyPageName NVARCHAR(600)
										,MediaPath NVARCHAR(Max),MediaId INT,CMSSEODetailId int,SEOTitle nvarchar(MAX),SEOKeywords  nvarchar(MAX), SEOURL  nvarchar(MAX)
									, ModifiedDate  datetime,  SEODescription  nvarchar(MAX) ,MetaInformation  nvarchar(MAX) ,IsRedirect bit,CMSSEODetailLocaleId INT,SEOId INT,BrandName NVARCHAR(max) )


        INSERT INTO #TBL_BrandDetails
		EXEC [dbo].[Znode_GetBrandDetailsLocale] 0,'+CAST(@LocaleId AS VARCHAR(10))+'




		SELECT * FROM #TBL_BrandDetails


	 --  ;With Cte_PimAttributeDefaultValue AS 
	 --  (
	 --  SELECT ZBD.BrandId,ZBD.BrandCode,[dbo].[Fn_GetMediaThumbnailMediaPath](Zm.path) MediaPath,ZBD.WebsiteLink,ZBD.DisplayOrder,ZBD.IsActive
		--	,ZBD.CreatedBy,ZBD.CreatedDate,ZBD.ModifiedBy,ZBD.ModifiedDate, ZPADV.AttributeDefaultValueCode,ZPADVL.AttributeDefaultValue BrandName,ZPADVL.PimAttributeDefaultValueId,ZPADVL.LocaleId,Zm.MediaId
	 --  FROM ZnodeBrandDetails ZBD
	 --  INNER JOIN ZnodePimAttributeDefaultValue ZPADV ON (ZPADV.AttributeDefaultValueCode = ZBD.BrandCode)
	 --  INNER JOIN ZnodePimAttribute ZA ON (ZA.PimAttributeId = ZPADV.PimAttributeId AND ZA.AttributeCode =''Brand'' ) 
	 --  INNER JOIN ZnodePimAttributeDefaultValueLocale ZPADVL ON (ZPADVL.PimAttributeDefaultValueId = ZPADV.PimAttributeDefaultValueId AND LocaleId IN ('+CAST(@DefaultLocaleId AS VARCHAR(10))+','+CAST(@LocaleId AS VARCHAR(10))+'))
	 --  LEFT JOIN ZnodeMedia ZM ON (ZM.MediaId = ZBD.MediaId )
	 --  )
	 --  , Cte_FirstPimAttributeDefaultValue AS 
	 --  (
		--SELECT * 
		--FROM Cte_PimAttributeDefaultValue CTPADV 
		--WHERE CTPADV.LocaleId = '+CAST(@LocaleId AS VARCHAR(10))+'
	 --  )
	 --  , Cte_SecondPimAttributeDefaultvalue AS 
	 --  ( 
		-- SELECT * 
		-- FROM  Cte_FirstPimAttributeDefaultValue 
		-- UNION ALL 
		-- SELECT * 
		-- FROM Cte_PimAttributeDefaultValue CTPADV 
		-- WHERE CTPADV.LocaleId = '+CAST(@DefaultLocaleId AS VARCHAR(10))+'
		-- AND  NOT EXISTS (SELECT TOP 1 1 FROM Cte_FirstPimAttributeDefaultValue CTFPADV WHERE CTFPADV.BrandId = CTPADV.BrandId  )
	 --  )
	 --  , Cte_finalPimAttributeDefaultValue AS 
	 --  (  
		--SELECT *,DENSE_RANK()OVER('+@Order_BY+') RowId , COUNT(*)Over() CountId 
		--FROM Cte_SecondPimAttributeDefaultvalue CTSPADV
		--'+@WhereClause+'
	 --  )

	 --  SELECT * 
	 --  FROM Cte_finalPimAttributeDefaultValue A 
	 --  WHERE RowId BETWEEN '+@RowsStart+' AND '+@RowsEnd+'

	   '
       PRINT @SQL
	  INSERT INTO @TBL_PimBrandDetail
	  EXEC (@SQL)

	 SET @RowsCount = ISNULL(( SELECT TOP 1  CountId FROM  @TBL_PimBrandDetail),0)

	  SELECT * FROM @TBL_PimBrandDetail 


  END TRY 
  BEGIN CATCH 
  SELECT ERROR_MESSAGE(),ERROR_LINE(),ERROR_PROCEDURE()
  END CATCH 
END
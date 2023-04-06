CREATE PROCEDURE [dbo].[Znode_GetCatalogProductSEODetail]
(
  @WhereClause      NVARCHAR(MAX),
  @Rows             INT           = 100,
  @PageNo           INT           = 1,
  @Order_BY         VARCHAR(1000) = '',
  @RowsCount        INT OUT,
  @LocaleId         INT           = 1,
  @PortalId INT
 
  )
AS
   
/*
  Summary:  Get product List  Catalog / category / respective product list    
  Unit Testing  
  begin tran
  declare @p7 int = 0  
  EXEC Znode_GetCatalogProductSEODetail @WhereClause=N'',@Rows=100,@PageNo=1,@Order_By=N'',
  @RowsCount=@p7 output,@PortalId= 1 ,@LocaleId=1
  rollback tran
 
    declare @p7 int = 0  
  EXEC Znode_GetCatalogProductSEODetail @WhereClause=N'',@Rows=10,@PageNo=1,@Order_By=N'',
  @RowsCount=@p7 output,@PortalId= 5 ,@LocaleId=1
 */

BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @DefaultLocaleId INT= dbo.Fn_GetDefaultLocaleId();
        DECLARE @SQL NVARCHAR(MAX)
		DECLARE @PimProductId TransferId
		DECLARE @PimAttributeId VARCHAR(MAX)
        DECLARE @TransferPimProductId TransferId


	IF OBJECT_ID('TEMPDB..#ProductDetailPivot') IS NOT NULL
	DROP TABLE #ProductDetailPivot

	IF OBJECT_ID('TEMPDB..##TempProductDetail') IS NOT NULL
	DROP TABLE ##TempProductDetail

	IF OBJECT_ID('TEMPDB..#znodeCatalogProduct') IS NOT NULL
	DROP TABLE #znodeCatalogProduct

	IF OBJECT_ID('TEMPDB..#ProductAttributeDetailS') IS NOT NULL
	DROP TABLE #ProductAttributeDetailS


	IF OBJECT_ID('TEMPDB..#TBL_MediaValue') IS NOT NULL
	DROP TABLE #TBL_MediaValue


	Declare @PimCatalogId INT

	SELECT @PimCatalogId = ZPC.PublishCatalogId
	FROM ZnodePortalCatalog ZPC	WHERE PortalId = @PortalId

	SELECT DISTINCT ZPCH.PimCatalogId,ZPCP.PimProductId 
	INTO #ZnodeCatalogProduct
	FROm ZnodePimCategoryProduct ZPCP
	INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCP.PimCategoryId = ZPCH.PimCategoryId 
	where ZPCH.PimCatalogId = @PimCatalogId

	-----------------------------------========

	Create TABLE #ProductAttributeDetailS ( pimproductId int ,attributecode varchar(300),AttributeValue  varchar(1000) ,LocaleId int )

	INSERT INTO #ProductAttributeDetailS (pimproductId,attributecode,AttributeValue ,LocaleId)
	SELECT d.pimproductId,PA.attributecode,e.AttributeValue ,e.LocaleId--, ZPS.DisplayName  PublishState
	FROM ZnodePimAttributeValue d
	inner join ZnodePimAttributeValueLocale e on (d.PimAttributeValueId = e.PimAttributeValueId)
	inner join ZnodePimAttribute PA ON (PA.PimAttributeId = d.PimAttributeId)
	where  PA.Attributecode IN ('SKU','ProductName','ProductImage','IsActive')
	and exists(select * from #znodeCatalogProduct ZPC where d.pimproductId = ZPC.PimProductId )

	CREATE TABLE #ProductDetailPivot( PimProductid INT,SKU VARCHAR(1000),ProductName VARCHAR(1000),ProductImage VARCHAR(1000),IsActive VARCHAR(1000),LocaleId  INT)

	INSERT INTO #ProductDetailPivot(pimproductId, LocaleId)
	SELECT distinct PimProductId,LocaleId from #ProductAttributeDetailS

	--Create Index Ind_#ProductAttributeDetailS on  #ProductAttributeDetailS(PimProductid)
	--Create Index Ind_#ProductAttributeDetailS_IdAndCode on  #ProductAttributeDetailS(attributecode,PimProductid)

	--Create Index #ProductDetail on  #ProductDetailPivot(PimProductid)

		----------------------------------------------------------
	--SELECT
	--PD.PimProductid ,Piv.SKU ,Piv.ProductName ,Piv.ProductImage ,Piv.IsActive ,PD.LocaleId  into #ProductDetail  
	--FROM #ProductDetailPivot AS PD
	--INNER JOIN
	--(
	--SELECT PimProductId,AttributeValue,AttributeCode FROM #ProductAttributeDetailS
	--)   TB PIVOT(MAX(AttributeValue) FOR AttributeCode IN([SKU], [ProductName],ProductImage ,IsActive)) AS Piv ON(Piv.PimProductId = PD.PimProductid)
             
	----     -----------------------------------========

	UPDATE b SET b.SKU = a.AttributeValue, B.ProductName = a1.AttributeValue 
				 , b.IsActive = a11.AttributeValue
	FROM #ProductAttributeDetailS A
	INNER JOIN #ProductDetailPivot B ON A.PimProductid = B.PimProductid
	INNER JOIN #ProductAttributeDetailS A1 ON B.PimProductid = A1.PimProductid
	LEFT JOIN #ProductAttributeDetailS A11 ON A.PimProductid = A11.PimProductid AND A11.attributecode = 'IsActive'
	WHERE A.attributecode = 'SKU' 
	AND A1.attributecode = 'ProductName' 

	UPDATE b SET b.ProductImage = a.AttributeValue
	FROM #ProductAttributeDetailS A
	INNER JOIN #ProductDetailPivot B ON A.PimProductid = B.PimProductid
	WHERE A.attributecode = 'ProductImage'


	--update b set b.ProductName = a.AttributeValue
	--FROM #ProductAttributeDetailS A
	--INNER JOIN #ProductDetailPivot B ON A.PimProductid = B.PimProductid
	--WHERE A.attributecode = 'SKU'


	--update b set b.ProductName = a.AttributeValue
	--FROM #ProductAttributeDetailS A
	--INNER JOIN #ProductDetailPivot B ON A.PimProductid = B.PimProductid
	--WHERE A.attributecode = 'ProductName'

	--update b set b.ProductImage = a.AttributeValue
	--FROM #ProductAttributeDetailS A
	--INNER JOIN #ProductDetailPivot B ON A.PimProductid = B.PimProductid
	--WHERE A.attributecode = 'ProductImage'

	--update b set b.IsActive = a.AttributeValue
	--FROM #ProductAttributeDetailS A
	--INNER JOIN #ProductDetailPivot B ON A.PimProductid = B.PimProductid
	--WHERE A.attributecode = 'IsActive'

	CREATE TABLE #TBL_MediaValue(PimAttributeValueId INT,PimProductId INT,MediaPath INT,PimAttributeId INt,LocaleId INT )
	INSERT INTO #TBL_MediaValue
	SELECT ZPAV.PimAttributeValueId ,ZPAV.PimProductId ,ZPPAM.MediaId MediaPath,ZPAV.PimAttributeId , ZPPAM.LocaleId
	FROM ZnodePimAttributeValue ZPAV
	INNER JOIN ZnodePimProductAttributeMedia ZPPAM ON ( ZPPAM.PimAttributeValueId = ZPAV.PimAttributeValueId)
	INNER JOIN #ProductDetailPivot PD ON (PD.PimProductId = ZPAV.PimProductId)
	LEFT JOIN ZnodeMedia ZM ON (Zm.Path = ZPPAM.MediaPath)
	WHERE  ZPAV.PimAttributeId = (select PimAttributeId from ZnodePimAttribute pa where attributecode = 'ProductImage')


	SELECT PD.PimProductId  ,
	URL+ZMSM.ThumbnailFolderName+'/'+ zm.PATH  AS ProductImagePath
	INTO #Cte_ProductMedia
	FROM ZnodeMedia AS ZM
    INNER JOIN ZnodeMediaConfiguration ZMC  ON (ZM.MediaConfigurationId = ZMC.MediaConfigurationId)
	INNER JOIN ZnodeMediaServerMaster ZMSM ON (ZMSM.MediaServerMasterId = ZMC.MediaServerMasterId)
	INNER JOIN #TBL_MediaValue PD ON (PD.MediaPath = ZM.MediaId)--CAST(ZM.MediaId AS VARCHAR(50)))
	--INNER JOIN  @TBL_PimMediaAttributeId AS FNMA ON (FNMA.PImAttributeId = PD.PimATtributeId )

	SELECT DISTINCT  CD.pimproductId, SKU,ProductName,
	case WHEN  CD.IsActive = 'true' THEN 1 ELSE 0 END IsActive, ISNULL(CSD.SEOCode,SKU) as SEOCode, CSD.SEOUrl, CSDL.SEOTitle, CSDL.SEODescription, CSDL.SEOKeywords,
	case when  ZPS.DisplayName IS NULL THEN '' ELSE ZPS.DisplayName END IsPublish  , CPM.ProductImagePath, PC.CatalogName, CD.LocaleId, CSDL.CanonicalURL , CSDL.RobotTag
	INTO #CTE_ProductDetail
	FROM #ProductDetailPivot CD
	INNER JOIN #znodeCatalogProduct PCC on CD.PimProductId = PCC.PimProductId
	INNER JOIN ZnodePimCatalog PC on PCC.PimCatalogId = PC.PimCatalogId
	LEFT JOIN ZnodeCMSSEOType CST ON CST.Name = 'Product'
	LEFT JOIN ZnodeCMSSEODetail CSD on LTRIM(RTRIM(CD.SKU)) = LTRIM(RTRIM(CSD.SEOCode)) and CSD.CMSSEOTypeId = CST.CMSSEOTypeId AND CSD.PortalId = @PortalId 
	LEFT JOIN ZnodeCMSSEODetailLocale CSDL ON  CSD.CMSSEODetailId = CSDL.CMSSEODetailId AND CSDL.LocaleId =  @LocaleId 
	LEFT JOIN ZnodePublishState ZPS ON (ZPS.PublishStateId = CSD.PublishStateId)
	LEFT JOIN #Cte_ProductMedia CPM ON (CPM.PimProductId = CD.PimProductId)
	WHERE PCC.PimCatalogId = @PimCatalogId AND CD.LocaleId IN (@LocaleId ,@DefaultLocaleId )

	SELECT pimproductId, SKU,ProductName,IsActive , SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords, IsPublish,ProductImagePath,CatalogName, LocaleId, CanonicalURL, RobotTag
	INTO #CTE_ProductLocale
	FROM #CTE_ProductDetail CPD
	WHERE CPD.LocaleId = @LocaleId


	SELECT pimproductId, SKU,ProductName,IsActive , SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords, IsPublish,ProductImagePath,CatalogName, CanonicalURL, RobotTag
	INTO #CTE_ProductBothLocale
	FROM #CTE_ProductLocale PL
	UNION ALL
	SELECT pimproductId, SKU,ProductName,IsActive , SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords, IsPublish,ProductImagePath,CatalogName, CanonicalURL, RobotTag
	FROM #CTE_ProductDetail PD
	WHERE LocaleId = @DefaultLocaleId AND
	NOT EXISTS (select TOP 1 1 from #CTE_ProductLocale PCL WHERE PCL.pimproductId = PD.pimproductId AND PCL.CatalogName = PD.CatalogName )

	SET @SQL =
	'SELECT  pimproductId, SKU,ProductName, cast(IsActive as bit) IsActive , SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords, 
	         IsPublish,ProductImagePath,CatalogName, CanonicalURL, RobotTag,'
			 +[dbo].[Fn_GetPagingRowId](@Order_BY, 'PimProductId')+',Count(*)Over() CountId
	into #CTE_ProductDetail_WhereClause
	FROM #CTE_ProductBothLocale CD
	WHERE 1 = 1  '+dbo.Fn_GetFilterWhereClause(@WhereClause)+'

	SELECT  pimproductId, SKU,ProductName,IsActive, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,
	        ProductImagePath,CatalogName, CanonicalURL, RobotTag, CountId
	INTO ##TempProductDetail
	FROM #CTE_ProductDetail_WhereClause
	'+[dbo].[Fn_GetPaginationWhereClause](@PageNo, @Rows);
	--select @SQL for xml path,type
	EXEC (@SQL)

	SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM ##TempProductDetail ),0)

	SELECT PimProductId,LTRIM(RTRIM(SKU)) AS SKU,ProductName,IsActive,LTRIM(RTRIM(SEOCode)) AS SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ProductImagePath, CanonicalURL, RobotTag
	FROM ##TempProductDetail
	--GROUP by pimproductId, SKU,ProductName,IsActive, SEOCode, SEOUrl, SEOTitle, SEODescription, SEOKeywords,IsPublish,ProductImagePath


	IF OBJECT_ID('TEMPDB..ProductDetailPivot') IS NOT NULL
	DROP TABLE ProductDetailPivot

	IF OBJECT_ID('TEMPDB..##TempProductDetail') IS NOT NULL
	DROP TABLE ##TempProductDetail


	IF OBJECT_ID('TEMPDB..#znodeCatalogProduct') IS NOT NULL
	   DROP TABLE #znodeCatalogProduct

	IF OBJECT_ID('TEMPDB..#TBL_MediaValue') IS NOT NULL
	   DROP TABLE #TBL_MediaValue

END TRY
BEGIN CATCH
   SELECT ERROR_message()
    DECLARE @Status BIT ;
    SET @Status = 0;
    DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(), @ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(), @ErrorLine VARCHAR(100)= ERROR_LINE(),
	 @ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetCatalogCategoryProducts @WhereClause = '''+ISNULL(CAST(@WhereClause AS VARCHAR(MAX)),'''''')+''',@Rows='+ISNULL(CAST(@Rows AS VARCHAR(50)),'''''')
	 +',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')
	 +',@LocaleId = '+ISNULL(CAST(@LocaleId AS VARCHAR(50)),'''')+',@PortalId='+ISNULL(CAST(@PortalId AS VARCHAR(50)),'''');
             
            -- SELECT 0 AS ID,CAST(0 AS BIT) AS Status;                    
 
             EXEC Znode_InsertProcedureErrorLog
			 @ProcedureName = 'Znode_GetCatalogProductSEODetail',
			 @ErrorInProcedure = 'Znode_GetCatalogProductSEODetail',
			 @ErrorMessage = @ErrorMessage,
			 @ErrorLine = @ErrorLine,
			 @ErrorCall = @ErrorCall;
END CATCH;
END;
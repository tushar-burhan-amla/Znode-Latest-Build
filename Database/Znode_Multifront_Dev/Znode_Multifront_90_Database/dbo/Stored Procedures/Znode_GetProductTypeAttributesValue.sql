CREATE PROCEDURE [dbo].[Znode_GetProductTypeAttributesValue]
(
	@WhereClause	NVARCHAR(MAX),
	@Rows			INT = 100,
	@PageNo			INT = 1,
	@Order_BY		VARCHAR(100) = '',
	@RowsCount		INT OUT
)
AS
/*
	Summary :- This Procedure is used to get the AttributeCode='ProductType' and SelectValues.Value='Configurable Product'
	Unit Testig 
	EXEC Znode_GetProductTypeAttributesValue 'ProductType','',100,1,'',0
	EXEC Znode_GetProductTypeAttributesValue 'ProductType',null,100,1,'',0
*/
BEGIN 
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @SQL NVARCHAR(MAX);--ZnodePublishProductEntity

		DECLARE @TBL_ZnodePublishProductEntity TABLE
		(PublishProductEntityId INT, VersionId INT, IndexId VARCHAR(300), ZnodeProductId INT, ZnodeCatalogId INT,
			SKU VARCHAR(300), LocaleId INT, [Name] VARCHAR(300), ZnodeCategoryIds INT, IsActive BIT,
			Attributes NVARCHAR(MAX), Brands NVARCHAR(MAX), CategoryName VARCHAR(300), CatalogName VARCHAR(300),
			DisplayOrder INT, RevisionType VARCHAR(50), AssociatedProductDisplayOrder INT, ProductIndex INT,
			SalesPrice VARCHAR(50), RetailPrice VARCHAR(50), CultureCode VARCHAR(50), CurrencySuffix VARCHAR(50),
			CurrencyCode VARCHAR(50), SeoDescription VARCHAR(1000), SeoKeywords VARCHAR(1000),
			SeoTitle VARCHAR(1000), SeoUrl VARCHAR(1000), ImageSmallPath VARCHAR(500), SKULower VARCHAR(50),
			RowId INT ,CountId INT
		)
		SET @SQL = '
		;With Cte_ZnodePublishProductEntity
		AS
		(
		SELECT DISTINCT PPE.PublishProductEntityId,PPE.VersionId,PPE.IndexId,PPE.ZnodeProductId,PPE.ZnodeCatalogId,PPE.SKU,
			PPE.LocaleId,PPE.[Name],PPE.ZnodeCategoryIds,PPE.IsActive,PPE.Attributes,PPE.Brands,PPE.CategoryName,
			PPE.CatalogName,PPE.DisplayOrder,PPE.RevisionType,PPE.AssociatedProductDisplayOrder,PPE.ProductIndex,
			PPE.SalesPrice,PPE.RetailPrice,PPE.CultureCode,PPE.CurrencySuffix,PPE.CurrencyCode,PPE.SeoDescription,
			PPE.SeoKeywords,PPE.SeoTitle,PPE.SeoUrl,PPE.ImageSmallPath,PPE.SKULower--,PPAJ.AttributeCode
			,PP2.ProductType
		FROM dbo.ZnodePublishProductEntity PPE WITH (NOLOCK)
		INNER JOIN ZnodePublishProductEntity PP WITH (NOLOCK) ON PPE.ZnodeProductId=PP.ZnodeProductId
			AND PPE.ZnodeCatalogId=PP.ZnodeCatalogId
		--INNER JOIN ZnodePublishProductAttributeJson PPAJ WITH (NOLOCK) ON PP.PimProductId=PPAJ.PimProductId
		INNER JOIN ZnodePimProduct PP2 WITH (NOLOCK) ON PPE.ZnodeProductId=PP2.PimProductId
		)

		,Cte_ZnodePublishProductEntity1
		AS (
		SELECT PublishProductEntityId,VersionId,IndexId,ZnodeProductId,ZnodeCatalogId,SKU,
			LocaleId,[Name],ZnodeCategoryIds,IsActive,Attributes,Brands,CategoryName,
			CatalogName,DisplayOrder,RevisionType,AssociatedProductDisplayOrder,ProductIndex,
			SalesPrice,RetailPrice,CultureCode,CurrencySuffix,CurrencyCode,SeoDescription,
			SeoKeywords,SeoTitle,SeoUrl,ImageSmallPath,SKULower,
		'+[dbo].[Fn_GetPagingRowId](@Order_BY,'PublishProductEntityId DESC')+',COUNT(*) OVER() CountId
		FROM Cte_ZnodePublishProductEntity
		WHERE 1=1 '+[dbo].[Fn_GetFilterWhereClause](@WhereClause)+')

		SELECT PublishProductEntityId,VersionId,IndexId,ZnodeProductId,ZnodeCatalogId,SKU,LocaleId,[Name],
			ZnodeCategoryIds,IsActive,Attributes,Brands,CategoryName,CatalogName,DisplayOrder,RevisionType,
			AssociatedProductDisplayOrder,ProductIndex,SalesPrice,RetailPrice,CultureCode,CurrencySuffix,
			CurrencyCode,SeoDescription,SeoKeywords,SeoTitle,SeoUrl,ImageSmallPath,SKULower,RowId,CountId
		FROM Cte_ZnodePublishProductEntity1 
		'+[dbo].[Fn_GetPaginationWhereClause](@PageNo,@Rows)+' '
	
		PRINT @sql
		INSERT INTO @TBL_ZnodePublishProductEntity
		EXEC (@SQL)

		SELECT PublishProductEntityId,VersionId,IndexId,ZnodeProductId,ZnodeCatalogId,SKU,LocaleId,[Name],
			ZnodeCategoryIds,IsActive,Attributes,Brands,CategoryName,CatalogName,DisplayOrder,RevisionType,
			AssociatedProductDisplayOrder,ProductIndex,SalesPrice,RetailPrice,CultureCode,CurrencySuffix,
			CurrencyCode,SeoDescription,SeoKeywords,SeoTitle,SeoUrl,ImageSmallPath,SKULower
		FROM @TBL_ZnodePublishProductEntity

		SET @RowsCount = ISNULL((SELECT TOP 1 CountId FROM @TBL_ZnodePublishProductEntity),0)
	END TRY
	BEGIN CATCH
	DECLARE @Status BIT ;
		SET @Status = 0;
		DECLARE @Error_procedure VARCHAR(1000)= ERROR_PROCEDURE(),
				@ErrorMessage NVARCHAR(MAX)= ERROR_MESSAGE(),
				@ErrorLine VARCHAR(100)= ERROR_LINE(), 
				@ErrorCall NVARCHAR(MAX)= 'EXEC Znode_GetProductTypeAttributesValue @WhereClause = '''+ISNULL(@WhereClause,'''''')
					+''',@Rows='+ISNULL(CAST(@Rows AS VARCHAR(50)),'''''')+',@PageNo='+ISNULL(CAST(@PageNo AS VARCHAR(50)),'''')
					+',@Order_BY='''+ISNULL(@Order_BY,'''''')+''',@RowsCount='+ISNULL(CAST(@RowsCount AS VARCHAR(50)),'''')

		SELECT 0 AS ID,CAST(0 AS BIT) AS Status;

		EXEC Znode_InsertProcedureErrorLog
				@ProcedureName = 'Znode_GetProductTypeAttributesValue',
				@ErrorInProcedure = 'Znode_GetProductTypeAttributesValue',
				@ErrorMessage = @ErrorMessage,
				@ErrorLine = @ErrorLine,
				@ErrorCall = @ErrorCall;
	END CATCH
END

CREATE PROCEDURE [dbo].[Znode_GetSEODefaultSetting]
( 
	@PortalId INT = 0,
	@SEOType Varchar(200) = '',
	@Id INT = 0
)
as 
Begin
	SET NOCOUNT ON;
	DECLARE @Title VARCHAR(MAX), @Description VARCHAR(MAX),@Keyword VARCHAR(MAX)
	DECLARE @SQL VARCHAR(MAX)
	 
	IF @SEOType = 'Product'
	BEGIN
			SELECT @Title = case when ProductTitle = '<NAME>' then 'ProductName' when ProductTitle = '<Product_Num>' then 'ProductCode'  when ProductTitle = '<SKU>' then 'SKU' when ProductTitle = '<Brand>' then 'Brand' end,
				   @Description = case when ProductDescription = '<NAME>' then 'ProductName' when ProductDescription = '<Product_Num>' then 'ProductCode' when ProductDescription = '<SKU>' then 'SKU' when ProductDescription = '<Brand>' then 'Brand' end,
				   @Keyword = case when ProductKeyword = '<NAME>' then 'ProductName' when ProductKeyword = '<Product_Num>' then 'ProductCode' when ProductKeyword = '<SKU>' then 'SKU' when ProductKeyword = '<Brand>' then 'Brand' end
			FROM ZnodeCMSPortalSEOSetting WHERE PortalId = @PortalId

			SELECT PimProductId, max(LongDescription) as LongDescription, max(SKU) as SKU, max(ShortDescription) as ShortDescription, 
			       max(ProductName) as ProductName,max(Brand) as Brand, max(ProductCode) as ProductCode
			into #ProductDetail
			FROM  
			(
				select PimProductId,	AttributeValue,	AttributeCode
				from View_LoadManageProductInternal 
				where PimProductId = @Id and AttributeCode in ('LongDescription', 'SKU', 'ShortDescription','ProductName','ProductCode')
				union all
				select a.PimProductId, d.AttributeDefaultValueCode as AttributeValue , 'Brand' as AttributeCode
				from ZnodePimAttributeValue a
				inner join ZnodePimProductAttributeDefaultValue b on a.PimAttributeValueId = b.PimAttributeValueId
				inner join ZnodePimAttributeDefaultValue d on b.PimAttributeDefaultValueId = d.PimAttributeDefaultValueId
				inner join ZnodePimAttribute c on a.PimAttributeId = c.PimAttributeId
				where c.AttributeCode = 'Brand' and a.PimProductId = @Id
			) AS SourceTable  
			PIVOT  
			(  
			max(AttributeValue)  
			FOR AttributeCode IN (LongDescription, SKU, ShortDescription, ProductName,Brand,ProductCode)  
			) AS PivotTable
			group by PimProductId 

			if (@Title <> '' or @Description <> '' or @Keyword <> '')
			begin
				SET @SQL = '
				select '+CASE WHEN isnull(@Title,'') = '' THEN '''''' ELSE @Title END  +' as SEOTitle,'+ 
						 CASE WHEN isnull(@Description,'') = '' THEN '''''' ELSE @Description END+ ' as SEODescription,'+ 
						 CASE WHEN isnull(@Keyword,'') = '' THEN '''''' ELSE @Keyword END+' as SEOKeywords   
				from #ProductDetail'
				print @SQL
				exec (@SQL)
			end

		end
		IF @SEOType = 'Category'
		BEGIN
			
			SELECT @Title = case when CategoryTitle = '<NAME>' then 'CategoryName' when CategoryTitle = '<Product_Num>' then 'cast(PimCategoryId as varchar(10))' when CategoryTitle = '<SKU>' then 'CategoryCode' end,
				   @Description = case when CategoryDescription = '<NAME>' then 'CategoryName' when CategoryDescription = '<Product_Num>' then 'cast(PimCategoryId as varchar(10))' when CategoryDescription = '<SKU>' then 'CategoryCode' end,
				   @Keyword = case when CategoryKeyword = '<NAME>' then 'CategoryName' when CategoryKeyword = '<Product_Num>' then 'cast(PimCategoryId as varchar(10))' when CategoryKeyword = '<SKU>' then 'CategoryCode' end
		    FROM ZnodeCMSPortalSEOSetting WHERE PortalId = @PortalId

			SELECT ZPCAV.PimCategoryId, ZPCAVL.CategoryValue as CategoryCode,  ZPCAVL1.CategoryValue as CategoryName
			INTO #CategoryDetail
			FROM ZnodePimCategoryAttributeValue ZPCAV
			INNER JOIN ZnodePimCategoryAttributeValueLocale ZPCAVL ON ZPCAV.PimCategoryAttributeValueId = ZPCAVL.PimCategoryAttributeValueId
			INNER JOIN ZnodePimAttribute ZPA ON ZPCAV.PimAttributeId = ZPA.PimAttributeId
			INNER JOIN ZnodePimCategoryAttributeValue ZPCAV1 ON ZPCAV.PimCategoryId = ZPCAV1.PimCategoryId
			INNER JOIN ZnodePimCategoryAttributeValueLocale ZPCAVL1 ON ZPCAV1.PimCategoryAttributeValueId = ZPCAVL1.PimCategoryAttributeValueId
			INNER JOIN ZnodePimAttribute ZPA1 ON ZPCAV1.PimAttributeId = ZPA1.PimAttributeId
			WHERE ZPA.AttributeCode = 'CategoryCode' AND ZPA1.AttributeCode = 'CategoryName'
			and ZPCAV.PimCategoryId = @Id

			if (@Title <> '' or @Description <> '' or @Keyword <> '')
			begin
				SET @SQL = '
				select '+CASE WHEN isnull(@Title,'') = '' THEN '''''' ELSE @Title END  +' as SEOTitle,'+ 
						 CASE WHEN isnull(@Description,'') = '' THEN '''''' ELSE @Description END+ ' as SEODescription,'+ 
						 CASE WHEN isnull(@Keyword,'') = '' THEN '''''' ELSE @Keyword END+' as SEOKeywords   
				from #CategoryDetail'
				
				exec (@SQL)
			end

		end

		IF @SEOType = 'Content_Page'
		BEGIN
			
			SELECT @Title = case when ContentTitle = '<Name>' then 'PageName' when ContentTitle = '<Product_Num>' then 'cast(CMSContentPagesId as varchar(10))' when ContentTitle = '<SKU>' then 'PageName' end,
					@Description = case when ContentDescription = '<Name>' then 'PageName' when ContentDescription = '<Product_Num>' then 'cast(CMSContentPagesId as varchar(10))' when ContentDescription = '<SKU>' then 'PageName' end,
					@Keyword = case when ContentKeyword = '<Name>' then 'PageName' when ContentKeyword = '<Product_Num>' then 'cast(CMSContentPagesId as varchar(10))' when ContentKeyword = '<SKU>' then 'PageName' end
			FROM ZnodeCMSPortalSEOSetting WHERE PortalId = @PortalId

			SELECT CMSContentPagesId, PageName into #ContentPageDetail from ZnodeCMSContentPages where CMSContentPagesId = @Id

			if (@Title <> '' or @Description <> '' or @Keyword <> '')
			begin

				SET @SQL = '
				select '+CASE WHEN isnull(@Title,'') = '' THEN '''''' ELSE @Title END  +' as SEOTitle,'+ 
						 CASE WHEN isnull(@Description,'') = '' THEN '''''' ELSE @Description END+ ' as SEODescription,'+ 
						 CASE WHEN isnull(@Keyword,'') = '' THEN '''''' ELSE @Keyword END+' as SEOKeywords   
				from #ContentPageDetail'

				exec (@SQL)
			end


		end

	

end
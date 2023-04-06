CREATE PROCEDURE [dbo].[Znode_GetCatalogCategoryHierarchy] 
	-- Add the parameters for the stored procedure here
	@PimProductId int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DEclare @PimAttributeId int

	set  @PimAttributeId = (SELECT TOP 1 PimAttributeId from ZnodePimAttribute where AttributeCode='CategoryName')

	SELECT Distinct c.PimCatalogId,c.CatalogName,locale.CategoryValue, ZPCP.PimCategoryId 
	FROM ZnodePimCategoryProduct as ZPCP
	INNER JOIN ZnodePimCategoryHierarchy ZPCH ON ZPCP.PimCategoryId = ZPCH.PimCategoryId
	INNER JOIN ZnodePimCatalog as c on c.PimCatalogId =ZPCH.PimCatalogId
	INNER JOIN ZnodePimCategoryAttributeValue as categoryAttribute on categoryAttribute.PimCategoryId = ZPCP.PimCategoryId
	INNER JOIN ZnodePimCategoryAttributeValueLocale as locale on locale.PimCategoryAttributeValueId = categoryAttribute.PimCategoryAttributeValueId
	WHERE ZPCP.PimProductId=@PimProductId and categoryAttribute.PimAttributeId = @PimAttributeId

END;
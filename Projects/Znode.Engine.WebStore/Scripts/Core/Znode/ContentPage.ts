class ContentPage extends ZnodeBase {
    Init() {
        Product.prototype.GetPriceAsync();
        window.sessionStorage.removeItem("lastCategoryId");
        window.sessionStorage.setItem("lastCategoryId", $("#categoryId").val());
        localStorage.setItem("isFromCategoryPage", "true");
        Category.prototype.changeProductViewDisplay();
        Category.prototype.setProductViewDisplay();
        Category.prototype.GetCompareProductList()
        ZSearch.prototype.Init();
    }
}
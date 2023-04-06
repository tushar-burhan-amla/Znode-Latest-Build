class Brand extends ZnodeBase {

    Init() {
        ZSearch.prototype.Init();
        Category.prototype.changeProductViewDisplay();
        Category.prototype.setProductViewDisplay();
        Category.prototype.GetCompareProductList()
    }

    GetBrandData(): void {
        $('.brand-popup').modal('toggle');
        Endpoint.prototype.GetBrandData(function (response) {
            $("#brand-popup-content").html(response);
        });
    }

    GetSelectedBrand(brandId: number) {
        Endpoint.prototype.SelectBrand(brandId, function (response) {
            location.reload();
        });
    }

    SearchBrand(control): void {
        Endpoint.prototype.SearchBrand(control.value, function (response) {
            $("#brand-popup-content").html(response);
        });
    }
}
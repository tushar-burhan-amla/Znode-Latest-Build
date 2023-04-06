declare function InitbLazy();
class Category extends ZnodeBase {
    Init() {
        Product.prototype.GetPriceAsync();
        window.sessionStorage.removeItem("lastCategoryId");
        window.sessionStorage.setItem("lastCategoryId", $("#categoryId").val());
        localStorage.setItem("isFromCategoryPage", "true");
        Category.prototype.changeProductViewDisplay();
        Category.prototype.setProductViewDisplay();
        Category.prototype.GetCompareProductList();
        Category.prototype.GetCategoryBreadCrumb($("#categoryId").val());
        ZSearch.prototype.Init(); 
    }

    changeProductViewDisplay(): any {
        $(".productview").on("click", function () {
            var previousClass = $("#view-option-productgrid").attr('class').split(' ')[1];
            var newClass = $(this).attr('title').toLowerCase().replace(" ", "-");

            $(".productview").each(function () {
                if ($(this).attr("class").indexOf('-active') >= 0) {
                    var baseClass = $(this).attr('class').replace('-active', '');
                    $(this).removeClass($(this).attr('class'));
                    $(this).addClass(baseClass);
                }
            });

            var activeclass = $(this).attr('class') + '-active';
            $(this).removeClass($(this).attr('class'));
            $(this).addClass(activeclass);

            if (previousClass != undefined && previousClass.length > 0) {
                $("#view-option-productgrid").removeClass(previousClass).addClass(newClass)
            } else {
                $("#view-option-productgrid").addClass(newClass)
            }
            localStorage["currentDisplayType"] = newClass;

            InitbLazy();
        });
    }

    setProductViewDisplay(): any {
        var displayType = localStorage["currentDisplayType"];

        if ($("#view-option-productgrid").html() != undefined) {
            var previousClass = $("#view-option-productgrid").attr('class').split(' ')[1];

            $(".productview").each(function () {
                if ($(this).attr("class").indexOf('-active') >= 0) {
                    var baseClass = $(this).attr('class').replace('-active', '');
                    $(this).removeClass($(this).attr('class'))
                    $(this).addClass(baseClass);
                }
            });

            $(".productview").each(function () {
                if (!displayType) {
                    if ($(this).attr("class").indexOf("grid-view") >= 0) {
                        var firstClass = $(this).attr('class');
                        $(this).removeClass(firstClass);
                        $(this).addClass(firstClass + "-active");
                    }
                }
                else {
                    if ($(this).attr("class").indexOf(displayType) >= 0) {
                        var activeclass = $(this).attr('class') + '-active';
                        $(this).removeClass($(this).attr('class'));
                        $(this).addClass(activeclass);
                    }
                }
            });
            if (!displayType) {
                $("#view-option-productgrid").removeClass(previousClass).addClass("grid-view");
            }
            else {
                $("#view-option-productgrid").removeClass(previousClass).addClass(displayType);
            }
        }
    }

    AddToCompare(productId, categoryId): any {

        Endpoint.prototype.GlobalLevelProductComapre(productId, categoryId, function (response) {
            Category.prototype.UpdateProductCompareDetails(response);
        });
        return false;
    }

    RemoveProduct(productId: any): any {
        var url = window.location.href.toString().split('/')
        var control = url[3];
        Endpoint.prototype.RemoveProduct(productId, control, function (response) {
            if (response != null) {
                $("#compareProductList").html(response.data.html);
                if (response.count > 0) {
                    $("#compareProductBox").removeAttr("style");
                }
                $(".remove-compare").off("click");
                $(".remove-compare").on("click", function () { Category.prototype.RemoveProduct($(this).attr("data-productid")); });
            }
            if (response.data.html == undefined || response.data.html.length < 1) {
                $("#compareProductList").hide();
            }
            else {
                $("#compareProductList").show();
            }
            return true;
        });
    }

    GetCompareProductList(): any {
        Endpoint.prototype.GetCompareProductList(function (response) {
            if (response != null) {
                $("#compareProductList").html(response.data.html);
                if (response.count > 0) {
                    $("#compareProductBox").removeAttr("style");
                }
                $(".remove-compare").off("click");
                $(".remove-compare").on("click", function () { Category.prototype.RemoveProduct($(this).attr("data-productid")); });
            }
            if (response.data.html == undefined || response.data.html.length < 1) {
                $("#compareProductList").hide();
            }
            else {
                $("#compareProductList").show();
            }
            return true;
        });
    }

    GetProductComparison() {
        Endpoint.prototype.GetProductComparison(function (response) {
            if (response.success == true) {
                $("#btnAddCompare").click();
                $("#popUp_content").html(response.data.popuphtml);
            }
            else {
                window.location.href = "/Product/ViewComparison";
            }
        });
    }

    CategoryLevelComparison(productId: any, categoryId: any): any {
        Endpoint.prototype.GlobalLevelProductComapre(productId, categoryId, function (response) {
            Category.prototype.UpdateProductCompareDetails(response);
        });
    }

    UpdateProductCompareDetails(response: any) {
        if (response.success == true) {
            $("#compareProductList").html(response.data.html);
            $("#compareProductBox").removeAttr("style");
            $(".remove-compare").off("click");
            $(".remove-compare").on("click", function () { Category.prototype.RemoveProduct($(this).attr("data-productid")); });
            $("#btnAddCompare").click();
            $("#popUp_content").html(response.data.popuphtml);
        } else {
            $("#btnAddCompare").click();
            $("#popUp_content").html(response.data.popuphtml);
        }
        if (response.data.html == undefined || response.data.html.length < 1) {
            $("#compareProductList").hide();
        }
        else {
            $("#compareProductList").show();
        }
    }

    GetCategoryBreadCrumb(categoryId: number): void {
        Endpoint.prototype.GetCategoryBreadCrumb(categoryId, function (response) {
            $("#breadCrumb").html(response.breadCrumb)

        });
    }
}
var CategoriesArray = [];
class SiteMap extends ZnodeBase {
    constructor() {
        super();
    }
    Init() {
        SiteMap.prototype.LoadCatalog(1, 20);
        SiteMap.prototype.CheckAndBindProducts(1, 50);
        $(document).ajaxStop(function () {
            //Load ContentPage after footer gets loaded
            if ($("#SiteMapMenu li").length <= 0) {
                SiteMap.prototype.LoadSiteMap();
            }
        });
    }
    LoadCatalog(pageSize, pageLength): any {
        Checkout.prototype.ShowLoader();
        Endpoint.prototype.GetSiteMapCategory(pageSize, pageLength, function (response) {
            var _link;  
            if (response.Result.CategoryList == null || response.Result.CategoryList.length <= 0) {
                SiteMap.prototype.LoadBrands(response.Result.BrandList);
                $("#divCatelogMap").html('<button data-loadmorecontrol="category" data-test-selector="btnLoadMore"  class="btn-text red  btn-color-primary" disabled >Load More</button>');
                return;
            }
            $.each(response.Result.CategoryList, function (e, v) {
                if (v.SEOUrl !== null) {
                    _link = "<i class='zf-caret-right'></i><a href='/" + v.SEOUrl + "'>" + v.CategoryName + "</a>";
                }
                else {
                    _link = "<i class='zf-caret-right'></i><a href='/Category/" + v.ZnodeCategoryId + "'>" + v.CategoryName + "</a>";
                }
              
                $("#ulCatelogMap").append("<li data-categorytype='parentcategory'> " + _link + "" + SiteMap.prototype.SubCategory(v.SubCategoryItems) + "</li>");   

                $("#divCatelogMap").html('<button data-loadmorecontrol="category" data-test-selector="btnLoadMore"  class="btn-text red  btn-color-primary" onclick="SiteMap.prototype.LoadCatalog(' + (pageSize + 1) + ',' + pageLength + ')">Load More</button>');
            });
            Checkout.prototype.HideLoader();
        });
        Checkout.prototype.HideLoader();
    }

    LoadBrands(brands): any {
        var _link;
        var _html = "<li><i class='zf-caret-right'></i><a href='/Brand/List'>" + ZnodeBase.prototype.getResourceByKeyName("LableBrand") + "</a><ul class='sub-menu'>";
        Checkout.prototype.ShowLoader();
        $.each(brands, function (e, v) {
            if (v.SEOFriendlyPageName !== null) {
                _link = "<i class='zf-arrow-right-small'></i><a href='/" + v.SEOFriendlyPageName + "'>" + v.BrandName + "</a>";
            }
            else {
                _link = "<i class='zf-arrow-right-small'></i><a href='/brand/" + v.BrandId + "'>" + v.BrandName + "</a>";
            }
            _html += "<li>" + _link + "</li>"

        });
        _html += "</ul>"
        $("#ulCatelogMap").append(_html);
        Checkout.prototype.HideLoader();
    }

    SubCategory(subcategory): any {
            if (typeof subcategory !== typeof undefined && subcategory.length > 0) {
                var _html = "<ul class='sub-menu'>";
                var _link;
                $.each(subcategory, function (e, v) {
                    if (v.SEOUrl !== null) {
                        _link = "<i class='zf-arrow-right-small'></i><a href='/" + v.SEOUrl + "'>" + v.CategoryName + "</a>";
                    }
                    else {
                        _link = "<i class='zf-arrow-right-small'></i><a href='/Category/" + v.ZnodeCategoryId + "'>" + v.CategoryName + "</a>";
                    }

                    _html += "<li  data-categorytype='subcategory'>" + _link + "" + SiteMap.prototype.SubCategory(v.SubCategoryItems) + " </li>";
                });

                _html += "</ul>";

                return _html;
        }
        return "";
    }


    LoadSiteMap(): any {
        var MainLength = $("#layout-footer").find("z-widget .footer-help-section-link").length;
        for (var j = 0; j < MainLength; j++) {
            var arr = $("#layout-footer").find("z-widget .footer-help-section-link:eq(" + j + ") ul>li")
            $.each(arr, function (e, v) {
                $("#SiteMapMenu").append("<li>" + $(v).find("a:eq(0)").parent().html() + "</li>")
            });
        }
    }
    CheckAndBindCategory(category): any {
        var attr = $(category).find("a:eq(0)").parent().find("ul").html();
        if (typeof attr !== typeof undefined) {
            var e = $(category).clone()
            var d = $(e).find("ul").removeClass("dropdown-menu list-unstyled fadeInUp animated").addClass("sub-menu");
            return $(d).parent().find("a:eq(0)").parent().html();
        }
        return $(category).find("a:eq(0)").parent().html()
    }

    CheckAndBindProducts(pageNo, pageSize): any {
        Checkout.prototype.ShowLoader();
        Endpoint.prototype.GetPublishedProductList(pageNo, pageSize, function (result) {
            var strHtml = "";
            var prodName = "";
            var cateName = "";
            if (result.result.ProductList == null) {
                $("#divProductMap").html('<button class="btn-text red  btn-color-primary" disabled>Load More</button>');
                return;
            }
            else {
                $.each(result.result.ProductList, function (e, v) {
                    var SEOUrl = v.SEOUrl;
                    if (v.SEOUrl == null || v.SEOUrl == "") { SEOUrl = "product/" + v.ZnodeProductId };

                    if ((v.CategoryName != cateName) && ($("#ulProductMap li").not(".sub-menu").last().text() != v.CategoryName)) {
                        if (v.CategoryName != "") {
                            strHtml = strHtml + "<li><i class='zf-caret-right'></i>" + SiteMap.prototype.BindCategoryLink(v.CategoryName) + "</li>";
                            if (v.Name != prodName) { strHtml = strHtml + "<li class='sub-menu'><i class='zf-arrow-right-small'></i><a href='/" + SEOUrl + "'>" + v.Name + "</a></li>"; prodName = v.Name; }
                            cateName = v.CategoryName;
                        }
                    } else { if (v.Name != prodName) { strHtml = strHtml + "<li class='sub-menu'><i class='zf-arrow-right-small'></i><a href='/" + SEOUrl + "'>" + v.Name + "</a></li>"; prodName = v.Name; } }
                });
            }
            if ($("#ulProductMap li").length >= result.result.TotalResults) {
                $("#divProductMap").html('<button data-loadmorecontrol="product" data-test-selector="btnLoadMore"  class="btn-text red  btn-color-primary" disabled onclick="SiteMap.prototype.CheckAndBindProducts(' + (pageNo) + ',' + pageSize + ')">Load More</button>');
            }
            else {
                $("#divProductMap").html('<button data-loadmorecontrol="product" data-test-selector="btnLoadMore"  class="btn-text red  btn-color-primary" onclick="SiteMap.prototype.CheckAndBindProducts(' + (pageNo + 1) + ',' + pageSize + ')">Load More</button>');

            }
            $("#ulProductMap").append(strHtml);
        });
        Checkout.prototype.HideLoader();
    }

    BindCategoryLink(CategoryName: string): any {
        var toReturn = "";
        $.each(CategoriesArray, function (e, v) {
            if (CategoryName == v.CategoryName) {
                if (v.SEOPageName != null) {
                    toReturn = "<a href='/" + v.SEOPageName + "'>" + v.CategoryName + "</a>";
                    return false;
                }
                else {
                    toReturn = "<a href='/Category/" + v.CategoryId + "'>" + v.CategoryName + "</a>";
                    return false;
                }
            }
            else {
                if (v.SubCategoryItems.length > 0) {
                    var _link;
                    $.each(v.SubCategoryItems, function (e, d) {
                        if (CategoryName == d.CategoryName) {
                            if (d.SEOPageName != null) {
                                toReturn = "<a href='/" + d.SEOPageName + "'>" + d.CategoryName + "</a>";
                                return false;
                            }
                            else {
                                toReturn = "<a href='/Category/" + d.CategoryId + "'>" + d.CategoryName + "</a>";
                                return false;
                            }
                        }
                    });
                }
            }
        });

        return toReturn;
    }

    BindSubCategoryLink(subcategory, CategoryName: string): any {
        var toReturn = "";
        if (subcategory.length > 0) {
            var _link;
            $.each(subcategory, function (e, v) {
                if (CategoryName == v.CategoryName) {
                    if (v.SEOPageName != null) {
                        toReturn = "<a href='/" + v.SEOPageName + "'>" + v.CategoryName + "</a>";
                        return false;
                    }
                    else {
                        toReturn = "<a href='/Category/" + v.CategoryId + "'>" + v.CategoryName + "</a>";
                        return false;
                    }
                }
                else
                    SiteMap.prototype.BindSubCategoryLink(v.ChildCategoryItems, CategoryName);
            });
            return toReturn;
        }
    }
}
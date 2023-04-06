declare function purl(): any;
var totalPages = 0;
class ZSearch extends ZnodeBase {
    constructor() {
        super();
    }
    Init() {
        $.getScript("/Scripts/lib/purl.js");
        ZSearch.prototype.SearchSort();
        ZSearch.prototype.NextClickFunction();
        ZSearch.prototype.PrevClickFunction();
        totalPages = parseInt($("#hdnTotalPages").val(), 10);
    }
    // BINDINGS
    
    // Redirects to new search results page when the sorting dropdown is changed
    // Uses purl library
    SearchSort(): any {
        $("#layout-search .search-sorting select").on("change", function () {
            $("#layout-search .search-results").html('<div class="search-results-wait">...</div>');

            var url = purl(),
                query = url.param();

            query.sort = $(this).val();
            query.pagenumber = 1;
            window.location.href = url.attr("path") + "?" + $.param(query) + "#product-grid";
        });
        ZSearch.prototype.SearchPaging();
    }

    SearchPaging(): any {

        $("#layout-paging .search-paging select").on("change", function () {
            $("#layout-search .search-results").html('<div class="search-results-wait">...</div>');

            var url = purl(),
                query = url.param();

            query.pageSize = $(this).val();
            query.pagenumber = 1;

            window.location.href = url.attr("path") + "?" + $.param(query) + "#product-grid";
        });
    }

    SetPager(control, mode): any {
        var currentPageNo = $(control).data("pageno");
        if (currentPageNo == "") {
            currentPageNo = 1;
        }

        if (mode == 1) {
            currentPageNo++;
        }
        else
            currentPageNo--;

        var url = purl(),
            query = url.param();

        query.pageSize = $("#PageSize").val();
        query.pagenumber = currentPageNo;

        window.location.href = url.attr("path") + "?" + $.param(query);
    }

    GetQueryStringParameterByName(name): any {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    PrevClickFunction(): any {
        $(".prev-page-search").on("click", function () {
            var pageno = ZSearch.prototype.GetQueryStringParameterByName("pagenumber");
            if (totalPages == 1) {
                $('.prev-page-search').addClass('disabled');
                return false;
            }
            if (parseInt(pageno) == 1) {
                $('.prev-page-search').addClass('disabled');
                return false;
            }
            ZSearch.prototype.SetPager(this, 0)
        });
    }
    NextClickFunction(): any {
        $(".next-page-search").on("click", function () {
            var pageno = ZSearch.prototype.GetQueryStringParameterByName("pagenumber");
            if (totalPages == 1) {
                $('.next-page-search').addClass('disabled');
                return false;
            }
            if (parseInt(pageno) == (totalPages)) {
                $('.next-page-search').addClass('disabled');
                return false;
            }
            ZSearch.prototype.SetPager(this, 1)
        });
    }
}
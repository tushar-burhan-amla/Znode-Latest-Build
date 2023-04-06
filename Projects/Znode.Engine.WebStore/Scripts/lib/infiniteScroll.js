var page = 1,
    inCallback = false,
    hasReachedEndOfInfiniteScroll = false;

var scrollHandler = function () {
    if (hasReachedEndOfInfiniteScroll == false &&
            ($(window).scrollTop() == $(document).height() - $(window).height())) {
        loadMoreToInfiniteScrollTable('/Search/ProductsPaging');
    }
}

var ulScrollHandler = function () {
    //$("#layout-category").scrollTop() + $("#layout-category").innerHeight() >= $("#layout-category")[0].scrollHeight

    if (hasReachedEndOfInfiniteScroll == false &&
            (($(window).scrollTop() - 300) >= 700)) {
        loadMoreToInfiniteScrollUl('/Search/ProductsPaging');
    }
}

function loadMoreToInfiniteScrollUl(loadMoreRowsUrl) {
    if (page > -1 && !inCallback) {
        inCallback = true;
        page++;
        var category = $("#layout-category").data("category");
        var _categoryId = $("#layout-category").data("categoryid");

        $.ajax({
            type: 'GET',
            url: loadMoreRowsUrl,
            data: { categoryName: category, categoryId: _categoryId, pageNum: page },
            success: function (data, textstatus) {
                if (data != '') {
                    $("ul#view-option-productgrid").append(data);
                    Product.prototype.GetPriceAsync();
                }
                else {
                    page = -1;
                }

                inCallback = false;
                $("div#loading").hide();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            }
        });
    }
}

function loadMoreToInfiniteScrollTable(loadMoreRowsUrl) {
    if (page > -1 && !inCallback) {
        inCallback = true;
        page++;
        $("div#loading").show();
        $.ajax({
            type: 'GET',
            url: loadMoreRowsUrl,
            data: "pageNum=" + page,
            success: function (data, textstatus) {
                if (data != '') {
                    $("table.infinite-scroll > tbody").append(data);
                    $("table.infinite-scroll > tbody > tr:even").addClass("alt-row-class");
                    $("table.infinite-scroll > tbody > tr:odd").removeClass("alt-row-class");
                }
                else {
                    page = -1;
                }

                inCallback = false;
                $("div#loading").hide();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
            }
        });
    }
}

function showNoMoreRecords() {
    hasReachedEndOfInfiniteScroll = true;
}
$(function () {
    $("div#loading").hide();

    var attr = $("ul#view-option-productgrid").attr('infinnity-loading');
    if (typeof attr !== typeof undefined && attr !== false) {
        $("div.search-paging").hide();
        $(window).scroll(ulScrollHandler);
    }
});
// Index of remembered keyword to be removed.
var removeRememberedKeywordIndex: number = 0;
// Maximum searched terms to be saved.
var maximumRememberedSearchTerms: number = 10; 

class Search extends ZnodeBase {

    Init() {
        ZSearch.prototype.Init();
        var categoryId: number = parseInt(ZSearch.prototype.GetQueryStringParameterByName("CategoryId"), 10);

        if (categoryId > 0)
        {
            window.sessionStorage.removeItem("lastCategoryId");
            window.sessionStorage.setItem("lastCategoryId", $("#categoryId").val());
            localStorage.setItem("isFromCategoryPage", "true");
        }

        Category.prototype.changeProductViewDisplay();
        Category.prototype.GetCompareProductList()
        Category.prototype.setProductViewDisplay();
    }

    GetFilterResult(item: any, facet: any): any {
        if ($(facet).prop("checked")) {
            var FacetDict = {};

            //Get Previously applied facet filters
            var facetFilters = this.GetPreviousAppliedFacetFilters(FacetDict);

            //Get current facet parameters
            this.GetCurrentFacetFilters(item, FacetDict);

            //build querystring 
            var queryString = this.BuildQuerystring(FacetDict);

            //build new url
            var newUrl;
            newUrl = this.BuildUrl(queryString, facetFilters, false);
            var _pageNumber = this.getUrlVars(newUrl)["pagenumber"];
            if (_pageNumber != undefined)
                newUrl = newUrl.replace("pagenumber=" + _pageNumber, "pagenumber=" + 1);

            window.location.href = newUrl;
        }
        else {
            this.RemoveFacet(item);
        }
    }

    private BuildUrl(queryString: string, facetFilters: string[], isfromRemove: boolean) {
        var _customUri = new CustomJurl();
        queryString = encodeURIComponent(queryString);
        var newUrlParameter = queryString == "" ? queryString : _customUri.setQueryParameter("FacetGroup", queryString);
        newUrlParameter = _customUri.setQueryParameter("fromSearch", true);
        var newUrl;
        if (facetFilters[1] != undefined) {
            facetFilters[1].split('&').forEach(function (param) {
                var item = param.split("=");
                if (item[0].toLowerCase() != "facetgroup") {
                    newUrlParameter = _customUri.setQueryParameter(item[0], item[1]);
                }
            });
        }
        if (isfromRemove) {
            if (queryString == "") {
                newUrlParameter = _customUri.removeQueryParameter("fromSearch");
            }
        }
        newUrl = _customUri.build(facetFilters[0], newUrlParameter);
        return newUrl;
    }

    private GetPreviousAppliedFacetFilters(FacetDict: {}) {

        var groupAndValueSeparator = '|', groupsSeparator = ',', valuesSeperator = '~';

        var currentUrlParameters = this.GetUrlParameters(window.location.href);
        var facetParameterValue = this.getUrlVars(window.location.href)["FacetGroup"]; // Get FacetGroup parameter values
        if (facetParameterValue != undefined) {
            facetParameterValue = decodeURIComponent(facetParameterValue);
            var FacetList = [];
            FacetList = facetParameterValue.split(groupsSeparator);
            //Iterate through each facet in 
            FacetList.forEach(function (item) {
                var facetItemValues = [];
                var facetItem = item.split(groupAndValueSeparator);
                var facetItemName = facetItem[0];
                facetItemValues = facetItem[1].split(valuesSeperator);
                FacetDict[facetItemName] = facetItemValues;
            });
        }
        else {
            var _pageNumber = this.getUrlVars(window.location.href)["pagenumber"];
            if (currentUrlParameters.length > 1) {
                currentUrlParameters[1] = currentUrlParameters[1].replace("pagenumber=" + _pageNumber, "pagenumber=" + 1);
            }
        }
        return currentUrlParameters;
    }


    private BuildQuerystring(FacetDict: {}) {
        var groupAndValueSeparator = '|', groupsSeparator = ',', valuesSeperator = '~';
        var queryString = "";
        for (var key in FacetDict) {
            if (typeof FacetDict[key] == "string")
                queryString += ((queryString == "") ? "" : groupsSeparator) + key + groupAndValueSeparator + FacetDict[key];
            else
                queryString += ((queryString == "") ? "" : groupsSeparator) + key + groupAndValueSeparator + FacetDict[key].join(valuesSeperator);
        }
        return queryString;
    }

    private GetCurrentFacetFilters(item: any, FacetDict: {}) {
        var valuesSeperator = '~';
        var newfacetGroup = this.getUrlVars(item)["FacetGroup"];//read facet group name
        var newfacetValue = this.getUrlVars(item)["FacetValue"];//read facet group value
        if (this.ExistsKey(FacetDict, newfacetGroup)) //if already exits
        {
            FacetDict[newfacetGroup] = FacetDict[newfacetGroup].join(valuesSeperator) + valuesSeperator + newfacetValue;
        }
        else {
            FacetDict[newfacetGroup] = newfacetValue;
        }
    }

    private ExistsKey(dict: {}, item: any) {
        return dict.hasOwnProperty(item);
    }

    private GetUrlParameters(url: string) {
        return url.split('?');
    }

    RemoveFacet(item: any): any {
        var FacetDict = {};
        var facetName = this.getUrlVars(item)["FacetGroup"];//read facet group name
        var facetValue = this.getUrlVars(item)["FacetValue"];//read facet group value

        //Get Previously applied facet filters
        var facetFilters = this.GetPreviousAppliedFacetFilters(FacetDict);

        var isRemoveAll = this.getUrlVars(item)["IsRemoveAll"];
        if (isRemoveAll == "true") {
            FacetDict = {};
        }
        else {
            this.DeleteFacet(FacetDict, facetName, facetValue);
        }
        //build querystring 
        var queryString = this.BuildQuerystring(FacetDict);

        //build  new url

        var newUrl;
        newUrl = this.BuildUrl(queryString, facetFilters, true);

        var _pageNumber = this.getUrlVars(newUrl)["pagenumber"];
        if(_pageNumber != undefined)
            newUrl = newUrl.replace("pagenumber=" + _pageNumber, "pagenumber=" + 1);

        window.location.href = newUrl;

    }

    private DeleteFacet(FacetDict: {}, facetName: any, facetValue: any) {
        if (this.ExistsKey(FacetDict, facetName)) {
            if (FacetDict[facetName].length > 1) {
                var index = FacetDict[facetName].indexOf(facetValue);
                if (index > -1) {
                    FacetDict[facetName].splice(index, 1);
                }
            }
            else {
                delete FacetDict[facetName];
            }
        }
    }

    // Read a page's GET URL variables and return them as an associative array.
    private getUrlVars(item) {
        var vars = [], hash;
        var hashes = item.slice(item.indexOf('?') + 1).split('&');
        for (var i = 0; i < hashes.length; i++) {
            hash = hashes[i].split('=');
            vars.push(hash[0]);
            vars[hash[0]] = hash[1];
        }
        return vars;
    }

    ValidateSearch(): boolean {
        $("#btnSearchTerm").on("click", function () {
            var searchTerm = $("input[name=SearchTerm]").val();
            if (searchTerm != null && searchTerm != undefined && searchTerm.trim().length <= 0) {
                return false;
            }
            localStorage.setItem('refreshSearch', 'true');
            return true;
        });
        return true;
    }
  
    // Save previously search terms in local storage
    SaveSearchKeywords(): any {
        var searchTerm = $("#hdnSearchTerm").val();
        var searchResultCount = parseInt($("#hdnProductCount").val());
        if (localStorage.getItem(Constant.RememberedSearchTerms) == null) {
            localStorage.setItem(Constant.RememberedSearchTerms, "[]");
        }
        var oldSearchTerms = JSON.parse(localStorage.getItem(Constant.RememberedSearchTerms));
        if (searchTerm != null && searchTerm != undefined && searchTerm.trim().length > 0) {
            if (oldSearchTerms.indexOf(searchTerm) < 0) {
                if (searchResultCount > 0 && searchResultCount != undefined && searchResultCount != null) {
                    if (oldSearchTerms.length < maximumRememberedSearchTerms) {
                        this.SetSearchedKeywordInLocalStorage(searchTerm, oldSearchTerms);
                    }
                    else {
                        oldSearchTerms.splice(0, 1);
                        localStorage.removeItem(Constant.RememberedSearchTerms);
                        this.SetSearchedKeywordInLocalStorage(searchTerm, oldSearchTerms);
                    }
                }
            }
            else {
                var indexOfExistingSearchedKeyword = oldSearchTerms.indexOf(searchTerm);
                oldSearchTerms.splice(indexOfExistingSearchedKeyword, 1);
                localStorage.removeItem(Constant.RememberedSearchTerms);
                this.SetSearchedKeywordInLocalStorage(searchTerm, oldSearchTerms);
            }
        }
    }

    // Set searched terms in local storage
    SetSearchedKeywordInLocalStorage(searchTerm: string, rememberedSearchedTerms: any): any {
        rememberedSearchedTerms.push(searchTerm);
        localStorage.setItem(Constant.RememberedSearchTerms, JSON.stringify(rememberedSearchedTerms));
    }

    // Append searched terms to dropdown
    AppendElement(): any {
        var searchedTerms = JSON.parse(localStorage.getItem(Constant.RememberedSearchTerms));
        if (searchedTerms != null) {
            var reversedSearchedTerms = searchedTerms.reverse().slice(removeRememberedKeywordIndex, maximumRememberedSearchTerms);
            reversedSearchedTerms.forEach(function (keyword) {
                $("#suggestionList").append('<a href="/Search?SearchTerm=' + keyword + '" onclick="Product.prototype.IsCategoryLinkClicked()"><div class="search-term py-1">' + '<span class="icon-up-left-arrow"></span>' + '<span class="search-name ml-2">' + keyword + '</span>' + "</div></a>");
            });
        }
    }

    // Show dropdown list of searched terms
    ShowRememberedSearchTerms(): any {
        if (localStorage.getItem(Constant.RememberedSearchTerms) != null) {
            if (JSON.parse(localStorage.getItem(Constant.RememberedSearchTerms)).length > 0) {
                $("#suggestionList").html("");
                Search.prototype.AppendElement();
                $("#suggestionList").show();
            }
        }
    }

    // Hide dropdown list of searched terms
    HideRememberedSearchTerms(event: any): any {
        var container = $("#suggestionList, #searchTextBox");
        if (!$(event.target).closest(container).length) {
            $("#suggestionList").hide();
        }
    }

    //Save search report data
    SaveSearchReportData(model: any): any {
        Endpoint.prototype.SaveSearchReportData(model, function (response) {
        });
    }

    //Get cmspage based on keyword search
    GetSearchCMSPage(item: any): any {
        if (item) {
            var pageNumber = "";
            var pageSize = "";
            var searchTerm = item.dataset['searchterm'];
            var targetId = item.dataset['targetid'];
            var hdnPageNumber = $("#hdncontentPageNumber").val();
            var hdnPageSize = $("#hdncontentPageSize").val();
            var refreshSearch = localStorage.getItem('refreshSearch');

            if (!refreshSearch) {
                if (!hdnPageNumber || (hdnPageNumber == "0")) {
                    pageNumber = $("#currentContentPageNumber").val();
                } else {
                    pageNumber = hdnPageNumber;
                }
                if (!hdnPageSize || (hdnPageSize == "0")) {
                    pageSize = $("#layout-cms-paging .search-paging select").val()
                } else {
                    pageSize = hdnPageSize;
                }
            }
            else {
                pageNumber = Constant.CMSDefaultPageNumber;
                pageSize = Constant.CMSDefaultPageSize;
                localStorage.removeItem("refreshSearch");
            }

            $("#hdncontentPageNumber").val("0");
            $("#hdncontentPageSize").val("0");
            Endpoint.prototype.GetSearchCMSPages(searchTerm, pageNumber, pageSize, function (response) {
                if (response != null) {
                    $(targetId).html(response);
                }
            });
        }
    }

    //Cms page tab previous click 
    PreviousPageClick(): any {
        var currentPageNumber = $("#currentContentPageNumber").val();
        var totalPages = parseInt($("#hdnContentPageTotalPages").val(), 10);
        if (totalPages == 1) {
            $('.prev-content-page-search').addClass('disabled');
            return false;
        }
        if (parseInt(currentPageNumber) == 1) {
            $('.prev-content-page-search').addClass('disabled');
            return false;
        }
        if (!currentPageNumber) {
            currentPageNumber = 1;
        }
        currentPageNumber--;
        $("#currentContentPageNumber").val(currentPageNumber);
        this.ScrollUp();
        $("#formcmspagesearch").click();
    }

    //Cms page tab next click
    NextPageClick(): any {
        var currentPageNumber = $("#currentContentPageNumber").val();
        var totalPages = parseInt($("#hdnContentPageTotalPages").val(), 10);
        if (totalPages == 1) {
            $('.next-content-page-search').addClass('disabled');
            return false;
        }
        if (parseInt(currentPageNumber) == (totalPages)) {
            $('.next-content-page-search').addClass('disabled');
            return false;
        }
        if (!currentPageNumber) {
            currentPageNumber = 1;
        }
        currentPageNumber++;
        $("#currentContentPageNumber").val(currentPageNumber);
        this.ScrollUp();
        $("#formcmspagesearch").click();
    }

    //Scroll up focus
    ScrollUp(): any {
        $('html, body').animate({
            scrollTop: 0
        }, 'slow');
    }
}
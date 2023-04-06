class SearchReport extends ZnodeBase {
    _endpoint: Endpoint;

    constructor() {
        super();
        this._endpoint = new Endpoint();

    }

    Init(): any {
    }

    IntializeDatePicker(): any {
        ZnodeDateRangePicker.prototype.Init(SearchReport.prototype.DateTimePickerRangeForSearchReport());
    }
    //This method is used to select store from fast select and show it on textbox
    OnSelectStoreTopKeywordList(item: any): any {
        if (item != undefined) {
            let portalName: string = item.text;
            let portalId: number = item.Id;

            Endpoint.prototype.GetTopKeywordsReport(portalId, portalName, function (response) {
                $("#divTopKeywordList").html("");
                $("#divTopKeywordList").html(response);
            });
        }
    }

    OnSelectStoreNoResultFoundKeywordList(item: any): any {
        if (item != undefined) {
            let portalName: string = item.text;
            let portalId: number = item.Id;

            Endpoint.prototype.GetNoResultsFoundReport(portalId, portalName, function (response) {
                $("#divNoResultKeywordList").html("");
                $("#divNoResultKeywordList").html(response);
                ZnodeDateRangePicker.prototype.Init(SearchReport.prototype.DateTimePickerRangeForSearchReport());
            });
        }
    }


    DateTimePickerRangeForSearchReport(): any {
        var ranges = {
            'Last Hour': [],
            'Last Day': [],
            'Last 7 Days': [],
            'Last 30 Days': [],
        }
        return ranges;
    }
}
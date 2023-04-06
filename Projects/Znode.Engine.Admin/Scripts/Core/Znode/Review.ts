class Review extends ZnodeBase {
    _endPoint: Endpoint;
    constructor() {
        super();
        this._endPoint = new Endpoint();
    }

    DeleteCustomerReview(control): any {
        var customerReviewIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (customerReviewIds.length > 0) {
            Endpoint.prototype.DeleteCustomerReview(customerReviewIds, function (res) {
                DynamicGrid.prototype.RefreshGridOndelete(control, res);
            });
        }
    }

    ChangeStatusNew(control): any {
        var statusId = $(".page-container").find('ul li:nth-child(2) a').attr("id");
        var customerReviewIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (customerReviewIds.length > 0) {
            Endpoint.prototype.ChangeStatus(customerReviewIds, statusId, function (res) {
                window.location.href = window.location.protocol + "//" + window.location.host + "/Review/List";
            });
        }
    }

    ChangeStatusActive(control): any {
        var statusId = $(".page-container").find('ul li:nth-child(3) a').attr("id");
        var customerReviewIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (customerReviewIds.length > 0) {
            Endpoint.prototype.ChangeStatus(customerReviewIds, statusId, function (res) {
                window.location.href = window.location.protocol + "//" + window.location.host + "/Review/List";
            });
        }
    }

    ChangeStatusInactive(control): any {
        var statusId = $(".page-container").find('ul li:nth-child(4) a').attr("id");
        var customerReviewIds = DynamicGrid.prototype.GetMultipleSelectedIds();
        if (customerReviewIds.length > 0) {
            Endpoint.prototype.ChangeStatus(customerReviewIds, statusId, function (res) {
                window.location.href = window.location.protocol + "//" + window.location.host + "/Review/List";
            });
        }
    }
}
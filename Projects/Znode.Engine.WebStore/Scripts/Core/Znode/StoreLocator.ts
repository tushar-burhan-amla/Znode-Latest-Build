declare function initMap(): any;
class StoreLocator extends ZnodeBase {
    constructor() {
        super();
    }

    Init() {
        if ($("#map").length > 0) {
            navigator.geolocation.getCurrentPosition(function (position) {
                if ($("#Latitude").val() == null || $("#Latitude").val() == "") {
                    $("#Latitude").val(position.coords.latitude);
                    $("#Longitude").val(position.coords.longitude);
                }
            },
                function (error) { },
                {
                    enableHighAccuracy: true
                });
        }
    }

    //Get co-ordinates of address
    GetLatLng(callback): any {
        console.log(super.GetGeoLocatorAPI() + '?address='
            + $("#PostalCode").val() + ',+'
            + $("#CityName").val() + ',+'
            + $("#StateName").val() + '&key=' + super.GetGeoLocatorAPIKey());
        super.ajaxRequest(
            super.GetGeoLocatorAPI() + '?address='
            + $("#PostalCode").val() + ',+'
            + $("#CityName").val() + ',+'
            + $("#StateName").val() + '&key=' + super.GetGeoLocatorAPIKey()
            , "GET"
            , {}
            , function (data) {
                console.log(data);
                if (data.status == "OK") {
                    //Other responses : 
                    //ZERO_RESULTS(indicates that the geocode was successful but returned no results.This may occur if the geocoder was passed a non- existent address), 
                    //OVER_QUERY_LIMIT(indicates that you are over your quota), 
                    //REQUEST_DENIED(indicates that your request was denied), 
                    //INVALID_REQUEST(generally indicates that the query (address, components or latlng) is missing), 
                    //UNKNOWN_ERROR ( indicates that the request could not be processed due to a server error. The request may succeed if you try again.)

                    var browserLocation = {
                        lat: data.results[0]
                            .geometry
                            .location
                            .lat,
                        lng: data.results[0]
                            .geometry
                            .location
                            .lng
                    };
                    callback(browserLocation, data.status);

                } else {
                    if (data.error_message == null) {
                        ZnodeBase.prototype.errorAsAlert = false;
                        ZnodeBase.prototype.errorOutfunction("Invalid address");
                    } else {
                        ZnodeBase.prototype.errorAsAlert = false;
                        ZnodeBase.prototype.errorOutfunction(data.error_message);
                    }
                    callback(null, data.status);
                }

            }
            , "json");
    }

    //Show/hide store locations in grid
    showDistanceWiseData() {
        document.getElementById('right-panel').innerHTML = "";
        $(".storeLocationCoordinate").each(function (index, storeLocation) {
            //Show/hide store locations in grid
            var storeDistance = $(storeLocation).data("distance");
            if (parseFloat(storeDistance) <= parseFloat($("#Radius").val()))
                $(storeLocation).show();
            else
                $(storeLocation).hide();

            //If distance could not be calculated
            if ($("#Radius").val() == 0)
                $(storeLocation).show();
        });
        initMap();
    }
}
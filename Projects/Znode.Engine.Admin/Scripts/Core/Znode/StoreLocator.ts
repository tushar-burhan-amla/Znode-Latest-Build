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
                    StoreLocator.prototype.GetLatLng();
                }
            },
                function (error) {
                    StoreLocator.prototype.GetLatLng();
                },
                {
                    enableHighAccuracy: true
                });
        }
        Account.prototype.BindStates();
    }

    //Get co-ordinates of address
    GetLatLng(): any {
        super.ajaxRequest(
            super.GetGeoLocatorAPI() + '?address='
            + $("#Address2").val() + ',+'
            + $("#Address3").val() + ',+'
            + $("#PostalCode").val() + ',+'
            + $("#CityName").val() + ',+'
            + $("#StateName").val() + ',+'
            + $("#ddlCountryList").val() + '&key=' + super.GetGeoLocatorAPIKey()
            , "GET"
            , {}
            , function (data) {
                if (data.status == "OK") {
                    //Other responses : 
                    //ZERO_RESULTS(indicates that the geocode was successful but returned no results.This may occur if the geocoder was passed a non- existent address), 
                    //OVER_QUERY_LIMIT(indicates that you are over your quota), 
                    //REQUEST_DENIED(indicates that your request was denied), 
                    //INVALID_REQUEST(generally indicates that the query (address, components or latlng) is missing), 
                    //UNKNOWN_ERROR ( indicates that the request could not be processed due to a server error. The request may succeed if you try again.)

                    $("#Latitude").val(data.results[0]
                        .geometry
                        .location
                        .lat);

                    $("#Longitude").val(data.results[0]
                        .geometry
                        .location
                        .lng);

                    initMap();
                } else {
                    if (data.error_message == null) {
                        ZnodeBase.prototype.errorAsAlert = false;
                        ZnodeBase.prototype.errorOutfunction("Invalid address");
                    } else {
                        ZnodeBase.prototype.errorAsAlert = false;
                        ZnodeBase.prototype.errorOutfunction(data.error_message);
                    }
                }

                initMap();
            }
            , "json");
    }
}
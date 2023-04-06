function initMap() {
    getBrowserLocation(function (browserLocation) {
        setMap(browserLocation);
    });
}

//Get source geo-location
function getBrowserLocation(callback) {
    var browserLocation;
    var defaultLocation = { lat: -25.363, lng: 131.044 };
    if ($("#PostalCode").val() == "" && $("#CityName").val() == "" && $("#StateName").val() == "") {
        navigator.geolocation.getCurrentPosition(function (position) {
            browserLocation = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };
            return callback(browserLocation);
        }, function (error) {
            return callback(defaultLocation);
        });
    } else {
        StoreLocator.prototype.GetLatLng(function (browserLocation, status) {
            if (browserLocation != null)
                return callback(browserLocation);
            else {
                navigator.geolocation.getCurrentPosition(function (position) {
                    browserLocation = {
                        lat: position.coords.latitude,
                        lng: position.coords.longitude
                    };
                    return callback(browserLocation);
                }, function (error) {
                    return callback(defaultLocation);
                });
            }
        });
    }
}

//Display map
function setMap(centerLocation) {
    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 18,
        center: centerLocation,
        minZoom: 3,
        mapTypeId: 'hybrid',
        animation: 'bounce'
    });
    var marker = new google.maps.Marker({
        position: centerLocation,
        animation: google.maps.Animation.BOUNCE,
        map: map
    });

    var directionsService = new google.maps.DirectionsService;

    addStoreMarker(map, centerLocation, directionsService);
}

//Display all store markers on map
function addStoreMarker(map, centerLocation, directionsService) {
    document.getElementById('right-panel').innerHTML = "";
    $(".storeLocationCoordinate").each(function (index, storeLocation) {
        if ($(storeLocation).css("display") != "none") {
            var storeLocationCo = { lat: $(storeLocation).data("lat"), lng: $(storeLocation).data("lng") };

            var goldStar = {
                path: 'M 125,5 155,90 245,90 175,145 200,230 125,180 50,230 75,145 5,90 95,90 z',
                fillColor: '#cc0008',
                fillOpacity: 0.8,
                scale: 0.1,
                strokeColor: 'red',
                strokeWeight: 3
            };

            var marker = new google.maps.Marker({
                position: storeLocationCo,
                animation: google.maps.Animation.DROP,
                title: $(storeLocation).data("title"),
                icon: goldStar
            });
            addInfoWindow(marker, map, storeLocation);
            marker.setMap(map);
            calculateAndShowRoute(map, centerLocation, storeLocationCo, directionsService, storeLocation)
        }
    });
}

//Add info window for marker
function addInfoWindow(marker, map, storeLocation) {
    var infowindow = new google.maps.InfoWindow({
        content: '<b>' + $(storeLocation).data("title") + '</b><br /><p>' + $(storeLocation).data("address") + '</p>'
    });
    marker.addListener('click', function () {
        infowindow.open(map, marker);
    });
}

//Show route on map
function displayRoute(origin, destination, service, display, store) {
    var selectedMode = document.getElementById('mode').value;

    service.route({
        origin: origin,
        destination: destination,
        travelMode: google.maps.TravelMode[selectedMode],
        unitSystem: google.maps.UnitSystem.IMPERIAL,
        avoidTolls: true
    }, function (response, status) {
        if (status === 'OK') {
            var distance = parseFloat((response.routes[0].legs[0].distance.text.split(' ')[0]).replace(/[^\d\.]/g, ''));
            $(store).data("distance", distance);
            $(store).find("td:last").find(".distanceLabel").remove();
            $(store).find("td:last").append("<span class='distanceLabel text-info'>&nbsp;(" + distance + " miles away)</span>");
            display.setDirections(response);
        } else {
            $(store).find("td:last").find(".distanceLabel").remove();
            $(store).find("td:last").append("<span class='distanceLabel text-warning'>&nbsp;(invalid source address)</span>");
            // alert('Could not display directions due to: ' + status);
        }
    });
}

//show navigation
function showDirectionToStore(store) {
    getBrowserLocation(function (centerLocation) {
        var map = new google.maps.Map(document.getElementById('map'), {
            zoom: 5,
            center: centerLocation,
            minZoom: 3,
            mapTypeId: 'hybrid',
            animation: 'bounce'
        });
        var marker = new google.maps.Marker({
            position: centerLocation,
            animation: google.maps.Animation.BOUNCE,
            map: map
        });

        var directionsService = new google.maps.DirectionsService;
        addSingleStoreMarker(store, map, centerLocation, directionsService);
    });
}

//add selected store marker on map
function addSingleStoreMarker(store, map, centerLocation, directionsService) {
    document.getElementById('right-panel').innerHTML = "";
    $(store).parents(".storeLocationCoordinate").each(function (index, storeLocation) {
        var storeLocationCo = { lat: $(storeLocation).data("lat"), lng: $(storeLocation).data("lng") };

        var goldStar = {
            path: 'M 125,5 155,90 245,90 175,145 200,230 125,180 50,230 75,145 5,90 95,90 z',
            fillColor: '#cc0008',
            fillOpacity: 0.8,
            scale: 0.1,
            strokeColor: 'red',
            strokeWeight: 3
        };

        var marker = new google.maps.Marker({
            position: storeLocationCo,
            animation: google.maps.Animation.DROP,
            title: $(storeLocation).data("title"),
            icon: goldStar
        });

        addInfoWindow(marker, map, storeLocation);
        marker.setMap(map);
        calculateAndShowRoute(map, centerLocation, storeLocationCo, directionsService, storeLocation);
    });
}

//show route
function calculateAndShowRoute(map, centerLocation, storeLocationCo, directionsService, storeLocation) {
    var directionsDisplay = new google.maps.DirectionsRenderer({
        draggable: false,
        map: map,
        panel: document.getElementById('right-panel')
    });
    displayRoute(centerLocation, storeLocationCo, directionsService, directionsDisplay, storeLocation);
}
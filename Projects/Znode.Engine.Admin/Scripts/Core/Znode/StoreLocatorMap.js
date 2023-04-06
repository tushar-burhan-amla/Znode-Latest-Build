//Initialize Map
function initMap() {
    var lat = $("#Latitude").val();
    var lng = $("#Longitude").val();
    var browserLocation = new google.maps.LatLng({
        lat: parseFloat(lat),
        lng: parseFloat(lng)
    });
    setMap(browserLocation);
}

//Display map
function setMap(centerLocation) {
    if (!$("#map").html()) {
        map = new google.maps.Map(document.getElementById('map'), {
            zoom: 7,
            center: centerLocation,
            minZoom: 3,
            mapTypeId: 'hybrid',
            animation: 'bounce'
        });
    } else {
        map.setCenter(centerLocation);
    }
    if (typeof (marker) != typeof (undefined)) {
        marker.setMap(null);
        marker = null;
    }
    marker = new google.maps.Marker({
        position: centerLocation,
        animation: google.maps.Animation.BOUNCE,
        draggable: true,
        map: map
    });

    google.maps.event.addListener(marker, 'dragend', function (event) {
        $("#Latitude").val(event.latLng.lat());
        $("#Longitude").val(event.latLng.lng());
    });
}
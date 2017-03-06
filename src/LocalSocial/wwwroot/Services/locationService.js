var LocationService = function getLocation($window) {
    this.currentLocation = function() {
        $window.navigator.geolocation.getCurrentPosition(fn_ok);
    }
    function fn_ok(position) {
        this.longitude = position.coords.longitude;
    }
};
/*
$window.navigator.geolocation.getCurrentPosition(function(position) {
            var pos = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };
        });
*/
//if ($window.navigator.geolocation) {
        //    $window.navigator.geolocation.getCurrentPosition(function (position) {
        //        var pos = {
        //            lat: position.coords.latitude,
        //            lng: position.coords.longitude
        //        };
        //        $scope.$apply(function () {
        //            $scope.lat = pos.lat;
        //            $scope.lng = pos.lng;
        //        });
        //    });
        //} else {
        //    // Browser doesn't support Geolocation
        //    //handleLocationError(false, infoWindow, map.getCenter());
        //    console.log('cos nie dziala');
        //}
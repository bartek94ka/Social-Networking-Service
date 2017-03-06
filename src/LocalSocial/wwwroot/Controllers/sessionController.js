//2.
var SessionController = function ($scope, $cookieStore, $rootScope, $localStorage, $route, $window, SessionService) {
    
    //my declarations
    $scope.response = "";
    $scope.Email = "";
    $scope.Password = "";
    $scope.ConfirmPassword = "";
    $scope.EmailError = "";
    $scope.PasswordError = "";
    $scope.ConfirmPasswordError = "";

    //Function to logout user
    $scope.logout = function () {
        var promiselogoff = SessionService.logout();
        $localStorage.$reset();
        window.location.href = "#/login";
        $window.location.reload();
    };
    $scope.login = function () {
        var userLogin = {
            Email: $scope.Email,
            Password: $scope.Password
        };

        var promiselogin = SessionService.login(userLogin);

        promiselogin.then(function (resp) {

            $scope.Email = resp.data.Email;
            $localStorage.IsLogged = true;
            $window.location.reload();
            window.location.href = "#/myposts";
            $route.reload();
        }, function (err) {
            $scope.EmailError = "";
            $scope.PasswordError = "";
            $scope.EmailError = err.data.email;
            $scope.PasswordError = err.data.Password;
        });
        //window.location.href = "https://www.google.pl/";
    };
    //Function to register user
    $scope.registerUser = function () {

        $scope.response = "";

        //The User Registration Information
        var userRegistrationInfo = {
            Email: $scope.Email,
            Password: $scope.Password,
            ConfirmPassword: $scope.ConfirmPassword
        };

        var promiseregister = SessionService.register(userRegistrationInfo);

        promiseregister.then(function (resp) {

            $scope.Email = resp.data.Email;
            $localStorage.IsLogged = true;
            window.location.href = "#/myposts";
            $window.location.reload();
        }, function (err) {
            $scope.EmailError = "";
            $scope.PasswordError = "";
            $scope.ConfirmPasswordError = "";
            $scope.EmailError = err.data.email;
            $scope.PasswordError = err.data.Password;
            $scope.ConfirmPasswordError = err.data.ConfirmPassword;
        });
    };
};

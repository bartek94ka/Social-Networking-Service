//1.
var SessionService = function ($http) {

    this.register = function (userInfo) {
        var resp = $http({
            url: "/api/account/register",
            method: "POST",
            data: { Email: userInfo.Email, Password: userInfo.Password, ConfirmPassword: userInfo.ConfirmPassword },
            headers: { 'Content-Type': 'application/json' },
        });
        return resp;
    };

    this.login = function (userlogin) {

        var resp = $http({
            url: "/api/account/login",
            method: "POST",
            data: { Email: userlogin.Email, Password: userlogin.Password },
            headers: { 'Content-Type': 'application/json' },
        });
        return resp;
    };

    this.logout = function ($cookieStore) {
        var resp = $http({
            url: "/api/account/logoff",
            method: "POST",
        });
    }
};
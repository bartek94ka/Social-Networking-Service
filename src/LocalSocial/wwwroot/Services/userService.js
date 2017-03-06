var UserService = function ($http) {
    this.UpdateData = function (data) {
        var resp = $http({
            url: "/api/users/edit",
            method: "PUT",
            data: { Name: data.Name, Surname: data.Surname, SearchRange: data.SearchRange, Avatar: data.Avatar },
            //data: { Name: data.Name, Surname: data.Surname, OldPassword: data.OldPassword, NewPassword: data.NewPassword },
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    }
    this.GetData = function () {
        var resp = $http({
            url: "/api/users/get",
            method: "GET",
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    }
    this.GetUserById = function (data) {
        var resp = $http({
            url: "/api/users/getUser",
            method: "POST",
            data: { Id: data.UserId },
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    };
};
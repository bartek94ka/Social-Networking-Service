var FriendService = function($http) {
    this.GetMyFriends = function() {
        var resp = $http({
            url: "/api/userfriends/myfriends",
            method: "GET",
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    };
    this.FindFriend = function (data) {
        var resp = $http({
            url: "/api/userfriends/find",
            method: "POST",
            data: { Name: data.Name, Surname: data.Surname, Email: data.Email },
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    };
    this.AddFriend = function (data) {
        var resp = $http({
            url: "/api/userfriends/add",
            method: "POST",
            data: { Id: data.Id},
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    };
    this.RemoveFriend = function (data) {
        var resp = $http({
            url: "/api/userfriends/remove",
            method: "DELETE",
            data: { Id: data.Id },
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    };
    this.GetMyFriendsPosts = function() {
        var resp = $http({
            url: "/api/userfriends/posts",
            method: "GET",
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    };
};
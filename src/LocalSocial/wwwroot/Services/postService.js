var PostService = function ($http) {
    
    this.addComment = function (data) {
        var resp = $http({
            url: "/api/comments/add",
            method: "POST",
            data: { Content: data.Content, PostId: data.PostId },
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    };

    this.deletePost = function(id) {
        var resp = $http({
            url: "/api/posts/delete/" + id,
            method: "DELETE",
            headers: { 'Content-Type': 'application/json' }
        })
        return resp;
    };
    this.addPost = function (data) {
        var resp = $http({
            url: "/api/posts/add",
            method: "POST",
            data: { Title: data.Title, Description: data.Description, Latitude: data.Latitude, Longitude: data.Longitude, Tags: data.Tags },
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    };
    this.editPost = function (data, id) {
        var resp = $http({
            url: "/api/posts/edit/" + id,
            method: "PUT",
            data: { Title: data.Title, Description: data.Description, Latitude: data.Latitude, Longitude: data.Longitude },
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    };
    this.getPostsById = function (TagId) {
        var resp = $http({
            url: "/api/posts/tag",
            method: "POST",
            data: { TagId: TagId },
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    };
    this.getPost = function (id) {
        var resp = $http({
            url: "/api/posts/post/" + id,
            method: "GET",
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    };
    this.getMyPost = function() {
        var resp = $http({
            url: "/api/posts/myposts",
            method: "GET",
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    };
    this.getPostsFromRange = function (data) {
        console.log(data)
        var resp = $http({
            url: "/api/posts/inrange",
            method: "POST",
            data: { Longitude: data.Longitude, Latitude: data.Latitude },
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    };
    this.getAll = function () {
        var resp = $http.get('api/posts/all');
        return resp;
    };
}
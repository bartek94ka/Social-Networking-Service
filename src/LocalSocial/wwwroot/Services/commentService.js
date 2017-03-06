var CommentService = function ($http)
{
    this.AddComment = function (data) {
        var resp = $http({
            url: "/api/comments/add",
            method: "POST",
            data: { Content: data.Content, PostId: data.PostId},
            headers: { 'Content-Type': 'application/json' }
        });
        return resp;
    };
};
var PostController = function ($scope, $routeParams, $mdConstant, $location, PostService, UserService, CommentService) {
    // Use common key codes found in $mdConstant.KEY_CODE...
    this.keys = [$mdConstant.KEY_CODE.ENTER, $mdConstant.KEY_CODE.COMMA];

    $scope.Lat = null;
    $scope.Lng = null;
    $scope.post = {
        userName: "",
        userSurname: "",
        avatar: "",
        title: '',
        description: '',
        Id: '',
        Comments: [],
        Tags: [],
    };
    $scope.UserId = "";
    $scope.comment = {
        Content: '',
        PostId: ''
    }
    $scope.userPosts = [];
    $scope.addComment = function (Post) {
        var commentData = {
            Content: Post.comment,
            PostId: Post.Id
        };
        console.log(Post);
        console.log(commentData);
        var promiseComment = CommentService.AddComment(commentData);

        promiseComment.then(function (resp) {
            window.location.href = "#/posts/" + Post.Id;
        }, function (err) {
            console.log('blad w add comment');
        });
    };

    $scope.GetLocation = function() {
        if (navigator.geolocation) {

            $scope.myLocation = navigator.geolocation.getCurrentPosition(
                function (position) {
                    
                    $scope.Lat = position.coords.latitude;
                    $scope.Lng = position.coords.longitude;
                    $scope.$digest();
                }
            );
        }
    };
    $scope.GetPosts= function () {
        if (navigator.geolocation) {

            $scope.myLocation = navigator.geolocation.getCurrentPosition(
                function (position) {

                    $scope.Lat = position.coords.latitude;
                    $scope.Lng = position.coords.longitude;
                    $scope.GetPostFromRange();
                    $scope.$digest();
                }
            );
        }
    };
    $scope.GetPostByTag = function ()
    {
        console.log($routeParams)
        var searchObject = $location.search();
        console.log(searchObject);
        var promisePosts = PostService.getPostsById($routeParams.id);

        promisePosts.then(function (resp) {
            $scope.userPosts = resp.data;
        },
            function (err) {
                console.log('error w GetPostByTag');
            });
    };
    $scope.GetPostFromRange = function () {
        
        var locationData = {
            Longitude: $scope.Lng,
            Latitude: $scope.Lat
        };

        var promisePosts = PostService.getPostsFromRange(locationData);

        promisePosts.then(function(resp) {
            $scope.userPosts = resp.data;
            $scope.userPosts.forEach(function (item) {
                item.comment = "";
            });
            },
            function(err) {
                console.log('error w getpostfromrange');
            });
    };
    $scope.GetMyPosts = function () {
        
        var promiseMyPost = PostService.getMyPost();

        promiseMyPost.then(function (resp) {
            $scope.userPosts = resp.data;
        },function(err) {
                console.log('blad w getmyposts');
            }
        );

    };
    $scope.DeleteMyPost = function(Post) {
        var promiseDelete = PostService.deletePost(Post.Id);

        promiseDelete.then(function(resp) {
                window.location.href = "#/myposts";
            },
            function(err) {

            });
    };

    $scope.GetPost = function () {
        var promisePost = PostService.getPost($routeParams.postId);
        
        promisePost.then(function(resp) {
                $scope.post.title = resp.data.Title;
                $scope.post.description = resp.data.Description;
                $scope.post.Id = resp.data.Id;
                $scope.post.Comments = resp.data.Comments;
                $scope.post.userName = resp.data.user.Name;
                $scope.post.userSurname = resp.data.user.Surname;
                $scope.post.Tags = resp.data.PostTags;
                $scope.post.avatar = resp.data.user.Avatar;
            },
            function(err) {
            });
    };
    $scope.SavePost = function () {
        var postData = {
            Title: $scope.post.title,
            Description: $scope.post.description,
            Latitude: 1.3,
            Longitude: 1.3,
        };
        var promisePost = PostService.editPost(postData, $scope.post.Id);
        promisePost.then(function (resp) {
            window.location.href = "#/myposts";
        }, function (err) {
            //message z bledem
        });
    }
    $scope.AddPost = function() {
        var postData = {
            Title: $scope.post.title,
            Description: $scope.post.description,
            Latitude: $scope.Lat,
            Longitude: $scope.Lng,
            Tags: $scope.post.Tags,
        };

        var promisePost = PostService.addPost(postData);

        promisePost.then(function(resp) {
            window.location.href = "#/myposts";
        }, function(err) {
            //message z bledem
        });
    };
    $scope.AddComment = function () {
        var commentData = {
            Content: $scope.comment.Content,
            PostId: $scope.comment.PostId,
        }
        var promiseComment = PostService.addComment(commentData)

        promiseComment.then(function (resp) {
            window.location.href = "#/posts/"  + $scope.comment.PostId;
        }, function (err) {

        });
    };
};
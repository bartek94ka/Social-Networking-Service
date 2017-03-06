var FriendController = function ($scope, FriendService, CommentService) {

    $scope.users = [];

    $scope.userPosts = [];

    $scope.user = {
        Name: '',
        Surname: '',
        Email: '',
    };
    $scope.comment = "";

    $scope.findFriend = function() {
        var promise = FriendService.FindFriend($scope.user);
        promise.then(function(resp) {
                $scope.users = resp.data;
            },
            function(err) {
                console.log("blad w findFriend");
            });
    };

    $scope.addFriend = function (userItem) {
        var promise = FriendService.AddFriend(userItem);

        promise.then(function (succes) {
                $scope.users.splice($scope.users.findIndex(x => x.Email === userItem.Email), 1);
            },
            function(err) {

            });
    };
    $scope.removeFriend = function(userItem) {
        var promise = FriendService.RemoveFriend(userItem);

        promise.then(function (succes) {
            $scope.users.splice($scope.users.findIndex(x => x.Email === userItem.Email), 1);
        },
            function (err) {

            });
    };
    $scope.getMyFriends = function() {
        var promiseFriends = FriendService.GetMyFriends();

        promiseFriends.then(function(resp) {
                $scope.users = resp.data;
            },
            function(err) {
                console.log('error');
            });
    };
    $scope.getMyFriendsPosts = function() {
        var promisePosts = FriendService.GetMyFriendsPosts();

        promisePosts.then(function (resp) {
            $scope.userPosts = resp.data;
            $scope.userPosts.forEach(function(item){
                item.comment = "";
            });
            console.log($scope.userPosts);
        },function(err) {
            console.log('blad w getmyfriendsposts');
        });
        console.log($scope.userPosts);
    };
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
};
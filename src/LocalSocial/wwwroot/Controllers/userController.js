var UserController = function($scope, UserService) {
    $scope.Name = '';
    $scope.Surname = '';
    $scope.SearchRange;
    $scope.Email = '';
    $scope.OldPassword = '';
    $scope.NewPassword = '';
    $scope.ConfirmPassword = '';
    $scope.Message = '';
    $scope.Avatar = '';

    $scope.avatarData = [
        {
            id: "default",
            title: 'default',
            value: 'default'
        }, {
            id: "boy-1",
            title: 'boy-1',
            value: 'boy-1'
        }, {
            id: "girl-1",
            title: 'girl-1',
            value: 'girl-1'
        }, {
            id: "man-2",
            title: 'man-2',
            value: 'man-2'
        }, {
            id: "girl-8",
            title: 'girl-8',
            value: 'girl-8'
        }
    ];

    $scope.SaveChanges = function ()
    {
        var UserData = {
            Name: $scope.Name,
            Surname: $scope.Surname,
            SearchRange: $scope.SearchRange,
            Avatar: $scope.Avatar
        };

        var promiseSave = UserService.UpdateData(UserData);

        promiseSave.then(function(resp) {
            $scope.Message = "Zapisano";
                console.log(resp);
                window.location.href = "#/myposts";
            },
            function(err) {

            });
    };
    $scope.LoadData = function() {
        var promiseGet = UserService.GetData();

        promiseGet.then(function(resp) {
            console.log(resp);
                $scope.Name = resp.data.Name;
                $scope.Surname = resp.data.Surname;
                $scope.SearchRange = resp.data.SearchRange;
                $scope.Avatar = resp.data.Avatar;
                console.log($scope.Avatar);
            },
        function(err) {
            console.log('Blad w loaddata');
        });
    };
};
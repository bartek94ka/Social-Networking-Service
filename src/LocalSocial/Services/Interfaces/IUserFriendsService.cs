using LocalSocial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalSocial.Services.Interfaces
{
    public interface IUserFriendsService
    {
        IEnumerable<User> GetFriends(string userId);

        IEnumerable<User> FindFriends(string userId, UserBindingModel searchedUser);

        IEnumerable<Post> GetMyFriendsPosts(string userId);
    }
}

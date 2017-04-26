﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocalSocial.Models;
using LocalSocial.Services.Interfaces;

namespace LocalSocial.Services.EntityFrameworkServices
{
    public class UserFriendsService : IUserFriendsService
    {
        private readonly LocalSocialContext _context;

        public UserFriendsService()
        {
            _context = new LocalSocialContext();
        }

        public IEnumerable<User> FindFriends(string userId, UserBindingModel searchedUser)
        {
            var user = _context.User.FirstOrDefault(x => x.Id == userId);
            var userfriends = (from u in _context.UserFriends
                               where u.UserId == userId
                               select u.FriendId).ToList();
            userfriends.Add(userId);
            var friends = (from u in _context.User
                           where !userfriends.Contains(u.Id)
                           select u);

            friends = friends.Where(x => x.Name == searchedUser.Name || x.Surname == searchedUser.Surname || x.Email == searchedUser.Email);
            return friends.AsEnumerable();
        }

        public IEnumerable<User> GetFriends(string userId)
        {
            var friends = (from uf in _context.UserFriends
                           join us in _context.User on uf.FriendId equals us.Id
                           where uf.UserId == userId
                           select us).AsEnumerable();
            return friends;
        }

        public IEnumerable<Post> GetMyFriendsPosts(string userId)
        {
            throw new NotImplementedException();
        }
    }
}

using LocalSocial.Models;
using LocalSocial.Services.Interfaces;
using System.Linq;

namespace LocalSocial.Services.EntityFrameworkServices
{
    public class UsersService : IUsersService
    {
        private readonly LocalSocialContext _context;

        public UsersService()
        {
            _context = new LocalSocialContext();
        }

        public UserBindingModel GetMyUserData(string userId)
        {
            var user = _context.User.FirstOrDefault(x => x.Id == userId);
            if (user != null)
            {
                var userData = new UserBindingModel { Name = user.Name, Surname = user.Surname, SearchRange = user.SearchRange, Avatar = user.Avatar };
                return userData;
            }
            else
            {
                return null;
            }
        }

        public User GetUserData(UserBindingModel user)
        {
            var userData = _context.User.FirstOrDefault(x => x.Id == user.Id);
            return userData;
        }
    }
}
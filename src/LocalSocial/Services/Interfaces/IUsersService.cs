using LocalSocial.Models;

namespace LocalSocial.Services.Interfaces
{
    public interface IUsersService
    {
        User GetUserData(UserBindingModel user);

        UserBindingModel GetMyUserData(string userId);
    }
}
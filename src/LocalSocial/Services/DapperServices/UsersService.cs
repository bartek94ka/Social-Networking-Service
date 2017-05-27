using LocalSocial.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocalSocial.Models;
using System.Data.SqlClient;
using LocalSocial.Providers;
using Dapper;

namespace LocalSocial.Services.DapperServices
{
    public class UsersService : IUsersService
    {
        private readonly ConnectionProvider _connectionProvider;
        private readonly LocalSocialContext _context;

        public UsersService()
        {
            _context = new LocalSocialContext();
            _connectionProvider = new ConnectionProvider();
        }

        public UserBindingModel GetMyUserData(string userId)
        {
            using (var connection = new SqlConnection(_connectionProvider.GetConnectionString()))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                var query = @"SELECT users.[Name], users.[Surname], users.[SearchRange], users.[Avatar] 
                              FROM [dbo].[AspNetUsers] users WHERE users.[Id] = '" + userId + "'";
                var queryResult = connection.QueryAsync(query);
                var userData = queryResult.Result.FirstOrDefault();
                var user = new UserBindingModel
                {
                    Avatar = (string)userData.Avatar,
                    Name = (string)userData.Name,
                    Surname = (string)userData.Surname,
                    SearchRange = (float)userData.SearchRange
                };
                connection.Close();
                return user;
            }
        }

        public User GetUserData(UserBindingModel user)
        {
            var userData = _context.User.FirstOrDefault(x => x.Id == user.Id);
            return userData;
        }

    }
}
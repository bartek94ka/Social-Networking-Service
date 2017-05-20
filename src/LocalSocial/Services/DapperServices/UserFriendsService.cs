using LocalSocial.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocalSocial.Models;
using LocalSocial.Providers;
using System.Data.SqlClient;
using Dapper;

namespace LocalSocial.Services.DapperServices
{
    public class UserFriendsService : IUserFriendsService
    {
        private readonly ConnectionProvider _connectionProvider;

        public UserFriendsService()
        {
            _connectionProvider = new ConnectionProvider();
        }

        public IEnumerable<User> FindFriends(string userId, UserBindingModel searchedUser)
        {
            using (var connection = new SqlConnection(_connectionProvider.GetConnectionString()))
            {
                //otwarcie połączenia do bazy
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                var query = @"SELECT habababa.[Id], habababa.[Name], habababa.[Surname], habababa.[SearchRange], habababa.[Avatar] 
                                FROM [dbo].[AspNetUsers] habababa
                                WHERE habababa.[Name] LIKE '%" + searchedUser.Name + @"%'
                                AND habababa.[Surname] LIKE '%" + searchedUser.Surname + @"%'
                                AND habababa.[Email] LIKE '%" + searchedUser.Email + @"%'
                                
                                ";
                //stworzenie metody do wyciagania postow w oparciu o lokalizacje Location i zasięg wzięty z User
                var queryResult = connection.QueryAsync(query);
                var users = queryResult.Result.Select(user => new User
                {
                    Id = (string)user.Id,
                    Name = (string)user.Name,
                    Surname = (string)user.Surname,
                    SearchRange = (float)user.SearchRange,
                    Avatar = (string)user.Avatar
                });
                connection.Close();
                return users;
            }
        }

        public IEnumerable<User> GetFriends(string userId)
        {
            using (var connection = new SqlConnection(_connectionProvider.GetConnectionString()))
            {
                //otwarcie połączenia do bazy
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }

                var query = @"SELECT habababa.[Id], habababa.[Name], habababa.[Surname], habababa.[SearchRange], habababa.[Avatar] 
                                FROM [dbo].[AspNetUsers] habababa
                                FULL OUTER JOIN [dbo].[UserFriends] userf
                                ON habababa.[Id] = userf.[FriendId]
                                WHERE userf.[UserId] = '"+userId+"'";
                //stworzenie metody do wyciagania postow w oparciu o lokalizacje Location i zasięg wzięty z User
                var queryResult = connection.QueryAsync(query);
                var users = queryResult.Result.Select(user => new User
                {
                    Id = (string)user.Id,
                    Name = (string)user.Name,
                    Surname = (string)user.Surname,
                    SearchRange = (float)user.SearchRange,
                    Avatar = (string)user.Avatar
                });
                connection.Close();
                return users;
            }
        }

        public IEnumerable<Post> GetMyFriendsPosts(string userId)
        {
            using (var connection = new SqlConnection(_connectionProvider.GetConnectionString()))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                var query = @"SELECT posty.[Id], posty.[AddDate], posty.[Description], posty.[Latitude], posty.[Longitude], posty.[Title], posty.[_UserId]  
                                FROM [dbo].[AspNetUsers] habababa
                                FULL OUTER JOIN [dbo].[UserFriends] userf
                                ON habababa.[Id] = userf.[FriendId]
                                JOIN [dbo].[Post] posty
                                ON userf.[FriendId] = posty.[_UserId]
                                WHERE userf.[UserId] = '" + userId + "'";

                var queryResult = connection.QueryAsync(query);
                var posts = queryResult.Result.Select(post => new Post
                {
                    AddDate = (DateTime)post.AddDate,
                    Description = (string)post.Description,
                    Id = (int)post.Id,
                    Latitude = (float)post.Latitude,
                    Longitude = (float)post.Longitude,
                    Title = (string)post.Title,
                    _UserId = (string)post._UserId
                });
                connection.Close();
                return posts;
            }
        }
    }
}

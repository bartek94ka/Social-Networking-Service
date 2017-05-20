using Dapper;
using LocalSocial.Models;
using LocalSocial.Providers;
using LocalSocial.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;

namespace LocalSocial.Services.DapperServices
{
    public class PostService: IPostService
    {
        private readonly ConnectionProvider _connectionProvider;

        public PostService()
        {
            _connectionProvider = new ConnectionProvider();
        }

        public IEnumerable<Post> GetPostsInRange(User user, Location location)
        {
            using (var connection = new SqlConnection(_connectionProvider.GetConnectionString()))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                var query = @"SELECT [Id], [AddDate], [Description],[Latitude], [Longitude], [Title], [_UserId] 
                                FROM [dbo].[Post]";
                //stworzenie metody do wyciagania postow w oparciu o lokalizacje Location i zasięg wzięty z User
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

        public IEnumerable<Post> GetPostsByTag(TagBindingModel tag)
        {
            using (var connection = new SqlConnection(_connectionProvider.GetConnectionString()))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                var query = @"SELECT post.[Id],post.[AddDate],post.[Description],post.[Latitude],post.[Longitude],post.[Title],post.[_UserId],
		                        postTag.[TagId],
		                        users.[Id], users.[Email], users.[SearchRange], users.[Avatar]
                                FROM[dbo].[Post] post 
                                join[dbo].[PostTags] postTag 
                                on post.[Id] = postTag.[PostId] 
                                join [dbo].[AspNetUsers] users
                                on users.[Id] = post.[_UserId]
                                where postTag .[TagId] = '";
                var queryResult = connection.QueryAsync(query + tag.TagId + "'");
                var posts = queryResult.Result.Select(post => new Post
                {
                    AddDate = (DateTime)post.AddDate,
                    Id = (int)post.Id,
                    Description = (string)post.Description,
                    Latitude = (float)post.Latitude,
                    Longitude = (float)post.Longitude,
                    Title = (string)post.Title,
                    //na razie bez tagów
                    user = new User
                    {
                        Email = (string)post.Email,
                        Avatar = (string)post.Avatar,
                        SearchRange = (float)post.SearchRange
                    }
                });
                connection.Close();
                return posts;
            }
        }

        public Post GetPost(int Id)
        {
            using (var connection = new SqlConnection(_connectionProvider.GetConnectionString()))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                var query = @"SELECT post.[Id], post.[AddDate], post.[Description], post.[Latitude], post.[Longitude], post.[Title], post.[_UserId] 
                                FROM [dbo].[Post] post
                                where post.[Id] = '";
                var queryResult = connection.QueryAsync(query + Id + "'");

                var post = queryResult.Result.Select(p => new Post
                {
                    AddDate = (DateTime)p.AddDate,
                    Description = (string)p.Description,
                    Id = (int)p.Id,
                    Latitude = (float)p.Latitude,
                    Longitude = (float)p.Longitude,
                    Title = (string)p.Title,
                    _UserId = (string)p._UserId
                }).FirstOrDefault();
                connection.Close();
                return post;
            }
        }

        public IEnumerable<Post> GetAllPosts()
        {
            using (var connection = new SqlConnection(_connectionProvider.GetConnectionString()))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                var query = @"SELECT [Id], [AddDate], [Description],[Latitude], [Longitude], [Title], [_UserId] 
                                FROM [dbo].[Post]";
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

        public IEnumerable<Post> GetMyPosts(string userId)
        {
            using (var connection = new SqlConnection(_connectionProvider.GetConnectionString()))
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                var query = @"SELECT post.[Id], post.[AddDate], post.[Description], post.[Latitude], post.[Longitude], post.[Title], post.[_UserId] 
                                FROM [dbo].[Post] post
                                where post.[_UserId] = '";
                var queryResult = connection.QueryAsync(query + userId + "'");
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

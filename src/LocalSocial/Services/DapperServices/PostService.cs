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
            //user zawiera daną dotyczącą zasięgu w jakim będzie pobierał posty
            using (var connection = new SqlConnection(_connectionProvider.GetConnectionString()))
            {
                if(connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                }
                var posts = connection.Query<Post>(string.Format("SELECT [Id], [AddDate], [Description],[Latitude], [Longitude], [Title], [_UserId] FROM[dbo].[Post]"));
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
                var posts = connection.Query(query + tag.TagId + "'").Select(post => new Post
                {
                    AddDate = (DateTime) post.AddDate,
                    Id = (int) post.Id,
                    user = new User
                    {
                        Email = (string) post.Email
                    }
                });

                //for (int i = 0; i < posts.Count; i++)
                //{
                //    var user = connection.Query<User>(string.Format("SELECT [Email],[Name],[Surname],[Avatar] FROM [dbo].[AspNetUsers] users join [dbo].[Post] post on post._userId = users.[Id] and post.[Id] = {0}", posts[i].Id));
                //    posts[i].user = user.FirstOrDefault();
                //}
                connection.Close();
                return posts;
            }
        }

        public Post GetPost(int Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Post> GetAllPosts()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Post> GetMyPosts(string userId)
        {
            throw new NotImplementedException();
        }
    }
}

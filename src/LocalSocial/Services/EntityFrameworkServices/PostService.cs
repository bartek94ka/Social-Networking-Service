using Geolocation;
using LocalSocial.Models;
using LocalSocial.Services.Interfaces;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalSocial.Services.EntityFrameworkServices
{
    public class PostService : IPostService
    {
        private readonly LocalSocialContext _context;
 
        public PostService()
        {
            _context = new LocalSocialContext();
        }

        public IEnumerable<Post> GetPostsInRange(User user, Location location)
        {
            var posts = (from p in _context.Post.Include(p => p.PostTags)
                            let range = user.SearchRange / 1000
                            where
                            range >=
                            GeoCalculator.GetDistance(location.Latitude, location.Longitude, p.Latitude, p.Longitude, 5) / 1.6
                            orderby p.AddDate descending
                            select p
                                ).ToList();
            for (int i = 0; i < posts.Count; i++)
            {
                posts[i].user = _context.User.FirstOrDefault(x => x.Id == posts[i]._UserId);
            }
            return posts;
        }

        public IEnumerable<Post> GetPostsByTag(TagBindingModel tag)
        {
            var posts = (from p in _context.Post.Include(p => p.PostTags)
                            join pt in _context.PostTags on p.Id equals pt.PostId
                            where pt.TagId == tag.TagId
                            orderby p.AddDate descending
                            select p).ToList();
            for (int i = 0; i < posts.Count; i++)
            {
                var user = (from us in _context.User
                            where us.Id == posts[i]._UserId
                            select new User { Name = us.Name, Surname = us.Surname, Email = us.Email, Avatar = us.Avatar });
                posts[i].user = user.FirstOrDefault();
            }

            return posts;
        }

        public Post GetPost(int Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Post> GetPosts()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Post> GetMyPosts(string userId)
        {
            throw new NotImplementedException();
        }
    }
}

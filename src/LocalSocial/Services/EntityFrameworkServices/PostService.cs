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
            var post = _context.Post.Include(x => x.Comments).ThenInclude(p => p.User).Include(x => x.PostTags).First(x => x.Id == Id);
            var user = (from us in _context.User
                        where us.Id == post._UserId
                        select new User { Name = us.Name, Surname = us.Surname, Email = us.Email, Avatar = us.Avatar });
            if (user != null && post != null)
            {
                post.user = user.FirstOrDefault();
                return post;
            }
            return null;
        }

        public IEnumerable<Post> GetAllPosts()
        {
            var posts = _context.Post.AsEnumerable();
            return posts;
        }

        public IEnumerable<Post> GetMyPosts(string userId)
        {
            var posts = _context.Post.Include(x => x.PostTags).Where(x => x._UserId == userId).OrderByDescending(p => p.AddDate).ToList();
            for (int i = 0; i < posts.Count; i++)
            {
                posts[i].user = _context.User.FirstOrDefault(x => x.Id == posts[i]._UserId);
            }
            return posts;
        }
    }
}

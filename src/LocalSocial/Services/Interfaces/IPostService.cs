using LocalSocial.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalSocial.Services.Interfaces
{
    public interface IPostService
    {
        IEnumerable<Post> GetPostsInRange(User user, Location location);

        IEnumerable<Post> GetPostsByTag(TagBindingModel tag);

        Post GetPost(int Id);

        IEnumerable<Post> GetAllPosts();

        IEnumerable<Post> GetMyPosts(string userId);
        
    }
}
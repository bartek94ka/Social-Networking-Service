using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

namespace LocalSocial.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            this.Posts = new HashSet<Post>();
            this.Friends = new HashSet<UserFriends>();
            //this.Friends = new HashSet<User>();
            //this.Users = new HashSet<User>();

        }
        public string Name { get; set; }
        public string Surname { get; set; }
        public float SearchRange { get; set; }

        public virtual ICollection<Post> Posts { get; private set; }
        public virtual ICollection<UserFriends> Friends { get; private set; }
        
        public string Avatar { get; set; }

    }
    public class UserBindingModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public float SearchRange { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public string Avatar { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LocalSocial.Models
{
    public class UserFriends
    {
        [ForeignKey("User")]
        public string UserId { get; set; }
        [ForeignKey("User2")]
        public string FriendId { get; set; }
    }
}

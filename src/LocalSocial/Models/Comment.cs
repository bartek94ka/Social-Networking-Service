using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LocalSocial.Models
{
    public class Comment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
    public class CommentBindingModel
    {
        [Required]
        public string Content { get; set; }
        public int PostId { get; set; }
    }
}

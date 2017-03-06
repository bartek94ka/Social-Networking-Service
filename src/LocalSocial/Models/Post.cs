using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LocalSocial.Models
{
    public class Post
    {
        public Post()
        {
            this.Comments = new HashSet<Comment>();
            this.PostTags = new HashSet<PostTags>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AddDate { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        [ForeignKey("User")]
        public string _UserId { get; set; }
        public virtual User user { get; set; }
        public virtual ICollection<Comment> Comments { get; private set; }
        public virtual ICollection<PostTags> PostTags { get; private set; }
    }
    public class PostBindingModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public float Latitude { get; set; }
        [Required]
        public float Longitude { get; set; }
        [Required]
        public string[] Tags { get; set; }
    }
}

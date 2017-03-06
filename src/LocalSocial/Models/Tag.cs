using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace LocalSocial.Models
{
    public class Tag
    {
        public Tag()
        {
            this.PostTags = new HashSet<PostTags>();
        }
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public string Id { get; set; }
        public virtual ICollection<PostTags> PostTags { get; private set; }
    }
    public class TagBindingModel
    {
        [Required]
        public string TagId { get; set; }
    }
}

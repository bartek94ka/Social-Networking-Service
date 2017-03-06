using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LocalSocial.Models
{
    public class PostTags
    {
        [ForeignKey("Post")]
        public int PostId { get; set; }
        [ForeignKey("Tag")]
        public string TagId { get; set; }
    }
}

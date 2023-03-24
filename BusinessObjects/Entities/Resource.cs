using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Entities
{
    public class Resource
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResourceId { get; set; }
        [Required]
        [StringLength(40)]
        public string ResourceName { get; set; } = null!;
        public DateTime CreateDate { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        [NotMapped]
        public Course Course { get; set; }
        public virtual ICollection<File> Files { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Entities
{
    public class Course
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CourseId { get; set; }
        [Required]
        [StringLength(40)]
        public string CourseName { get; set; }

        [ForeignKey("ApplicationUser")]
        public string TeacherId { get; set; }
        [NotMapped]
        public ApplicationUser Teacher { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ICollection<ApplicationUser> Students { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }


    }
}

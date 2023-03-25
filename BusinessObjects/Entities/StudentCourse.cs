using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Entities
{
    public class StudentCourse
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentCourseId { get; set; }
        //public int StudentId { get; set; }
        public virtual ApplicationUser Student { get; set; }

        //public int CourseId { get; set; }
        public virtual Course Course { get; set; }

        public DateTime CreateDate { get; set; }
    }
}

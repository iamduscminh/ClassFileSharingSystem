using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public virtual ICollection<StudentCourse> CourseOwners { get; set; }
        public string? StudentNumber { get; set; }
    }
}

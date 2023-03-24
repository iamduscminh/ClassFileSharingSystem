using BusinessObjects.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Server.Dto
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string TeacherId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

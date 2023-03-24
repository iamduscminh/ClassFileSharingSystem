namespace Client.Models
{
    public class CourseModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string TeacherEmail { get; set; }
        public DateTime CreateDate { get; set; }

    }
}

namespace Server.Dto
{
    public class ResourceDto
    {
        public int ResourceId { get; set; }
        public string ResourceName { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public int CourseId { get; set; }
      
    }
}

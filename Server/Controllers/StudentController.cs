using AutoMapper;
using BusinessObjects;
using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentController : Controller
    {
        private readonly ClassFileSharingContext _context;
        private readonly IMapper _mapper;
        public StudentController(IMapper mapper, ClassFileSharingContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            var students = (from stu in _context.ApplicationUsers
                            join userRole in _context.UserRoles on stu.Id equals userRole.UserId
                            join role in _context.Roles on userRole.RoleId equals role.Id
                            where role.Name == "Student"
                            select new ApplicationUser
                            {
                                Id = stu.Id,
                                UserName = stu.UserName,
                                Email = stu.Email,
                                StudentNumber = stu.StudentNumber,
                                FullName = stu.FullName,
                            }).ToList();
            return Ok(students);
        }
        [Route("{strSearch}")]
        [HttpGet]
        public IActionResult GetStudentsSearch(string strSearch)
        {
            var students = (from stu in _context.ApplicationUsers
                            join userRole in _context.UserRoles on stu.Id equals userRole.UserId
                            join role in _context.Roles on userRole.RoleId equals role.Id
                            where role.Name == "Student"
                            &&(strSearch == "empty" || stu.Email.ToLower().Contains(strSearch.ToLower()))
                            select new ApplicationUser
                            {
                                Id = stu.Id,
                                UserName = stu.UserName,
                                Email = stu.Email,
                                StudentNumber = stu.StudentNumber,
                                FullName = stu.FullName,
                            }).ToList();
            return Ok(students);
        }

        [Route("{strSearch}/{courseId}")]
        [HttpGet]
        public IActionResult GetStudentsSearchInCourse(string strSearch, int courseId)
        {
            var students = (from stu in _context.ApplicationUsers                          
                            join userRole in _context.UserRoles on stu.Id equals userRole.UserId
                            join role in _context.Roles on userRole.RoleId equals role.Id
                            where role.Name == "Student"
                            && (strSearch == "empty" || stu.Email.ToLower().Contains(strSearch.ToLower()))
                            select new ApplicationUser
                            {
                                Id = stu.Id,
                                UserName = stu.UserName,
                                Email = stu.Email,
                                StudentNumber = stu.StudentNumber,
                                FullName = stu.FullName,
                            }).ToList();
            var studenIdInCourse = (from stuC in _context.StudentCourses
                                    where stuC.Course.CourseId == courseId
                                    select new ApplicationUser
                                    {
                                        Id = stuC.Student.Id
                                    }).ToList();
            students = students.Where(x => studenIdInCourse.Select(c => c.Id).Contains(x.Id)).ToList();

            return Ok(students);
        }


        [Route("{id}")]
        [HttpGet]
        public IActionResult GetStudentsByCourse(int id)
        {
            var listStudentId = _context.StudentCourses.Where(x => x.Course.CourseId == id).Select(s => s.Student.Id);
            var students = (from stu in _context.ApplicationUsers
                            join userRole in _context.UserRoles on stu.Id equals userRole.UserId
                            join role in _context.Roles on userRole.RoleId equals role.Id
                            where role.Name == "Student"
                            && listStudentId.Contains(stu.Id)
                            select new ApplicationUser
                            {
                                Id = stu.Id,
                                UserName = stu.UserName,
                                Email = stu.Email,
                                StudentNumber = stu.StudentNumber,
                                FullName = stu.FullName,
                            }).ToList();
            return Ok(students);
        }
        [Route("{studentId}/{courseId}")]
        [HttpPost]
        public IActionResult AddStudentCourse(string studentId, string courseId)
        {
            var student = _context.ApplicationUsers.FirstOrDefault(x => x.Id == studentId);
            var courId = Convert.ToInt32(courseId);
            var course = _context.Courses.FirstOrDefault(x => x.CourseId == courId);
            var stdCourseCheck = _context.StudentCourses.Where(x => x.Course.CourseId == courId && x.Student.Id == studentId).FirstOrDefault();
            if (student == null || course ==null) return NotFound();
            if(stdCourseCheck != null) return BadRequest(); 


            var studentCourse = new StudentCourse()
            {
                Student = student,
                Course = course,
                CreateDate = DateTime.Now,
            };


            _context.StudentCourses.Add(studentCourse);
            _context.SaveChanges();
            return Ok("Successfully created");

        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(string id)
        {
            var student = _context.ApplicationUsers.Where(x => x.Id == id).FirstOrDefault();
            if(student==null) return NotFound();
            var studentCourse = _context.StudentCourses.Where(x => x.Student.Id == id).ToList();
            studentCourse.ForEach(x => _context.Remove(x));

            _context.ApplicationUsers.Remove(student);
            _context.SaveChanges();
            return NoContent();
        }
        [Route("{studentId}/{courseId}")]
        [HttpDelete]
        public IActionResult RemoveStudent(string studentId, int courseId)
        {
            var student = _context.ApplicationUsers.FirstOrDefault(x => x.Id == studentId);
            var courId = Convert.ToInt32(courseId);
            var course = _context.Courses.FirstOrDefault(x => x.CourseId == courId);
            var stdCourse = _context.StudentCourses.Where(x => x.Course.CourseId == courId && x.Student.Id == studentId).FirstOrDefault();
            if (student == null || course == null) return NotFound();
            if (stdCourse == null) return BadRequest();


            _context.StudentCourses.Remove(stdCourse);
            _context.SaveChanges();
            return NoContent();
        }
    }
}

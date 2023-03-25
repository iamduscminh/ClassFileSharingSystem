using AutoMapper;
using BusinessObjects;
using BusinessObjects.Entities;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Dto;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : Controller
    {
        private readonly ClassFileSharingContext _context;
        private readonly IMapper _mapper;
        public CourseController(IMapper mapper, ClassFileSharingContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCourses()
        {
            var courses = _context.Courses
                .Include(c => c.Resources)
                .Include(s => s.Students)
                .ToList();
            return Ok(courses);
        }
        [HttpGet("{id}")]
        public IActionResult GetCourseDetail(int id)
        {
            var course = _context.Courses.Where(x => x.CourseId == id).Include(c => c.Resources).Include(s => s.Students).FirstOrDefault();
            return Ok(course);
        }
        [Route("{userId}/{role}")]
        [HttpGet]
        public IActionResult GetCoursesByRole(string userId, string role)
        {
            List<Course> courses = new();
            if (role == "Teacher")
            {
                courses = (from c in _context.Courses.Include(x=>x.Students)
                           join t in _context.ApplicationUsers on c.TeacherId equals t.Id
                           where c.TeacherId == userId
                           select new Course
                           {
                               CourseId = c.CourseId,
                               CourseName = c.CourseName,
                               CourseCode = c.CourseCode,
                               CreateDate = c.CreateDate,
                               Teacher = t,
                               Students = c.Students
                           }).ToList();
            }
            else if (role == "Student")
            {
                var listCourseId = _context.StudentCourses.Where(x => x.Student.Id == userId).Select(c => c.Course.CourseId).ToList();
                //courses = _context.Courses.Where(x=> listCourseId.Contains(x.CourseId)).ToList();
                courses = (from c in _context.Courses
                           join t in _context.ApplicationUsers on c.TeacherId equals t.Id
                           where listCourseId.Contains(c.CourseId)
                           select new Course
                           {
                               CourseId = c.CourseId,
                               CourseName = c.CourseName,
                               CourseCode = c.CourseCode,
                               CreateDate = c.CreateDate,
                               Teacher = t
                           }).ToList();
            }

            return Ok(courses);
        }

        [HttpPost]
        public IActionResult CreateCourse([FromBody] CourseDto c)
        {
            var CourseMap = _mapper.Map<Course>(c);
            _context.Courses.Add(CourseMap);
            _context.SaveChanges();
            return Ok("Successfully created");

        }

        [HttpPut("{id}")]
        public IActionResult EditCourse(int id, [FromBody] CourseDto c)
        {
            var course = _context.Courses.FirstOrDefault(x => x.CourseId == c.CourseId);
            if (course == null) return NotFound();
            course.CourseName = c.CourseName;
            course.CourseCode = c.CourseCode;

            _context.SaveChanges();
            return Ok("Successfully updated");

        }
    }
}

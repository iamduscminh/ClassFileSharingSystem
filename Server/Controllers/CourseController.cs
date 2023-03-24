using AutoMapper;
using BusinessObjects;
using BusinessObjects.Entities;
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
        IMapper _mapper;
        public CourseController(IMapper mapper, ClassFileSharingContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCourses()
        {
            var courses = _context.Courses.ToList();
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
    }
}

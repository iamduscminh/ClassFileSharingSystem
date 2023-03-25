using AutoMapper;
using BusinessObjects;
using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [Route("api/[controller]")]
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
    }
}

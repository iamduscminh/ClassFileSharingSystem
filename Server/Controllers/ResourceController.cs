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
    public class ResourceController : Controller
    {
        private readonly ClassFileSharingContext _context;
        private readonly IMapper _mapper;
        public ResourceController(IMapper mapper, ClassFileSharingContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetResourceDetail(int id)
        {
            var course = _context.Resourses.Where(x => x.ResourceId == id).Include(c => c.Files).FirstOrDefault();
            return Ok(course);
        }

        [HttpPost]
        public IActionResult CreateResource([FromBody] ResourceDto r)
        {
            var ResourceMap = _mapper.Map<Resource>(r);
            _context.Resourses.Add(ResourceMap);
            _context.SaveChanges();
            return Ok("Successfully created");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteResource(int id)
        {
            var rs = _context.Resourses.FirstOrDefault(x => x.ResourceId == id);
            if (rs == null) return NotFound();

            _context.Resourses.Remove(rs);
            _context.SaveChanges();
            return Ok("Successfully deleted");
        }

        [HttpPut("{id}")]
        public IActionResult EditResource(int id, [FromBody] ResourceDto r)
        {
            var re = _context.Resourses.FirstOrDefault(x => x.ResourceId == r.ResourceId);
            if (re == null) return NotFound();
            re.ResourceName = r.ResourceName;

            _context.SaveChanges();
            return Ok("Successfully updated");

        }
    }
}

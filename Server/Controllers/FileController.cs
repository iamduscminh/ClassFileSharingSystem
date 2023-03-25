using AutoMapper;
using BusinessObjects;
using BusinessObjects.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private readonly ClassFileSharingContext _context;
        public FileController(IMapper mapper, ClassFileSharingContext context)
        {
            _context = context;
        }
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> InsertFile(BusinessObjects.Entities.File file)
        {
            try
            {
                _context.Files.Add(file);
                await _context.SaveChangesAsync();
                return Ok(file);
            }
            catch (Exception e)
            {
                return new ObjectResult("Upload Failed" + e) { StatusCode = 500 };
            }
        }
        [HttpDelete("{cloudId}")]
        public async Task<IActionResult> DeleteFile(string cloudId)
        {
            try
            {
                var existedFile = _context.Files.FirstOrDefault(e => e.CloudId == cloudId);
                _context.Files.Remove(existedFile);
                _context.SaveChanges();
                return Ok(existedFile);
            }
            catch (Exception ex)
            {
                return new ObjectResult("Upload Failed" + ex) { StatusCode = 500 };
            }
        }
    }
}

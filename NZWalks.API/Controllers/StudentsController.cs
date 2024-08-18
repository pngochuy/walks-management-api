using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NZWalks.API.Controllers
{
    // https://localhost:port_number/api/controller_name
    // https://localhost:port_number/api/Students
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        // GET: https://localhost:port_number/api/Students
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            string[] studentNames = new string[] { "Huy", "Uyên", "Phạm", "Ngọc", "Phan", "THảo" };

            return Ok(studentNames);
        }

    }
}

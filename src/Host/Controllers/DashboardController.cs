using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Host.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;
        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        [HttpGet()]
        [Authorize]
        public async Task<IActionResult> GastoPendienteArea()
        {
            var origin = Request.Headers["origin"];
            return Ok("test");
        }


    }
}

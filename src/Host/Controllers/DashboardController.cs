using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using ApplicationCore.Wrappers;
using ApplicationCore.Commands;
using ApplicationCore.DTOs;

namespace Host.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;
        private readonly IMediator _mediator;
        public DashboardController(IDashboardService service, IMediator mediator)
        {
            _service = service;
            _mediator = mediator;
        }

        [HttpGet()]
        [Authorize]
        public async Task<IActionResult> GastoPendienteArea()
        {
            var origin = Request.Headers["origin"];
            return Ok("test");
        }

        //<summary>
        //Get de todos los elementos
        //</summary>

        [Route("getData")]
        [HttpGet]
        public async Task<IActionResult> GetUsuario()
        {
            var result = await _service.GetData();
            return Ok(result);
        }

        //<summary>
        //Create de Personas
        //</summary>
        [HttpPost("create")]
        public async Task<ActionResult<Response<int>>> Create(CreatePersonaCommand request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [Route("getIP")]
        [HttpGet]
        public async Task<IActionResult> GetIpAddress()
        {
            var result = await _service.GetClientIpAddress();
            return Ok(result);
        }


        //<summary>
        //Create de Logs
        //</summary>
        [HttpPost("logs")]
        public async Task<ActionResult<Response<int>>> Create([FromBody] LogDto request)
        {
            var result = await _service.CreateLog(request);
            return Ok(result);
        }



    }
}

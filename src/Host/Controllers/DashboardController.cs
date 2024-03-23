using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using ApplicationCore.Wrappers;
using ApplicationCore.Commands;
using ApplicationCore.DTOs;
using Microsoft.EntityFrameworkCore;

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
        //<return></return>
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
        //<return></return>
        [HttpPost("create")]
        public async Task<ActionResult<Response<int>>> Create([FromBody] PersonaDto request)
        {
            var result = await _service.CreatePersona(request);
            //var result = await _mediator.Send(request);
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
        //<return></return>
        [HttpPost("logs")]
        public async Task<ActionResult<Response<int>>> Create([FromBody] LogDto request)
        {
            var result = await _service.CreateLog(request);
            return Ok(result);
        }


        //<summary>
        //Update de Persona
        //</summary>
        //<return></return>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PersonaDto request)
        {
            var result = await _service.UpdatePersona(id, request);
            return Ok(result);
        }



        //<summary>
        //Delete de Persona
        //</summary>
        //<return></return>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeletePersona(id);
            return Ok(result);
        }

        //<summary>
        //Get de todos los elementos
        //</summary>
        //<return></return>
        [Route("getDataPagination")]
        [HttpGet]
        public async Task<IActionResult> GetPagination()
        {
            var result = await _service.GetDataPagination();
            return Ok(result);
        }

    }
}

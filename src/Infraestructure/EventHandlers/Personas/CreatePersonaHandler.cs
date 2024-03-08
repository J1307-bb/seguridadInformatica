using ApplicationCore.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ApplicationCore.Commands;
using Infraestructure.Persistence;
using ApplicationCore.Interfaces;
using ApplicationCore.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.EventHandlers.Personas
{
    public class CreatePersonaHandler : IRequestHandler<CreatePersonaCommand, Response<int>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDashboardService _dashboardService;

        public CreatePersonaHandler(ApplicationDbContext context, IMapper mapper, IDashboardService dashboardService)
        {
            _context = context;
            _mapper = mapper;
            _dashboardService = dashboardService;
        }

        public async Task<Response<int>> Handle(CreatePersonaCommand request, CancellationToken cancellationToken)
        {

            var p = new CreatePersonaCommand();
            p.Nombre = request.Nombre;
            p.Ciudad = request.Ciudad;
            p.ComidaFav = request.ComidaFav;
            p.ColorFav = request.ColorFav;
            p.CancionFav = request.CancionFav;

            var pe = _mapper.Map<Domain.Entities.persona>(p);

            await _context.persona.AddAsync(pe);

            var req = await _context.SaveChangesAsync();
            var dd = req;

            var res = new Response<int>(pe.PkPersona, "Registro creado");

            var log = new LogDto();
            log.Datos = "Datos";
            log.fecha = DateTime.Now.ToString();
            log.NomFuncion = "Create";
            log.mensaje = res.Message;
            log.StatusLog = "200";


            if (req == 1)
            {

                await _dashboardService.CreateLog(log);
                return res;

            } else
            {

                await _dashboardService.CreateLog(log);
                return res;

            }






            //try {



            //}

            //catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 400)
            //{
            //    // Si hay un error de validación en la base de datos (por ejemplo, clave duplicada),
            //    // puedes manejarlo aquí
            //    var errorMessage = "Error de validación en la base de datos: " + ex.Message;

            //    var log = new LogDto();
            //    log.Datos = "Datos";
            //    log.fecha = DateTime.Now.ToString();
            //    log.NomFuncion = "Create";
            //    log.mensaje = errorMessage;
            //    log.StatusLog = "400";

            //    await _dashboardService.CreateLog(log);

            //    return new Response<int>("Error de validación en la base de datos: " + errorMessage);
            //}

            //catch (Exception ex) {

            //    var mensajeError = ex.Message;

            //    var log = new LogDto();
            //    log.Datos = "Datos";
            //    log.fecha = DateTime.Now.ToString();
            //    log.NomFuncion = "Create";
            //    log.mensaje = mensajeError;
            //    log.StatusLog = "500";

            //    await _dashboardService.CreateLog(log);

            //    return new Response<int>("Error al crear persona: " + mensajeError); ;

            //}


        }
    }
}

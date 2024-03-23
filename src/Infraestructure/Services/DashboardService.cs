using ApplicationCore.DTOs;
using ApplicationCore.Interfaces;
using ApplicationCore.Wrappers;
using AutoMapper;
using Dapper;
using Infraestructure.Persistence;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Net.Sockets;
using System.Net;
using ApplicationCore.Commands;
using Domain.Entities;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using MediatR;

namespace Infraestructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardService(ApplicationDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<object>> GetData()
        {
            object list = new object();
            list = await _dbContext.persona.ToListAsync();

            return new Response<object>(list);
        }

        public async Task<Response<string>> GetClientIpAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            IPAddress ipAddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            var ipAddressString = ipAddress?.ToString() ?? "No se pudo determinar la direccion";

            return new Response<string>(ipAddressString);

        }

        public async Task<Response<int>> CreatePersona(PersonaDto request)
        {
            try
            {
                var p = new CreatePersonaCommand();
                p.Nombre = request.Nombre;
                p.Ciudad = request.Ciudad;
                p.ComidaFav = request.ComidaFav;
                p.ColorFav = request.ColorFav;
                p.CancionFav = request.CancionFav;

                var pe = _mapper.Map<Domain.Entities.persona>(p);
                await _dbContext.persona.AddAsync(pe);
                var req = await _dbContext.SaveChangesAsync();
                var res = new Response<int>(pe.PkPersona, "Registro creado");

                //Logs
                var log = new LogDto();
                log.Datos = JsonConvert.SerializeObject(request);
                log.fecha = DateTime.Now.ToString();
                log.NomFuncion = "Create";
                log.mensaje = res.Message;
                log.StatusLog = "200";

                await CreateLog(log);
                return res;

            }
            catch (Exception ex)
            {
                // Manejar otras excepciones
                var errorLog = new LogDto();
                errorLog.Datos = JsonConvert.SerializeObject(request); ;
                errorLog.fecha = DateTime.Now.ToString();
                errorLog.NomFuncion = "Create";

                if (ex.InnerException != null)
                {
                    errorLog.mensaje = $"Error desconocido al crear el registro. Mensaje interno: {ex.InnerException.Message}";
                }
                else
                {
                    errorLog.mensaje = "Error desconocido al crear el registro";
                }

                errorLog.StatusLog = "500";

                await CreateLog(errorLog);
                throw;

            }
        }

        public async Task<Response<int>> CreateLog(LogDto request)
        {

            var ipAddress = await GetClientIpAddress();
            var ip = ipAddress.Message;

            var l = new LogDto();
            l.fecha = request.fecha;
            l.mensaje = request.mensaje;
            l.ipAddress = ip;
            l.NomFuncion = request.NomFuncion;
            l.StatusLog = request.StatusLog;
            l.Datos = request.Datos;


            var lo = _mapper.Map<Domain.Entities.logs>(l);
            await _dbContext.logs.AddAsync(lo);
            await _dbContext.SaveChangesAsync();
            return new Response<int>(lo.idLog, "Registro creado");

        }

        public async Task<Response<int>> UpdatePersona(int id, PersonaDto request)
        {
            try
            {
                var persona = await _dbContext.persona.FindAsync(id);

                persona.Nombre = request.Nombre;
                persona.Ciudad = request.Ciudad;
                persona.ComidaFav = request.ComidaFav;
                persona.ColorFav = request.ColorFav;
                persona.CancionFav = request.CancionFav;

                await _dbContext.SaveChangesAsync();
                var res = new Response<int>(id,"Persona actualizada");

                //Logs
                var log = new LogDto();
                log.Datos = JsonConvert.SerializeObject(request);
                log.fecha = DateTime.Now.ToString();
                log.NomFuncion = "Update";
                log.mensaje = res.Message;
                log.StatusLog = "200";

                await CreateLog(log);

                return res; 
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones
                var errorLog = new LogDto();
                errorLog.Datos = JsonConvert.SerializeObject(request); ;
                errorLog.fecha = DateTime.Now.ToString();
                errorLog.NomFuncion = "Update";

                if (ex.InnerException != null)
                {
                    errorLog.mensaje = $"Error desconocido al Actualizar la persona. Mensaje interno: {ex.InnerException.Message}";
                }
                else
                {
                    errorLog.mensaje = "Error desconocido al Actualizar la persona:" + ex.Message;
                }

                errorLog.StatusLog = "500";

                await CreateLog(errorLog);
                throw;
            }
        }

        public async Task<Response<int>> DeletePersona(int id)
        {
            try
            {
                var persona = await _dbContext.persona.FindAsync(id);

                _dbContext.persona.Remove(persona);
                await _dbContext.SaveChangesAsync();
                var res = new Response<int>(id, "Persona eliminada");

                //Logs
                var log = new LogDto();
                log.Datos = "ID: " + id.ToString();
                log.fecha = DateTime.Now.ToString();
                log.NomFuncion = "Delete";
                log.mensaje = res.Message;
                log.StatusLog = "200";

                await CreateLog(log);

                return res;
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones
                var errorLog = new LogDto();
                errorLog.Datos = $"ID: {id}";
                errorLog.fecha = DateTime.Now.ToString();
                errorLog.NomFuncion = "Delete";

                if (ex.InnerException != null)
                {
                    errorLog.mensaje = $"Error desconocido al Eliminar la persona. Mensaje interno: {ex.InnerException.Message}";
                }
                else
                {
                    errorLog.mensaje = "Error desconocido al Eliminar la persona:" + ex.Message;
                }

                errorLog.StatusLog = "500";

                await CreateLog(errorLog);
                throw;
            }
        }

        public async Task<Response<object>> GetDataPagination()
        {
            object list = new object();

            int pageNumber = 1;
            int pageSize = 10;

            list = await _dbContext.persona.OrderBy(x => x.PkPersona)
                .Skip((pageNumber -1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new Response<object>(list);
        }

    }
}

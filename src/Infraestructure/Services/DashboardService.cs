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

    }
}

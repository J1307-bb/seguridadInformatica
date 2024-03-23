using ApplicationCore.DTOs;
using ApplicationCore.Wrappers;

namespace ApplicationCore.Interfaces
{
    public interface IDashboardService
    {
        Task<Response<object>> GetData();
        //Task<Response<int>> CreateUserAsync(PersonaDto request);
        Task<Response<string>> GetClientIpAddress();
        Task<Response<int>> CreateLog(LogDto logsDto);
        Task<Response<int>> CreatePersona(PersonaDto personaDto);
        Task<Response<int>> UpdatePersona(int idPersona, PersonaDto personaDto);
        Task<Response<int>> DeletePersona(int idPersona);

        Task<Response<object>> GetDataPagination();

    }
}

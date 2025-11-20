using MembershipMicroservice.MembershipMicroserviceApplication.Interfaces;
using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using MembershipMicroservice.MembershipMicroserviceDomain.Shared;
using System.Net.Http;
using System.Net.Http.Json;

namespace MembershipMicroservice.MembershipMicroserviceInfrastructure.Persistence
{
    public class DisciplineServiceClient : IDisciplineServiceClient
    {
        private readonly HttpClient _httpClient;

        public DisciplineServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Result<IEnumerable<Discipline>>> GetAllDisciplines()
        {
            try
            {
                var disciplines = await _httpClient.GetFromJsonAsync<IEnumerable<Discipline>>("api/disciplines");
                return Result<IEnumerable<Discipline>>.Success(disciplines ?? new List<Discipline>());
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<Discipline>>.Failure($"Error al obtener todas las disciplinas: {ex.Message}");
            }
        }

        public async Task<Result<Discipline>> GetDisciplineById(int id)
        {
            try
            {
                var discipline = await _httpClient.GetFromJsonAsync<Discipline>($"api/disciplines/{id}");
                if (discipline == null)
                    return Result<Discipline>.Failure($"No se encontró la disciplina con ID {id}");

                return Result<Discipline>.Success(discipline);
            }
            catch (Exception ex)
            {
                return Result<Discipline>.Failure($"Error al obtener disciplina {id}: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<Discipline>>> GetDisciplinesByIdsAsync(IEnumerable<int> ids)
        {
            if (ids == null || !ids.Any())
                return Result<IEnumerable<Discipline>>.Success(new List<Discipline>());

            var disciplines = ids.Select(id => new Discipline
            {
                Id = id,
                Name = $"Disciplina {id}",
                IsActive = true
            });

            return Result<IEnumerable<Discipline>>.Success(disciplines);
        }
    }
}

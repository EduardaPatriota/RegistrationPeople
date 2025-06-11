using RegistrationPeople.Application.DTOs;
using RegistrationPeople.Application.DTOs.V2;
using RegistrationPeople.Domain.Entities;

namespace RegistrationPeople.Application.Interfaces
{
    public interface IPersonService
    { 
        // v1
        Task<Person> CreateAsync(RegisterPersonDto person);
        Task<IEnumerable<PersonSummaryDto>> GetAllAsync();
        Task<PersonDetailsDto?> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, UpdatePersonDto request);
        Task DeleteAsync(Guid id);


        // v2
        Task<Person> CreateV2Async(RegisterV2PersonDto personDto);
        Task UpdateV2Async(Guid id, UpdateV2PersonDto personDto);
    }
}

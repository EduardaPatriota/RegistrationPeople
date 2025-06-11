using RegistrationPeople.Domain.Entities;

namespace RegistrationPeople.Domain.Interfaces
{
    public interface IPersonRepository
    {
        Task<Person> InsertAsync(Person person);
        Task<Person?> GetByIdAsync(Guid id);
        Task<IEnumerable<Person>> GetAllAsync();
        Task UpdateAsync(Person person);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsCpfAsync(string cpf, Guid? ignoreId = null);
        Task<Person?> GetByEmailAsync(string email);
    }
}
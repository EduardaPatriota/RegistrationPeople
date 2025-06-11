using Microsoft.EntityFrameworkCore;
using RegistrationPeople.Domain.Entities;
using RegistrationPeople.Domain.Interfaces;
using RegistrationPeople.Infrastructure.Persistence;


namespace RegistrationPeople.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly AppDbContext _context;

        public PersonRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Person> InsertAsync(Person person)
        {
            _context.People.Add(person);
            await _context.SaveChangesAsync();
            return person;
        }

        public async Task<Person?> GetByIdAsync(Guid id)
        {
            return await _context.People.FindAsync(id);
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _context.People.ToListAsync();
        }

        public async Task UpdateAsync(Person person)
        {
            _context.People.Update(person);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var person = await GetByIdAsync(id);
            if (person == null) return;
            _context.People.Remove(person);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsCpfAsync(string cpf, Guid? ignoreId = null)
        {
            return await _context.People.AnyAsync(p =>
                p.Cpf == cpf && (ignoreId == null || p.Id != ignoreId));
        }

        public async Task<Person?> GetByEmailAsync(string email)
        {
            return await _context.People.FirstOrDefaultAsync(p => p.Email == email);
        }

    }
}
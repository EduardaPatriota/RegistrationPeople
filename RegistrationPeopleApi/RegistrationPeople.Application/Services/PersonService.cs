
using RegistrationPeople.Application.DTOs;
using RegistrationPeople.Application.DTOs.V2;
using RegistrationPeople.Application.Factories;
using RegistrationPeople.Application.Interfaces;
using RegistrationPeople.Application.Responses;
using RegistrationPeople.Domain.Entities;
using RegistrationPeople.Domain.Interfaces;
using System.Net;

namespace RegistrationPeople.Application.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _repository;

        public PersonService(IPersonRepository repository)
        {
            _repository = repository;
        }

        public async Task<Person> CreateAsync(RegisterPersonDto personDto)
        {
            var person = PersonFactory.CreateFromRegisterDto(personDto);

            ValidatePerson(person);

            if (await _repository.ExistsCpfAsync(person.Cpf))
                throw new ArgumentException("CPF already exists.");

            return await _repository.InsertAsync(person);
        }

        public async Task<IEnumerable<PersonSummaryDto>> GetAllAsync()
        {
            var people = await _repository.GetAllAsync();
            return people.Select(PersonFactory.ToSummaryDto).ToList();
        }

        public async Task<PersonDetailsDto?> GetByIdAsync(Guid id)
        {
            var person = await _repository.GetByIdAsync(id);
            if (person == null)
                return null;

            return PersonFactory.ToDetailsDto(person);
        }

        public async Task UpdateAsync(Guid id, UpdatePersonDto request)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Person not found.");

            PersonFactory.UpdatePersonFromDto(existing, request);

            ValidatePerson(existing);

            if (await _repository.ExistsCpfAsync(existing.Cpf, id))
                throw new ArgumentException("CPF already exists for another person.");

            await _repository.UpdateAsync(existing);
        }



        public async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
        }

        private void ValidatePerson(Person person)
        {
            if (string.IsNullOrWhiteSpace(person.Name))
                throw new ArgumentException("Name is required.");

            if (!string.IsNullOrEmpty(person.Email) &&
                !IsValidEmail(person.Email))
                throw new ArgumentException("Invalid email format.");

            if (!IsValidDate(person.BirthDate))
                throw new ArgumentException("Invalid birth date.");
  
        }

        private bool IsValidDate(DateTime date)
        {
            return date < DateTime.UtcNow && date > DateTime.UtcNow.AddYears(-120);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        //V2

        public async Task<Person> CreateV2Async(RegisterV2PersonDto personDto)
        {
            if (string.IsNullOrWhiteSpace(personDto.Address))
                throw new ArgumentException("Address is required.");

            var person = PersonV2Factory.CreateV2FromRegisterDto(personDto);


            ValidatePerson(person);

            if (await _repository.ExistsCpfAsync(person.Cpf))
                throw new ArgumentException("CPF already exists.");

            return await _repository.InsertAsync(person);
        }

        public async Task UpdateV2Async(Guid id, UpdateV2PersonDto personDto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Person not found.");

            PersonV2Factory.UpdateV2PersonFromDto(existing, personDto);

            ValidatePerson(existing);

            await _repository.UpdateAsync(existing);
        }
    }
}


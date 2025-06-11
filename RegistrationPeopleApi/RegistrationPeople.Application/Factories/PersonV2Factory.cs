using RegistrationPeople.Application.DTOs;
using RegistrationPeople.Application.DTOs.V2;
using RegistrationPeople.Domain.Entities;


namespace RegistrationPeople.Application.Factories
{
    public static class PersonV2Factory
    {
        public static Person CreateV2FromRegisterDto(RegisterV2PersonDto dto)
        {
            return new Person
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Gender = dto.Gender,
                Email = dto.Email,
                BirthDate = dto.BirthDate,
                Birthplace = dto.Birthplace,
                Nationality = dto.Nationality,
                Cpf = dto.Cpf,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static void UpdateV2PersonFromDto(Person existingPerson, UpdateV2PersonDto updateDto)
        {
            if (existingPerson == null)
                throw new ArgumentNullException(nameof(existingPerson));

            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));
            
            if (!string.IsNullOrWhiteSpace(updateDto.Name))
                existingPerson.Name = updateDto.Name;

            if (!string.IsNullOrWhiteSpace(updateDto.Gender))
                existingPerson.Gender = updateDto.Gender;

            if (!string.IsNullOrWhiteSpace(updateDto.Email))
                existingPerson.Email = updateDto.Email;

            if (updateDto.BirthDate.HasValue)
                existingPerson.BirthDate = updateDto.BirthDate.Value;

            if (!string.IsNullOrWhiteSpace(updateDto.Birthplace))
                existingPerson.Birthplace = updateDto.Birthplace;

            if (!string.IsNullOrWhiteSpace(updateDto.Nationality))
                existingPerson.Nationality = updateDto.Nationality;

            if (!string.IsNullOrWhiteSpace(updateDto.Cpf))
                existingPerson.Cpf = updateDto.Cpf;

            if (!string.IsNullOrWhiteSpace(updateDto.Address))
                existingPerson.Address = updateDto.Address;

            existingPerson.UpdatedAt = DateTime.UtcNow;
        }

        public static PersonDetailsDto ToDetailsV2Dto(Person person)
        {
            return new PersonDetailsDto
            {
                Id = person.Id,
                Name = person.Name,
                Email = person.Email,
                Cpf = person.Cpf,
                Gender = person.Gender,
                BirthDate = person.BirthDate,
                Birthplace = person.Birthplace,
                Nationality = person.Nationality,
                CreatedAt = person.CreatedAt,
                UpdatedAt = person.UpdatedAt,
                Address = person.Address
            };
        }

        public static PersonSummaryDto ToSummaryV2Dto(Person person)
        {
            return new PersonSummaryDto
            {
                Id = person.Id,
                Name = person.Name,
                Email = person.Email,
                Cpf = person.Cpf,
                Address = person.Address
            };
        }
    }
}

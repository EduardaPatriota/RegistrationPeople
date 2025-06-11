
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using RegistrationPeople.Application.DTOs;
using RegistrationPeople.Application.Services;
using RegistrationPeople.Domain.Entities;
using RegistrationPeople.Domain.Interfaces;
using Xunit;
namespace RegistrationPeople.Tests
{
    public class PersonServiceTests
    {
        private readonly Mock<IPersonRepository> _repositoryMock;
        private readonly PersonService _service;

        public PersonServiceTests()
        {
            _repositoryMock = new Mock<IPersonRepository>();
            _service = new PersonService(_repositoryMock.Object);
        }

        [Fact]
        public async Task CreateAsync_Should_CreatePerson_When_Valid()
        {
            var dto = new RegisterPersonDto
            {
                Name = "John Doe",
                Cpf = "12345678901",
                BirthDate = DateTime.UtcNow.AddYears(-30),
                Email = "john@example.com"
            };

            _repositoryMock.Setup(r => r.ExistsCpfAsync(dto.Cpf, It.IsAny<Guid?>())).ReturnsAsync(false);

            _repositoryMock.Setup(r => r.InsertAsync(It.IsAny<Person>())).ReturnsAsync((Person p) => p);

            var result = await _service.CreateAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Cpf, result.Cpf);
            Assert.Equal(dto.Email, result.Email);

            _repositoryMock.Verify(r => r.ExistsCpfAsync(dto.Cpf, It.IsAny<Guid?>()), Times.Once);
            _repositoryMock.Verify(r => r.InsertAsync(It.IsAny<Person>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_ThrowArgumentException_When_CpfExists()
        {
            var dto = new RegisterPersonDto
            {
                Name = "Jane Doe",
                Cpf = "12345678901",
                BirthDate = DateTime.UtcNow.AddYears(-30)
            };

            _repositoryMock.Setup(r => r.ExistsCpfAsync(dto.Cpf, It.IsAny<Guid?>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));

            _repositoryMock.Verify(r => r.ExistsCpfAsync(dto.Cpf, It.IsAny<Guid?>()), Times.Once);
            _repositoryMock.Verify(r => r.InsertAsync(It.IsAny<Person>()), Times.Never);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CreateAsync_Should_ThrowArgumentException_When_NameIsEmpty(string invalidName)
        {
            var dto = new RegisterPersonDto
            {
                Name = invalidName,
                Cpf = "12345678901",
                BirthDate = DateTime.UtcNow.AddYears(-30)
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
            _repositoryMock.Verify(r => r.ExistsCpfAsync(dto.Cpf, It.IsAny<Guid?>()), Times.Never);
            _repositoryMock.Verify(r => r.InsertAsync(It.IsAny<Person>()), Times.Never);
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("missing-at.com")]
        [InlineData("missingdomain@")]
        public async Task CreateAsync_Should_ThrowArgumentException_When_EmailInvalid(string invalidEmail)
        {
            var dto = new RegisterPersonDto
            {
                Name = "Valid Name",
                Cpf = "12345678901",
                BirthDate = DateTime.UtcNow.AddYears(-30),
                Email = invalidEmail
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_Should_ThrowArgumentException_When_BirthDateInvalid()
        {
            var dto = new RegisterPersonDto
            {
                Name = "Valid Name",
                Cpf = "12345678901",
                BirthDate = DateTime.UtcNow.AddYears(1), // future date
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateAsync(dto));
        }

        [Fact]
        public async Task GetAllAsync_Should_ReturnPersonSummaryDtos()
        {
            var people = new List<Person>
        {
            new Person { Id = Guid.NewGuid(), Name = "John", Cpf = "11111111111", BirthDate = DateTime.UtcNow.AddYears(-20) },
            new Person { Id = Guid.NewGuid(), Name = "Jane", Cpf = "22222222222", BirthDate = DateTime.UtcNow.AddYears(-25) }
        };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(people);

            var result = await _service.GetAllAsync();

            Assert.Equal(people.Count, result.Count());
            Assert.All(result, dto => Assert.False(string.IsNullOrWhiteSpace(dto.Name)));
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnPersonDetailsDto_When_Found()
        {
            var id = Guid.NewGuid();
            var person = new Person
            {
                Id = id,
                Name = "John",
                Cpf = "11111111111",
                BirthDate = DateTime.UtcNow.AddYears(-20)
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(person);

            var result = await _service.GetByIdAsync(id);

            Assert.NotNull(result);
            Assert.Equal(person.Name, result!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_Should_ReturnNull_When_NotFound()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Person?)null);

            var result = await _service.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_Should_UpdatePerson_When_Valid()
        {
            var id = Guid.NewGuid();
            var existingPerson = new Person
            {
                Id = id,
                Name = "Old Name",
                Cpf = "12345678901",
                BirthDate = DateTime.UtcNow.AddYears(-30)
            };

            var updateDto = new UpdatePersonDto
            {
                Name = "New Name",
                Cpf = "12345678901",
                Email = "newemail@example.com",
                BirthDate = DateTime.UtcNow.AddYears(-31)
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingPerson);
            _repositoryMock.Setup(r => r.ExistsCpfAsync(updateDto.Cpf, id)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.UpdateAsync(existingPerson)).Returns(Task.CompletedTask);

            await _service.UpdateAsync(id, updateDto);

            Assert.Equal(updateDto.Name, existingPerson.Name);
            Assert.Equal(updateDto.Email, existingPerson.Email);
            Assert.Equal(updateDto.Cpf, existingPerson.Cpf);

            _repositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);
            _repositoryMock.Verify(r => r.ExistsCpfAsync(updateDto.Cpf, id), Times.Once);
            _repositoryMock.Verify(r => r.UpdateAsync(existingPerson), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_ThrowKeyNotFoundException_When_PersonNotFound()
        {
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Person?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(Guid.NewGuid(), new UpdatePersonDto()));

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Person>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_Should_ThrowArgumentException_When_CpfExistsForOtherPerson()
        {
            var id = Guid.NewGuid();
            var existingPerson = new Person
            {
                Id = id,
                Name = "Old Name",
                Cpf = "12345678901",
                BirthDate = DateTime.UtcNow.AddYears(-30)
            };
            var updateDto = new UpdatePersonDto
            {
                Cpf = "98765432100",
                Name = "Name",
                BirthDate = DateTime.UtcNow.AddYears(-30)
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingPerson);
            _repositoryMock.Setup(r => r.ExistsCpfAsync(updateDto.Cpf, id)).ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdateAsync(id, updateDto));

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Person>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_Should_CallRepositoryDelete()
        {
            var id = Guid.NewGuid();

            _repositoryMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);

            await _service.DeleteAsync(id);

            _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }
    }
}
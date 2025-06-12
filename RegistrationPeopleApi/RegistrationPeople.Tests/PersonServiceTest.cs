using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using RegistrationPeople.Application.DTOs;
using RegistrationPeople.Application.DTOs.V2;
using RegistrationPeople.Application.Services;
using RegistrationPeople.Domain.Entities;
using RegistrationPeople.Domain.Interfaces;
using Xunit;

public class PersonServiceTests
{
    private readonly Mock<IPersonRepository> _repositoryMock;
    private readonly PersonService _personService;

    public PersonServiceTests()
    {
        _repositoryMock = new Mock<IPersonRepository>();
        _personService = new PersonService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidPerson_ReturnsCreatedPerson()
    {
        var dto = new RegisterPersonDto
        {
            Name = "Test User",
            Cpf = "12345678900",
            Email = "test@example.com",
            BirthDate = DateTime.UtcNow.AddYears(-30),
            Password = "password"
        };

        _repositoryMock.Setup(r => r.ExistsCpfAsync(dto.Cpf, null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.InsertAsync(It.IsAny<Person>()))
                       .ReturnsAsync((Person p) => p);

        var result = await _personService.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        _repositoryMock.Verify(r => r.InsertAsync(It.IsAny<Person>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateCpf_ThrowsArgumentException()
    {
        var dto = new RegisterPersonDto
        {
            Name = "Test User",
            Cpf = "12345678900",
            Email = "test@example.com",
            BirthDate = DateTime.UtcNow.AddYears(-30),
            Password = "password"
        };

        _repositoryMock.Setup(r => r.ExistsCpfAsync(dto.Cpf, null)).ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _personService.CreateAsync(dto));
        Assert.Equal("CPF already exists.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidEmail_ThrowsArgumentException()
    {
        var dto = new RegisterPersonDto
        {
            Name = "Test User",
            Cpf = "12345678900",
            Email = "invalid-email",
            BirthDate = DateTime.UtcNow.AddYears(-30),
            Password = "password"
        };

        _repositoryMock.Setup(r => r.ExistsCpfAsync(dto.Cpf, null)).ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _personService.CreateAsync(dto));
        Assert.Equal("Invalid email format.", ex.Message);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPersonSummaryDtoList()
    {
        var people = new List<Person>
        {
            new Person { Id = Guid.NewGuid(), Name = "User1", Email = "user1@example.com" },
            new Person { Id = Guid.NewGuid(), Name = "User2", Email = "user2@example.com" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(people);

        var result = await _personService.GetAllAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.Name == "User1");
        Assert.Contains(result, r => r.Name == "User2");
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingPerson_ReturnsPersonDetailsDto()
    {
        var id = Guid.NewGuid();
        var person = new Person { Id = id, Name = "User", Email = "user@example.com" };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(person);

        var result = await _personService.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal(person.Name, result!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingPerson_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Person?)null);

        var result = await _personService.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_UpdatesPerson()
    {
        var id = Guid.NewGuid();
        var existingPerson = new Person
        {
            Id = id,
            Name = "Old Name",
            Cpf = "12345678900",
            Email = "old@example.com",
            BirthDate = DateTime.UtcNow.AddYears(-30)
        };

        var updateDto = new UpdatePersonDto
        {
            Name = "New Name",
            Cpf = "12345678900",
            Email = "new@example.com",
            BirthDate = DateTime.UtcNow.AddYears(-25)
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingPerson);
        _repositoryMock.Setup(r => r.ExistsCpfAsync(updateDto.Cpf, id)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.UpdateAsync(existingPerson)).Returns(Task.CompletedTask).Verifiable();

        await _personService.UpdateAsync(id, updateDto);

        _repositoryMock.Verify(r => r.UpdateAsync(existingPerson), Times.Once);
        Assert.Equal(updateDto.Name, existingPerson.Name);
        Assert.Equal(updateDto.Email, existingPerson.Email);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingPerson_ThrowsKeyNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Person?)null);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _personService.UpdateAsync(Guid.NewGuid(), new UpdatePersonDto()));

        Assert.Equal("Person not found.", ex.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateCpf_ThrowsArgumentException()
    {
        var id = Guid.NewGuid();
        var existingPerson = new Person
        {
            Id = id,
            Name = "Old Name",
            Cpf = "12345678900",
            Email = "old@example.com",
            BirthDate = DateTime.UtcNow.AddYears(-30)
        };

        var updateDto = new UpdatePersonDto
        {
            Name = "New Name",
            Cpf = "12345678900",
            Email = "new@example.com",
            BirthDate = DateTime.UtcNow.AddYears(-25)
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingPerson);
        _repositoryMock.Setup(r => r.ExistsCpfAsync(updateDto.Cpf, id)).ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _personService.UpdateAsync(id, updateDto));

        Assert.Equal("CPF already exists for another person.", ex.Message);
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        var id = Guid.NewGuid();

        _repositoryMock.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask).Verifiable();

        await _personService.DeleteAsync(id);

        _repositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
    }
    
    [Fact]
    public async Task CreateV2Async_WithValidPerson_ReturnsCreatedPerson()
    {
        var dto = new RegisterV2PersonDto
        {
            Name = "Test User",
            Cpf = "12345678900",
            Email = "test@example.com",
            BirthDate = DateTime.UtcNow.AddYears(-30),
            Password = "password",
            Address = "Some Address"
        };

        _repositoryMock.Setup(r => r.ExistsCpfAsync(dto.Cpf, null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.InsertAsync(It.IsAny<Person>()))
                       .ReturnsAsync((Person p) => p);

        var result = await _personService.CreateV2Async(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
    }

    [Fact]
    public async Task CreateV2Async_WithoutAddress_ThrowsArgumentException()
    {
        var dto = new RegisterV2PersonDto
        {
            Name = "Test User",
            Cpf = "12345678900",
            Email = "test@example.com",
            BirthDate = DateTime.UtcNow.AddYears(-30),
            Password = "password",
            Address = ""
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _personService.CreateV2Async(dto));

        Assert.Equal("Address is required.", ex.Message);
    }

    [Fact]
    public async Task UpdateV2Async_WithValidPerson_UpdatesPerson()
    {
        var id = Guid.NewGuid();
        var existingPerson = new Person
        {
            Id = id,
            Name = "Old Name",
            Cpf = "12345678900",
            Email = "old@example.com",
            BirthDate = DateTime.UtcNow.AddYears(-30)
        };

        var updateDto = new UpdateV2PersonDto
        {
            Name = "New Name",
            Cpf = "12345678900",
            Email = "new@example.com",
            BirthDate = DateTime.UtcNow.AddYears(-25),
            Address = "New Address"
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existingPerson);
        _repositoryMock.Setup(r => r.UpdateAsync(existingPerson)).Returns(Task.CompletedTask).Verifiable();

        await _personService.UpdateV2Async(id, updateDto);

        _repositoryMock.Verify(r => r.UpdateAsync(existingPerson), Times.Once);
        Assert.Equal(updateDto.Name, existingPerson.Name);
    }

    [Fact]
    public async Task UpdateV2Async_WithNonExistingPerson_ThrowsKeyNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Person?)null);

        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _personService.UpdateV2Async(Guid.NewGuid(), new UpdateV2PersonDto()));

        Assert.Equal("Person not found.", ex.Message);
    }
}

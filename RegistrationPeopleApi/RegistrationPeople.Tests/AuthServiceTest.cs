using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using RegistrationPeople.Application.Services;
using RegistrationPeople.Application.DTOs;
using RegistrationPeople.Domain.Entities;
using RegistrationPeople.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AuthServiceTest
{
    private readonly Mock<IPersonRepository> _personRepoMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly AuthService _authService;

    public AuthServiceTest()
    {
        _personRepoMock = new Mock<IPersonRepository>();
        _configMock = new Mock<IConfiguration>();

        // Mock do token JWT
        _configMock.Setup(c => c["Jwt:Key"]).Returns("supersecretkey1234567890");

        _authService = new AuthService(_personRepoMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithAdminCredentials_ReturnsToken()
    {
        var loginDto = new LoginDto { Email = "admin@admin.com", Password = "admin" };

        var result = await _authService.LoginAsync(loginDto);

        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public async Task LoginAsync_WithValidUser_ReturnsToken()
    {
        var password = "test123";
        var hashed = BCrypt.Net.BCrypt.HashPassword(password);

        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            PasswordHash = hashed
        };

        _personRepoMock.Setup(r => r.GetByEmailAsync(person.Email)).ReturnsAsync(person);

        var loginDto = new LoginDto { Email = person.Email, Password = password };

        var result = await _authService.LoginAsync(loginDto);

        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctPassword")
        };

        _personRepoMock.Setup(r => r.GetByEmailAsync(person.Email)).ReturnsAsync(person);

        var loginDto = new LoginDto { Email = person.Email, Password = "wrongPassword" };

        var result = await _authService.LoginAsync(loginDto);

        Assert.Null(result);
    }

    [Fact]
    public async Task RegisterAsync_WithUniqueEmail_ReturnsSuccess()
    {
        var registerDto = new RegisterPersonDto
        {
            Name = "Test User",
            Email = "newuser@example.com",
            Password = "123456"
        };

        _personRepoMock.Setup(r => r.GetByEmailAsync(registerDto.Email)).ReturnsAsync((Person)null!);
        _personRepoMock.Setup(r => r.InsertAsync(It.IsAny<Person>())).Returns((Task<Person>)Task.CompletedTask);

        var result = await _authService.RegisterAsync(registerDto);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ReturnsFailure()
    {
        var registerDto = new RegisterPersonDto
        {
            Name = "Test User",
            Email = "duplicate@example.com",
            Password = "123456"
        };

        _personRepoMock.Setup(r => r.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync(new Person { Email = registerDto.Email });

        var result = await _authService.RegisterAsync(registerDto);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, e => e.Code == "DuplicateEmail");
    }
}

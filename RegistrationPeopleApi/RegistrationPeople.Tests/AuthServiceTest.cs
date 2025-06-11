using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using RegistrationPeople.Application.DTOs;
using RegistrationPeople.Application.Services;
using RegistrationPeople.Domain.Entities;
using RegistrationPeople.Domain.Interfaces;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class AuthServiceTest
{
    private readonly Mock<IPersonRepository> _personRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTest()
    {
        _personRepositoryMock = new Mock<IPersonRepository>();
        _configurationMock = new Mock<IConfiguration>();

        // Setup do IConfiguration para retornar valores do JWT
        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("ThisIsASecretKeyForJwt1234");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _authService = new AuthService(_personRepositoryMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithAdminCredentials_ReturnsToken()
    {
        var loginDto = new LoginDto { Email = "admin@admin.com", Password = "admin" };

        var token = await _authService.LoginAsync(loginDto);

        Assert.NotNull(token);
        Assert.False(AuthService.IsJwtExpired(token));
    }

    [Fact]
    public async Task LoginAsync_WithValidUser_ReturnsToken()
    {
        var password = "mypassword";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "user@test.com",
            PasswordHash = passwordHash
        };

        _personRepositoryMock.Setup(r => r.GetByEmailAsync(person.Email))
                             .ReturnsAsync(person);

        var loginDto = new LoginDto { Email = person.Email, Password = password };

        var token = await _authService.LoginAsync(loginDto);

        Assert.NotNull(token);
        Assert.False(AuthService.IsJwtExpired(token));
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "user@test.com",
            PasswordHash = passwordHash
        };

        _personRepositoryMock.Setup(r => r.GetByEmailAsync(person.Email))
                             .ReturnsAsync(person);

        var loginDto = new LoginDto { Email = person.Email, Password = "wrongpassword" };

        var token = await _authService.LoginAsync(loginDto);

        Assert.Null(token);
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ReturnsNull()
    {
        _personRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                             .ReturnsAsync((Person?)null);

        var loginDto = new LoginDto { Email = "nonexistent@test.com", Password = "password" };

        var token = await _authService.LoginAsync(loginDto);

        Assert.Null(token);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ReturnsFailedIdentityResult()
    {
        var existingPerson = new Person
        {
            Id = Guid.NewGuid(),
            Email = "duplicate@test.com"
        };

        _personRepositoryMock.Setup(r => r.GetByEmailAsync(existingPerson.Email))
                             .ReturnsAsync(existingPerson);

        var registerDto = new RegisterPersonDto
        {
            Email = existingPerson.Email,
            Name = "Duplicate User",
            Cpf = "12345678901",
            Password = "password"
        };

        var result = await _authService.RegisterAsync(registerDto);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, e => e.Code == "DuplicateEmail");
    }

    [Fact]
    public async Task RegisterAsync_WithNewEmail_ReturnsSuccess()
    {
        _personRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                             .ReturnsAsync((Person?)null);

        _personRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Person>()))
                             .Returns((Task<Person>)Task.CompletedTask)
                             .Verifiable();

        var registerDto = new RegisterPersonDto
        {
            Email = "newuser@test.com",
            Name = "New User",
            Cpf = "12345678905",
            Password = "password"
        };

        var result = await _authService.RegisterAsync(registerDto);

        Assert.True(result.Succeeded);
        _personRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Person>()), Times.Once);
    }

    [Fact]
    public void IsJwtExpired_WithExpiredToken_ReturnsTrue()
    {
        // Criar token expirado manualmente para teste
        var token = CreateJwtToken(DateTime.UtcNow.AddMinutes(-10));

        var isExpired = AuthService.IsJwtExpired(token);

        Assert.True(isExpired);
    }

    [Fact]
    public void IsJwtExpired_WithValidToken_ReturnsFalse()
    {
        var token = CreateJwtToken(DateTime.UtcNow.AddMinutes(10));

        var isExpired = AuthService.IsJwtExpired(token);

        Assert.False(isExpired);
    }

    private string CreateJwtToken(DateTime expires)
    {
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("ThisIsASecretKeyForJwt1234");
        var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
        {
            Expires = expires,
            SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

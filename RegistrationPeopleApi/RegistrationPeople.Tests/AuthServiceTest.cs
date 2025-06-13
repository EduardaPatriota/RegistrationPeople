using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using RegistrationPeople.Application.DTOs;
using RegistrationPeople.Application.Factories;
using RegistrationPeople.Application.Services;
using RegistrationPeople.Domain.Entities;
using RegistrationPeople.Domain.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class AuthServiceTests
{
    private readonly Mock<IPersonRepository> _personRepositoryMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _personRepositoryMock = new Mock<IPersonRepository>();
        _configMock = new Mock<IConfiguration>();

        _configMock.Setup(c => c["Jwt:Key"]).Returns("ThisIsASecretKeyForJwt1234_256bits!");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");



        _authService = new AuthService(_personRepositoryMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithAdminCredentials_ReturnsToken()
    {
        var dto = new LoginDto
        {
            Email = "admin@admin.com",
            Password = "admin"
        };

        var result = await _authService.LoginAsync(dto);
        if (result == null)
        {
            Assert.Fail("Login failed, expected a valid token.");
        }
        Assert.NotNull(result);
        Assert.False(AuthService.IsJwtExpired(result.Token));
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ReturnsNull()
    {
        _personRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                             .ReturnsAsync((Person?)null);

        var dto = new LoginDto
        {
            Email = "user@notfound.com",
            Password = "123"
        };

        var result = await _authService.LoginAsync(dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            Name = "Test User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
        };

        _personRepositoryMock.Setup(x => x.GetByEmailAsync(person.Email))
                             .ReturnsAsync(person);

        var dto = new LoginDto
        {
            Email = person.Email,
            Password = "wrongpassword"
        };

        var result = await _authService.LoginAsync(dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsToken()
    {
        var password = "mypassword";
        var person = new Person
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            Name = "Test User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _personRepositoryMock.Setup(x => x.GetByEmailAsync(person.Email))
                             .ReturnsAsync(person);

        var dto = new LoginDto
        {
            Email = person.Email,
            Password = password
        };

        var result = await _authService.LoginAsync(dto);
        if (result == null){
            Assert.Fail("Login failed, expected a valid token.");
        }
        Assert.NotNull(result);
        Assert.False(AuthService.IsJwtExpired(result.Token));
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ReturnsFailedResult()
    {
        var existing = new Person
        {
            Id = Guid.NewGuid(),
            Email = "existing@test.com"
        };

        _personRepositoryMock.Setup(x => x.GetByEmailAsync(existing.Email))
                             .ReturnsAsync(existing);

        var dto = new RegisterPersonDto
        {
            Email = existing.Email,
            Name = "Someone",
            Cpf = "12345678901",
            Password = "pass"
        };

        var result = await _authService.RegisterAsync(dto);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Errors, e => e.Code == "DuplicateEmail");
    }

    [Fact]
    public async Task RegisterAsync_WithNewEmail_ReturnsSuccess()
    {
        _personRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                             .ReturnsAsync((Person?)null);

        _personRepositoryMock.Setup(x => x.InsertAsync(It.IsAny<Person>()))
                         .ReturnsAsync((Person p) => p)
                         .Verifiable();


        var dto = new RegisterPersonDto
        {
            Email = "newuser@test.com",
            Name = "New User",
            Cpf = "12345678909",
            Password = "securePassword"
        };

        var result = await _authService.RegisterAsync(dto);

        Assert.True(result.Succeeded);
        _personRepositoryMock.Verify(x => x.InsertAsync(It.IsAny<Person>()), Times.Once);
    }


    [Fact]
    public void IsJwtExpired_WithExpiredToken_ReturnsTrue()
    {
        var expiredToken = CreateJwtToken(DateTime.UtcNow.AddMinutes(-10));

        var result = AuthService.IsJwtExpired(expiredToken);

        Assert.True(result);
    }

    [Fact]
    public void IsJwtExpired_WithValidToken_ReturnsFalse()
    {
        var validToken = CreateJwtToken(DateTime.UtcNow.AddMinutes(10));

        var result = AuthService.IsJwtExpired(validToken);

        Assert.False(result);
    }

    private string CreateJwtToken(DateTime expires)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsASecretKeyForJwt1234_256bits!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "TestIssuer",
            audience: "TestAudience",
            claims: new[] { new Claim(ClaimTypes.Name, "TestUser") },
            expires: expires,
            signingCredentials: creds
        );

        return tokenHandler.WriteToken(token);
    }

}

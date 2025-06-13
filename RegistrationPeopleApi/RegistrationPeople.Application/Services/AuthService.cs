using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.AppService.ApiApps.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RegistrationPeople.Application.DTOs;
using RegistrationPeople.Application.Factories;
using RegistrationPeople.Application.Interfaces;
using RegistrationPeople.Application.Responses;
using RegistrationPeople.Domain.Entities;
using RegistrationPeople.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RegistrationPeople.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IConfiguration _config;

        public AuthService(IPersonRepository personRepository, IConfiguration config)
        {
            _personRepository = personRepository;
            _config = config;
        }

        public async Task<LoginResponse> LoginAsync(LoginDto loginDto)
        {
            // Usuário Admin fixo
            if (loginDto.Email == "admin@admin.com" && loginDto.Password == "admin")
            {
                var token = GenerateToken(Guid.Empty, "Admin");
                return LoginResponse.Ok(token);
            }

            var person = await _personRepository.GetByEmailAsync(loginDto.Email);
            if (person == null)
                return LoginResponse.Fail("Usuário não encontrado");

            var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, person.PasswordHash);
            if (!isPasswordValid)
                return LoginResponse.Fail("Senha incorreta");

            var jwt = GenerateToken(person.Id, person.Name);
            return LoginResponse.Ok(jwt);
        }

        public async Task<IdentityResult> RegisterAsync(RegisterPersonDto registerDto)
        {
            
            var existing = await _personRepository.GetByEmailAsync(registerDto.Email);
            if (existing != null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateEmail",
                    Description = "Email already in use."
                });
            }

            var person = PersonFactory.CreateFromRegisterDto(registerDto);
           

            await _personRepository.InsertAsync(person);

            return IdentityResult.Success;
        }

        private string GenerateToken(Guid userId, string userName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(JwtRegisteredClaimNames.Jti, userId.ToString())
            };

           

            var expires = DateTime.UtcNow.AddHours(5);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return tokenHandler.WriteToken(tokenDescriptor);
        }
        public static bool IsJwtExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var expClaim = jwtToken.Payload.Expiration;
            if (expClaim == null)
                return true; 

            var expDate = DateTimeOffset.FromUnixTimeSeconds((long)expClaim).UtcDateTime;
            return expDate < DateTime.UtcNow;
        }

    }
}

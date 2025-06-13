using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.AppService.ApiApps.Service;
using RegistrationPeople.Application.DTOs;
using RegistrationPeople.Application.Responses;

namespace RegistrationPeople.Application.Interfaces
{
    public interface IAuthService
    {

        Task<LoginResponse> LoginAsync(LoginDto loginDto);
        Task<IdentityResult> RegisterAsync(RegisterPersonDto registerDto);
       
    }
}

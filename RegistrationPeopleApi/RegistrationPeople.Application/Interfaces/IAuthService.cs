using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.AppService.ApiApps.Service;
using RegistrationPeople.Application.DTOs;

namespace RegistrationPeople.Application.Interfaces
{
    public interface IAuthService
    {

        Task<string?> LoginAsync(LoginDto loginDto);
        Task<IdentityResult> RegisterAsync(RegisterPersonDto registerDto);
       
    }
}

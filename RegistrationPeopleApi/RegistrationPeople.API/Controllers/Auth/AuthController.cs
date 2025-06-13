using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistrationPeople.Application.DTOs;
using RegistrationPeople.Application.Interfaces;
using RegistrationPeople.Application.Responses;
using System.Net;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var token = await _authService.LoginAsync(model);

        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized(ApiResponse<string>.Fail("Credenciais inválidas", HttpStatusCode.Unauthorized));
        }

        return Ok(ApiResponse<string>.Ok(token));
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<string>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterPersonDto model)
    {
        var result = await _authService.RegisterAsync(model);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return BadRequest(ApiResponse<List<string>>.Fail(errors, HttpStatusCode.BadRequest));
        }

        return Ok(ApiResponse<string>.Ok("Usuário registrado com sucesso"));
    }
}

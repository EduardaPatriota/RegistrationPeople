using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RegistrationPeople.Application.DTOs;
using RegistrationPeople.Application.DTOs.V2;
using RegistrationPeople.Application.Interfaces;
using RegistrationPeople.Application.Responses;
using RegistrationPeople.Domain.Entities;
using System.Net;

namespace RegistrationPeople.API.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegisterV2PersonDto personDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode((int)HttpStatusCode.BadRequest, ApiResponse<IEnumerable<string>>.Fail(errors, HttpStatusCode.BadRequest));
            }

            try
            {
                var created = await _personService.CreateV2Async(personDto);
                var response = ApiResponse<Person>.Ok(created, HttpStatusCode.Created);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
            }
            catch (ArgumentException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ApiResponse<Person>.Fail(ex.Message, HttpStatusCode.BadRequest));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ApiResponse<Person>.Fail("Erro interno: " + ex.Message, HttpStatusCode.InternalServerError));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateV2PersonDto personDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode((int)HttpStatusCode.BadRequest, ApiResponse<IEnumerable<string>>.Fail(errors, HttpStatusCode.BadRequest));
            }

            try
            {
                await _personService.UpdateV2Async(id, personDto);
                var response = ApiResponse<string>.Ok("Pessoa atualizada com sucesso", HttpStatusCode.OK);
                return StatusCode((int)response.StatusCode, response);
            }
            catch (KeyNotFoundException)
            {
                return StatusCode((int)HttpStatusCode.NotFound, ApiResponse<string>.Fail("Pessoa não encontrada", HttpStatusCode.NotFound));
            }
            catch (ArgumentException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ApiResponse<string>.Fail(ex.Message, HttpStatusCode.BadRequest));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ApiResponse<string>.Fail("Erro interno: " + ex.Message, HttpStatusCode.InternalServerError));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var person = await _personService.GetByIdAsync(id);
            if (person == null)
                return StatusCode((int)HttpStatusCode.NotFound, ApiResponse<Person>.Fail("Pessoa não encontrada", HttpStatusCode.NotFound));

            var response = ApiResponse<PersonDetailsDto>.Ok(person);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}

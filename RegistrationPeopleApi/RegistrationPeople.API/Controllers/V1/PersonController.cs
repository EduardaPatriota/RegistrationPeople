using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegistrationPeople.Application.DTOs;
using RegistrationPeople.Application.Interfaces;
using RegistrationPeople.Application.Responses;
using RegistrationPeople.Domain.Entities;
using System.Net;

namespace RegistrationPeople.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var people = await _personService.GetAllAsync();
            var response = ApiResponse<IEnumerable<PersonSummaryDto>>.Ok(people);
            return StatusCode((int)response.StatusCode, response);
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegisterPersonDto personDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode((int)HttpStatusCode.BadRequest, ApiResponse<IEnumerable<string>>.Fails(errors, HttpStatusCode.BadRequest));
            }

            try
            {
                var created = await _personService.CreateAsync(personDto);
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
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePersonDto personDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode((int)HttpStatusCode.BadRequest, ApiResponse<IEnumerable<string>>.Fails(errors, HttpStatusCode.BadRequest));
            }

            try
            {
                await _personService.UpdateAsync(id, personDto);
                return NoContent();
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _personService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return StatusCode((int)HttpStatusCode.NotFound, ApiResponse<string>.Fail("Pessoa não encontrada", HttpStatusCode.NotFound));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ApiResponse<string>.Fail("Erro interno: " + ex.Message, HttpStatusCode.InternalServerError));
            }
        }
    }
}

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
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PersonSummaryDto>>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetAll()
        {
            var people = await _personService.GetAllAsync();
            var response = ApiResponse<IEnumerable<PersonSummaryDto>>.Ok(people);
            return StatusCode((int)HttpStatusCode.OK, response);
        }


        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<PersonDetailsDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)] 
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var person = await _personService.GetByIdAsync(id);

            if (person == null)
                return StatusCode((int)HttpStatusCode.NotFound,
                    ApiResponse<string>.Fail("Pessoa não encontrada"));

            var response = ApiResponse<PersonDetailsDto>.Ok(person);
            return StatusCode((int)HttpStatusCode.OK, response);
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<Person>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Create([FromBody] RegisterPersonDto personDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode((int)HttpStatusCode.BadRequest, ApiResponse<IEnumerable<string>>.Fail(errors));
            }

            try
            {
                var created = await _personService.CreateAsync(personDto);
                var response = ApiResponse<Person>.Ok(created);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
            }
            catch (ArgumentException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ApiResponse<Person>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ApiResponse<Person>.Fail("Erro interno: " + ex.Message));
            }
        }


        [HttpPut("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePersonDto personDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return StatusCode((int)HttpStatusCode.BadRequest,
                    ApiResponse<IEnumerable<string>>.Fail(errors));
            }

            try
            {
                await _personService.UpdateAsync(id, personDto);
                return Ok(ApiResponse<string>.Ok("Pessoa atualizada com sucesso"));
            }
            catch (KeyNotFoundException)
            {
                return StatusCode((int)HttpStatusCode.NotFound,
                    ApiResponse<string>.Fail("Pessoa não encontrada"));
            }
            catch (ArgumentException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest,
                    ApiResponse<string>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    ApiResponse<string>.Fail("Erro interno: " + ex.Message));
            }
        }


        [HttpDelete("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _personService.DeleteAsync(id);
                return Ok(ApiResponse<string>.Ok("Pessoa deletada com sucesso"));
            }
            catch (KeyNotFoundException)
            {
                return StatusCode((int)HttpStatusCode.NotFound, ApiResponse<string>.Fail("Pessoa não encontrada"));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ApiResponse<string>.Fail("Erro interno: " + ex.Message));
            }
        }
    }
}

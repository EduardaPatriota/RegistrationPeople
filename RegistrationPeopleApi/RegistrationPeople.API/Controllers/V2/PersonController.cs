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
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<Person>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] RegisterV2PersonDto personDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode((int)HttpStatusCode.BadRequest, ApiResponse<IEnumerable<string>>.Fail(errors));
            }

            try
            {
                var created = await _personService.CreateV2Async(personDto);
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
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<string>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateV2PersonDto personDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return StatusCode((int)HttpStatusCode.BadRequest, ApiResponse<IEnumerable<string>>.Fail(errors));
            }

            try
            {
                await _personService.UpdateV2Async(id, personDto);
                var response = ApiResponse<string>.Ok("Pessoa atualizada com sucesso");
                return StatusCode((int)HttpStatusCode.OK, response);
            }
            catch (KeyNotFoundException)
            {
                return StatusCode((int)HttpStatusCode.NotFound, ApiResponse<string>.Fail("Pessoa não encontrada"));
            }
            catch (ArgumentException ex)
            {
                return StatusCode((int)HttpStatusCode.BadRequest, ApiResponse<string>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ApiResponse<string>.Fail("Erro interno: " + ex.Message));
            }
        }

        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<PersonDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var person = await _personService.GetByIdAsync(id);
            if (person == null)
                return StatusCode((int)HttpStatusCode.NotFound, ApiResponse<Person>.Fail("Pessoa não encontrada"));

            var response = ApiResponse<PersonDetailsDto>.Ok(person);
            return StatusCode((int)HttpStatusCode.OK, response);
        }
    }
}

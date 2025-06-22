using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalData.Application.Commands;
using PersonalData.Application.DTOs;
using PersonalData.Application.Queries;
using PersonalData.Infrastructure.Data;

namespace PersonalData.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(IMediator mediator, ILogger<PersonsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedResult<PersonResponseDto>>>> GetAllPersons([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            try
            {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 100) pageSize = 100;

                var query = new GetAllPersonsQuery
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = search
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting persons");
                return StatusCode(500, ApiResponse<PagedResult<PersonResponseDto>>.ErrorResult(
                    "เกิดข้อผิดพลาดในการดึงข้อมูล",
                    new List<string> { ex.Message }));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PersonResponseDto>>> CreatePerson([FromBody] CreatePersonCommand request)
        {
            try
            {
                var result = await _mediator.Send(request);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return CreatedAtAction(
                    nameof(GetAllPersons),
                    new { },
                    result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating person");
                return StatusCode(500, ApiResponse<PersonResponseDto>.ErrorResult(
                    "เกิดข้อผิดพลาดในการเพิ่มข้อมูล",
                    new List<string> { ex.Message }));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<PersonResponseDto>>> UpdatePerson(int id, [FromBody] UpdatePersonCommand request)
        {
            try
            {
                // Ensure ID from route matches request
                request.Id = id;
                var result = await _mediator.Send(request);

                if (!result.Success)
                {
                    return result.Message.Contains("ไม่พบข้อมูล") ? NotFound(result) : BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating person with ID: {PersonId}", id);
                return StatusCode(500, ApiResponse<PersonResponseDto>.ErrorResult(
                    "เกิดข้อผิดพลาดในการแก้ไขข้อมูล",
                    new List<string> { ex.Message }));
            }
        }

        [HttpGet("test")]
        public async Task<ActionResult> TestDatabase()
        {
            try
            {
                using var scope = HttpContext.RequestServices.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var canConnect = await context.Database.CanConnectAsync();
                var count = await context.Persons.CountAsync();

                return Ok(new
                {
                    success = true,
                    canConnect = canConnect,
                    personCount = count,
                    connectionString = context.Database.GetConnectionString()?.Substring(0, 50) + "..."
                });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }
    }
}

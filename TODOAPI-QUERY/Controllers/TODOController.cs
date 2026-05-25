using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TODOAPI_QUERY.DTOs;
using TODOAPI_QUERY.Responses;
using TODOAPI_QUERY.Services;

namespace TODOAPI_QUERY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoService _service;

        public TodoController(ITodoService service)
        {
            _service = service;
        }
        // GET /api/todo
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status)
        {
            var todos = await _service.GetAll(status);
            return Ok(todos);

        }
        // GET /api/todo/search?q=meeting
        [HttpGet("search")]
        public async Task<IActionResult> SearchTodos([FromQuery] string? q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(BuildErrorResponse(400, "Search query cannot be empty"));
            }
            var results = await _service.Search(q);

            return Ok(new
            {
                Query = q,
                Count = results.Count,
                Results = results
            });
        }
        // GET /api/todo/filter
        [HttpGet("GetFilteredTodos")]
        public async Task<ActionResult<object>> GetTodos(
            [FromQuery] string? status,
            [FromQuery] string? priority,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
           
            var result = await _service.GetTodos(status, priority, sortBy, sortOrder, page, pageSize);
            return Ok(result);
        }
        // GET /api/todo/stats
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetStatistics()
        {
            var stats = await _service.GetStatistics();
            return Ok(stats);
        }


        // GET /api/todo/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var todo = await _service.GetById(id);
            if (todo == null)
                return NotFound(BuildErrorResponse(404, $"Todo with id {id} not found"));
            return Ok(todo);
        }
        // POST /api/todo
        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] CreateTodoDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(BuildErrorResponse(
                    400, "Validation failed", GetValidationErrors()
                ));
            var createTodo = await _service.Create(dto);
            return CreatedAtAction(nameof(GetById), new { id = createTodo.Id }, createTodo);
        }

        // PUT /api/todo/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateToDo(int id, [FromBody] UpdateTodoDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(BuildErrorResponse(
                    400, "Validation failed", GetValidationErrors()
                ));

            var updateTodo = await _service.Update(id, dto);
            if (updateTodo == null)
                return NotFound(BuildErrorResponse(
                     404, $"Todo with id {id} not found"
                 ));
            return Ok(updateTodo);

        }
        // DELETE /api/todo/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(int id)
        {
            var success = await _service.Delete(id);
            if (!success)
                return NotFound(BuildErrorResponse(
                    404, $"Todo with id {id} not found"
                ));
            return NoContent();
        }

        private ErrorResponse BuildErrorResponse(int statusCode, string message, Dictionary<string, string[]>? errors = null)
        {
            return new ErrorResponse
            {
                StatusCode = statusCode,
                Message = message,
                Errors = errors,
                Timestamp = DateTime.Now
            };
        }

        private Dictionary<string, string[]> GetValidationErrors()
        {
            return ModelState
                .Where(e => e.Value != null && e.Value.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value!.Errors
                             .Select(x => x.ErrorMessage)
                             .ToArray()
                );
        }



    }
}

using BookNexusAPI.DTOs.Books;
using BookNexusAPI.Services.Books;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookNexusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _service;

        public BookController(IBookService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<List<BookDto>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDto>> GetById(int id)
        {
            var book = await _service.GetByIdAsync(id);
            return book is null ? NotFound() : Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult<BookDto>> Add(AddBookDto dto)
        {
            try
            {
                var added = await _service.AddAsync(dto);
                // 201 Created + a Location header pointing to GET /api/books/{id}
                return CreatedAtAction(nameof(GetById), new { id = added.BookId }, added);
            }catch(InvalidOperationException ex)
            {
                // Translate the business-rule failure into an HTTP 409 Conflict
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
